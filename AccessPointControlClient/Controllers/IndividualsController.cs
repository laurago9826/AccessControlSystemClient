using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AccessPointControlClient.DTO;
using AccessPointControlClient.HttpClientHelpers;
using AccessPointControlClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessPointControlClient.Controllers
{
    [Authorize(Roles = SupportedRoles.PERSON_MANAGEMENT)]
    public class IndividualsController : Controller
    {
        //private static int COUNTER = 60;
        private static int nonFinalRegistrationValidDuration;

        private const string GET_ALL_PEOPLE = "people/all";
        private const string GET_PERSON_BY_ID = "people/{id}";
        private const string UPDATE_PERSON = "people/update";
        private const string INSERT_PERSON = "people/register";
        private const string DELETE_PERSON = "people/delete/{id}";
        private const string CLEAR_REGISTRATION = "people/clear-registration";
        private static List<SimpleIndividual> globalIndividualsList = new List<SimpleIndividual>();

        private static Individual waitingForFinalization;
        private static DateTime startOfReg;

        private readonly IHttpClient httpClient;
        public IndividualsController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public IActionResult Individuals()
        {
            HttpResponseMessage response = httpClient.GetAsync(GET_ALL_PEOPLE);
            List<SimpleIndividual> individuals = new List<SimpleIndividual>();
            individuals = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, individuals);
            if (waitingForFinalization != null)
            {
                individuals.Add(new SimpleIndividual()
                {
                    FullName = waitingForFinalization.FullName,
                    PersonId = waitingForFinalization.PersonId,
                    RequiresImageUpload = true
                });
            }
            SortIndividualsList(individuals);

            IndividualsModel model = new IndividualsModel()
            {
                Individuals = individuals,
                SelectedIndividual = individuals.Count == 0 ? Individual.CreateEmptyIndividual() :
                GetIndividualById(individuals.First().PersonId),
                RegistrationModel = new Individual()
            };
            globalIndividualsList = model.Individuals;
            return View(model);
        }
        private Individual GetIndividualById(string id)
        {
            if (waitingForFinalization != null && waitingForFinalization.PersonId == id)
            {
                waitingForFinalization.Counter = (int)(nonFinalRegistrationValidDuration - (DateTime.Now - startOfReg).TotalSeconds);
                return waitingForFinalization;
            }
            HttpResponseMessage response = httpClient.GetAsync(GET_PERSON_BY_ID.Replace("{id}", id));
            PersonDTO personDto = null;
            personDto = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, personDto);
            return new Individual(personDto);
        }

        private void SortIndividualsList(List<SimpleIndividual> list)
        {
            list.Sort((x1, x2) => x1.FullName.CompareTo(x2.FullName));
        }

        [HttpPost]
        public IActionResult DeletePerson(IFormCollection form)
        {
            string selectedPersonId = form["selectedPersonId"];
            string newSuffix = DELETE_PERSON.Replace("{id}", selectedPersonId);
            if (waitingForFinalization == null ||
                (waitingForFinalization != null && selectedPersonId != waitingForFinalization.PersonId))
            {
                HttpResponseMessage response = httpClient.DeleteAsync(newSuffix);
            }
            return RedirectToAction(nameof(Individuals));
        }

        [HttpPost]
        public IActionResult SelectIndividual(IFormCollection form)
        {
            string selectedPersonId = form["selectedPersonId"];
            Individual selected = GetIndividualById(selectedPersonId);
            IndividualsModel model = new IndividualsModel()
            {
                Individuals = globalIndividualsList,
                RegistrationModel = new Individual(),
                SelectedIndividual = selected
            };
            return View(nameof(Individuals), model);
        }

        [HttpPost]
        public IActionResult UpdateIndividual(IndividualsModel model)
        {
            PersonDTO dtoObj = new PersonDTO(model.SelectedIndividual);
            HttpResponseMessage response = httpClient.PostAsync(UPDATE_PERSON,
                new StringContent(JsonConvert.SerializeObject(dtoObj), Encoding.UTF8, "application/json"));
            globalIndividualsList.First(x => x.PersonId == dtoObj.PersonId).FullName = dtoObj.FullName;
            SortIndividualsList(globalIndividualsList);
            Individual updatedPerson = GetIndividualById(dtoObj.PersonId);
            IndividualsModel individualsModel = new IndividualsModel()
            {
                Individuals = globalIndividualsList,
                RegistrationModel = new Individual(),
                SelectedIndividual = updatedPerson
            };

            return View(nameof(Individuals), individualsModel);
        }

        [HttpPost]
        public IActionResult RegisterIndividual(IndividualsModel model)
        {
            PersonDTO dtoObj = new PersonDTO(model.RegistrationModel);
            HttpResponseMessage response = httpClient.PostAsync(INSERT_PERSON,
                new StringContent(JsonConvert.SerializeObject(dtoObj), Encoding.UTF8, "application/json"));
            nonFinalRegistrationValidDuration = JsonConvert.DeserializeObject<int>(response.Content.ReadAsStringAsync().Result);
            //todo set nonFinalRegistrationValid
            SimpleIndividual tempInd = new SimpleIndividual() { FullName = dtoObj.FullName, RequiresImageUpload = true };
            globalIndividualsList.Add(tempInd);
            SortIndividualsList(globalIndividualsList);
            Individual newPerson = new Individual()
            {
                Counter = nonFinalRegistrationValidDuration,
                PersonId = tempInd.PersonId,
                FullName = dtoObj.FullName,
                BirthDate = dtoObj.BirthDate,
                RequiresImageUpload = true
            };
            startOfReg = DateTime.Now;
            waitingForFinalization = newPerson;
            IndividualsModel individualsModel = new IndividualsModel()
            {
                Individuals = globalIndividualsList,
                RegistrationModel = new Individual(),
                SelectedIndividual = newPerson
            };

            System.Timers.Timer timer = new System.Timers.Timer(nonFinalRegistrationValidDuration * 1000);
            timer.Elapsed += ClearPersonWaiting;
            timer.Elapsed += (src, e) => timer.Close();
            timer.Start();
            return View(nameof(Individuals), individualsModel);
        }
        public void ClearPersonWaiting(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (waitingForFinalization == null)
            {
                return;
            }
            SimpleIndividual ind = globalIndividualsList.FirstOrDefault(x => x.PersonId == waitingForFinalization.PersonId);
            if (ind != null)
            {
                globalIndividualsList.Remove(ind);
            }
            waitingForFinalization = null;
        }
    }
}
