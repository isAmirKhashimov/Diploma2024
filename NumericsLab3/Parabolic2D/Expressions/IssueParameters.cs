namespace Numerics8
{
    internal class IssueParameters
    {
        /// <summary>
        /// Step of x-dim partition
        /// </summary>
        public double Hx { get; set; }
        
    
        /// <summary>
        /// Step of y-dim partition
        /// </summary>
        public double Hy { get; set; }



        /// <summary>
        /// The last x-dim value
        /// </summary>
        public double RightX { get; set; }

        /// <summary>
        /// The last y-dim value
        /// </summary>
        public double TopY { get; set; }

        /// <summary>
        /// The last x-dim number
        /// </summary>
        public int Nx { get; set; }

        /// <summary>
        /// The last y-dim number
        /// </summary>
        public int Ny { get; set; }

        /// <summary>
        /// Step of k-dim partition
        /// </summary>
        public double Tau { get; set; }

        /// <summary>
        /// The last k-dim number
        /// </summary>
        public int K { get; set; }

        public int To1DIndex(int xIndex, int yIndex) => (Nx + 1) * yIndex + xIndex - 1 - (yIndex >= 1 ? 1 : 0) - (yIndex == Ny ? 1 : 0);
        public (int xIndex, int yIndex) To2Indexes(int index)
        {
            var indexMod = index + 1 + (index >= Nx - 1 ? 1 : 0) + (index >= (Nx + 1) * Ny - 2 ? 1 : 0);
            return (indexMod / (Nx + 1), indexMod % (Nx + 1));
        }
    }
}
