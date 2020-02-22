using System;
using System.Collections.Generic;
using System.Linq;

using PeopleSearch.Models;

namespace PeopleSearch
{
	public static class SampleDataGenerator
	{
		public static IEnumerable<Person> GeneratePeople()
		{
			var last_names    = SampleDemographicData.GetLastNameData().ToList();
			var first_names_f = SampleDemographicData.GetFemaleFirstNameData().ToList();
			var first_names_m = SampleDemographicData.GetMaleFirstNameData().ToList();
			var street_names  = SampleDemographicData.GetStreetNames().ToList();
			var postal_codes  = SampleDemographicData.GetPostalCodes().ToList();
			var rnd           = new Random();

			while( true ) {
				var postal_code = postal_codes[rnd.Next(0, postal_codes.Count)];

				yield return new Person() {
					FirstName  = rnd.Next(0, 2) == 1 ? GetRandomName(rnd, first_names_f) : GetRandomName(rnd, first_names_m),
					LastName   = GetRandomName(rnd, last_names),
					Address    = $"{rnd.Next(1, 9999)} {street_names[rnd.Next(0, street_names.Count)]}",
					City       = postal_code.City,
					State      = postal_code.State,
					PostalCode = postal_code.PostalCode,
				};
			}
		}

		private static string GetRandomName(Random random, List<(double Frequency, List<string> Names)> names)
		{
			var rnd_freq = random.NextDouble() * names[names.Count - 1].Frequency;
			var bucket   = names.First(n => n.Frequency >= rnd_freq);

			// if this bucket has only a single name in it, return it
			if( bucket.Names.Count == 0 )
				return bucket.Names[0];

			// otherwise, pick one of the names at random
			return bucket.Names[random.Next(0, bucket.Names.Count)];
		}
	}
}
