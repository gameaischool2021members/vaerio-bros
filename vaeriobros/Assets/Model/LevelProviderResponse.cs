using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Model
{
    public class LevelProviderResponse
    {
        public string ExperimentName { get; set; }
        public List<List<float>> LatentVectors { get; set; }
        public List<List<List<int>>> LevelRepresentation { get; set; }
        public string RequestId { get; set; }

    }
}
