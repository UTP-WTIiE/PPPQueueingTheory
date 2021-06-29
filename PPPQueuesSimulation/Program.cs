using Newtonsoft.Json;
using PPPQueuesSimulation.Functions;
using PPPQueuesSimulation.Models;
using PPPQueuesSimulation.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PPPQueuesSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // This code allows to perform Pentose Phosphate pathway simulation
            // or find Michaelis-Menten constants used in PPP simulation

            string output_folder_path = @"C:\Users\Praca\Desktop\PPP Tests"; //path to folder where outputs should be stored

            // Modify the object below to set new starting points of PPP simulation
            PPPCellState start_products = PPPCellState.Default();

            // Set up Pentose Phosphate Pathway constants to run simulation/training
            PPPConstants constants = PPPConstants.GetLiteratureConstants();

            // In case you trained your own weights uncomment the line below to load them
            constants = PPPTrainedConstants.GetTrainedConstants();

            // comment lines below if you don't want to run simulation

            var result = Simulation.Simulate(
                timeSteps: 100,                 // How many time steps are to be simulated 
                noiseAmplitude: 0.03,           // Amplitude of Gaussian Noise
                cellsSimulated: 2,              // How many cells should be simulated. Each cell has its own csv file describing state of PPP products in each step
                constants: constants,           // Michaelis-Menten kinetic constants used during the simulation
                state: start_products,          // Starting point of simulation
                timeStepsPerSavingSample: 1,    // After how many time steps should cell state be stored for saving. This option is important for simulating very long experiments with a lot of cells. 
                                                // Default value is 1, which means that on every time step cell internal state is saved
                rate3_inhibitions: null,        // How much rate 6GP + NADP -> OR5P + NADPH + H + CO2 should be inhibited. The default is null, which means no inhibition.
                                                // If you want to conduct a simulation with inhibition equal to 90% type here rate3_inhibitions: new double[] { 0.9 }.
                keep_f6p_unchanged: false       // Wether simulation should simulate arrivals and departures of F6P packages or should this substrate stay unchaged. Default is false
            );

            string destinationPath = Path.Combine(output_folder_path, "results", $"{DateTime.Now.Ticks}");
            Directory.CreateDirectory(destinationPath);

            for (int i = 0; i < result.Count; i++)
            {
                string name = $"{i}";
                string inhibition_folder_path = Path.Combine(destinationPath, $"rate3_inhibition_{result[i].Rate3Inhibition}");
                if (Directory.Exists(inhibition_folder_path) == false)
                    Directory.CreateDirectory(inhibition_folder_path);

                CsvSaver.SavePPPResults(result[i].CellStates.ToList(), inhibition_folder_path, name, constants, rate3_inhibition: result[i].Rate3Inhibition);
            }

            // uncomment lines below if you want to start training

            var training_settings = new GeneticAlgorithmSettings()
            {
                Epochs = 100,                                                   // How many generations should be spawned in order to perform Genetic Algorithm search
                EvaluationSettings = new SimulationSettings()
                {
                    CellsSimulated = 1,                                         // How many cells should be simulated at the same time to evaluate results of one sets of kinetic constants
                    NoiseAmplitude = 0.03,                                      // Amplitude of Gaussian Noise
                    StartState = start_products,                                // Starting point of simulation
                    TimeSteps = 100                                             // How many time steps are to be simulated during evaluation of one chromosome
                },
                InitialPopulation = new List<PPPConstants>() { constants },     // The starting point of population (default is literature table of kinetic constants)
                MutationChance = 0.1,                                           // What is the probability, that each gene will be affected by the mutation
                MutationMagnitude = 0.1,                                        // How severe is the mutation
                OutputFolderPath = output_folder_path,                          // Where results of Genetic Search should be saved
                PopulationForReproduction = 10,                                 // How many chromosomes should be selected for the reproduction
                PopulationLength = 100                                          // How many chromosomes should be in one population
            };

            GeneticAlgorithm.Find(training_settings);
        }
    }
}
