using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RateOR5P__X5P_Constants : IDoubleArray, IRate
    {
        public const int Length = 4;
        public const double ReferenceRate = 5.1e-8;
        public const double Delta = 4.01e-5;
        public const double ReferenceRateProbability = 0.01;

        public double K_OR5P { get; set; }
        public double K_X5P { get; set; }
        public double V4BF { get; set; }
        public double V4BR { get; set; }

        public bool IsReversible => true;

        public string Name => $"OR5P = X5P (rate4b, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1000;


        public RateOR5P__X5P_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            K_OR5P = array[0];
            K_X5P = array[1];
            V4BF = array[2];
            V4BR = array[3];
        }

        public static RateOR5P__X5P_Constants GetLiteratureConstants()
        {
            var consts = new RateOR5P__X5P_Constants();
            consts.K_OR5P = 1.9 * 1e-4 * 1e3;
            consts.K_X5P = 5 * 1e-4 * 1e3;
            consts.V4BF = 5.9 * 1e-9 * 1e3;
            consts.V4BR = 8.48 * 1e-9 * 1e3;

            return consts;
        }

        public double CalculateRate(double or5p, double x5p)
        {
            var V4b = (V4BF * (or5p / K_OR5P) - (V4BR * (x5p / K_X5P))) / (1 + (or5p / K_OR5P) + (x5p / K_X5P));

            return V4b / ReferenceRate * ReferenceRateProbability; ;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.OR5P, state.X5P);
        }

        /// <summary>
        /// This queue is reversible. If reaction is performed then OR5P is decremented and X5P is incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            

            var rate = CalculateRate(currentState.OR5P, currentState.X5P);
            var sign = rate < 0 ? -1.0 : 1.0;

            if (sign == 1.0 && currentState.OR5P <= delta)
                return new PPPCellState();
            if (sign == -1.0 && currentState.X5P <= delta)
                return new PPPCellState();

            if (univariate_random_fn() < Math.Abs(rate))
                return new PPPCellState()
                {
                    OR5P = -1 * sign * delta,
                    X5P = sign * delta
                };
            else return new PPPCellState();
        }

        public RateOR5P__X5P_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RateOR5P__X5P_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.K_OR5P *= n();
            c.K_X5P *= n();
            c.V4BF *= n();
            c.V4BR *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                K_OR5P,
                K_X5P,
                V4BF,
                V4BR
            };
        }

        public IRate ShallowCopy()
        {
            return new RateOR5P__X5P_Constants(this.ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RateOR5P__X5P_Constants(array);
        }
    }
}
