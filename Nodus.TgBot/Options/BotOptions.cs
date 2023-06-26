using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.TgBot.Options
{
    internal class BotOptions
    {
        public string Token { get; set; }
        public string AdminDbConnectionString { get; set; }
        public string AuthServiceURI { get; set; }
        public string NodusApiURI { get; set; }
    }
}
