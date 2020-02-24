using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using PeopleSearch.Models;

namespace PeopleSearch.GraphQL
{
	public class PersonType : ObjectGraphType<Person>
	{
		public PersonType()
		{
			Field(x => x.PersonId, type: typeof(IdGraphType)).Description("the PersonId");
			Field(x => x.FirstName).Description("First name");
			Field(x => x.LastName).Description("Last name");
			Field(x => x.Address).Description("Address");
			Field(x => x.City).Description("City");
			Field(x => x.State).Description("State");
			Field(x => x.PostalCode).Description("Postal code");
		}
	}
}
