using AkkaClient.Interfaces;
using Shared.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Models
{
    public class ProcessInfo : IProcessBase
    {
        public ProcessInfo(int requiredCores, IParamBase param, string exePath, string taskName, int timeout = 10)
        {
            _requiredCores = requiredCores;
            _params = param;
            _exePath = exePath;
            TaskName = taskName;
            Timeout = timeout; //from minutes
        }

        public int _requiredCores { get; private set; }
        public IParamBase _params { get; private set; }
        public string _exePath { get; private set; }
        public string TaskName { get; private set; }
        public int Timeout { get; private set; }
    }
}
