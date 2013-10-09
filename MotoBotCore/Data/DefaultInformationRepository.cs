using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Data
{
    [Export(typeof(IInformationRepository))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DefaultInformationRepository : IInformationRepository
    {
        /// <summary>
        /// Gets the base directory of the bot.
        /// </summary>
        private DirectoryInfo BaseDirectory
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var fi = new FileInfo(assembly.Location);
                return fi.Directory;
            }
        }

        /// <summary>
        /// Gets the data directory of the bot.
        /// </summary>
        private DirectoryInfo DataDirectory
        {
            get
            {
                var path = Path.Combine(BaseDirectory.FullName, "Data");
                return new DirectoryInfo(path);
            }
        }


        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string ReadAllFromFile(string fileName, Encoding encoding)
        {
            // Check file exists
            if (DataDirectory.GetFiles().All(f => f.Name != fileName))
                throw new FileNotFoundException("File '{0}' not found.".F(fileName));

            // Get full file path
            var fileInfo = DataDirectory.GetFiles().First(x => x.Name == fileName);

            // Read all
            using (var reader = new StreamReader(fileInfo.OpenRead(), encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public Session GetNextSession(Series series)
        {
            // Get current year
            var year = DateTime.Now.Year;
            if (DateTime.Today.Month >= 11)
            {
                year++;
            }

            // Get Linq2Xml document (XDocument)
            var filename = String.Format("Schedule{0}.xml", year);
            var xml = ReadAllFromFile(filename, Encoding.UTF8);
            var doc = XDocument.Parse(xml);

            // Used to enumerate races
            int i = 1;

            // Parse XML
            var gps = doc.Descendants()
                .Where(x => x.Name == "GrandPrix")
                .Select(gp => new GrandPrix
                {
                    Name = gp.Attribute("Name").Value,
                    Sessions = gp.Descendants()
                        .Select(s => new Session()
                        {
                            Name = s.Attribute("Name").Value,
                            DateTimeUtc = new DateTime(DateTime.Parse(s.Attribute("Date").Value).Ticks),
                        }).ToList(),
                    Number = i++,
                }).ToList();

            foreach (var gp in gps)
            {
                gp.Sessions.ForEach(s => s.GrandPrix = gp);
            }

            // Get the next GP
            var nextGp = gps
                .Where(x => x.Sessions.Any(y => y.DateTimeUtc >= DateTime.Now))
                .OrderBy(x => x.Sessions.First().DateTimeUtc)
                .First();

            // Get the next session.
             var session = nextGp.Sessions
                .Where(x => x.DateTimeUtc >= DateTime.Now)
                .OrderBy(x => x.DateTimeUtc)
                .First();

            return session;
        }
    }
}
