namespace Numerics6
{
    internal class IssueParameters
    {
        /// <summary>
        /// Step of x-dim partition
        /// </summary>
        public double H { get; set; }


        /// <summary>
        /// The first x-dim value
        /// </summary>
        public double LeftX { get; set; }

        /// <summary>
        /// The last x-dim value
        /// </summary>
        public double RightX { get; set; }

        /// <summary>
        /// The last x-dim number
        /// </summary>
        public int N { get; set; }



        /// <summary>
        /// Step of t-dim partition
        /// </summary>
        public double Tau { get; set; }

        /// <summary>
        /// The last t-dim number
        /// </summary>
        public int K { get; set; }

    }
}
