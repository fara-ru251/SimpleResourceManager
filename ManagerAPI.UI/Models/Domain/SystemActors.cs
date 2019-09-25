using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerAPI.UI.Models.Domain
{
    public static class SystemActors
    {
        public static ActorSystem WebActorSystem;

        public static IActorRef LeaderActor = ActorRefs.Nobody;
    }

}
