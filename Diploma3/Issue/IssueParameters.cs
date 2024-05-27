namespace Diploma3
{
	internal class IssueParameters
    {
		public required MainEquation MainEquation { get; init; }
		public required BorderEquation BorderEquation { get; init; }
		public required int M { get; set; }
		public required double Xl { get; init; }
		public required double Xr { get; init; }
		public required double Hx { get; init; }
		public required double Yl { get; init; }
		public required double Yr { get; init; }
		public required double Hy { get; init; }
		public required int Nx { get; init; }
		public required int Ny { get; init; }
	}
}
