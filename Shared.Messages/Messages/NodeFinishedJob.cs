using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Messages
{
    public class NodeFinishedJob
    {
        public NodeFinishedJob(int _releasedProcesses, IActorRef _actorPath)
        {
            ReleasedProcesses = _releasedProcesses;
            ActorPath = _actorPath;
        }

        public int ReleasedProcesses;
        public IActorRef ActorPath;
    }
}
