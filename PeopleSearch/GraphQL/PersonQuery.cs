using System;

using GraphQL.Types;

using PeopleSearch.Models;

namespace PeopleSearch.GraphQL
{
	public class PersonQuery : ObjectGraphType
	{
		public PersonQuery(PeopleSearchContext context)
		{
			Field<ListGraphType<PersonType>>("people", resolve: c => context.People);
		}
	}
}
