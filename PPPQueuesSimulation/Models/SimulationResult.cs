using PPPQueuesSimulation.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPPQueuesSimulation.Models
{
    public class SimulationResult
    {
        public IEnumerable<PPPCellState> CellStates { get; set; }
        public double Rate3Inhibition { get; set; }
    }
}
