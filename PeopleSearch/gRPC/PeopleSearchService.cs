using System;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PeopleSearch.Models;

namespace PeopleSearch.gRPC
{
	public class PeopleSearchService : PeopleSearchGrpc.PeopleSearchGrpcBase
	{
		private readonly PeopleSearchContext m_dbcontext;

		public PeopleSearchService(PeopleSearchContext context)
		{
			m_dbcontext = context;
		}

		public override Task<FindResponse> FindPersons(FindRequest request, ServerCallContext context)
		{
			if( string.IsNullOrWhiteSpace(request?.Query) )
				return Task.FromResult(new FindResponse());

			// we want our wildcard to be * like globbing, not % like sql
			var find    = request.Query.Replace('*', '%');
			var resp    = new FindResponse();
			var results = m_dbcontext.People.Where(p => EF.Functions.Like(p.FirstName, find) || EF.Functions.Like(p.LastName, find)).Skip(request.Skip).Take(request.Take);

			resp.Results.AddRange(results.Select(p => new GrpcPerson() {
				PersonId   = p.PersonId,
				FirstName  = p.FirstName,
				LastName   = p.LastName,
				Address    = p.Address,
				City       = p.City,
				State      = p.State,
				PostalCode = p.PostalCode,
			}));

			return Task.FromResult(resp);
		}
	}
}
