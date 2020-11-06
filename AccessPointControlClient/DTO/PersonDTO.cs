using AccessPointControlClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.DTO
{
    public class PersonDTO
    {
        public string PersonId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public byte[] ProcessedPalmImage { get; set; }

        public byte[] DebugImage { get; set; }
        public PersonDTO()
        {

        }
        public PersonDTO(Individual individual)
        {
            this.PersonId = individual.PersonId;
            this.FullName = individual.FullName;
            this.BirthDate = individual.BirthDate;
            if (individual.ProcessedPalmImageSource != null)
            {
                this.ProcessedPalmImage = Convert.FromBase64String(individual.ProcessedPalmImageSource);
            }
            if (individual.HandGeometryDebugImageSource != null)
            {
                this.DebugImage = Convert.FromBase64String(individual.HandGeometryDebugImageSource);
            }
        }
    }
}
