using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using DPoint = Delaune.Point;
using IPoint = Delaune.IPoint;
using Microsoft.Win32;
using ScottPlot.Panels;
using Diploma3.ContoureDetection;
using Diploma3.ViewModels;
using System.ComponentModel;
using System.Text.Json;
using System.IO;
using System.Windows.Markup;
using SkiaSharp;
using System.Windows.Media.Imaging;

namespace Diploma3
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Константы
		private const int borderEllipseZIndex = 2;
		private const int borderPolygonZIndex = 0;
		private const int dummyEllipseZIndex = 1;

		private const double defaultPointRadius = 5;

		private const double PointStrokeThicknessNormalState = 0;
		private const double PointStrokeThicknessOverState = 1;
		private const double EdgeStrokeThickness = 1;
		private const double BorderStrokeThickness = 2;
		private const double ContoureStrokeThickness = 3;

		private static readonly SolidColorBrush BorderPointBrushNormalState = new(Color.FromRgb(0, 0, 0));
		private static readonly SolidColorBrush PointBrushNormalState = new(Color.FromRgb(100, 200, 0));
		private static readonly SolidColorBrush PointBrushMouseOverState = new(Color.FromRgb(200, 100, 0));
		private static readonly SolidColorBrush DummyPointBrushNormalState = new(Color.FromArgb(10, 100, 100, 0));
		private static readonly SolidColorBrush DummyPointBrushMouseOverState = new(Color.FromRgb(200, 100, 0));

		#endregion

		private readonly List<CanvasPoint> borderEllipses = [];
		private readonly List<Ellipse> dummyEllipses = [];
		private readonly List<Ellipse> innerEllipses = [];

		private readonly CanvasTransformer canvasTransformer = new();
		private readonly Controller controller = new();
		private readonly Scaler scaler = new();

		private Contoures contoures;
		private CanvasPoint? capturedBorderPoint;

		private bool unlockAction = false;
		private Polygon borderPolygon = new();
		private Ellipse? capturedPoint;
		private Delaune.Delaunator delaunator;
		private AppMode appMode = AppMode.Bordering;
		private AppMode previousAppMode = AppMode.Bordering;
		private ContouresSettingsViewModel contouresSettingsViewModel;
		private BusyIndicatorController busyIndicatorController;

		public WorkObject CurrentObject { get; set; }
		public List<WorkObject> WorkObjects { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			busyIndicatorController = new BusyIndicatorController(BusyIndicator);
			canvasTransformer.PropertyChanged += CanvasTransformer_PropertyChanged;

			contouresSettingsViewModel = new(scaler);
			contouresSettingsViewModel.ContouresChanged += (_, _) => UpdateContoures();
			contouresSettingsViewModel.TriangulationChanged += (_, _) => UpdateTriangulation();

			SettingsPanel.DataContext = contouresSettingsViewModel;

			InitField();

			PlayGroundCanvas.Children.Add(borderPolygon);

			controller.Calculate();
			RefreshAnalyticalGraph();
			RefreshIterGraph();

			appMode = AppMode.Scaling;
		}

		private void CanvasTransformer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			CanvasTranslateTransform.X = canvasTransformer.CenterX;
			CanvasTranslateTransform.Y = canvasTransformer.CenterY;
		}

		private void RemoveInnerPoints()
		{
			foreach (var ellipse in innerEllipses)
			{
				PlayGroundCanvas.Children.Remove(ellipse);
			}

			innerEllipses.Clear();
			CurrentObject.InnerPoints.Clear();
		}

		private Line GetLineBetween(DPoint p, DPoint q)
		{
			var line = new Line()
			{
				X1 = p.X,
				X2 = q.X,
				Y1 = p.Y,
				Y2 = q.Y,
				StrokeThickness = EdgeStrokeThickness,
				Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
				Tag = new EdgeTag(),
			};

			return line;
		}

		private CanvasPoint GetBorderCircleAround(ReferencePoint point)
		{
			var ellipse = new Ellipse
			{
				Width = defaultPointRadius * 2,
				Height = defaultPointRadius * 2,
				Fill = BorderPointBrushNormalState,
				StrokeThickness = PointStrokeThicknessNormalState,
				Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
			};

			var canvasPoint = new CanvasPoint(ellipse, point, PlayGroundCanvas);
			ellipse.Tag = canvasPoint;
			Canvas.SetLeft(ellipse, point.Value.X - defaultPointRadius);
			Canvas.SetTop(ellipse, point.Value.Y - defaultPointRadius);
			Canvas.SetZIndex(ellipse, borderEllipseZIndex);

			ellipse.MouseDown += (sender, e) =>
			{
				if (appMode.HasFlag(AppMode.Bordering))
				{
					FinishBordering();
				}
				else
				{
					canvasPoint.MouseDown(sender, e);
				}
			};

			ellipse.MouseUp += canvasPoint.MouseUp;
			ellipse.MouseMove += canvasPoint.MouseMove;
			ellipse.IsMouseDirectlyOverChanged += BorderEllipse_IsMouseDirectlyOverChanged;

			canvasPoint.IsCapturedChanged += CanvasPoint_IsCapturedChanged;
			canvasPoint.ValueChanged += CanvasPoint_ValueChanged;
			canvasPoint.PointRemoved += CanvasPoint_PointRemoved;

			return canvasPoint;
		}

		private void CanvasPoint_PointRemoved(object? sender, EventArgs e)
		{
			if (sender is not CanvasPoint cp) throw new ArgumentException();
			CurrentObject.OuterPoints.Remove(cp.CurrentPoint);

			AddBorderPolygon(CurrentObject.OuterPoints);
		}

		private void DrawDummyPoints(List<ReferencePoint> borderPoints)
		{
			RemoveDummyPoints();

			if (borderPoints.Count == 0) return;

			var previousPoint = borderPoints[borderPoints.Count - 1];

			for (int i = 0; i < borderPoints.Count; i++)
			{
				var currentPoint = borderPoints[i];
				var ellipse = new Ellipse
				{
					Width = defaultPointRadius * 2,
					Height = defaultPointRadius * 2,
					Fill = DummyPointBrushNormalState,
					StrokeThickness = PointStrokeThicknessNormalState,
					Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
				};

				ellipse.IsMouseDirectlyOverChanged += DummyEllipse_IsMouseDirectlyOverChanged;
				ellipse.Tag = previousPoint;

				Canvas.SetLeft(ellipse, (currentPoint.Value.X + previousPoint.Value.X) / 2.0 - defaultPointRadius);
				Canvas.SetTop(ellipse, (currentPoint.Value.Y + previousPoint.Value.Y) / 2.0 - defaultPointRadius);
				Canvas.SetZIndex(ellipse, dummyEllipseZIndex);

				ellipse.MouseDown += DummyEllipse_MouseDown;

				PlayGroundCanvas.Children.Add(ellipse);
				dummyEllipses.Add(ellipse);
				previousPoint = currentPoint;
			}
		}

		private void RemoveDummyPoints()
		{
			foreach (var el in dummyEllipses)
			{
				PlayGroundCanvas.Children.Remove(el);
			}

			dummyEllipses.Clear();
		}

		private void DummyEllipse_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is not Ellipse ellipse) throw new Exception();
			if (ellipse.Tag is not ReferencePoint pointref) throw new Exception();

			var pointNode = CurrentObject.OuterPoints.Find(pointref)!;
			var point = new ReferencePoint(e.GetPosition(PlayGroundCanvas));
			
			CurrentObject.OuterPoints.AddAfter(pointNode, new LinkedListNode<ReferencePoint>(point));
			var canvasPoint = GetBorderCircleAround(point);
			PlayGroundCanvas.Children.Add(canvasPoint.Ellipse);
			borderEllipses.Add(canvasPoint);
			CurrentObject.InnerPoints.Add(new(point, true));

			canvasPoint.MouseDown(sender, e);

			AddBorderPolygon(CurrentObject.OuterPoints);
			contouresSettingsViewModel.Contoure = CurrentObject.OuterPoints.ToList();
		}

		private void DummyEllipse_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not Ellipse ellipse) throw new Exception();
			if (ellipse.IsMouseDirectlyOver)
			{
				capturedPoint = ellipse;
				ellipse.Fill = DummyPointBrushMouseOverState;
				ellipse.StrokeThickness = PointStrokeThicknessOverState;
			}
			else
			{
				capturedPoint = null;
				ellipse.StrokeThickness = PointStrokeThicknessNormalState;
				ellipse.Fill = DummyPointBrushNormalState;
			}

		}

		private void BorderEllipse_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not Ellipse ellipse) throw new Exception();
			if (ellipse.IsMouseDirectlyOver)
			{
				capturedPoint = ellipse;
				ellipse.Fill = PointBrushMouseOverState;
				ellipse.StrokeThickness = PointStrokeThicknessOverState;
			}
			else
			{
				capturedPoint = null;
				ellipse.StrokeThickness = PointStrokeThicknessNormalState;
				ellipse.Fill = BorderPointBrushNormalState;
			}

		}

		private void CanvasPoint_ValueChanged(object? sender, EventArgs e)
		{
			if (sender is not CanvasPoint cp) throw new ArgumentException();

			Canvas.SetLeft(cp.Ellipse, cp.CurrentPoint.Value.X - defaultPointRadius);
			Canvas.SetTop(cp.Ellipse, cp.CurrentPoint.Value.Y - defaultPointRadius);

			AddBorderPolygon(CurrentObject.OuterPoints);
			contouresSettingsViewModel.Contoure = CurrentObject.OuterPoints.ToList();

			if (appMode.HasFlag(AppMode.Triangulation) || CurrentObject.InnerPoints.Any(el => !el.IsOnBorder)) FindDelaune();
		}

		private void CanvasPoint_IsCapturedChanged(object? sender, EventArgs e)
		{
			if (sender is not CanvasPoint cp) throw new ArgumentException();

			capturedBorderPoint = cp.IsCaptured ? cp : null;
		}

		private Ellipse GetCircleAround(Point point)
		{
			var ellipse = new Ellipse
			{
				Width = defaultPointRadius * 2,
				Height = defaultPointRadius * 2,
				Fill = PointBrushNormalState,
				StrokeThickness = PointStrokeThicknessNormalState,
				Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
			};

			Canvas.SetLeft(ellipse, point.X - defaultPointRadius);
			Canvas.SetTop(ellipse, point.Y - defaultPointRadius);

			ellipse.IsMouseDirectlyOverChanged += Ellipse_IsMouseDirectlyOverChanged;
			innerEllipses.Add(ellipse);

			var pointReference = new ReferencePoint(point);
			var innerPoint = new CanvasInnerPoint(pointReference, false);
			ellipse.Tag = innerPoint;

			CurrentObject.InnerPoints.Add(innerPoint);
			return ellipse;
		}

		#region MouseOver
		private void Ellipse_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not Ellipse ellipse) throw new Exception();
			if (ellipse.IsMouseDirectlyOver)
			{
				capturedPoint = ellipse;
				ellipse.Fill = PointBrushMouseOverState;
				ellipse.StrokeThickness = PointStrokeThicknessOverState;
			}
			else
			{
				capturedPoint = null;
				ellipse.StrokeThickness = PointStrokeThicknessNormalState;
				ellipse.Fill = PointBrushNormalState;
			}
		}
		#endregion
		#region MouseDown 
		private void PlayGroundCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (appMode.HasFlag(AppMode.Scaling)) ScalingModeCanvas_MouseDown(sender, e);
			if (appMode.HasFlag(AppMode.Bordering)) BorderingModeCanvas_MouseDown(sender, e);
			if (appMode.HasFlag(AppMode.Triangulation)) TriangulationModeCanvas_MouseDown(sender, e);
		}
		private void ScalingModeCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released)
			{
				canvasTransformer.ButtonDown(e.GetPosition(PlayGroundCanvas));
			}
		}

		private void BorderingModeCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
			{
				var mousePosition = e.GetPosition(PlayGroundCanvas);
				var refPoint = new ReferencePoint(mousePosition);
				var canvasPoint = GetBorderCircleAround(refPoint);
				PlayGroundCanvas.Children.Add(canvasPoint.Ellipse);
				CurrentObject.OuterPoints.AddLast(refPoint);
			}
		}


		private void TriangulationModeCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released || e.RightButton == MouseButtonState.Pressed) return;

			if (capturedPoint == null)
			{
				var point = e.GetPosition(PlayGroundCanvas);
				if (!PointGenerator.InsideTheBorder(
					CurrentObject.OuterPoints.ToList(), 
					scaler.TransformBack(contouresSettingsViewModel.TriangulationMinimalDistance.Value), 
					point.X, 
					point.Y)) return;
				
				var ellipse = GetCircleAround(point);
				PlayGroundCanvas.Children.Add(ellipse);
				FindDelaune();
			}
			else
			{
				if (capturedPoint.Tag is not CanvasInnerPoint) return;

				var innerPoint = (CanvasInnerPoint)capturedPoint.Tag;
				PlayGroundCanvas.Children.Remove(capturedPoint);
				if (!CurrentObject.InnerPoints.Remove(innerPoint)) throw new Exception();
				capturedPoint = null;
				FindDelaune();
			}

			if (unlockAction)
			{
				Calculate();
			}
		}

		#endregion
		#region MouseUp
		private void PlayGroundCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (appMode.HasFlag(AppMode.Scaling)) ScalingModeCanvas_MouseUp(sender, e);
		}

		private void ScalingModeCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Released)
			{
				canvasTransformer.ButtonUp(e.GetPosition(PlayGroundCanvas));
			}

			if (e.LeftButton == MouseButtonState.Released)
			{
				capturedBorderPoint?.MouseUp(sender, e);
			}
		}
		#endregion
		#region MouseMove
		private void PlayGroundCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(PlayGroundCanvas);
			MousePosition.Content = $"{pos.X:0.000}, {pos.Y:0.000}";

			if (appMode.HasFlag(AppMode.Bordering)) BorderingModeCanvas_MouseMove(sender, e);
			if (appMode.HasFlag(AppMode.Scaling)) ScalingModeCanvas_MouseMove(sender, e);
		}


		private void BorderingModeCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (CurrentObject == null) return;
			if (CurrentObject.OuterPoints.Count == 0) return;

			AddBorderPolygon(CurrentObject.OuterPoints.Concat([new(e.GetPosition(PlayGroundCanvas))]));
			//AddBorderPolygon(borderPoints);
		}

		private void ScalingModeCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released)
			{
				canvasTransformer.MouseMoveTo(e.GetPosition(PlayGroundCanvas));
			}
			else if (e.RightButton == MouseButtonState.Released && e.LeftButton == MouseButtonState.Pressed)
			{
				capturedBorderPoint?.MouseMove(sender, e);
			}
		}
		#endregion
		#region MouseWheel
		private void PlayGroundCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (appMode.HasFlag(AppMode.Scaling)) ScalingModeCanvas_MouseWheel(sender, e);
		}

		private void ScalingModeCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var scale = e.Delta > 0
				? 1 + e.Delta / 120 * 0.1
				: 1 / (1 - e.Delta / 120 * 0.1);
			var pos = e.GetPosition(PlayGroundCanvas);
			var matrix = CanvasMatrixTransform.Matrix;
			matrix.ScaleAtPrepend(scale, scale, pos.X, pos.Y);
			CanvasMatrixTransform.Matrix = matrix;
		}
		#endregion


		private void InitField()
		{
			PlayGroundCanvas.Children.Clear();
			
			for (int y = -5000; y <= 5000; y += 50)
			{
				var line = new Line()
				{
					X1 = -5000,
					X2 = 5000,
					Y1 = y,
					Y2 = y,
					StrokeThickness = y == 0 ? 3 : 1,
					Stroke = new SolidColorBrush(y == 0 ? Color.FromArgb(40, 255, 0, 0) : Color.FromArgb(40, 0, 0, 255)),
				};

				PlayGroundCanvas.Children.Add(line);
			}
			for (int x = -5000; x <= 5000; x += 50)
			{
				var line = new Line()
				{
					Y1 = -5000,
					Y2 = 5000,
					X1 = x,
					X2 = x,

					StrokeThickness = x == 0 ? 3 : 1,
					Stroke = new SolidColorBrush(x == 0 ? Color.FromArgb(40, 255, 0, 0) : Color.FromArgb(40, 0, 0, 255)),
				};

				PlayGroundCanvas.Children.Add(line);
			}
		}


		private void AddBorderPolygon(IEnumerable<ReferencePoint> points)
		{
			DrawDummyPoints(points.ToList());
			PlayGroundCanvas.Children.Remove(borderPolygon);

			borderPolygon = new Polygon()
			{
				Points = new PointCollection(points.Select(el => el!.Value)),
				StrokeThickness = BorderStrokeThickness,
				Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
			};

			borderPolygon.MouseDown += PlayGroundCanvas_MouseDown;
			Canvas.SetZIndex(borderPolygon, borderPolygonZIndex);
			PlayGroundCanvas.Children.Add(borderPolygon);
		}


		private void FillBackground()
		{
			if (CurrentObject == null) return;
			var bitmapSource = CurrentObject.ImageBitmap.ToBitmapImage();

			var im = new Image()
			{
				Source = bitmapSource,
			};
			im.Opacity = 0.4;
			InitField();
			PlayGroundCanvas.Children.Add(im);
			Canvas.SetLeft(im, 0);
			Canvas.SetTop(im, 0);

		}


		#region RemovePoints
		private void RemovePoints()
		{
			RemoveOuterPoints();
			RemoveInnerPoints();
			RemoveDummyPoints();

			PlayGroundCanvas.Children.Remove(borderPolygon);

			RemoveEdges();
		}

		private void RemoveOuterPoints()
		{
			foreach (var canvasPoint in borderEllipses)
			{
				PlayGroundCanvas.Children.Remove(canvasPoint.Ellipse);
			}

			borderEllipses.Clear();
			CurrentObject.OuterPoints.Clear();
		}


		private void RemoveEdges()
		{
			var itemstoremove = new List<UIElement>();

			foreach (UIElement uiElement in PlayGroundCanvas.Children)
			{
				if (uiElement is Line line && line.Tag is EdgeTag)
				{
					itemstoremove.Add(uiElement);
				}
			}

			foreach (UIElement ui in itemstoremove)
			{
				PlayGroundCanvas.Children.Remove(ui);
			}
		}
		#endregion

		#region МКЭ
		private void DiffButton_Click(object sender, RoutedEventArgs e)
		{
			unlockAction = true;
			Calculate();
		}

		private void Calculate()
		{
			controller.Issue.Parameters.M = CurrentObject.InnerPoints.Count;

			var nodes = CurrentObject.InnerPoints.Select((el, Index) => new Node(scaler.Transform(el.Point.Value).X, scaler.Transform(el.Point.Value).Y, Index, el.IsOnBorder)).ToList();
			var finiteElements = delaunator.GetTriangles().Select(tr =>
			{
				var vertexes = tr.Points.ToList();

				var v1 = scaler.Transform(vertexes[0].ToPoint());
				var v2 = scaler.Transform(vertexes[1].ToPoint());
				var v3 = scaler.Transform(vertexes[2].ToPoint());

				var firstPoint = nodes.Single(el => v1.X == el.X && v1.Y == el.Y);
				var secondPoint = nodes.Single(el => v2.X == el.X && v2.Y == el.Y);
				var thirdPoint = nodes.Single(el => v3.X == el.X && v3.Y == el.Y);

				return new FiniteElement(firstPoint, secondPoint, thirdPoint, controller.Issue.Parameters);
			}).ToList();


			controller.Calculate(nodes, finiteElements);
			RefreshAnalyticalGraph();
			RefreshIterGraph();
		}


		private void RefreshIterGraph()
		{
			HeatVisualizer.Plot.Clear();
			var hm = HeatVisualizer.Plot.Add.Heatmap(controller.Result);
			hm.Smooth = true;
			hm.Colormap = new ScottPlot.Colormaps.Thermal();
			if (cb1 == null)
			{
				cb1 = HeatVisualizer.Plot.Add.ColorBar(hm);
			}
			else
			{
				cb1.Source = hm;
			}
			HeatVisualizer.Plot.XLabel("x");
			HeatVisualizer.Plot.YLabel("y");
			HeatVisualizer.Refresh();
		}
		ColorBar? cb1;
		ColorBar? cb2;
		private void RefreshAnalyticalGraph()
		{
			AnanylitcalHeatVisualizer.Plot.Clear();
			var hm = AnanylitcalHeatVisualizer.Plot.Add.Heatmap(controller.AnalyticalResult);
			hm.Colormap = new ScottPlot.Colormaps.Thermal();
			hm.Smooth = true;
			if (cb2 == null)
			{
				cb2 = HeatVisualizer.Plot.Add.ColorBar(hm);
			}
			else
			{
				cb2.Source = hm;
			}
			AnanylitcalHeatVisualizer.Plot.XLabel("x");
			AnanylitcalHeatVisualizer.Plot.YLabel("y");
			AnanylitcalHeatVisualizer.Refresh();
		}

		#endregion

		#region Контуризация

		private void FinishBordering()
		{
			if (!appMode.HasFlag(AppMode.Bordering)) return;

			appMode = AppMode.Scaling;
			AddBorderPolygon(CurrentObject.OuterPoints);

			foreach (var point in CurrentObject.OuterPoints)
			{
				CurrentObject.InnerPoints.Add(new(point, true));
			}

			contouresSettingsViewModel.Contoure = CurrentObject.OuterPoints.ToList();
			//FinishManualContouring();
		}

		private void UpdateContoures()
		{
			if (CurrentObject == null) return;

			RemovePoints();
			foreach (var point in contouresSettingsViewModel.Contoure)
			{
				CurrentObject.OuterPoints.AddLast(point);
				var canvasPoint = GetBorderCircleAround(point);
				PlayGroundCanvas.Children.Add(canvasPoint.Ellipse);
				borderEllipses.Add(canvasPoint);
				CurrentObject.InnerPoints.Add(new(point, true));
			}

			AddBorderPolygon(CurrentObject.OuterPoints);
			//appMode = AppMode.Triangulation;
		}
		#endregion

		#region Триангуляция

		private void FindDelaune()
		{
			RemoveEdges();

			if (CurrentObject.InnerPoints.Count >= 3)
			{
				delaunator = new Delaune.Delaunator(CurrentObject.InnerPoints.Select(point => (IPoint)point.Point.Value.ToDPoint()).ToArray());
				delaunator.ForEachTriangleEdge(edge =>
				{
					if (PointGenerator.InsideTheBorder(CurrentObject.OuterPoints.ToList(), contouresSettingsViewModel.TriangulationMinimalDistance.Value, (edge.P.X + edge.Q.X) / 2, (edge.P.Y + edge.Q.Y) / 2))
					{
						var line = GetLineBetween((DPoint)edge.P, (DPoint)edge.Q);
						PlayGroundCanvas.Children.Add(line);
					}
				});
			}
		}

		private void GeneratePoints()
		{
			RemoveInnerPoints();

			foreach (var point in CurrentObject.OuterPoints)
			{
				CurrentObject.InnerPoints.Add(new(point, true));
			}

			var pointGenerator = new PointGenerator(
				CurrentObject.OuterPoints.Select(point => new ReferencePoint(scaler.Transform(point!.Value))).ToList(),
				contouresSettingsViewModel.TriangulationMinimalDistance.Value,
				contouresSettingsViewModel.TriangulationBahdWidth.Value);
			PointGenerator.MinimalDistanceRelaxer = contouresSettingsViewModel.TriangulationMinimalDistanceRelaxer.Value;

			var pointsToAdd = pointGenerator.Generate();

			foreach (var point in pointsToAdd)
			{
				var ellipse = GetCircleAround(scaler.TransformBack(point));
				PlayGroundCanvas.Children.Add(ellipse);
				innerEllipses.Add(ellipse);
			}
		}

		private void UpdateTriangulation()
		{
			GeneratePoints();
			FindDelaune();
			appMode &= ~AppMode.Bordering;
		}
		#endregion

		#region Events
		private void OpenFileButton_Click(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog
			{
				Filter = "Image files (*.png)|*.png|Bitmap files (*.bmp)|*.bmp|Image files (*.jpg)|*.jpg|Pdf files (*.pdf)|*.pdf",
			};

			if (fileDialog.ShowDialog() == true)
			{
				if (System.IO.Path.GetExtension(fileDialog.FileName) == ".pdf")
				{
					var worker = new BackgroundWorker { WorkerReportsProgress = true };
					var currentProgress = 0;

					worker.DoWork += (o, ea) =>
					{
						using var parser = new GroupDocs.Parser.Parser(fileDialog.FileName);
						IEnumerable<GroupDocs.Parser.Data.PageImageArea> images = parser.GetImages();
						WorkObjects = images.Select(im => new WorkObject(System.Drawing.Image.FromStream(im.GetImageStream()).SetOpacity(1.0f))).ToList();
					};

					worker.RunWorkerCompleted += (o, ea) =>
					{
						busyIndicatorController.SetNotBusy();
						imageList.ItemsSource = WorkObjects;
					};

					busyIndicatorController.SetBusyContent("Извлечение изображений из PDF-файла");
					busyIndicatorController.SetBusy();

					if (MessageBox.Show("Подтвердить извелечение изображений из PDF-файла. Данный процесс может занять до 1 минуты.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						worker.RunWorkerAsync();
					}
					else
					{
						busyIndicatorController.SetNotBusy();
					}
				}
				else
				{
					var bitmap = System.Drawing.Image.FromFile(fileDialog.FileName).SetOpacity(1.0f);
					WorkObjects = [new(bitmap)];
					imageList.ItemsSource = WorkObjects;
				}

				imageList.SelectedIndex = 0;
			}
		}


		private void AddFileButton_Click(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog
			{
				Filter = "Image files (*.png; *.jpg; *.pdf)|*.png;*.jpg;*.bmp|Pdf files (*.pdf)|*.pdf",
				Multiselect = true,
			};

			if (fileDialog.ShowDialog() == true)
			{
				if (WorkObjects == null)
				{
					WorkObjects = [];
				}

				foreach (var fileName in fileDialog.FileNames)
				{
					if (System.IO.Path.GetExtension(fileName) == ".pdf")
					{
						var worker = new BackgroundWorker { WorkerReportsProgress = true };
						var currentProgress = 0;

						worker.DoWork += (o, ea) =>
						{
							using var parser = new GroupDocs.Parser.Parser(fileName);
							IEnumerable<GroupDocs.Parser.Data.PageImageArea> images = parser.GetImages();
							WorkObjects.AddRange(images.Select(im => new WorkObject(System.Drawing.Image.FromStream(im.GetImageStream()).SetOpacity(1.0f))).ToList());
						};

						worker.RunWorkerCompleted += (o, ea) =>
						{
							busyIndicatorController.SetNotBusy();
							imageList.ItemsSource = null;
							imageList.ItemsSource = WorkObjects;
						};

						busyIndicatorController.SetBusyContent("Извлечение изображений из PDF-файла");
						busyIndicatorController.SetBusy();

						if (MessageBox.Show("Подтвердить извелечение изображений из PDF-файла. Данный процесс может занять до 1 минуты.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
						{
							worker.RunWorkerAsync();
						}
						else
						{
							busyIndicatorController.SetNotBusy();
						}
					}
					else
					{
						var bitmap = System.Drawing.Image.FromFile(fileName).SetOpacity(1.0f);
						WorkObjects.Add(new(bitmap));
						imageList.ItemsSource = null;
						imageList.ItemsSource = WorkObjects;
					}
				}

				imageList.SelectedIndex = 0;
			}
		}

		private void TriangulationButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateTriangulation();
		}

		private void imageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateImageSelection();
		}

		private void UpdateImageSelection()
		{
			if (imageList.SelectedItem == null) return;
			CurrentObject = (WorkObject)imageList.SelectedItem;
			contouresSettingsViewModel.TargetImage = CurrentObject?.ImageBitmap;
			FillBackground();

			
			if (SettingsPanel.SelectedIndex == (int)PanelIndex.Contouring)
			{
				contouresSettingsViewModel.SetAutoDefaultContoureValues();
				UpdateContoures();
			}
			else if (SettingsPanel.SelectedIndex != (int)PanelIndex.Control)
			{
				SettingsPanel.SelectedIndex = (int)PanelIndex.Contouring;
			}
		}

		private void ContouringToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			StartManualContouring();
		}

		private void TriangulationToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			StartManualTriangulation();
		}

		
		private void ContouringToggleButton_UnChecked(object sender, RoutedEventArgs e)
		{
			FinishManualContouring();
		}

		private void TriangulationToggleButton_UnChecked(object sender, RoutedEventArgs e)
		{
			FinishManualTriangulation();
		}

		private void StartManualContouring()
		{
			appMode |= AppMode.Bordering;
			appMode &= ~AppMode.Triangulation;
			RemovePoints();
		}

		private void FinishManualContouring()
		{
			UpdateContoures();
			appMode = AppMode.Scaling;
		}

		private void StartManualTriangulation()
		{
			appMode |= AppMode.Triangulation;
			appMode &= ~AppMode.Bordering;
			UpdateContoures();
		}

		private void FinishManualTriangulation()
		{
			appMode = AppMode.Scaling;
		}



		#endregion

		private void SaveTriangles_Click(object sender, RoutedEventArgs e)
		{
			var saveFileDialog = new SaveFileDialog
			{
				Filter = "json files (*.json)|*.json",
				RestoreDirectory = true
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				string jsonString = JsonSerializer.Serialize(delaunator.GetTriangles());
				File.WriteAllText(saveFileDialog.FileName, jsonString);
			}
		}

		private void Export_Canvas(object sender, RoutedEventArgs e)
		{
			var saveFileDialog = new SaveFileDialog
			{
				Filter = "xaml files (*.xaml)|*.xaml",
				RestoreDirectory = true
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				FileStream fs = File.Open(saveFileDialog.FileName, FileMode.Create);
				XamlWriter.Save(PlayGroundCanvas, fs);
				fs.Close();
			}
		}

		private void SettingsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.OriginalSource != sender) return;

			appMode = AppMode.Scaling;

			if (contouresSettingsViewModel.ImageSource != null)
			{
				if (SettingsPanel.SelectedIndex == (int)PanelIndex.Contouring)
				{
					contouresSettingsViewModel.SetAutoDefaultContoureValues();
					UpdateContoures();
				}
				else if (SettingsPanel.SelectedIndex == (int)PanelIndex.Triangulation)
				{
					contouresSettingsViewModel.SetAutoTriangulationDefaultValues();
					UpdateTriangulation();
				}
			}
				
		}

		private void ExportImage_Click(object sender, RoutedEventArgs e)
		{
			if (contouresSettingsViewModel.TargetImage == null) return;

			var saveFileDialog = new SaveFileDialog
			{
				Filter = "Images (*.png)|*.png",
				RestoreDirectory = true
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				RenderTargetBitmap rtb = new RenderTargetBitmap((int)PlayGroundCanvas.RenderSize.Width * 10, (int)PlayGroundCanvas.RenderSize.Height * 10, 96d, 96d, System.Windows.Media.PixelFormats.Default);
				rtb.Render(PlayGroundCanvas);

				var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)contouresSettingsViewModel.TargetImage.Width, (int)contouresSettingsViewModel.TargetImage.Height));

				BitmapEncoder pngEncoder = new PngBitmapEncoder();
				pngEncoder.Frames.Add(BitmapFrame.Create(crop));

				using (var fs = File.OpenWrite(saveFileDialog.FileName))
				{
					pngEncoder.Save(fs);
				}
			}
		}

		private void TriangulationAutoToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			contouresSettingsViewModel.SetAutoTriangulationDefaultValues();
		}

		private void TriangulationAutoToggleButton_UnChecked(object sender, RoutedEventArgs e)
		{

		}
	}
}