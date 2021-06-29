using System;
using PPPQueuesSimulation.Interfaces;

namespace PPPQueuesSimulation.Structs.Constants
{
    public struct Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants : IRate
    {
        public const int Length = 12;
        public static readonly double ReferenceRate = 2.08e-11;
        public static readonly double Delta = 1.37e-05;
        public static readonly double ReferenceRateProbability = 0.01;
        public static readonly int _Frequency = 1;

        public double V3F { get; set; }
        public double V3R { get; set; }
        public double K_NADP { get; set; }
        public double Ki_NADP { get; set; }
        public double Ki_NADPH { get; set; }
        public double K_6PG { get; set; }
        public double K_CO2 { get; set; }
        public double K_OR5P { get; set; }
        public double K_NADPH { get; set; }
        public double Ki_6PG { get; set; }
        public double Ki_CO2 { get; set; }
        public double Ki_OR5P { get; set; }

        public bool IsReversible => false;

        public static string _Name => $"6PG + NADP = OR5P + NADPH + H + CO2 (rate3, frequency {_Frequency}, increment {Delta})";
        public string Name => _Name;

        public bool IsBalancingFlow => false;
        public int Frequency => Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants._Frequency;


        public Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Length of array: {array.Length} is not equal to desired length: {Length}");

            V3F = array[0];
            V3R = array[1];
            K_NADP = array[2];
            Ki_NADP = array[3];
            Ki_NADPH = array[4];
            K_6PG = array[5];
            K_CO2 = array[6];
            K_OR5P = array[7];
            K_NADPH = array[8];
            Ki_6PG = array[9];
            Ki_CO2 = array[10];
            Ki_OR5P = array[11];
        }

        public static Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants GetLiteratureConstants()
        {
            var consts = new Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants();
            consts.V3F = 4.93 * 1e-9 * 1e3;
            consts.V3R = 1.064 * 1e-16 * 1e3;
            consts.K_NADP = 1.35 * 1e-5 * 1e3;
            consts.Ki_NADP = 4.8 * 1e-6 * 1e3;
            consts.Ki_NADPH = 5.1 * 1e-6 * 1e3;
            consts.K_6PG = 2.92 * 1e-5 * 1e3;
            consts.K_CO2 = 3.4 * 1e-2 * 1e3;
            consts.K_OR5P = 2 * 1e-5 * 1e3;
            consts.K_NADPH = 2.2 * 1e-7 * 1e3;
            consts.Ki_6PG = 2.176 * 1e-3 * 1e3;
            consts.Ki_CO2 = 1.387 * 1e-5 * 1e3;
            consts.Ki_OR5P = 4.488 * 1e-11 * 1e3;

            return consts;
        }

        public double CalculateRate(double _6pg, double nadp, double co2, double or5p, double nadph)
        {
            var a = (V3F * _6pg * nadp) - ((V3R / V3F) * ((Ki_NADP * K_6PG) / (K_CO2 * Ki_OR5P * Ki_NADPH)) * co2 * or5p * nadph);//NUM

            var b = (Ki_NADP * K_6PG) + (K_6PG * nadp) + (K_NADP * _6pg) + (_6pg * nadp) + ((Ki_NADP * K_6PG * K_OR5P) / (K_CO2 * Ki_OR5P)) * co2 + (((Ki_NADP * K_6PG) / (Ki_NADPH * K_CO2 * Ki_OR5P)) * co2 * or5p * nadph) + (((K_6PG * K_OR5P) / (Ki_6PG * K_CO2 * Ki_OR5P)) * nadp * _6pg * co2) + (((Ki_NADP * K_6PG) / (Ki_OR5P * Ki_NADPH)) * or5p * nadph) + (((K_6PG * K_OR5P) / (Ki_OR5P * K_CO2)) * nadp * co2) + ((K_NADP / (Ki_OR5P * Ki_NADPH)) * _6pg * nadph * or5p) + (((K_6PG * Ki_NADP * K_NADPH) / (K_CO2 * Ki_OR5P * Ki_NADPH)) * or5p * co2) + (((Ki_NADP * K_6PG) / Ki_NADPH) * nadph) + ((K_NADP / Ki_NADPH) * _6pg * nadph) + (((Ki_NADP * K_6PG * K_OR5P) / (K_CO2 * Ki_OR5P * Ki_NADPH)) * co2 * nadph) + (((K_NADPH * K_6PG * Ki_CO2) / (Ki_6PG * K_CO2 * Ki_OR5P * Ki_NADPH)) * _6pg * nadp * or5p) + (((K_6PG * K_NADPH) / (K_CO2 * Ki_OR5P * Ki_NADPH)) * nadp * co2 * or5p) + (K_6PG * K_NADPH * Ki_6PG * K_CO2 * Ki_OR5P * Ki_NADPH) * (nadp * _6pg * co2 * or5p) + ((K_NADP / (Ki_CO2 * Ki_OR5P * Ki_NADPH)) * _6pg * co2 * or5p * nadph);//DENOM

            var V3 = a / b;
            return V3 / ReferenceRate * ReferenceRateProbability;
        }

        public double CalculateRate(PPPCellState state)
        {
            return CalculateRate(state._6GP, state.NADP, state.CO2, state.OR5P, state.NADPH);
        }

        /// <summary>
        /// This queue is non-reversible. If reaction is performed then 6PG and NADP are decremented and NADPH is incremented.
        /// </summary>
        /// <param name="currentState">Current amount of products and substrates in PPP</param>
        /// <param name="delta">The smallest increment of products</param>
        /// <param name="univariate_random_fn">Function for generating random number from 0 to 1</param>
        /// <returns>How current state has changed</returns>
        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn, bool modify_nadph_level = false, double rate3_inhibition = 0)
        {
            if (currentState._6GP <= delta || currentState.NADP <= delta)
                return new PPPCellState();

            var rate = CalculateRate(currentState._6GP, currentState.NADP, currentState.CO2, currentState.OR5P, currentState.NADPH);
            rate = rate * (1.0 - rate3_inhibition);
            if (univariate_random_fn() < rate)
            {
                var change = new PPPCellState();
                change._6GP = -1 * delta;
                change.NADP = -1 * delta;
                change.NADPH = modify_nadph_level ? delta : 0;
                change.OR5P = delta;
                return change;
            }
            else return new PPPCellState();
        }

        public PPPCellState Queue(PPPCellState currentState, double delta, Func<double> univariate_random_fn)
        {
            return Queue(currentState, delta, univariate_random_fn, true);
        }
        public Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var c = (Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants)this.ShallowCopy();
            double n() => 1 + noiseAmplitude * gaussian_random_fn();

            c.V3F *= n();
            c.V3R *= n();
            c.K_NADP *= n();
            c.Ki_NADP *= n();
            c.Ki_NADP *= n();
            c.K_6PG *= n();
            c.K_CO2 *= n();
            c.K_OR5P *= n();
            c.K_NADPH *= n();
            c.Ki_6PG *= n();
            c.Ki_CO2 *= n();
            c.Ki_OR5P *= n();
            return c;
        }   

        public double[] ToDoubleArray()
        {
            return new double[Length]
            {
                V3F,
                V3R,
                K_NADP,
                Ki_NADP,
                Ki_NADP,
                K_6PG,
                K_CO2,
                K_OR5P,
                K_NADPH,
                Ki_6PG,
                Ki_CO2,
                Ki_OR5P
            };
        }

        public IRate ShallowCopy()
        {
            return new Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants(this.ToDoubleArray());
        }

        public IRate ModifyWeights(Func<int, double, double> modification_fn)
        {
            var array = this.ToDoubleArray();
            for (int i = 0; i < array.Length; i++)
                array[i] = modification_fn(i, array[i]);

            return new Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants(array);
        }

        
    }
}
