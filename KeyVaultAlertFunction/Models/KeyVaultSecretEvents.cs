using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultAlertFunction.Models
{
    public class KeyVaultSecretEvents
    {
        public string Id { get; set; }
        public string VaultName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string Version { get; set; }
        public long? NBF { get; set; }
        public long? EXP { get; set; }
    }
}
