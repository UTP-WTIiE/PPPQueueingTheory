using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RatePGL_H20__6PG_H_Constants : IDoubleArray, IRate
    {
        public const int Length = 4;
        public const double ReferenceRate = 7.5e-11;
        public const double Delta = 4.93e-06;
        public const double ReferenceRateProbability = 0.01;

        public double K_PGL { get; set; }
        public double K_6PG { get; set; }
        public double Vf { get; set; }
        public double Vr { get; set; }

        public bool IsReversible => false;

        public string Name => $"PGL + H20 = 6PG + H (rate2, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1;


        public RatePGL_H20__6PG_H_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            K_PGL = array[0];
            K_6PG = array[1];
            Vf = array[2];
            Vr = array[3];
        }

        public static RatePGL_H20__6PG_H_Constants GetLiteratureConstants()
        {
            var consts = new RatePGL_H20__6PG_H_Constants();
            consts.K_PGL = 8 * 1e-5 * 1e3;
            consts.K_6PG = 8 * 1e-5 * 1e3;
            consts.Vf = 5.9 * 1e-9 * 1e3;
            consts.Vr = 1.232 * 1e-12 * 1e3;

            return consts;
        }

        public double CalculateRate(double pgl, double _6pg)
        {
            var V4a = (Vf * (pgl / K_PGL) - (Vr * (_6pg / K_6PG))) / (1 + (pgl / K_PGL) + (_6pg / K_6PG));

            return V4a / ReferenceRate * ReferenceRateProbability; ;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.PGL, state._6GP);
        }

        /// <summary>
        /// This queue is non-reversible. If reaction is performed then PGL is decremented and 6PH is incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            if (currentState.PGL <= delta)
                return new PPPCellState();

            var rate = CalculateRate(currentState.PGL, currentState._6GP);
            if (univariate_random_fn() < rate)
                return new PPPCellState()
                {
                    PGL = -1 * delta,
                    _6GP = delta
                };
            else return new PPPCellState();
        }

        public RatePGL_H20__6PG_H_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RatePGL_H20__6PG_H_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.K_PGL *= n(); 
            c.K_6PG *= n(); 
            c.Vf *= n(); 
            c.Vr *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                K_PGL, K_6PG, Vf, Vr
            };
        }

        public IRate ShallowCopy()
        {
            return new RatePGL_H20__6PG_H_Constants(ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RatePGL_H20__6PG_H_Constants(array);
        }
    }
}
