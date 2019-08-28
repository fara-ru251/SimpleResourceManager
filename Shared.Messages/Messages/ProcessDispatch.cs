using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Messages
{
    public class ProcessDispatch
    {
        public ProcessDispatch(ProcessInfo processInfo)
        {
            _processInfo = processInfo;
        }

        public ProcessInfo _processInfo;
    }
}
