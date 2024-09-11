using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetApp.Common
{
    public sealed class EncryptionOptions
    {
        public string Key { get; set; }
        public string Iv { get; set; }
    }
}
