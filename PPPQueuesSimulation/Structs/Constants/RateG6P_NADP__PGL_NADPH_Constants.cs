using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RateG6P_NADP__PGL_NADPH_Constants : IDoubleArray, IRate
    {
        public const int Length = 5;
        public const double ReferenceRate = 3.5e-8;
        public const double Delta = 2.3e-5;
        public const double ReferenceRateProbability = 0.01;

        public double Vf { get; set; }
        public double K_NADP { get; set; }
        public double K_G6P { get; set; }
        public double Ki_NADP { get; set; }
        public double Ki_NADPH { get; set; }

        public bool IsReversible => false;

        public string Name => $"G6P + NADP = PGL + NADPH (rate1, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1000;


        public RateG6P_NADP__PGL_NADPH_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            Vf = array[0];
            K_NADP = array[1];
            K_G6P = array[2];
            Ki_NADP = array[3];
            Ki_NADPH = array[4];
        }

        public static RateG6P_NADP__PGL_NADPH_Constants GetLiteratureConstants()
        {
            var consts = new RateG6P_NADP__PGL_NADPH_Constants();
            consts.Vf = 5.9 * 1e-9 * 1e3;
            consts.K_NADP = 4.8 * 1e-6 * 1e3;
            consts.K_G6P = 3.6 * 1e-5 * 1e3;
            consts.Ki_NADP = 9 * 1e-6 * 1e3;
            consts.Ki_NADPH = 1.1 * 1e-6 * 1e3;

            return consts;
        }

        public double CalculateRate(double nadp, double g6p, double nadph)
        { 
            var i = (Ki_NADP * K_G6P) + (K_G6P * nadp) + (K_NADP * g6p) + (nadp * g6p) + (((K_G6P * Ki_NADP) / Ki_NADPH) * nadph) + ((Ki_NADP / Ki_NADPH) * g6p * nadph);
            var V1 = (Vf * nadp * g6p) / i;

            return V1 / ReferenceRate * ReferenceRateProbability;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.NADP, state.G6P, state.NADPH);
        }

        /// <summary>
        /// This queue is non-reversible. If reaction is performed then G6P and NADP are decremented and PGL and NADPH are incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn, bool modify_start_products = true, bool modify_nadph_level = false)
        {
            if (currentState.G6P <= delta || currentState.NADP <= delta)
                return new PPPCellState();

            double keep = modify_start_products ? 1 : 0;

            var rate = CalculateRate(currentState.NADP, currentState.G6P, currentState.NADPH);

            if (univariate_random_fn() < rate)
            {
                var change = new PPPCellState();
                change.G6P = keep * -1 * delta;
                change.NADP = keep * -1 * delta;
                change.PGL = delta;
                change.NADPH = modify_nadph_level ? delta : 0;

                return change;
            }
            else return new PPPCellState();
        }

        public RateG6P_NADP__PGL_NADPH_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RateG6P_NADP__PGL_NADPH_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.Vf *= n(); 
            c.K_NADP *= n(); 
            c.K_G6P *= n(); 
            c.Ki_NADP *= n();
            c.Ki_NADPH *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                Vf, K_NADP, K_G6P, Ki_NADP, Ki_NADPH
            };
        }

        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            return Queue(currentState, delta, univariate_random_fn, true);
        }

        public IRate ShallowCopy()
        {
            return new RateG6P_NADP__PGL_NADPH_Constants(this.ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RateG6P_NADP__PGL_NADPH_Constants(array);
        }
    }
}
