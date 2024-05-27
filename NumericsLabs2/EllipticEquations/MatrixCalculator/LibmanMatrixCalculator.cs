using System.Collections.Generic;
using Row = System.Collections.Generic.List<double>;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;

namespace Numerics7
{
    internal class LibmanMatrixCalculator : IterationMethod, IMatrixCalculator
    {
        public LibmanMatrixCalculator(Issue issue) : base(issue) { }

        protected override Matrix CalculateIteration(Matrix prevMatrix)
        {
            var nextMatrix = ListUtils.New<Row>(prevMatrix.Count);

            var cx1 = issue.BaseExpression.Cx1;
            var cx2 = issue.BaseExpression.Cx2;
            var cy1 = issue.BaseExpression.Cy1;
            var cy2 = issue.BaseExpression.Cy2;
            var c = issue.BaseExpression.C;
            var hx = issue.Parameters.Hx;
            var hy = issue.Parameters.Hy;
            var Nx = issue.Parameters.Nx;
            var Ny = issue.Parameters.Ny;

            for (int j = 0; j < prevMatrix.Count; j++)
            {
                nextMatrix[j] = ListUtils.New<double>(prevMatrix[0].Count);
            }

            for (int j = 1; j < prevMatrix.Count - 1; j++)
            {
                for (int i = 1; i < prevMatrix[j].Count - 1; i++)
                {
                    var leftKoef = (cx2 / (hx * hx)) + (cx1 / (2.0 * hx));
                    var topKoef = (cy2 / (hy * hy)) + (cy1 / (2.0 * hy));
                    var bottomKoef = (cy2 / (hy * hy)) - (cy1 / (2.0 * hy));
                    var rightKoef = (cx2 / (hx * hx)) - (cx1 / (2.0 * hx));
                    var centerKoef = (2.0 * cx2 / (hx * hx)) + (2.0 * cy2 / (hy * hy)) + c;

                    var x = i * hx;
                    var y = j * hy;

                    var usum = leftKoef * prevMatrix[j][i - 1] + rightKoef * prevMatrix[j][i + 1] + topKoef * prevMatrix[j - 1][i] + bottomKoef * prevMatrix[j + 1][i];

                    nextMatrix[j][i] = (usum - issue.BaseExpression.F(x, y)) / centerKoef; 
                }
            }

            for (int xIndex = 1; xIndex < issue.Parameters.Nx; xIndex++)
            {
                var topApproximation = issue.TopBorderExpression.Approximate1D(issue.Parameters, xIndex);
                var bottomApproximation = issue.BottomBorderExpression.Approximate1D(issue.Parameters, xIndex);

                nextMatrix[0][xIndex] = (topApproximation.Result - nextMatrix[1][xIndex] * topApproximation.Coefs[1]) / topApproximation.Coefs[0];
                nextMatrix[Ny][xIndex] = (bottomApproximation.Result - nextMatrix[Ny - 1][xIndex] * bottomApproximation.Coefs[Ny - 1]) / bottomApproximation.Coefs[Ny];
            }

            for (int yIndex = 1; yIndex < issue.Parameters.Ny; yIndex++)
            {
                var leftApproximation = issue.LeftBorderExpression.Approximate1D(issue.Parameters, yIndex);
                var rightApproximation = issue.RightBorderExpression.Approximate1D(issue.Parameters, yIndex);

                nextMatrix[yIndex][0] = (leftApproximation.Result - nextMatrix[yIndex][1] * leftApproximation.Coefs[1]) / leftApproximation.Coefs[0];
                nextMatrix[yIndex][Nx] = (rightApproximation.Result - nextMatrix[yIndex][Nx - 1] * rightApproximation.Coefs[Nx - 1]) / rightApproximation.Coefs[Nx];
            }


            var topleftApproximation = issue.TopBorderExpression.Approximate1D(issue.Parameters, 0);
            var toprightApproximation = issue.TopBorderExpression.Approximate1D(issue.Parameters, Nx);
            var bottomleftApproximation = issue.BottomBorderExpression.Approximate1D(issue.Parameters, 0);
            var bottomrightApproximation = issue.BottomBorderExpression.Approximate1D(issue.Parameters, Nx);

            nextMatrix[0][0] = (topleftApproximation.Result - nextMatrix[1][0] * topleftApproximation.Coefs[1]) / topleftApproximation.Coefs[0];
            nextMatrix[0][Nx] = (toprightApproximation.Result - nextMatrix[1][Nx] * toprightApproximation.Coefs[1]) / toprightApproximation.Coefs[0];
            nextMatrix[Ny][0] = (bottomleftApproximation.Result - nextMatrix[Ny - 1][0] * bottomleftApproximation.Coefs[Ny - 1]) / bottomleftApproximation.Coefs[Ny];
            nextMatrix[Ny][Nx] = (bottomrightApproximation.Result - nextMatrix[Ny - 1][Nx] * bottomrightApproximation.Coefs[Ny - 1]) / bottomrightApproximation.Coefs[Ny];

            return nextMatrix;
        }
    }
}
