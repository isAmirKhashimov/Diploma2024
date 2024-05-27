namespace Diploma1
{
    internal class IssueParameters
    {

		/// <summary>
		/// The last x-dim value
		/// </summary>
		public required double LeftX { get; set; }

		/// <summary>
		/// The last x-dim value
		/// </summary>
		public required double RightX { get; set; }

        /// <summary>
        /// The last x-dim number
        /// </summary>
        public required int Nx { get; set; }

        /// <summary>
        /// Amount of polynoms
        /// </summary>
        public required int PolynomsCount { get; set; }
    }
}
