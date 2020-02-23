using System;
using System.Collections.Generic;
using System.Linq;

using PeopleSearch.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PeopleSearch.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PersonController : ControllerBase
	{
		private readonly ILogger<PersonController> m_logger;
		private readonly PeopleSearchContext       m_dbcontext;

		public PersonController(ILogger<PersonController> logger, PeopleSearchContext context)
		{
			m_logger    = logger;
			m_dbcontext = context;
		}

		[HttpGet]
		public IEnumerable<Person> Index(string find = null, int skip = 0, int take = 10)
		{
			if( string.IsNullOrWhiteSpace(find) )
				return Enumerable.Empty<Person>();

			// we want our wildcard to be * like globbing, not % like sql
			find = find.Replace('*', '%');

			return m_dbcontext.People.Where(p => EF.Functions.Like(p.FirstName, find) || EF.Functions.Like(p.LastName, find)).Skip(skip).Take(take);
		}

		[HttpPut("{id}")]
		public ActionResult Index(int id, Person person)
		{
			// make sure our IDs match
			if( person == null || person.PersonId != id || id == 0 )
				return BadRequest();

			// track the entity as modified
			m_dbcontext.Entry(person).State = EntityState.Modified;

			try {
				// optimistically update the patient
				m_dbcontext.SaveChanges();
			} catch( DbUpdateException ex ) {
				// if the problem was that there's no patient with that id, return a 404
				if( !m_dbcontext.People.Any(p => p.PersonId == id) )
					return NotFound();

				throw;
			}

			// and, we're done
			return NoContent();
		}

		[HttpPost]
		public ActionResult Index(Person person)
		{
			// make sure our IDs match
			if( person == null || person.PersonId != 0 )
				return BadRequest();

			// track the entity as added
			m_dbcontext.Entry(person).State = EntityState.Added;

			// save the patient
			m_dbcontext.SaveChanges();

			// and, we're done
			return NoContent();
		}
	}
}
