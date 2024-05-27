namespace Wavelet.Models.Wavelets
{
    internal interface IWavelet
    {
        double[] Cs { get; }

        double[]? Ds { get; }

        double[,]? Scalegramm { get; }

        double[] Xs { get; }

        string Name { get; }

        (double[] C, double[] D) Squeeze();

        double[] ExtendWithNoise();

        double[] ExtendWithoutNoise();
    }
}
