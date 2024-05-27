using ScottPlot.Plottable;
using System.Collections.Generic;
using System.Linq;

using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics8
{
    internal class MPNStepCalculator: IStepCalculator
    {  
        private readonly Issue issue;
        public MPNStepCalculator(Issue issue)
        {
            this.issue = issue;
        }

        public List<Matrix> Calculate()
        {
            var result = ListUtils.New<Matrix>(issue.Parameters.K + 1);
            for (int k = 0; k < result.Count; k++)
            {
                result[k] = ListUtils.New<Solution>(issue.Parameters.Ny + 1);
                for(int j = 0; j < result[k].Count; j++)
                {
                    result[k][j] = ListUtils.New<double>(issue.Parameters.Nx + 1);
                }
            }

            for (int j = 0; j <= issue.Parameters.Ny; j++)
            {
                for (int i = 0; i <= issue.Parameters.Nx; i++)
                {
                    var x = issue.Parameters.Hx * i;
                    var y = issue.Parameters.Hy * j;
                    result[0][j][i] = issue.StartExpression.f(x, y);
                }
            }

            for (int k = 1; k <= issue.Parameters.K; k++)
            {
                result[k] = CalculateNextStep(result[k - 1], k);
            }

            return result;
        }

        /*
         *  |   |   |   |   |   |   |
         *  | x0| x1| x2| x3| x4| x5|
         *  |   |   |   |   |   |   |
         *  |   |   |   |   |   |   |
         *  |   |   |   |   |   |   |
         *  |   |   |   |   |   |   |
         */


        private Matrix CalculateNextStep(Matrix previousStep, int k)
        {
            var t = issue.Parameters.Tau * k;
            var halfStepRows = Enumerable.Range(1, issue.Parameters.Ny - 1).Select(j => CalculateJthRow(previousStep, t, j)).ToList();
            var halfStepResult = ListUtils.ConcatinateRows(AddYBorderExpressions(halfStepRows, t));

            var fullStepCols = Enumerable.Range(1, issue.Parameters.Nx - 1).Select(i => CalculateIthColumn(halfStepResult, t, i)).ToList();
            var fullStepResult = ListUtils.ConcatinateColumns(AddXBorderExpressions(fullStepCols, t));

            return fullStepResult;
        }

        private List<Solution> AddYBorderExpressions(List<Solution> middleRows, double t)
        {
            var topRow = ListUtils.New<double>(issue.Parameters.Nx + 1);
            var bottomRow = ListUtils.New<double>(issue.Parameters.Nx + 1);
            for (int i = 0; i <= issue.Parameters.Nx; i++)
            {
                var topApproximation = issue.TopBorderExpression.Approximate1D(issue.Parameters, i, t);
                var bottomApproximation = issue.BottomBorderExpression.Approximate1D(issue.Parameters, i, t);

                topRow[i] = (topApproximation.Result - middleRows[1][i] * topApproximation.Coefs[1]) / topApproximation.Coefs[0];
                bottomRow[i] = (bottomApproximation.Result - middleRows[issue.Parameters.Ny - 2][i] * bottomApproximation.Coefs[issue.Parameters.Ny - 1]) / bottomApproximation.Coefs[issue.Parameters.Ny];
            }
            var rows = new List<Solution>();
            rows.Add(topRow);
            rows.AddRange(middleRows);
            rows.Add(bottomRow);
            return rows;
        }

        private List<Solution> AddXBorderExpressions(List<Solution> middleCols, double t)
        {
            var leftCol = ListUtils.New<double>(issue.Parameters.Ny + 1);
            var rightCol = ListUtils.New<double>(issue.Parameters.Ny + 1);
            for (int j = 0; j <= issue.Parameters.Ny; j++)
            {
                var leftApproximation = issue.LeftBorderExpression.Approximate1D(issue.Parameters, j, t + issue.Parameters.Tau / 2.0);
                var rightApproximation = issue.RightBorderExpression.Approximate1D(issue.Parameters, j, t + issue.Parameters.Tau / 2.0);

                leftCol[j] = (leftApproximation.Result - middleCols[1][j] * leftApproximation.Coefs[1]) / leftApproximation.Coefs[0];
                rightCol[j] = (rightApproximation.Result - middleCols[issue.Parameters.Nx - 2][j] * rightApproximation.Coefs[issue.Parameters.Nx - 1]) / rightApproximation.Coefs[issue.Parameters.Nx];
            }
            var cols = new List<Solution>();
            cols.Add(leftCol);
            cols.AddRange(middleCols);
            cols.Add(rightCol);
            return cols;
        }

        // 1st half of step
        private Solution CalculateJthRow(Matrix previousStep, double t, int j)
        {
            var equations = new List<Equation>();
            equations.Add(issue.LeftBorderExpression.Approximate1D(issue.Parameters, j, t));
            equations.AddRange(Enumerable.Range(1, issue.Parameters.Nx - 1).Select(i => GetIthEquationForJ(previousStep, t, i, j)));
            equations.Add(issue.RightBorderExpression.Approximate1D(issue.Parameters, j, t));

            var system = new LinearSystem(equations);
            return system.Solve();
        }

        private Equation GetIthEquationForJ(Matrix uPrev, double t, int i, int j)
        {
            var coefs = ListUtils.New<double>(issue.Parameters.Nx + 1);

            var cx1 = issue.BaseExpression.Cx1;
            var cx2 = issue.BaseExpression.Cx2;
            var cy1 = issue.BaseExpression.Cy1;
            var cy2 = issue.BaseExpression.Cy2;
            var c = issue.BaseExpression.C;
            var hx = issue.Parameters.Hx;
            var hy = issue.Parameters.Hy;
            var tau = issue.Parameters.Tau;

            coefs[i - 1] = cx2 / (hx * hx) - cx1 / (2.0 * hx);
            coefs[i] = c - 2.0 * cx2 / (hx * hx) - 1.0 / (tau / 2.0);
            coefs[i + 1] = cx2 / (hx * hx) + cx1 / (2.0 * hx);


            /*
             * d2u/dt2 = alpha / hx2 * (
             * 
             * a[i] = alpha / hx**2
             * b[i] = -2 * alpha / hx**2 - 1 / (tau / 2.0)
             * c[i] = alpha / hx**2
             * 
             * d[i] = -u[i,j] / (tau / 2) - 
             * 
             * b c 0 0 0 0 | d
             * a b c 0 0 0 | d
             * 0 a b c 0 0 | d
             * 0 0 a b c 0 | d
             * 0 0 0 a b c | d
             * 0 0 0 0 a b | d
             */


            var x = hx * i;
            var y = hy * j;

            double res;
            // -0.0000070844
            res = -uPrev[j][i] / (tau / 2.0)
                - (uPrev[j + 1][i] - 2 * uPrev[j][i] + uPrev[j - 1][i]) * (cy2 / (hy * hy))
                - (uPrev[j + 1][i] - uPrev[j - 1][i]) * (cy1 / (2.0 * hy))
                - issue.BaseExpression.F(x, y, t + tau / 2.0);
            return new Equation(coefs, res);
        }

        // 2nd half of step
        private Solution CalculateIthColumn(Matrix previousStep, double t, int i)
        {
            var equations = new List<Equation>();
            equations.Add(issue.TopBorderExpression.Approximate1D(issue.Parameters, i, t));
            equations.AddRange(Enumerable.Range(1, issue.Parameters.Ny - 1).Select(j => GetJthEquationForI(previousStep, t, i, j)));
            equations.Add(issue.BottomBorderExpression.Approximate1D(issue.Parameters, i, t));

            var system = new LinearSystem(equations);
            return system.Solve();
        }

        private Equation GetJthEquationForI(Matrix uPrev, double t, int i, int j)
        {
            var coefs = ListUtils.New<double>(issue.Parameters.Ny + 1);

            var cx1 = issue.BaseExpression.Cx1;
            var cx2 = issue.BaseExpression.Cx2;
            var cy1 = issue.BaseExpression.Cy1;
            var cy2 = issue.BaseExpression.Cy2;
            var c = issue.BaseExpression.C;
            var hx = issue.Parameters.Hx;
            var hy = issue.Parameters.Hy;
            var tau = issue.Parameters.Tau;

            coefs[j - 1] = cy2 / (hy * hy) - cy1 / (2.0 * hy);
            coefs[j] = c - 2.0 * cy2 / (hy * hy) - 1.0 / (tau / 2.0);
            coefs[j + 1] = cy2 / (hy * hy) + cy1 / (2.0 * hy);

            var x = issue.Parameters.Hx * i;
            var y = issue.Parameters.Hy * j;

            var res = -uPrev[j][i] / (tau / 2.0)
                - (uPrev[j][i + 1] - 2 * uPrev[j][i] + uPrev[j][i - 1]) * (cx2 / (hx * hx))
                - (uPrev[j][i + 1] - uPrev[j][i - 1]) * (cx1 / (2.0 * hx))
                - issue.BaseExpression.F(x, y, t + tau / 2.0);

            return new Equation(coefs, res);
        }
    }
}
