namespace Diploma3.JsonModels
{
	public class Triangulation(List<TTriangle> triangles, List<TPoint> points)
    {
        public List<TTriangle> Triangles { get; set; } = triangles;
        public List<TPoint> Points { get; set; } = points;
    }
}
