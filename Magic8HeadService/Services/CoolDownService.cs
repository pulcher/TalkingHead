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
        private IOptions<CoolDownOptions> options;

        public CoolDownService(IOptions<CoolDownOptions> options)
        {
            this.options = options;
        }

        public DateTime Execute(string commandString)
        {
            var nextExecutionTime = DateTime.UtcNow.AddSeconds(options.Value.Options[commandString]);

            if (CurrentCoolsDowns.TryGetValue(commandString, out DateTime coolDownDateTime))
            {
                if (DateTime.UtcNow > coolDownDateTime)
                {
                    CurrentCoolsDowns[commandString]
                        = nextExecutionTime;
                }
                else
                {
                    return coolDownDateTime;
                }
            }
            else
            {
                CurrentCoolsDowns.Add(commandString, nextExecutionTime);
            }

            return nextExecutionTime;
        }

        public ReadOnlyDictionary<string, DateTime> GetAllCoolDowns()
        {
            var readOnlyCurrentCurrentCoolDowns = new ReadOnlyDictionary<string, DateTime>(CurrentCoolsDowns);
            return readOnlyCurrentCurrentCoolDowns;
        }

        public DateTime GetCurrentCoolDown(string commandString)
        {
            if (CurrentCoolsDowns.TryGetValue(commandString, out DateTime coolDownDateTime))
            {
                return coolDownDateTime;
            }

            return DateTime.MinValue;
        }
    }
}
