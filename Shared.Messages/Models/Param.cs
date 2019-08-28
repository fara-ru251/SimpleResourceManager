using Shared.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Shared.Messages.Models
{
    public class Param : IParamBase
    {
        public string Directory { get; set; }
        public ProcessPriorityClass Priority { get; set; }


        public Param(string directory, ProcessPriorityClass processPriority = ProcessPriorityClass.Normal)
        {
            this.Directory = Directory;
            Priority = processPriority;
        }
    }
}
