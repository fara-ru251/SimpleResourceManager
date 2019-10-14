using Akka.Actor;
using ManagerAPI.UI.Models.ActorSystemConfig;
using ManagerAPI.UI.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerAPI.UI.Models
{
    public static class AkkaStartupTasks
    {
        public static ActorSystem StartAkka()
        {

            SystemActors.WebActorSystem = ActorSystem.Create("ServerActorSystem", ConfigurationLoader.Load());

            SystemActors.LeaderActor = SystemActors.WebActorSystem.ActorOf(Props.Create(() => new LeaderActor()), "leader");

            return SystemActors.WebActorSystem;
        }
    }
}
