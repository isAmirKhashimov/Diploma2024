using System;
using System.Collections.Generic;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics7
{
    internal class Solver
    {
        public Solver(Issue issue)
        {
            Issue = issue;
        }
        public Issue Issue { get; set; }

        public List<double[,]> IntermediateResults { get; private set; } 

        public Matrix GetAnalyticalResult()
        {
            var result = ListUtils.New<Solution>(Issue.Parameters.Ny + 1);

            for (int j = 0; j <= Issue.Parameters.Ny; j++)
            {
                result[j] = ListUtils.New<double>(Issue.Parameters.Nx + 1);
                
                for (int i = 0; i <= Issue.Parameters.Nx; i++)
                {
                    var x = Issue.Parameters.Hx * i;
                    var y = Issue.Parameters.Hy * j;

                    result[j][i] = Issue.AnalyticalExpression.U(x, y);
                }
            }

            return result;
        }

        public Matrix SolveWith(CalculationMethod calculationMethod, double omega = 0.0)
        {
            IMatrixCalculator matrixCalculator;
            switch (calculationMethod)
            {
                case CalculationMethod._5DIAG:
                    matrixCalculator = new SystemMatrixCalculator(Issue);
                    break;
                case CalculationMethod.LIBMAN:
                    matrixCalculator = new LibmanMatrixCalculator(Issue);
                    break;
                case CalculationMethod.ZEIDEL:
                    matrixCalculator = new ZeidelMatrixCalculator(Issue);
                    break;
                case CalculationMethod.RELAXATION:
                    matrixCalculator = new RelaxationMatrixCalculator(Issue) { Omega = omega };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationMethod));
            }

            var res = matrixCalculator.Calculate();
            IntermediateResults = To2DimArrayEach(((IterationMethod)matrixCalculator).IterationEvolution);
            return res;
        }

        private List<double[,]> To2DimArrayEach(IReadOnlyList<Matrix> matrixes)
        {
            var result = new List<double[,]>();

            foreach (var matrix in matrixes)
            {
                var res = new double[matrix.Count, matrix[0].Count];
                for (int i = 0; i < matrix.Count; i++)
                {
                    for (int j = 0; j < matrix.Count; j++)
                    {
                        res[i, j] = matrix[i][j];
                    }
                }

                result.Add(res);
            }
            
            return result;
        }
    }
}
