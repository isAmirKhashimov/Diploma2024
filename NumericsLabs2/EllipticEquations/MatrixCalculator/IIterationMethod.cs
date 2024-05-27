using System;
using System.Collections.Generic;
using Row = System.Collections.Generic.List<double>;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;

namespace Numerics7
{
    internal abstract class IterationMethod: IMatrixCalculator
    {
        public const double Eps = 0.0000001;
        protected readonly Issue issue;

        public IterationMethod(Issue issue)
        {
            this.issue = issue;
        }

        public IReadOnlyList<Matrix> IterationEvolution { get; private set; }

        public Matrix Calculate()
        {
            var evolution = new List<Matrix>();
            var matrix = ListUtils.New<Row>(issue.Parameters.Ny + 1);

            for (int j = 0; j < matrix.Count; j++)
            {
                matrix[j] = ListUtils.New<double>(issue.Parameters.Nx + 1);
            }

            evolution.Add(matrix);
            Matrix oldMatrix;
            do
            {
                oldMatrix = matrix;
                matrix = CalculateIteration(oldMatrix);
                evolution.Add(matrix);
            } while (GetDistanceBetween(oldMatrix, matrix) > Eps);

            IterationEvolution = evolution;
            return matrix;
        }

        protected abstract Matrix CalculateIteration(Matrix oldMatrix);

        protected double GetDistanceBetween(Matrix a, Matrix b)
        {
            if (a.Count != b.Count) throw new ArgumentException();
            if (a[0].Count != b[0].Count) throw new ArgumentException();

            var maxdist = 0.0;

            for (int j = 0; j < a.Count; j++)
            {
                for (int i = 0; i < a[j].Count; i++)
                {
                    var dist = Math.Abs(a[j][i] - b[j][i]);
                    maxdist = Math.Max(maxdist, dist);
                }
            }

            return maxdist;
        }
    }
}
