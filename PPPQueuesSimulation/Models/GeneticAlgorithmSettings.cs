using PPPQueuesSimulation.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPPQueuesSimulation.Models
{
    public class GeneticAlgorithmSettings
    {
        public int Epochs { get; set; }
        public int PopulationLength { get; set; }
        public int PopulationForReproduction { get; set; }
        public double MutationChance { get; set; }
        public double MutationMagnitude { get; set; }
        public SimulationSettings EvaluationSettings { get; set; }
        /// <summary>
        /// Can be unset
        /// </summary>
        public List<PPPConstants> InitialPopulation { get; set; }
        public string OutputFolderPath { get; set; }
    }
}
