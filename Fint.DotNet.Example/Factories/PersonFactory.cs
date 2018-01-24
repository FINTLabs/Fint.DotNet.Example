using System;
using System.Collections.Generic;
using FINT.Model.Felles;
using FINT.Model.Felles.Kompleksedatatyper;
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace Fint.DotNet.Example
{
    public class PersonFactory
    {
        public static Person create(IReadOnlyDictionary<string, IStateValue> values)
        {
            return new Person
            {
                Kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(values["kontaktinformasjon"].Value),
                Postadresse = JsonConvert.DeserializeObject<Adresse>(values["postadresse"].Value),
                Bostedsadresse = JsonConvert.DeserializeObject<Adresse>(values["bostedsadresse"].Value),
                Fodselsnummer = JsonConvert.DeserializeObject<Identifikator>(values["fodselsnummer"].Value),
                Fodselsdato = DateTime.Parse(values["fodselsdato"].Value),
                Navn = JsonConvert.DeserializeObject<Personnavn>(values["navn"].Value)
            };
        }
    }
}