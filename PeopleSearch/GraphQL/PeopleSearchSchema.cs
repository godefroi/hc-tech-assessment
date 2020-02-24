using System;
using GraphQL;
using GraphQL.Types;

namespace PeopleSearch.GraphQL
{
	public class PeopleSearchSchema : Schema
	{
		public PeopleSearchSchema(IDependencyResolver resolver) : base(resolver)
		{
			Query = resolver?.Resolve<PersonQuery>();
		}
	}
}
