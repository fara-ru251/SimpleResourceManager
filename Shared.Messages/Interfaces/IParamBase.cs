using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Shared.Messages.Interfaces
{
    public interface IParamBase
    {
        string Directory { get; set; }
        ProcessPriorityClass Priority { get; set; }
    }
}
