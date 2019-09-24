using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Messages
{
    public class NodeFinishedJob
    {
        public NodeFinishedJob(int _releasedProcesses, IActorRef _actorPath, Guid guid, ProcessingResult result)
        {
            ReleasedProcesses = _releasedProcesses;
            ActorPath = _actorPath;
            Guid = guid;
            Result = result;
        }

        public Guid Guid { get; private set; }
        public int ReleasedProcesses;
        public ProcessingResult Result { get; private set; }
        public IActorRef ActorPath;
    }
}
