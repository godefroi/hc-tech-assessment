using System;

using PeopleSearch.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PeopleSearchTests
{
	/// <summary>
	/// A database context that can have a logger factory set.
	/// </summary>
	/// <remarks>
	/// This context is used for unit tests, in order to output the queries applied to the database.
	/// </remarks>
	public class PeopleSearchTestContext : PeopleSearchContext
	{
		private readonly ILoggerFactory m_loggerFactory;

		public PeopleSearchTestContext() { }

		public PeopleSearchTestContext(ILoggerFactory loggerFactory)
		{
			m_loggerFactory = loggerFactory;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseLoggerFactory(m_loggerFactory);
		}
	}
}
