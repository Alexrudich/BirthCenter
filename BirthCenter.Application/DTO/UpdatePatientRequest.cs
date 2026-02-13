using System;
using System.Collections.Generic;

namespace BirthCenter.Application.DTO
{
    public class UpdatePatientRequest
    {
        public PatientNameDto Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public bool? Active { get; set; }
    }
}