using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PeopleSearch
{
	public static class SampleDemographicData
	{
		public static IEnumerable<(double Frequency, List<string> Names)> GetLastNameData() => GetNameData(new Uri("http://www2.census.gov/topics/genealogy/1990surnames/dist.all.last?#"));

		public static IEnumerable<(double Frequency, List<string> Names)> GetFemaleFirstNameData() => GetNameData(new Uri("http://www2.census.gov/topics/genealogy/1990surnames/dist.female.first?#"));

		public static IEnumerable<(double Frequency, List<string> Names)> GetMaleFirstNameData() => GetNameData(new Uri("http://www2.census.gov/topics/genealogy/1990surnames/dist.male.first?#"));

		public static IEnumerable<string> GetStreetNames()
		{
			using( var hc = new System.Net.Http.HttpClient() ) {
				using( var resp = hc.GetAsync(new Uri("https://data.sfgov.org/api/views/6d9h-4u5v/rows.csv?accessType=DOWNLOAD")).GetAwaiter().GetResult() ) {
					resp.EnsureSuccessStatusCode();

					using( var sr = new StreamReader(resp.Content.ReadAsStreamAsync().GetAwaiter().GetResult()) ) {
						// skip past the first line, it's a header
						if( sr.Peek() > -1 )
							sr.ReadLine();

						while( sr.Peek() > -1 ) {
							// parse the comma-delimiated data
							var parts = sr.ReadLine().Split(',');

							if( parts.Length != 4)
								throw new Exception("Input data did not match expected format");

							// data looks like: FullStreetName,StreetName,StreetType,PostDirection
							yield return parts[0].Trim('"');
						}
					}
				}
			}
		}

		public static IEnumerable<(string PostalCode, string City, string State)> GetPostalCodes()
		{
			// Zip code database was downloaded from https://www.zip-codes.com/
			// The dataset was reduced by eliminating unused information (such as
			//   longitude and latitude) using the following PowerShell command:
			//       import-csv zip-codes-database-FREE.csv |
			//           foreach { [pscustomobject]@{Code = $_.ZipCode; City = $_.City; State = $_.State} } |
			//           convertto-csv -notypeinformation |
			//           out-file zip-codes-limited.csv
			// The dataset was then compressed using the following PowerShell commands:
			//     $fs = new-object system.io.filestream ("zip_code_data.bin", [system.io.filemode]::create)
			//     $ds = new-object system.io.compression.deflatestream ($fs, [system.io.compression.compressionlevel]::optimal)
			//     $buf = [system.io.file]::readallbytes('zip-codes-limited.csv')
			//     $ds.write($buf, 0, $buf.length)
			//     $ds.close()
			// The dataset was then embedded into the assembly as a resource.

			using( var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("hc_tech_assessment.zip_code_data.bin") )
			using( var ds = new System.IO.Compression.DeflateStream(rs, System.IO.Compression.CompressionMode.Decompress) )
			using( var sr = new StreamReader(ds) ) {
				// skip past the first line, it's a header
				if( sr.Peek() > -1 )
					sr.ReadLine();

				while( sr.Peek() > -1 ) {
					// parse the comma-delimiated data
					var parts = sr.ReadLine().Split(',');

					if( parts.Length != 3 )
						throw new Exception("Input data did not match expected format");

					// data looks like: "Code","City","State"
					yield return (PostalCode: parts[0].Trim('"'), City: parts[1].Trim('"'), State: parts[2].Trim('"'));
				}
			}
		}

		private static IEnumerable<(double Frequency, List<string> Names)> GetNameData(Uri dataUri)
		{
			// name data looks like this: SMITH          1.006  1.006      1
			// columns are: name, this_frequency, cumulative_frequency, rank
			var data_regex = new Regex(@"(?<name>.*?)\s+[\d\.]+\s+(?<frequency>[\d\.]+)\s+[\d\.]+");

			using( var hc = new System.Net.Http.HttpClient() ) {
				using( var resp = hc.GetAsync(dataUri).GetAwaiter().GetResult() ) {
					resp.EnsureSuccessStatusCode();

					using( var sr = new StreamReader(resp.Content.ReadAsStreamAsync().GetAwaiter().GetResult()) ) {
						var cumulative_freq = 0d;
						var bucket          = default(List<string>);

						while( sr.Peek() > -1 ) {
							// parse the data
							var match = data_regex.Match(sr.ReadLine());

							if( !match.Success )
								throw new Exception("Input data did not match expected format");

							var freq = Convert.ToDouble(match.Groups["frequency"].Value);

							// if our cumulative frequency has increased, we need to produce the current
							//   bucket and set up a new one
							if( freq > cumulative_freq ) {
								if( cumulative_freq > 0d )
									yield return (cumulative_freq, bucket);

								cumulative_freq = freq;
								bucket          = new List<string>();
							}

							// add the current name to the current bucket
							bucket.Add(match.Groups["name"].Value);
						}

						// produce the final bucket
						if( bucket?.Count > 0 )
							yield return (cumulative_freq, bucket);
					}
				}
			}
		}
	}
}
