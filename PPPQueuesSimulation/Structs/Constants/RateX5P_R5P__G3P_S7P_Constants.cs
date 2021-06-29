using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RateX5P_R5P__G3P_S7P_Constants : IDoubleArray, IRate
    {
        public const int Length = 20;
        public const double ReferenceRate = 2.34e-11;
        public const double Delta = 1.54e-5; 
        public const double ReferenceRateProbability = 0.01;

        public double K2 { get; set; }
        public double K3 { get; set; }
        public double K4 { get; set; }
        public double K5 { get; set; }
        public double K6 { get; set; }
        public double K7 { get; set; }
        public double K8 { get; set; }
        public double K9 { get; set; }
        public double K10 { get; set; }
        public double K11 { get; set; }
        public double K12 { get; set; }
        public double K13 { get; set; }
        public double K14 { get; set; }
        public double K15 { get; set; }
        public double K16 { get; set; }
        public double K17 { get; set; }
        public double K18 { get; set; }
        public double K19 { get; set; }
        public double Ki_R5P { get; set; }
        public double Ki_X5P { get; set; }

        public bool IsReversible => true;

        public string Name => $"X5P + R5P = G3P + S7P (rate5, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1;


        public RateX5P_R5P__G3P_S7P_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            K2 = array[0];
            K3 = array[1];
            K4 = array[2];
            K5 = array[3];
            K6 = array[4];
            K7 = array[5];
            K8 = array[6];
            K9 = array[7];
            K10 =array[8];
            K11 =array[9];
            K12 =array[10];
            K13 =array[11];
            K14 =array[12];
            K15 =array[13];
            K16 =array[14];
            K17 =array[15];
            K18 =array[16];
            K19 = array[17];
            Ki_R5P = array[18];
            Ki_X5P = array[19];
        }

        public static RateX5P_R5P__G3P_S7P_Constants GetLiteratureConstants()
        {
            var consts = new RateX5P_R5P__G3P_S7P_Constants();
            consts.K2 = 1.1 * 1e-12 * 1e3;//K2 [M]
            consts.K3 = 1.006 * 1e-8 * 1e3;//K3 [M]
            consts.K4 = 9.9 * 1e-13 * 1e3;//K4 [M]
            consts.K5 = 1.09 * 1e-3 * 1e3;//K5 [M]
            consts.K6 = 3.2 * 1e6 * 1e3;//K6 [M]
            consts.K7 = 1.55 * 1e-2 * 1e3;//K7 [M]
            consts.K8 = 3.8 * 1e-4 * 1e3;//K8 [M]
            consts.K9 = 1.548 * 1e-6 * 1e3;//K9 [M]
            consts.K10 = 3.8 * 1e-4 * 1e3;//K10 [M]
            consts.K11 = 1.267 * 1e3;//K11 [M]
            consts.K12 = 6.05 * 1e3;//K12 [M]
            consts.K13 = 1e-5 * 1e3;//K13 [M]
            consts.K14 = 1 * 1e3;//K14 [M]
            consts.K15 = 1e-5 * 1e3;//K15 [M]
            consts.K16 = 0.0086 * 1e3;//K16 [M]
            consts.K17 = 1 * 1e3;//K17 [M]
            consts.K18 = 86.4 * 1e3;//K18 [M]
            consts.K19 = 8.64 * 1e3;//K19 [M]
            consts.Ki_R5P = 0.82;//Ki(R5P) [mM]
            consts.Ki_X5P = 3.6;//Ki(X5P) [mM]


            return consts;
        }

        public double CalculateRate(double r5p, double x5p, double f6p, double s7p, double g3p, double e4p, double g6p)
        {
            var a = (K5 * r5p * x5p) + (K2 * f6p * r5p) - (K3 * s7p * g3p) - (K4 * s7p * e4p);//NUM5

            var b1 = K5 * s7p + K6 * g3p + K7 * f6p + K10 * e4p + K12 * s7p * g3p + K13 * s7p * e4p + K14 * r5p * x5p + K18 * g3p * f6p + K19 * f6p * e4p;//Km
            var b2 = K8 + K11 * s7p + K15 * f6p;//K(R5P)
            var b3 = K9 + K16 * g3p + K17 * e4p;//K(X5P)

            var b = (b1 * (1 + (g6p / Ki_R5P)) * (1 + (g6p / Ki_X5P)) + (b2 * r5p * (1 + (g6p / Ki_R5P))) + (b3 * x5p * (1 + (g6p / Ki_X5P))));//DENOM5

            var V5 = a / b;

            return V5 / ReferenceRate * ReferenceRateProbability;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.R5P, state.X5P, state.F6P, state.S7P, state.G3P, state.E4P, state.G6P);
        }

        /// <summary>
        /// This queue is reversible. If reaction is performed then X5P and R5P are decremented and G3P and S7P is incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            var rate = CalculateRate(currentState.R5P, currentState.X5P, currentState.F6P, currentState.S7P, currentState.G3P, currentState.E4P, currentState.G6P);
            var sign = rate < 0 ? -1.0 : 1.0;

            if (sign == 1.0 && (currentState.X5P <= delta || currentState.R5P <= delta))
                return new PPPCellState();

            if (sign == -1.0 && (currentState.G3P <= delta || currentState.S7P <= delta))
                return new PPPCellState();

            if (univariate_random_fn() < Math.Abs(rate))
                return new PPPCellState()
                {
                    X5P = -1 * delta * sign,
                    R5P = -1 * sign * delta,
                    G3P = sign * delta,
                    S7P = sign * delta
                };
            return new PPPCellState();
        }

        public RateX5P_R5P__G3P_S7P_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RateX5P_R5P__G3P_S7P_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.K2 *= n();
            c.K3 *= n();
            c.K4 *= n();
            c.K5 *= n();
            c.K6 *= n();
            c.K7 *= n();
            c.K8 *= n();
            c.K9 *= n();
            c.K10 *= n();
            c.K11 *= n();
            c.K12 *= n();
            c.K13 *= n();
            c.K14 *= n();
            c.K15 *= n();
            c.K16 *= n();
            c.K17 *= n();
            c.K18 *= n();
            c.K19 *= n();
            c.Ki_R5P *= n();
            c.Ki_X5P *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                K2,
                K3,
                K4,
                K5,
                K6,
                K7,
                K8,
                K9,
                K10,
                K11,
                K12,
                K13,
                K14,
                K15,
                K16,
                K17,
                K18,
                K19,
                Ki_R5P,
                Ki_X5P
            };
        }

        public IRate ShallowCopy()
        {
            return new RateX5P_R5P__G3P_S7P_Constants(ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RateX5P_R5P__G3P_S7P_Constants(array);
        }
    }
}
