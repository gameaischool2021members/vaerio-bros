using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Model
{
    public class TelemetryData
    { 
        public List<List<float>> latentVectors { get; set; }
        public List<List<List<int>>> levelRepresentation { get; set; }
        public string experimentName { get; set; }
        public string modelName { get; set; }
        public bool markedUnplayable { get; set; }
        public bool endedEarly { get; set; }
        public SurveyResults surveyResults { get; set; }
    }
}
