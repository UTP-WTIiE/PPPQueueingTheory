using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RateS7P_G3P__E4P_F6P_Constants : IDoubleArray, IRate
    {
        public const int Length = 8;
        public const double ReferenceRate = 7.64e-8;
        public const double Delta = 5e-5;
        public const double ReferenceRateProbability = 0.01 * 10; //artificial amplitude increase for test purposes

        public double V7F { get; set; }
        public double V7R { get; set; }
        public double K_S7P { get; set; }
        public double K_G3P { get; set; }
        public double K_F6P { get; set; }
        public double Ki_S7P { get; set; }
        public double Ki_F6P { get; set; }
        public double Ki_E4P { get; set; }

        public bool IsReversible => true;

        public string Name => $"S7P + G3P = E4P + F6P (rate7, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1000;


        public RateS7P_G3P__E4P_F6P_Constants(double[] array)
        {
            V7F = array[0];
            V7R = array[1];
            K_S7P = array[2];
            K_G3P = array[3];
            K_F6P = array[4];
            Ki_S7P = array[5];
            Ki_F6P = array[6];
            Ki_E4P = array[7];
        }

        public static RateS7P_G3P__E4P_F6P_Constants GetLiteratureConstants()
        {
            var consts = new RateS7P_G3P__E4P_F6P_Constants();

            consts.V7F = 5.9 * 1e-9 * 1e3;//V7F
            consts.V7R = 1.776 * 1e-9 * 1e3;//V7R
            consts.K_S7P = 1.8 * 1e-4 * 1e3;//K(S7P) [M]
            consts.K_G3P = 2.2 * 1e-4 * 1e3;//K(G3P) [M]
            consts.K_F6P = 2 * 1e-4 * 1e3;//K(F6P) [M]
            consts.Ki_S7P = 1.8 * 1e-4 * 1e3;//Ki(S7P) [M]
            consts.Ki_F6P = 2 * 1e-4 * 1e3;//Ki(F6P) [M]
            consts.Ki_E4P = 7 * 1e-6 * 1e3;//Ki(E4P) [M]

            return consts;
        }

        public double CalculateRate(double s7p, double g3p, double e4p, double f6p)
        {
            
            var a = V7F * ((s7p * g3p) - ((V7F / V7R) * ((Ki_S7P * K_G3P) / (K_F6P * Ki_E4P)) * (e4p * f6p)))     ;//NUM7
            var b = (K_G3P * s7p) + (K_S7P * g3p) + s7p * g3p + (((Ki_S7P * K_G3P) / Ki_E4P) * e4p) + (((Ki_S7P * K_G3P) / (Ki_E4P * K_F6P)) * f6p) + ((K_G3P / Ki_E4P) * s7p * e4p) + ((K_S7P / Ki_F6P) * g3p * f6p)     ;//DENOM7
            var V7 = a / b;

            return V7 / ReferenceRate * ReferenceRateProbability;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.S7P, state.G3P, state.E4P, state.F6P);
        }

        /// <summary>
        /// This queue is reversible. If reaction is performed then S7P and G3P are decremented and E4P and F6P are incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn, bool keep_f6p_unchanged = false)
        {
            var rate = CalculateRate(currentState.S7P, currentState.G3P, currentState.E4P, currentState.F6P);
            var sign = rate < 0 ? -1.0 : 1.0;

            if (sign == 1.0 && (currentState.S7P <= delta || currentState.G3P <= delta))
                return new PPPCellState();

            if (sign == -1.0 && (currentState.E4P <= delta || currentState.F6P <= delta))
                return new PPPCellState();

            if (univariate_random_fn() < Math.Abs(rate))
                return new PPPCellState()
                {
                    S7P = -1 * sign * delta,
                    G3P = -1 * sign * delta,
                    E4P = sign * delta,
                    F6P = keep_f6p_unchanged == false ? sign * delta : 0
                };
            return new PPPCellState();
        }

        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            return Queue(currentState, delta, univariate_random_fn, false);
        }

        public RateS7P_G3P__E4P_F6P_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RateS7P_G3P__E4P_F6P_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.V7F *= n();
            c.V7R *= n();
            c.K_S7P *= n();
            c.K_G3P *= n();
            c.K_F6P *= n();
            c.Ki_S7P *= n();
            c.Ki_F6P *= n();
            c.Ki_E4P *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                V7F,
                V7R,
                K_S7P,
                K_G3P,
                K_F6P,
                Ki_S7P,
                Ki_F6P,
                Ki_E4P
            };
        }

        public IRate ShallowCopy()
        {
            return new RateS7P_G3P__E4P_F6P_Constants(ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RateS7P_G3P__E4P_F6P_Constants(array);
        }

        
    }
}
