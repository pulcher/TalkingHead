using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Magic8HeadService.Services
{
    public class CoolDownOptions
    {
        public Dictionary<string, int> Options { get; set; }
    }
}
