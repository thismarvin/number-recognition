using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

namespace MLNR.Engine.AI
{
    class OutputLayer : Layer
    {
        public Matrix<double> WeightsInput { get; set; }
        public Matrix<double> Bias { get; set; }

        public OutputLayer(Matrix<double> weightsInput, int totalNodes) : base(weightsInput.RowCount, totalNodes, 0)
        {
            WeightsInput = weightsInput;

            Bias = Matrix<double>.Build.Dense(TotalNodes, 1, (i, j) => RandomHelper.Range(-1, 1));
        }

        public void UpdateReference(Matrix<double> weightsInput)
        {
            WeightsInput = weightsInput;
        }

        public override void Mutate(double probability, double standardDeviation)
        {
            Bias = Bias.Add(RandomHelper.Gaussian(0, standardDeviation));
        }

        public void AddBias()
        {
            Nodes = Nodes.Add(Bias);
        }

        public void TweakBiasBy(Matrix<double> gradients)
        {
            Bias = Bias.Add(gradients);
        }
    }
}
