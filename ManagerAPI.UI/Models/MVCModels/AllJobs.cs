using Shared.Messages.Messages;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerAPI.UI.Models.MVCModels
{
    public class AllJobs
    {
        public Guid Identifier { get; private set; }
        public string Status { get; private set; }
        public ProcessingResult Result { get; private set; }

        public AllJobs(Guid guid, string status, ProcessingResult result = null)
        {
            this.Identifier = guid;
            this.Status = status;
            this.Result = result;
        }

    }
}
