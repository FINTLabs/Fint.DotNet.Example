using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HalClient.Net;
using HalClient.Net.Parser;
using IdentityModel.Client;
using FINT.Model.Felles;

namespace Fint.DotNet.Example
{
    [Produces("application/json")]
    [Route("api/birthdays")]
    public class Birthdays : Controller
    {

        [HttpGet]
        [Route("jubilants")]
        public IActionResult Jubilants()
        {
            var jubilants = new List<PersonSimplyfied>();
            foreach(var p in GetPersonsFromFINTApi())
            {
                if(GetAge(p) % 10 == 0)
                {
                    jubilants.Add(MapToSimplyfied(p));
                }
            }
            return new OkObjectResult(jubilants);
        }

        [HttpGet]
        [Route("today")]
        public IActionResult PersonsWithBirthdayToday()
        {
            List<Person> persons = GetPersonsFromFINTApi().Where(p => p.Fodselsdato.GetValueOrDefault().Month == DateTime.Today.Month && p.Fodselsdato.GetValueOrDefault().Day == DateTime.Today.Day).ToList();
            List<PersonSimplyfied> personsWithBirtdayToday = new List<PersonSimplyfied>();

            foreach(var p in persons){
                personsWithBirtdayToday.Add(MapToSimplyfied(p));
            }
            return new OkObjectResult(personsWithBirtdayToday);
        }

        private PersonSimplyfied MapToSimplyfied(Person p){
            var s = new PersonSimplyfied();
            s.Name = p.Navn.Fornavn + " " + p.Navn.Etternavn;
            s.DayOfBirth = p.Fodselsdato.GetValueOrDefault().ToShortDateString();
            s.Age = GetAge(p);

            return s;
        }

        private int GetAge(Person p)
        {
            var age = DateTime.Now.Year - p.Fodselsdato.GetValueOrDefault().Year;  
            return age;
        }

        private List<Person> GetPersonsFromFINTApi()
        {
            var parser = new HalJsonParser();
            var factory = new HalHttpClientFactory(parser);

            var tokenClient = new TokenClient(OAuthSettings.accessTokenUri, OAuthSettings.clientId, OAuthSettings.clientSecret);
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(OAuthSettings.username, OAuthSettings.password, OAuthSettings.scope).Result;

            var persons = new List<Person>();

            using (var client = factory.CreateClient())
            {
                client.HttpClient.SetBearerToken(tokenResponse.AccessToken);

                var response = client
                    .GetAsync(new Uri(
                        OAuthSettings.felleskomponentUri + OAuthSettings.additionalAdministrasjonPersonalPersonUri)).Result;
                var links = response.Resource.Links;
                var embedded = response.Resource.Embedded;
                var entries = embedded["_entries"].ToList();
                entries.ForEach(e => persons.Add(PersonFactory.create(e.State)));
            }
            return persons;
        }
    }
}
