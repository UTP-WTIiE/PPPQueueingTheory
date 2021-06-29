using Newtonsoft.Json;
using PPPQueuesSimulation.Extensions;
using PPPQueuesSimulation.Interfaces;
using PPPQueuesSimulation.Models;
using PPPQueuesSimulation.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PPPQueuesSimulation.Functions
{
    public static class GeneticAlgorithm
    {
        public static double minimum_start_chance = 0.000005;
        public static double maximum_start_chance = 0.05;

        public static List<PPPConstants> Find(GeneticAlgorithmSettings settings)
        {
            var random = new Random();

            var population = settings.InitialPopulation;
            if (population == null)
                population = new List<PPPConstants>() { PPPConstants.GetLiteratureConstants() };

            void restore_population()
            {
                int pick() => (int)Math.Floor(random.NextDouble() * population.Count);
                List<PPPConstants> to_add = new List<PPPConstants>();
                for (int i = population.Count; i < settings.PopulationLength; i++)
                {
                    var pick1 = pick();
                    var pick2 = pick();

                    var chromosome = Reproduce(population[pick1], population[pick2], settings.EvaluationSettings.StartState, settings.MutationChance, settings.MutationMagnitude);
                    to_add.Add(chromosome);
                }
                population.AddRange(to_add);
            }

            restore_population();

            string research_path = Path.Combine(settings.OutputFolderPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(research_path);

            for(int i = 0; i < settings.Epochs; i++)
            {
                //Chromosomes evaluation and selection
                var reproductive_population = population
                    .AsParallel()
                    .Select(x => (x, Evaluate(x, settings.EvaluationSettings)))
                    .OrderBy(x => x.Item2)
                    .Take(settings.PopulationForReproduction)
                    .ToList();

                // Printing results of best chromosomes
                Console.WriteLine();
                Console.WriteLine($"Epoch: {i + 1}/{settings.Epochs} {DateTime.Now.ToShortTimeString()}");
                reproductive_population.ForEach(x => Console.WriteLine(x.Item2));

                // Saving results
                string path_to_directory = Path.Combine(research_path, reproductive_population[0].Item2.ToString());
                Directory.CreateDirectory(path_to_directory);
                reproductive_population.ForEach(x => {
                    string path = Path.Combine(path_to_directory, $"{x.Item2}.json");
                    string json = JsonConvert.SerializeObject(x.x);
                    File.WriteAllText(path, json);
                });

                // Trimming population
                population = reproductive_population.Select(x=>x.x).ToList();

                if (i <= settings.Epochs - 1)
                {
                    // Restoring population
                    restore_population();
                }
            }

            return population;
        }

        public static double Evaluate(PPPConstants chromosome, SimulationSettings settings)
        {
            var result = Simulation.Simulate(settings.TimeSteps, settings.NoiseAmplitude, settings.CellsSimulated, chromosome, settings.StartState);

            var evaluation = result.Select(x =>
            {
                var start = x.CellStates.First();
                var end = x.CellStates.Last();
                var rel = start.ToDoubleArray()
                    .Zip(end.ToDoubleArray(), (a, b) => Math.Min(a, b))
                    .Select(x=> x != 0 ? x : 1e-12)
                    .ToArray();

                var relative_differences = (start - end) / new PPPCellState(rel);
                var output_array = relative_differences
                    .ToDoubleArray()
                    .Select(x => Math.Abs(x))
                    .ToArray();

                var o = new PPPCellState(output_array);
                double output = 0;
                output += o.E4P;
                output += o.F6P;
                output += o.G3P;
                output += o.G6P;
                output += o.NADP;
                output += o.OR5P;
                //output += o.PGL;
                output += o.R5P;
                output += o.S7P;
                output += o.X5P;
                output += o._6GP;
                //output += o.NADPH;

                return output;
            })
            .Sum();

            evaluation = evaluation / (double)result.Count;

            return evaluation;
        }

        public static PPPConstants Reproduce(PPPConstants parent1, PPPConstants parent2, PPPCellState startState, double mutation_chance, double mutation_magnitude)
        {
            var random = new Random();

            double range_dist(double x, double xmin, double xmax)
            {
                if (x >= xmin && x <= xmax)
                    return 0;
                else
                {
                    var min_dist = Math.Abs(x - xmin);
                    var max_dist = Math.Abs(x - xmax);

                    return Math.Min(min_dist, max_dist);
                }
            }
            double rate_dist(IRate rate)
            {
                var r = rate.CalculateRate(startState);
                if (rate.IsReversible)
                    r = Math.Abs(r);

                return range_dist(r, minimum_start_chance, maximum_start_chance);
            }

            var rates = parent1.GetRatesList().Zip(parent2.GetRatesList(), (a, b) => (a, b)).ToList();
            List<IRate> new_rates = new List<IRate>();

            for(int i = 0; i < rates.Count; i++)
            {
                var p1 = rates[i].a;
                var p2 = rates[i].b;

                var p1_dist = rate_dist(p1);
                var p2_dist = rate_dist(p2);

                while (true)
                {
                    var new_rate = random.NextDouble() < 0.5 ? p1.ShallowCopy() : p2.ShallowCopy();

                    new_rate = new_rate.ModifyWeights((iteration, value) => random.NextDouble() < mutation_chance ? Math.Abs(value * (1.0 + mutation_magnitude * random.NextGaussian())) : value);
                    var new_rate_dist = rate_dist(new_rate);

                    // If rate is balancing flow then don't apply required checks
                    if (p1.IsBalancingFlow)
                        new_rate_dist = 0;

                    //if (p1_dist != rate_dist(p1) || p2_dist != rate_dist(p2))
                    //    throw new Exception("Reproduction affects parents values");

                    if (new_rate_dist == 0)
                    {
                        new_rates.Add(new_rate);
                        break;
                    }
                    else
                    {
                        

                        if (new_rate_dist < p1_dist)
                            p1 = new_rate;
                        if (new_rate_dist < p2_dist)
                            p2 = new_rate;
                    }
                }
            }

            var new_chromosome_array = new_rates
                .Select(x => x.ToDoubleArray())
                .SelectMany(x => x)
                .ToArray();

            return new PPPConstants(new_chromosome_array);
        }
    }
}
