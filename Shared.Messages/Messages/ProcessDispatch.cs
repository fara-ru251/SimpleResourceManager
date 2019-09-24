using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Messages
{
    public class ProcessDispatch
    {
        public ProcessDispatch(KeyValuePair<Guid, ProcessInfo> kvp)
        {
            _processInfo = kvp;
        }

        public KeyValuePair<Guid, ProcessInfo> _processInfo { get; private set; }
    }
}
