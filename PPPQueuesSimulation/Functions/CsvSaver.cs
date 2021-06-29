using PPPQueuesSimulation.Structs;
using PPPQueuesSimulation.Structs.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PPPQueuesSimulation.Functions
{
    public static class CsvSaver
    {
        public static void SavePPPResults(List<PPPCellState> states, string destination_folder_path, string file_name, PPPConstants weights_used, double rate3_inhibition = 0)
        {
            string header = "G6P;NADP;PGL;NADPH;6GP;OR5P;R5P;X5P;G3P;S7P;E4P;F6P;CO2";
            foreach (var rate in weights_used.GetRatesList())
                header += $";{rate.Name}";

            var data = states
                .Select(x => {
                    var s = $"{x.G6P};{x.NADP};{x.PGL};{x.NADPH};{x._6GP};{x.OR5P};{x.R5P};{x.X5P};{x.G3P};{x.S7P};{x.E4P};{x.F6P};{x.CO2}";
                    foreach(var rate in weights_used.GetRatesList())
                    {
                        var rateValue = rate.CalculateRate(x);
                        if (rate.Name == Rate6PG_NADP__OR5P_NADPH_H_CO2_Constants._Name)
                            rateValue = rateValue * (1.0 - rate3_inhibition);
                        s += $";{rateValue}";
                    }
                    s = s.Replace(',', '.');
                    return s;
                    })
                .ToList();

            var to_save = new List<string>();
            to_save.Add(header);
            to_save.AddRange(data);

            string path = Path.Combine(destination_folder_path, $"{file_name}.csv");

            File.WriteAllLines(path, to_save);
        }
    }
}
