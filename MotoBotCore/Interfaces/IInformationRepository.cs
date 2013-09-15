using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoBotCore.Data;
using MotoBotCore.Enums;

namespace MotoBotCore.Interfaces
{
    /// <summary>
    /// Reads content from a file.
    /// </summary>
    public interface IInformationRepository
    {
        /// <summary>
        /// Reads everything as a string.
        /// </summary>
        /// <param name="fileName">File to read from, relative to base directory in config. Example: "help.txt", "data/f1/schedule-2013.xml"</param>
        /// <param name="encoding">File encoding.</param>
        /// <returns></returns>
        string ReadAllFromFile(string fileName, Encoding encoding);

        /// <summary>
        /// Gets the next session for a given series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        Session GetNextSession(Series series);
    }
}
