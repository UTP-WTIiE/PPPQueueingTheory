using PPPQueuesSimulation.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPPQueuesSimulation.Models
{
    public class SimulationSettings
    {
        public int TimeSteps { get; set; }
        public double NoiseAmplitude { get; set; }
        public int CellsSimulated { get; set; }
        public PPPConstants Constants { get; set; }
        public PPPCellState StartState { get; set; }
        public int TimeStepsPerSavingSample { get; set; }
        /// <summary>
        /// In case of conducting experiment with no inhibition leave this field as null
        /// </summary>
        public double[] Rate3Inhibtions { get; set; }
    }
}
