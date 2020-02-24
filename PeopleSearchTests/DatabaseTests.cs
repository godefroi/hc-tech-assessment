using System;
using System.Linq;

using PeopleSearch.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;
using Xunit.Abstractions;

namespace PeopleSearchTests
{
	public class DatabaseTests
	{
		private ILoggerFactory m_loggerFactory;

		public DatabaseTests(ITestOutputHelper output)
		{
			var serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();

			m_loggerFactory = serviceProvider.GetService<ILoggerFactory>();

			m_loggerFactory.AddProvider(new XUnitLoggerProvider(output));
		}

		[Fact]
		public void DatabaseIsCreatedAndDataIsSeeded()
		{
			lock( typeof(PeopleSearchContext) ) {
				PeopleSearchContext.Initialize();

				using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
					Assert.True(ctx.People.Count() > 0);

					Assert.NotNull(ctx.People.First().FirstName);
				}
			}
		}

		[Fact]
		public void InitializingTheDatabaseDoesNotRecreateIt()
		{
			lock( typeof(PeopleSearchContext) ) {
				var rnd    = new Random().Next();
				var person = new Person() {
					FirstName = $"TEST PERSON {rnd}",
				};

				// initialize the database, round 1
				PeopleSearchContext.Initialize();

				// now, add a unique person to the database, so we can be sure it survived
				using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
					ctx.People.Add(person);
					ctx.SaveChanges();
				}

				// make sure we got a valid ID assigned to the test person
				Assert.True(person.PersonId > 0);

				// now, initialize the database, round 2
				PeopleSearchContext.Initialize();

				// finally, we should be able to find the person we added
				using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
					var person2 = ctx.People.Single(p => p.PersonId == person.PersonId);

					Assert.Equal(person.PersonId, person2.PersonId);
					Assert.Equal(person.FirstName, person2.FirstName);
				}
			}
		}

		[Fact]
		public void RecreatingTheDatabaseDoesRecreateIt()
		{
			lock( typeof(PeopleSearchContext) ) {
				var rnd    = new Random().Next();
				var person = new Person() {
					FirstName = $"TEST PERSON {rnd}",
				};

				// initialize the database, round 1
				PeopleSearchContext.Initialize();

				// now, add a unique person to the database, so we can be sure it DID NOT SURVIVE
				using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
					ctx.People.Add(person);
					ctx.SaveChanges();
				}

				// make sure we got a valid ID assigned to the test person
				Assert.True(person.PersonId > 0);

				// now, initialize the database, round 2, this time, asking for it to be recreated
				PeopleSearchContext.Initialize(true);

				// finally, we should NOT be able to find the person we added
				using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
					var person2 = ctx.People.SingleOrDefault(p => p.PersonId == person.PersonId);

					Assert.Null(person2);

					person2 = ctx.People.SingleOrDefault(p => p.FirstName == person.FirstName);

					Assert.Null(person2);
				}
			}
		}
	}
}
