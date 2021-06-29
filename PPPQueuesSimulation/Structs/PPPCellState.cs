using PPPQueuesSimulation.Interfaces;
using System;
namespace PPPQueuesSimulation.Structs
{
    public struct PPPCellState : IDoubleArray
    {
        public double G6P { get; set; }
        public double NADP { get; set; }
        public double PGL { get; set; }
        public double NADPH { get; set; }
        public double _6GP { get; set; }
        public double OR5P { get; set; }
        public double R5P { get; set; }
        public double X5P { get; set; }
        public double G3P { get; set; }
        public double S7P { get; set; }
        public double E4P { get; set; }
        public double F6P { get; set; }

        public double CO2 { get; set; }

        public static PPPCellState Default()
        {
            var state = new PPPCellState();
            state.NADP = 1 * 1e-6 * 1e3;
            state.G6P = 2.6 * 1e-6 * 1e3;
            state.NADPH = 2 * 1e-7 * 1e3;
            state.PGL = 5 * 1e-9 * 1e3;
            state._6GP = 1.8 * 1e-5 * 1e3;
            state.CO2 = 1 * 1e-6 * 1e3;
            state.OR5P = 1.2 * 1e-5 * 1e3;
            state.R5P = 9 * 1e-6 * 1e3;
            state.X5P = 1.8 * 1e-5 * 1e3;
            state.F6P = 8.3 * 1e-5 * 1e3;
            state.S7P = 6.8 * 1e-5 * 1e3;
            state.G3P = 2.34 * 1e-6 * 1e3;
            state.E4P = 4 * 1e-6 * 1e3;

            return state;
        }

        public PPPCellState(double[] array)
        {
            G6P = array[0];
            NADP = array[1];
            PGL = array[2];
            NADPH = array[3];
            _6GP = array[4];
            OR5P = array[5];
            R5P = array[6];
            X5P = array[7];
            G3P = array[8];
            S7P = array[9];
            E4P = array[10];
            F6P = array[11];
            CO2 = array[12];
        }

        public static PPPCellState operator + (PPPCellState a, PPPCellState b)
        {
            var c = a;
            c.CO2 += b.CO2;
            c.E4P += b.E4P;
            c.F6P += b.F6P;
            c.G3P += b.G3P;
            c.G6P += b.G6P;
            c.NADP += b.NADP;
            c.NADPH += b.NADPH;
            c.OR5P += b.OR5P;
            c.PGL += b.PGL;
            c.R5P += b.R5P;
            c.S7P += b.S7P;
            c.X5P += b.X5P;
            c._6GP += b._6GP;

            return c;
        }

        public static PPPCellState operator -(PPPCellState a, PPPCellState b)
        {
            var c = a;
            c.CO2 -= b.CO2;
            c.E4P -= b.E4P;
            c.F6P -= b.F6P;
            c.G3P -= b.G3P;
            c.G6P -= b.G6P;
            c.NADP -= b.NADP;
            c.NADPH -= b.NADPH;
            c.OR5P -= b.OR5P;
            c.PGL -= b.PGL;
            c.R5P -= b.R5P;
            c.S7P -= b.S7P;
            c.X5P -= b.X5P;
            c._6GP -= b._6GP;

            return c;
        }

        public static PPPCellState operator * (PPPCellState a, double b)
        {
            var c = a;
            c.CO2 *= b;
            c.E4P *= b;
            c.F6P *= b;
            c.G3P *= b;
            c.G6P *= b;
            c.NADP *= b;
            c.NADPH *= b;
            c.OR5P *= b;
            c.PGL *= b;
            c.R5P *= b;
            c.S7P *= b;
            c.X5P *= b;
            c._6GP *= b;

            return c;
        }

        public static PPPCellState operator * (double a, PPPCellState b)
        {
            return b * a;
        }

        public static PPPCellState operator / (PPPCellState a, double b)
        {
            var c = 1 / b;
            return a * c;
        }

        public static PPPCellState operator * (PPPCellState a, PPPCellState b)
        {
            var c = a;
            c.CO2 *= b.CO2;
            c.E4P *= b.E4P;
            c.F6P *= b.F6P;
            c.G3P *= b.G3P;
            c.G6P *= b.G6P;
            c.NADP *= b.NADP;
            c.NADPH *= b.NADPH;
            c.OR5P *= b.OR5P;
            c.PGL *= b.PGL;
            c.R5P *= b.R5P;
            c.S7P *= b.S7P;
            c.X5P *= b.X5P;
            c._6GP *= b._6GP;

            return c;
        }

        public static PPPCellState operator /(PPPCellState a, PPPCellState b)
        {
            var c = a;
            c.CO2 /= b.CO2;
            c.E4P /= b.E4P;
            c.F6P /= b.F6P;
            c.G3P /= b.G3P;
            c.G6P /= b.G6P;
            c.NADP /= b.NADP;
            c.NADPH /= b.NADPH;
            c.OR5P /= b.OR5P;
            c.PGL /= b.PGL;
            c.R5P /= b.R5P;
            c.S7P /= b.S7P;
            c.X5P /= b.X5P;
            c._6GP /= b._6GP;

            return c;
        }



        public bool Equals(PPPCellState obj)
        {
            if (this.NADP != obj.NADP)
                return false;
            if (this.G6P != obj.G6P)
                return false;
            if (this.NADPH != obj.NADPH)
                return false;
            if (this.PGL != obj.PGL)
                return false;
            if (this._6GP != obj._6GP)
                return false;
            if (this.CO2 != obj.CO2)
                return false;
            if (this.OR5P != obj.OR5P)
                return false;
            if (this.R5P != obj.R5P)
                return false;
            if (this.X5P != obj.X5P)
                return false;
            if (this.F6P != obj.F6P)
                return false;
            if (this.S7P != obj.S7P)
                return false;
            if (this.G3P != obj.G3P)
                return false;
            if (this.E4P != obj.E4P)
                return false;

            return true;
        }

        public double[] ToDoubleArray()
        {
            return new double[]
            {
                G6P,
                NADP,
                PGL,
                NADPH,
                _6GP,
                OR5P,
                R5P,
                X5P,
                G3P,
                S7P,
                E4P,
                F6P,
                CO2
            };
        }
    }
}
