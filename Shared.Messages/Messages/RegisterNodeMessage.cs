using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Messages
{
    public class RegisterNodeMessage
    {
        public RegisterNodeMessage(IActorRef nodeActor, int cores)
        {
            NodeActor = nodeActor;
            Cores = cores;
        }

        public IActorRef NodeActor { get; private set; }
        public int Cores { get; private set; }
    }
}
