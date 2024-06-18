using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic8HeadService.Services
{
    public class CoolDownService
    {
        private Dictionary<string, DateTime> CurrentCoolsDowns  
            = new Dictionary<string, DateTime>();
        private IOptionsSnapshot<CoolDownOptions> options;

        public CoolDownService(IOptionsSnapshot<CoolDownOptions> options)
        {
            this.options = options;
        }

        public void Execute(string commandString)
        {
            if (CurrentCoolsDowns.ContainsKey(commandString))
            {
                CurrentCoolsDowns[commandString] = DateTime.Now;
            }
            else
            {
                CurrentCoolsDowns.Add(commandString, DateTime.Now);
            }
        }

        public ReadOnlyDictionary<string, DateTime> GetAllCoolDowns()
        {
            var readOnlyCurrentCurrentCoolDowns = new ReadOnlyDictionary<string, DateTime>(CurrentCoolsDowns);
            return readOnlyCurrentCurrentCoolDowns;
        }
    }
}
