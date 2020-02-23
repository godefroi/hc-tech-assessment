using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PeopleSearch.Models
{
	public class PeopleSearchContext : DbContext
	{
		public DbSet<Person> People { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// we're going to hard-code the SQLite file location here
			// this is ONLY FOR DEMONSTRATION PURPOSES, never do this in practice
			optionsBuilder.UseSqlite("data source=./PeopleSearch.db");

			base.OnConfiguring(optionsBuilder);
		}

		public static void Initialize(bool recreate = false, bool skipSeeding = false)
		{
			using( var ctx = new PeopleSearchContext() ) {
				// remove the database if we've been asked to recreate it
				if( recreate )
					ctx.Database.EnsureDeleted();

				// ensure the database is created; we'll never migrate, so this is fine
				//   for demonstration purposes
				if( ctx.Database.EnsureCreated() && !skipSeeding ) {
					// add some interesting seed data; we'll use some census name data to make
					//   our test data resemble reality
					ctx.People.AddRange(SampleDataGenerator.GeneratePeople().Take(100));
					ctx.SaveChanges();
				}
			}
		}
	}
}
