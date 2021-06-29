using PPPQueuesSimulation.Extensions;
using PPPQueuesSimulation.Models;
using PPPQueuesSimulation.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPQueuesSimulation.Functions
{
    public static class Simulation
    {
        public static List<SimulationResult> Simulate(
            int timeSteps, 
            double noiseAmplitude, 
            int cellsSimulated, 
            PPPConstants constants, 
            PPPCellState state, 
            int timeStepsPerSavingSample = 1, 
            double[] rate3_inhibitions = null,
            bool keep_f6p_unchanged = false)
        {
            IEnumerable<PPPCellState> simulate_one_cell(double rate3_inhibition = 0)
            {
                var r = new Random();
                Func<double> univariate_random_fn = () => r.NextDouble();
                Func<double> gaussian_random_fn = () => r.NextGaussian();

                var states = new List<PPPCellState>();
                var lastState = state;
                
                while(true)
                {
                    yield return lastState;

                    for (int i = 0; i < timeStepsPerSavingSample; i++) {
                        // one time step of the simulation

                        // apply gaussian noise to weights
                        var currentConstants = constants.ApplyNoise(noiseAmplitude, gaussian_random_fn);

                        // apply gaussian noise to values that do not change in time
                        lastState.CO2 = state.CO2 * (1.0 + noiseAmplitude * gaussian_random_fn());
                        lastState.G6P = state.G6P * (1.0 + noiseAmplitude * gaussian_random_fn());
                        lastState.NADP = state.NADP * (1.0 + noiseAmplitude * gaussian_random_fn());
                        lastState.NADPH = state.NADPH * (1.0 + noiseAmplitude * gaussian_random_fn());

                        // compute queues for one time step (some rates wil fire up thousand of times and some only once
                        // (modify_input_products is set to false, because G6P and NADP should not be modified (it is constantly refilled by cell))
                        var currentState = currentConstants.ComputeOneTimeStep(lastState, univariate_random_fn, modify_input_products: false, rate3_inhibition: rate3_inhibition, keep_f6p_unchanged: keep_f6p_unchanged);

                        // save result
                        states.Add(currentState);
                        lastState = currentState;
                    }
                }
            }

            var experiments = new List<SimulationResult>();

            if (rate3_inhibitions == null)
                rate3_inhibitions = new double[] { 0 };

            foreach(double inhibition in rate3_inhibitions)
                for (int i = 0; i < cellsSimulated; i++)
                    experiments.Add(
                        new SimulationResult() { 
                        CellStates = simulate_one_cell(inhibition).Take(timeSteps), 
                        Rate3Inhibition = inhibition 
                        });

            return experiments
                .AsParallel()
                .Select(x => new SimulationResult()
                {
                    CellStates = x.CellStates.ToList(),
                    Rate3Inhibition = x.Rate3Inhibition
                })
                .ToList();

        }
        public static List<SimulationResult> Simulate(SimulationSettings settings)
        {
            return Simulate(settings.TimeSteps, settings.NoiseAmplitude, settings.CellsSimulated, settings.Constants, settings.StartState, settings.TimeStepsPerSavingSample, rate3_inhibitions: settings.Rate3Inhibtions);
        }
    }
}
