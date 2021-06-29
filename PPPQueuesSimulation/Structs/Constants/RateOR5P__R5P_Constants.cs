using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RateOR5P__R5P_Constants : IDoubleArray, IRate
    {
        public const int Length = 4;
        public const double ReferenceRate = 4.4e-8;
        public const double Delta = 2.89e-5;
        public const double ReferenceRateProbability = 0.01;

        public double K_OR5P { get; set; }
        public double K_R5P { get; set; }
        public double V4AF { get; set; }
        public double V4AR { get; set; }

        public bool IsReversible => true;

        public string Name => $"OR5P = R5P (rate4a, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1000;


        public RateOR5P__R5P_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            K_OR5P = array[0];
            K_R5P = array[1];
            V4AF = array[2];
            V4AR = array[3];
        }

        public static RateOR5P__R5P_Constants GetLiteratureConstants()
        {
            var consts = new RateOR5P__R5P_Constants();

            consts.K_OR5P = 7.8 * 1e-4 * 1e3;
            consts.K_R5P = 2.2 * 1e-3 * 1e3;
            consts.V4AF = 5.9 * 1e-9 * 1e3;
            consts.V4AR = 1.1225 * 1e-8 * 1e3;

            return consts;
        }

        public double CalculateRate(double or5p, double r5p)
        {
            var V4a = (V4AF * (or5p / K_OR5P) - (V4AR * (r5p / K_R5P))) / (1 + (or5p / K_OR5P) + (r5p / K_R5P));

            return V4a / ReferenceRate * ReferenceRateProbability; ;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.OR5P, state.R5P);
        }

        /// <summary>
        /// This queue is reversible. If reaction is performed then OR5P is decremented and R5P is incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            var rate = CalculateRate(currentState.OR5P, currentState.R5P);
            var sign = rate < 0 ? -1.0 : 1.0;
            
            if (sign == 1.0 && currentState.OR5P <= delta)
                return new PPPCellState();
            if (sign == -1.0 && currentState.R5P <= delta)
                return new PPPCellState();

            if (univariate_random_fn() < Math.Abs(rate))
                return new PPPCellState()
                {
                    OR5P = -1 * delta * sign,
                    R5P = delta * sign
                };
            else return new PPPCellState();
        }

        public RateOR5P__R5P_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RateOR5P__R5P_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.K_OR5P *= n();
            c.K_R5P *= n();
            c.V4AF *= n();
            c.V4AR *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                K_OR5P,
                K_R5P,
                V4AF,
                V4AR
            };
        }

        public IRate ShallowCopy()
        {
            return new RateOR5P__R5P_Constants(this.ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RateOR5P__R5P_Constants(array);
        }
    }
}
