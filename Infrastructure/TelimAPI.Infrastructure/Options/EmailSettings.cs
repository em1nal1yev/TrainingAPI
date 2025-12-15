using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Infrastructure.Options
{
    public class EmailSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; }
        public string From { get; set; } = null!;
    }
}
