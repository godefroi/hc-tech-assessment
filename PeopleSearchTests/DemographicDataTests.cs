using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace hc_assessment_tests
{
	public class DemographicDataTests
	{
		[Fact]
		public void LastNamesLookValid()
		{
			var last_bucket = default((double Frequency, List<string> Names));

			foreach( var bucket in PeopleSearch.SampleDemographicData.GetLastNameData() ) {
				// the list of names shouldn't be null
				Assert.NotNull(bucket.Names);

				// the list of names shouldn't be empty
				Assert.NotEmpty(bucket.Names);

				// the cumulative frequency for the bucket shouldn't be zero
				Assert.True(bucket.Frequency > 0d);

				if( last_bucket.Names != null ) {
					// the cumulative frequency must be increasing
					Assert.True(bucket.Frequency > last_bucket.Frequency);
				}

				// keep track of the previous bucket
				last_bucket = bucket;
			}
		}

		[Fact]
		public void StreetNamesLookValid()
		{
			Assert.True(PeopleSearch.SampleDemographicData.GetStreetNames().Count() > 0);
		}

		[Fact]
		public void PostalCodesLookValid()
		{
			Assert.True(PeopleSearch.SampleDemographicData.GetPostalCodes().Count() > 0);
		}

		[Fact]
		public void PeopleAreGeneratedSuccessfully()
		{
			foreach( var person in PeopleSearch.SampleDataGenerator.GeneratePeople().Take(10) ) {
				Assert.True(!string.IsNullOrWhiteSpace(person.FirstName));
				Assert.True(!string.IsNullOrWhiteSpace(person.LastName));
				Assert.True(!string.IsNullOrWhiteSpace(person.Address));
				Assert.True(!string.IsNullOrWhiteSpace(person.City));
				Assert.True(!string.IsNullOrWhiteSpace(person.State));
				Assert.True(!string.IsNullOrWhiteSpace(person.PostalCode));
			}
		}
	}
}
