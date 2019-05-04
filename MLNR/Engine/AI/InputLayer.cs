using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

namespace MLNR.Engine.AI
{
    class InputLayer : Layer
    {
        public Matrix<double> WeightsOutput { get; set; }

        public InputLayer(int totalNodes, int totalOutputNodes) : base(0, totalNodes, totalOutputNodes)
        {
            WeightsOutput = Matrix<double>.Build.Dense(totalOutputNodes, TotalNodes, (i, j) => RandomHelper.Range(-1, 1));
        }

        public override void Mutate(double probability, double standardDeviation)
        {
            WeightsOutput = WeightsOutput.Add(RandomHelper.Gaussian(0, standardDeviation));
        }

        public void TweakWeightsBy(Matrix<double> delta)
        {
            WeightsOutput = WeightsOutput.Add(delta);
        }
    }
}
