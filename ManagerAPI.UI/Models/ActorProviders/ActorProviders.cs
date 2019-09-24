using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerAPI.UI.Models.ActorProviders
{
    public delegate IActorRef LeaderActorProvider();
}
