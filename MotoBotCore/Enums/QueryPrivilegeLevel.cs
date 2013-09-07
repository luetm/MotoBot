using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Enums
{
    public enum QueryPrivilegeLevel
    {
        Undefined = -1,
        Public = 0,
        Voice = 1,
        Operator = 2,
        Admin = 3,
    }
}
