using Numerics5;
using ScottPlot.Drawing;
using System.Collections.Generic;
using System.Linq;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using Row = System.Collections.Generic.List<double>;

namespace Numerics7
{
    internal class SystemMatrixCalculator : IMatrixCalculator
    {
        private readonly Issue issue;

        public SystemMatrixCalculator(Issue issue)
        {
            this.issue = issue;
        }

        public Matrix Calculate()
        {
            var equations = new List<Equation>();

            for (int i = 1; i < issue.Parameters.Nx; i++)
            {
                for (int j = 1; j < issue.Parameters.Ny; j++)
                {
                    var x = i * issue.Parameters.Hx;
                    var y = j * issue.Parameters.Hy;

                    var coefs = ListUtils.New<double>((issue.Parameters.Nx + 1) * (issue.Parameters.Ny + 1) - 4);

                    coefs[issue.Parameters.To1DIndex(i - 1, j)] = issue.BaseExpression.Cx2 / (issue.Parameters.Hx * issue.Parameters.Hx) + issue.BaseExpression.Cx1 / (2.0 * issue.Parameters.Hx);
                    coefs[issue.Parameters.To1DIndex(i, j - 1)] = issue.BaseExpression.Cy2 / (issue.Parameters.Hy * issue.Parameters.Hy) + issue.BaseExpression.Cy1 / (2.0 * issue.Parameters.Hy);
                    coefs[issue.Parameters.To1DIndex(i, j)] = -2.0 * issue.BaseExpression.Cx2 / (issue.Parameters.Hx * issue.Parameters.Hx) - 2.0 * issue.BaseExpression.Cy2 / (issue.Parameters.Hy * issue.Parameters.Hy) - issue.BaseExpression.C;
                    coefs[issue.Parameters.To1DIndex(i, j + 1)] = issue.BaseExpression.Cy2 / (issue.Parameters.Hy * issue.Parameters.Hy) - issue.BaseExpression.Cy1 / (2.0 * issue.Parameters.Hy);
                    coefs[issue.Parameters.To1DIndex(i + 1, j)] = issue.BaseExpression.Cx2 / (issue.Parameters.Hx * issue.Parameters.Hx) - issue.BaseExpression.Cx1 / (2.0 * issue.Parameters.Hx);
                    
                    equations.Add(new Equation(coefs, issue.BaseExpression.F(x, y)));
                }
            }

            for (int xIndex = 1; xIndex < issue.Parameters.Nx; xIndex++)
            {
                equations.Add(issue.TopBorderExpression.Approximate2D(issue.Parameters, xIndex));
                equations.Add(issue.BottomBorderExpression.Approximate2D(issue.Parameters, xIndex));
            }

            for (int yIndex = 1; yIndex < issue.Parameters.Ny; yIndex++)
            {
                equations.Add(issue.LeftBorderExpression.Approximate2D(issue.Parameters, yIndex));
                equations.Add(issue.RightBorderExpression.Approximate2D(issue.Parameters, yIndex));
            }
            
            var syst = new LinearSystem(equations);
            var solution = syst.Solve();
            

            var res = ListUtils.New<Row>(issue.Parameters.Ny + 1);
            for (int j = 0; j < res.Count; j++)
            {
                res[j] = ListUtils.New<double>(issue.Parameters.Nx + 1);
            }
            
            for (int k = 0; k < solution.Count; k++)
            {
                (var y, var x) = issue.Parameters.To2Indexes(k);
                res[y][x] = solution[k];
            }


            return res;
        }
    }
}
