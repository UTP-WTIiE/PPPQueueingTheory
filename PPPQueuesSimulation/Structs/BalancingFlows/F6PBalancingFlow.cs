using PPPQueuesSimulation.Interfaces;
using PPPQueuesSimulation.Structs.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPPQueuesSimulation.Structs.BalancingFlows
{
    public struct F6PBalancingFlow : IDoubleArray, IRate
    {
        public const double ReferenceProbabilityRate = 0.01;
        public static double Delta = Math.Min(RateS7P_G3P__E4P_F6P_Constants.Delta, RateX5P_E4P__G3P_F6P_Constants.Delta);
        public const int Length = 1;

        public double C0 { get; set; }
        public bool IsReversible => false;

        public string Name => $"F6P Balance";

        public bool IsBalancingFlow => true;
        public int Frequency => 1;


        public F6PBalancingFlow(double[] array)
        {
            C0 = array[0];
        }

        public static F6PBalancingFlow GetLiteratureConstants()
        {
            var c = new F6PBalancingFlow();
            var p = PPPCellState.Default().F6P;
            c.C0 = ReferenceProbabilityRate / p;

            return c;
        }

        public F6PBalancingFlow ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (F6PBalancingFlow)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.C0 *= n();

            return c;
        }

        public double CalculateRate(PPPCellState state)
        {
            return state.F6P * C0;
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new F6PBalancingFlow(array);
        }

        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            if (currentState.F6P <= delta)
                return new PPPCellState();

            var rate = CalculateRate(currentState);
            if (univariate_random_fn() < rate)
            {
                return new PPPCellState()
                {
                    F6P = -1 * delta
                };
            }
            else return new PPPCellState();
        }

        public IRate ShallowCopy()
        {
            return new F6PBalancingFlow(this.ToDoubleArray());
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
