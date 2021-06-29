using PPPQueuesSimulation.Interfaces;
using PPPQueuesSimulation.Structs.BalancingFlows;
using PPPQueuesSimulation.Structs.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPPQueuesSimulation.Structs
{
    public class PPPConstants
    {
        public Simplified_RateG6P_NADP__PGL_NADPH_Constants rate1_simplified;
        public RatePGL_H20__6PG_H_Constants rate2;
        public Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants rate3;
        public RateOR5P__R5P_Constants rate4a;
        public RateOR5P__X5P_Constants rate4b;
        public RateX5P_R5P__G3P_S7P_Constants rate5;
        public RateX5P_E4P__G3P_F6P_Constants rate6;
        public RateS7P_G3P__E4P_F6P_Constants rate7;

        public G3PBalancingFlow g3p_balance;
        public F6PBalancingFlow f6p_balance;
        public E4PBalancingFlow e4p_balance;
        public G6PBalancingFlow g6p_balance;
        public R5PBalancingFlow r5p_balance;
        public S7PBalancingFlow s7p_balance;
        //public NadphBalancingFlow nadph_balance;

        public static readonly int Length = Simplified_RateG6P_NADP__PGL_NADPH_Constants.Length
            + RatePGL_H20__6PG_H_Constants.Length
            + Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants.Length
            + RateOR5P__R5P_Constants.Length
            + RateOR5P__X5P_Constants.Length
            + RateX5P_R5P__G3P_S7P_Constants.Length
            + RateX5P_E4P__G3P_F6P_Constants.Length
            + RateS7P_G3P__E4P_F6P_Constants.Length
            + G3PBalancingFlow.Length
            + F6PBalancingFlow.Length
            + E4PBalancingFlow.Length
            + G6PBalancingFlow.Length
            + R5PBalancingFlow.Length
            + S7PBalancingFlow.Length;
            //+ NadphBalancingFlow.Length;

        public static PPPConstants GetLiteratureConstants()
        {
            List<double[]> arrays = new List<double[]>()
            {
                Simplified_RateG6P_NADP__PGL_NADPH_Constants.GetLiteratureConstants().ToDoubleArray(),
                RatePGL_H20__6PG_H_Constants.GetLiteratureConstants().ToDoubleArray(),
                Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants.GetLiteratureConstants().ToDoubleArray(),
                RateOR5P__R5P_Constants.GetLiteratureConstants().ToDoubleArray(),
                RateOR5P__X5P_Constants.GetLiteratureConstants().ToDoubleArray(),
                RateX5P_R5P__G3P_S7P_Constants.GetLiteratureConstants().ToDoubleArray(),
                RateX5P_E4P__G3P_F6P_Constants.GetLiteratureConstants().ToDoubleArray(),
                RateS7P_G3P__E4P_F6P_Constants.GetLiteratureConstants().ToDoubleArray(),
                G3PBalancingFlow.GetLiteratureConstants().ToDoubleArray(),
                F6PBalancingFlow.GetLiteratureConstants().ToDoubleArray(),
                E4PBalancingFlow.GetLiteratureConstants().ToDoubleArray(),
                G6PBalancingFlow.GetLiteratureConstants().ToDoubleArray(),
                R5PBalancingFlow.GetLiteratureConstants().ToDoubleArray(),
                S7PBalancingFlow.GetLiteratureConstants().ToDoubleArray()
                //NadphBalancingFlow.GetLiteratureConstants().ToDoubleArray()
            };

            var array = arrays.SelectMany(x => x).ToArray();
            return new PPPConstants(array);
        }

        public PPPConstants()
        {

        }
        public PPPConstants(double[] array)
        {
            if (array.Length != Length)
                throw new ArgumentException($"Array length: {array.Length} is not equal to desired length: {Length}");

            var start = 0;
            rate1_simplified = new Simplified_RateG6P_NADP__PGL_NADPH_Constants(array.Take(Simplified_RateG6P_NADP__PGL_NADPH_Constants.Length).ToArray());
            start += Simplified_RateG6P_NADP__PGL_NADPH_Constants.Length;
            rate2 = new RatePGL_H20__6PG_H_Constants(array.Skip(start).Take(RatePGL_H20__6PG_H_Constants.Length).ToArray());
            start += RatePGL_H20__6PG_H_Constants.Length;
            rate3 = new Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants(array.Skip(start).Take(Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants.Length).ToArray());
            start += Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants.Length;
            rate4a = new RateOR5P__R5P_Constants(array.Skip(start).Take(RateOR5P__R5P_Constants.Length).ToArray());
            start += RateOR5P__R5P_Constants.Length;
            rate4b = new RateOR5P__X5P_Constants(array.Skip(start).Take(RateOR5P__X5P_Constants.Length).ToArray());
            start += RateOR5P__X5P_Constants.Length;
            rate5 = new RateX5P_R5P__G3P_S7P_Constants(array.Skip(start).Take(RateX5P_R5P__G3P_S7P_Constants.Length).ToArray());
            start += RateX5P_R5P__G3P_S7P_Constants.Length;
            rate6 = new RateX5P_E4P__G3P_F6P_Constants(array.Skip(start).Take(RateX5P_E4P__G3P_F6P_Constants.Length).ToArray());
            start += RateX5P_E4P__G3P_F6P_Constants.Length;
            rate7 = new RateS7P_G3P__E4P_F6P_Constants(array.Skip(start).Take(RateS7P_G3P__E4P_F6P_Constants.Length).ToArray());
            start += RateS7P_G3P__E4P_F6P_Constants.Length;

            g3p_balance = new G3PBalancingFlow(array.Skip(start).Take(G3PBalancingFlow.Length).ToArray());
            start += G3PBalancingFlow.Length;
            f6p_balance = new F6PBalancingFlow(array.Skip(start).Take(F6PBalancingFlow.Length).ToArray());
            start += F6PBalancingFlow.Length;
            e4p_balance = new E4PBalancingFlow(array.Skip(start).Take(E4PBalancingFlow.Length).ToArray());
            start += E4PBalancingFlow.Length;
            g6p_balance = new G6PBalancingFlow(array.Skip(start).Take(G6PBalancingFlow.Length).ToArray());
            start += G6PBalancingFlow.Length;
            r5p_balance = new R5PBalancingFlow(array.Skip(start).Take(R5PBalancingFlow.Length).ToArray());
            start += R5PBalancingFlow.Length;
            s7p_balance = new S7PBalancingFlow(array.Skip(start).Take(S7PBalancingFlow.Length).ToArray());
            start += S7PBalancingFlow.Length;
            //nadph_balance = new NadphBalancingFlow(array.Skip(start).Take(NadphBalancingFlow.Length).ToArray());
            //start += NadphBalancingFlow.Length;
        }

        public PPPConstants(
            Simplified_RateG6P_NADP__PGL_NADPH_Constants rate1, 
            RatePGL_H20__6PG_H_Constants rate2, 
            Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants rate3, 
            RateOR5P__R5P_Constants rate4a,
            RateOR5P__X5P_Constants rate4b,
            RateX5P_R5P__G3P_S7P_Constants rate5,
            RateX5P_E4P__G3P_F6P_Constants rate6,
            RateS7P_G3P__E4P_F6P_Constants rate7,
            G3PBalancingFlow g3p_balance,
            F6PBalancingFlow f6p_balance,
            E4PBalancingFlow e4p_balance,
            G6PBalancingFlow g6p_balance,
            R5PBalancingFlow r5p_balance,
            S7PBalancingFlow s7p_balance
            //NadphBalancingFlow nadph_balance
            )
        {
            this.rate1_simplified = rate1;
            this.rate2 = rate2;
            this.rate3 = rate3;
            this.rate4a = rate4a;
            this.rate4b = rate4b;
            this.rate5 = rate5;
            this.rate6 = rate6;
            this.rate7 = rate7;
            this.g3p_balance = g3p_balance;
            this.f6p_balance = f6p_balance;
            this.e4p_balance = e4p_balance;
            this.g6p_balance = g6p_balance;
            this.r5p_balance = r5p_balance;
            this.s7p_balance = s7p_balance;
            //this.nadph_balance = nadph_balance;
        }

        public PPPCellState ComputeOneTimeStep(
            PPPCellState state, 
            Func<double> univariate_random_fn, 
            bool modify_input_products = false, 
            double rate3_inhibition = 0,
            bool keep_f6p_unchanged = false)
        {
            var s = state;

            var r = rate2.Queue(s, RatePGL_H20__6PG_H_Constants.Delta, univariate_random_fn);
            s += r;

            r = rate3.Queue(s, Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants.Delta, univariate_random_fn, modify_nadph_level: false, rate3_inhibition: rate3_inhibition);
            s += r;

            if (modify_input_products == false)
                s.NADP = state.NADP;

            r = rate5.Queue(s, RateX5P_R5P__G3P_S7P_Constants.Delta, univariate_random_fn);
            s += r;
            r = rate6.Queue(s, RateX5P_E4P__G3P_F6P_Constants.Delta, univariate_random_fn, keep_f6p_unchanged: keep_f6p_unchanged);
            s += r;
            for (int i = 0; i < 1000; i++)
            {
                r = rate1_simplified.Queue(s, Simplified_RateG6P_NADP__PGL_NADPH_Constants.Delta, univariate_random_fn, modify_start_products: modify_input_products, modify_nadph_level: false);
                s += r;
                //r = nadph_balance.Queue(s, NadphBalancingFlow.Delta, univariate_random_fn);
                //s += r;
                r = rate4a.Queue(s, RateOR5P__R5P_Constants.Delta, univariate_random_fn);
                s += r;
                r = rate4b.Queue(s, RateOR5P__X5P_Constants.Delta, univariate_random_fn);
                s += r;
                r = rate7.Queue(s, RateS7P_G3P__E4P_F6P_Constants.Delta, univariate_random_fn, keep_f6p_unchanged: keep_f6p_unchanged);
                s += r;
            }

            r = g3p_balance.Queue(s, G3PBalancingFlow.Delta, univariate_random_fn);
            s += r;

            if (keep_f6p_unchanged == false)
            {
                r = f6p_balance.Queue(s, F6PBalancingFlow.Delta, univariate_random_fn);
                s += r;
            }

            r = e4p_balance.Queue(s, E4PBalancingFlow.Delta , univariate_random_fn);
            s += r;

            if (modify_input_products)
            {
                r = g6p_balance.Queue(s, G6PBalancingFlow.Delta, univariate_random_fn);
                s += r;
            }
            r = r5p_balance.Queue(s, R5PBalancingFlow.Delta , univariate_random_fn);
            s += r;
            r = s7p_balance.Queue(s, S7PBalancingFlow.Delta , univariate_random_fn);
            s += r;

            return s;
        }

        public PPPConstants ApplyNoise(double noiseAmplitude, Func<double> gaussian_random_fn)
        {
            var rate1 = this.rate1_simplified.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate2 = this.rate2.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate3 = this.rate3.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate4a = this.rate4a.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate4b = this.rate4b.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate5 = this.rate5.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate6 = this.rate6.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var rate7 = this.rate7.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var g3p_balance = this.g3p_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var f6p_balance = this.f6p_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var e4p_balance = this.e4p_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var g6p_balance = this.g6p_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var r5p_balance = this.r5p_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            var s7p_balance = this.s7p_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);
            //var nadph_balance = this.nadph_balance.ApplyNoise(noiseAmplitude, gaussian_random_fn);

            return new PPPConstants(rate1, rate2, rate3, rate4a, rate4b, rate5, rate6, rate7, g3p_balance, f6p_balance, e4p_balance, g6p_balance, r5p_balance, s7p_balance);
        }

        public List<IRate> GetRatesList()
        {
            return new List<IRate>()
            {
                rate1_simplified,
                rate2,
                rate3,
                rate4a,
                rate4b,
                rate5,
                rate6,
                rate7,
                g3p_balance,
                f6p_balance,
                e4p_balance,
                g6p_balance,
                r5p_balance,
                s7p_balance
                //nadph_balance
            };
        }
    }
}
