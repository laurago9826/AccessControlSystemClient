using AccessPointControlClient.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessPointControlClient.Models
{
    public class IndividualsModel
    {
        public Individual SelectedIndividual { get; set; }
        public List<SimpleIndividual> Individuals { get; set; }

        public Individual RegistrationModel { get; set; }
    }

    public class SimpleIndividual
    {
        public string PersonId { get; set; }
        public string FullName { get; set; }

        public bool RequiresImageUpload { get; set; }

        public SimpleIndividual()
        {
            PersonId = Guid.NewGuid().ToString();
        }
    }

    public class Individual 
    {
        public string PersonId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ProcessedPalmImageSource { get; set; }

        public string HandGeometryDebugImageSource { get; set; }
        public bool RequiresImageUpload { get; set; }

        [JsonIgnore]
        public int Counter { get; set; }

        public Individual()
        {
            
        }
        public Individual(PersonDTO person)
        {
            PersonId = person.PersonId;
            FullName = person.FullName;
            BirthDate = person.BirthDate;
            ProcessedPalmImageSource = person.ProcessedPalmImage == null ? "" : Convert.ToBase64String(person.ProcessedPalmImage);
            HandGeometryDebugImageSource = person.DebugImage == null ? "" : Convert.ToBase64String(person.DebugImage);
        }

        public static Individual CreateEmptyIndividual()
        {
            Individual ind = new Individual();
            ind.FullName = "";
            ind.HandGeometryDebugImageSource = "";
            ind.ProcessedPalmImageSource = "";
            ind.BirthDate = DateTime.MinValue;
            ind.PersonId = "";
            return ind;
        }
    }
}
