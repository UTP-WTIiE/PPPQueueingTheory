using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct RateX5P_E4P__G3P_F6P_Constants : IDoubleArray, IRate
    {
        public const int Length = 21;
        public const double ReferenceRate = 1.52e-11;
        public const double Delta = 1e-5;
        public const double ReferenceRateProbability = 0.01;

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
        public double K20 { get; set; }
        public double K21 { get; set; }
        public double K22 { get; set; }
        public double K23 { get; set; }
        public double Ki_R5P { get; set; }
        public double Ki_X5P { get; set; }

        public bool IsReversible => true;

        public string Name => $"X5P + E4P = G3P + F6P (rate6, frequency {Frequency}, increment {Delta})";

        public bool IsBalancingFlow => false;
        public int Frequency => 1;


        public RateX5P_E4P__G3P_F6P_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            K5 = array[0];
            K6 = array[1];
            K7 = array[2];
            K8 = array[3];
            K9 = array[4];
            K10 = array[5];
            K11 = array[6];
            K12 = array[7];
            K13 = array[8];
            K14 = array[9];
            K15 = array[10];
            K16 = array[11];
            K17 = array[12];
            K18 = array[13];
            K19 = array[14];
            K20 = array[15];
            K21 = array[16];
            K22 = array[17];
            K23 = array[18];
            Ki_R5P = array[19];
            Ki_X5P = array[20];
        }

        public static RateX5P_E4P__G3P_F6P_Constants GetLiteratureConstants()
        {
            var consts = new RateX5P_E4P__G3P_F6P_Constants();
            consts.K5 = 1.09 * 1e-3 * 1e3;//K5 [M]
            consts.K6 = 3.2 * 1e-6 * 1e3;//K6 [M]
            consts.K7 = 1.55 * 1e-2 * 1e3;//K7 [M]
            consts.K8 = 3.8 * 1e-4 * 1e3;//K8 [M]
            consts.K9 = 1.548 * 1e-6 * 1e3;//K9 [M]
            consts.K10 = 3.8 * 1e-4 * 1e3;//K10 [M]
            consts.K11 = 1.267 * 1e3;//K11 [M]
            consts.K12 = 6.05 * 1e3;//K12 [M]
            consts.K13 = 1e-5 * 1e3;//K13 [M]
            consts.K14 = 1.0 * 1e3;//K14 [M]
            consts.K15 = 1e-5 * 1e3;//K15 [M]
            consts.K16 = 0.0086 * 1e3;//K16 [M]
            consts.K17 = 1.0 * 1e3;//K17 [M]
            consts.K18 = 86.4 * 1e3;//K18 [M]
            consts.K19 = 8.64 * 1e3;//K19 [M]
            consts.K20 = 5.9 * 1e-9 * 1e3;//K20 [M]
            consts.K21 = 2.2 * 1e-12 * 1e3;//K21 [M]
            consts.K22 = 3.802 * 1e-10 * 1e3;//K22 [M]
            consts.K23 = 5.9 * 1e-13 * 1e3;//K23 [M]
            consts.Ki_R5P = 0.82;//Ki(R5P) [mM]
            consts.Ki_X5P = 3.6;//Ki(X5P) [mM]


            return consts;
        }

        public double CalculateRate(double r5p, double x5p, double f6p, double s7p, double g3p, double e4p, double g6p)
        {
            

            var a = (K20 * x5p * e4p) + (K21 * s7p * e4p) - (K22 * f6p * g3p) - (K23 * f6p * r5p);//NUM6

            var b1 = K5 * s7p + K6 * g3p + K7 * f6p + K10 * e4p + K12 * s7p * g3p + K13 * s7p * e4p + K14 * r5p * x5p + (K18 * g3p * f6p) + (K19 * f6p * e4p);//Km
            var b2 = K8 + K11 * s7p + K15 * f6p;//K(R5P)
            var b3 = K9 + K16 * g3p + K17 * e4p;//K(X5P)
            var b = (b1 * (1 + (g6p / Ki_R5P)) * (1 + (g6p / Ki_X5P)) + (b2 * r5p * (1 + (g6p / Ki_R5P))) + (b3 * x5p * (1 + (g6p / Ki_X5P))));//DENOM6
            
            var V6 = a / b;

            return V6 / ReferenceRate * ReferenceRateProbability; ;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state.R5P, state.X5P, state.F6P, state.S7P, state.G3P, state.E4P, state.G6P);
        }

        /// <summary>
        /// This queue is reversible. If reaction is performed then X5P and E4P are decremented and G3P and F6P are incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn, bool keep_f6p_unchanged = false)
        {
            var rate = CalculateRate(currentState.R5P, currentState.X5P, currentState.F6P, currentState.S7P, currentState.G3P, currentState.E4P, currentState.G6P);
            var sign = rate < 0 ? -1.0 : 1.0;

            if (sign == 1.0 && (currentState.X5P <= delta || currentState.E4P <= delta))
                return new PPPCellState();
            if (sign == -1.0 && (currentState.G3P <= delta || currentState.F6P <= delta))
                return new PPPCellState();

            if (univariate_random_fn() < Math.Abs(rate))
                return new PPPCellState()
                {
                    X5P = -1 * sign * delta,
                    E4P = -1 * sign * delta,
                    G3P = sign * delta,
                    F6P = keep_f6p_unchanged == false ? sign * delta : 0
                };
            else return new PPPCellState();
        }
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            return Queue(currentState, delta, univariate_random_fn, false);
        }

        public RateX5P_E4P__G3P_F6P_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (RateX5P_E4P__G3P_F6P_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

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
            c.K20 *= n();
            c.K21 *= n();
            c.K22 *= n();
            c.K23 *= n();
            c.Ki_R5P *= n();
            c.Ki_X5P *= n();

            return c;
        }

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
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
                K20,
                K21,
                K22,
                K23,
                Ki_R5P,
                Ki_X5P
            };
        }

        public IRate ShallowCopy()
        {
            return new RateX5P_E4P__G3P_F6P_Constants(ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new RateX5P_E4P__G3P_F6P_Constants(array);
        }

        
    }
}
