using PPPQueuesSimulation.Interfaces;
using PPPQueuesSimulation.Structs.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPPQueuesSimulation.Structs.BalancingFlows
{
    public struct G3PBalancingFlow : IDoubleArray, IRate
    {
        public const double ReferenceProbabilityRate = 0.01;
        public static double Delta = Math.Min(RateS7P_G3P__E4P_F6P_Constants.Delta, Math.Min(RateX5P_E4P__G3P_F6P_Constants.Delta, RateX5P_R5P__G3P_S7P_Constants.Delta));
        public const int Length = 1;

        public double C0 { get; set; }
        public bool IsReversible => false;

        public string Name => $"G3P Balance";

        public bool IsBalancingFlow => true;
        public int Frequency => 1;


        public G3PBalancingFlow(double[] array)
        {
            C0 = array[0];
        }

        public static G3PBalancingFlow GetLiteratureConstants()
        {
            var c = new G3PBalancingFlow();
            var p = PPPCellState.Default().G3P;
            c.C0 = ReferenceProbabilityRate / p;

            return c;
        }

        public double CalculateRate(PPPCellState state)
        {
            return state.G3P * C0;
        }

        public G3PBalancingFlow ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (G3PBalancingFlow)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.C0 *= n();

            return c;
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new G3PBalancingFlow(array);
        }

        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            if (currentState.G3P <= delta)
                return new PPPCellState();

            var rate = CalculateRate(currentState);
            if (univariate_random_fn() < rate)
            {
                return new PPPCellState()
                {
                    G3P = -1 * delta
                };
            }
            else return new PPPCellState();
        }

        public IRate ShallowCopy()
        {
            return new G3PBalancingFlow(this.ToDoubleArray());
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                C0
            };
        }
    }
}
