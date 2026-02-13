using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthCenter.DataLoader.Models
{
    public class GeneratedPatient
    {
        public PatientName Name { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }

    public class PatientName
    {
        public string Use { get; set; } = "official";
        public string Family { get; set; }
        public List<string> Given { get; set; }
    }
}
