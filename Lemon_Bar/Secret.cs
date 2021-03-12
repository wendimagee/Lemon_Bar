using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lemon_Bar
{
    public class Secret
    {
        public static string connection { get; } = "Server = tcp:wendiserver.database.windows.net,1433;Initial Catalog = Lemon_Bar; Persist Security Info=False;User ID = wendiserveradmin; Password=Admin123; MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30";


        //wendiserveradmin
        //Admin123
    }
}
