using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Data
{
    public partial class Series
    {
        public static readonly Series Formula1 = new Series { LongName = "Formula 1", ShortName = "F1", Website = new Uri("http://www.formula1.com/") };
        public static readonly Series Gp2 = new Series { LongName = "GP2 Series", ShortName = "GP2", Website = new Uri("http://www.gp2series.com/") };
        public static readonly Series Formula3 = new Series { LongName = "F3 European Championship", ShortName = "F3", Website = new Uri("http://www.fiaf3europe.com/") };
        public static readonly Series MotoGp1 = new Series { LongName = "MotoGP 1", ShortName = "Moto1", Website = new Uri("http://www.motogp.com/") };
        public static readonly Series MotoGp2 = new Series { LongName = "MotoGP 2", ShortName = "Moto2", Website = new Uri("http://www.motogp.com/") };
        public static readonly Series MotoGp3 = new Series { LongName = "MotoGP 3", ShortName = "Moto3", Website = new Uri("http://www.motogp.com/") };
        public static readonly Series Dtm = new Series { LongName = "Deutsche Tourenwagen Meisterschaft", ShortName = "DTM", Website = new Uri("http://www.dtm.de/") };

        public string LongName { get; set; }
        public string ShortName { get; set; }
        public Uri Website { get; set; }
    }
}
