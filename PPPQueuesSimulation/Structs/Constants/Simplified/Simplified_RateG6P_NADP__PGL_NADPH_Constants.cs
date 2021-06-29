using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct Simplified_RateG6P_NADP__PGL_NADPH_Constants : IDoubleArray, IRate
    {
        public const int Length = 3;
        public static double ReferenceRate = 3.96e-7;
        public const double Delta = 2.3e-6; //TAKE A LOOK ON IT
        public const double ReferenceRateProbability = 0.01;

        public double Vf { get; set; }
        public double K_G6P { get; set; }
        public double K_PGL { get; set; }

        public bool IsReversible => false;

        public string Name => $"G6P + NADP = PGL + NADPH (rate1, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1000;


        public Simplified_RateG6P_NADP__PGL_NADPH_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            Vf = array[0];
            K_G6P = array[1];
            K_PGL = array[2];
        }

        public static Simplified_RateG6P_NADP__PGL_NADPH_Constants GetLiteratureConstants()
        {
            var consts = new Simplified_RateG6P_NADP__PGL_NADPH_Constants();
            consts.Vf = 5.9 * 1e-9 * 1e3;
            consts.K_PGL = 4.8 * 1e-6 * 1e3;
            consts.K_G6P = 3.6 * 1e-5 * 1e3;

            return consts;
        }

        public double CalculateRate(double g6p, double pgl)
        {
            var i = Vf * g6p / K_G6P - 0.01 * Vf * pgl / K_PGL;
            var m = 1 + g6p / K_G6P + pgl / K_PGL;
            var V1 = i / m;

            return V1 / ReferenceRate * ReferenceRateProbability;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.G6P, state.PGL);
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

            var rate = CalculateRate(currentState);

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

        public Simplified_RateG6P_NADP__PGL_NADPH_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (Simplified_RateG6P_NADP__PGL_NADPH_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.Vf *= n(); 
            c.K_G6P *= n(); 
            c.K_PGL *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                Vf,K_G6P, K_PGL
            };
        }

        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            return Queue(currentState, delta, univariate_random_fn, true);
        }

        public IRate ShallowCopy()
        {
            return new Simplified_RateG6P_NADP__PGL_NADPH_Constants(this.ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new Simplified_RateG6P_NADP__PGL_NADPH_Constants(array);
        }
    }
}
