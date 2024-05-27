using System;
using System.Collections.Generic;
using System.Linq;

namespace Diploma1
{
    internal class Controller
    {

        private readonly double a = -1;
        private readonly double b = 1;
        private readonly int n = 10;

        private Solver solver;

        //public double[] RangeOfX => Enumerable.Range(0, Issue.Parameters.Nx + 1).Select(x => Issue.Parameters.Hx * x).ToArray();

		/*
         * -2 
         * 0
         * 2
         * 
         * 3x^2 - 3
         * x = +- 1
         * 
         * 
         * p(x) = 1
         * q(x) = 3
         * k(x) = 1/6 * x^2
         */
		public Issue Issue => new Issue()
        {
            Fis = new Legendre().GetPolynoms().Take(4).Select(p => new Function(p, a, b, n)).ToArray(),
            K = new Function(x => x * x, a, b, n),
            P = new Function(x => x, a, b, n),
            Q = new Function(x => 3 * x, a, b, n),
            Parameters = new IssueParameters()
            {
                LeftX = a,
                RightX = b,
                Nx = n,
                PolynomsCount = 4
            }
        };
        public List<double[,]> Result { get; private set; }
        public List<double[,]> AnalyticalResult { get; private set; }

        public double[,] GetResultByTIndex(int t) => Result[t];
        public double[,] GetAnalyticalResultByTIndex(int t) => AnalyticalResult[t];

        public double[] GetResultByTandYIndex(int t, int y)
        {
            var tresult = GetResultByTIndex(t);
            var res = new double[tresult.GetLength(1)];
            for (int i = 0; i < res.GetLength(0); i++)
            {
                res[i] = tresult[y, i];
            }

            return res;
        }

        public double[] GetAnalyticalResultByTandYIndex(int t, int y)
        {
            var tresult = GetAnalyticalResultByTIndex(t);
            var res = new double[tresult.GetLength(1)];
            for (int i = 0; i < res.GetLength(0); i++)
            {
                res[i] = tresult[y, i];
            }

            return res;
        }


        public double[] Calculate()
        {
            var res = Issue.Solve();
            return res;
        }
    }
}
