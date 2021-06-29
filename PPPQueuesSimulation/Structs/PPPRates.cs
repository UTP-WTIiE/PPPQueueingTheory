using System;
namespace PPPQueuesSimulation.Structs
{
    public struct PPPRates
    {
        public double G6PplusNADP_PGLplusNADPH { get; set; }
        public double PGL_6GP { get; set; }
        public double _6GPplusNADP_OR5PplusNADPH { get; set; }
        public double OR5P_R5P { get; set; }
        public double OR5P_X5P { get; set; }
        public double X5PplusR5P_G3PplusS7P { get; set; }
        public double X5PplusE4P_G3PplusF6P { get; set; }
        public double S7PplusG3P_E4PplusF6P { get; set; }
    }
}
