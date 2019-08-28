using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaClient
{
    public static class ActorPaths
    {
        public static readonly ActorMetaData CoordinatorActor = new ActorMetaData("coordinatorActor", "akka://ClientActorSystem/user/coordinatorActor");
        public static readonly ActorMetaData NodeActor = new ActorMetaData("nodeActor", "akka://ClientActorSystem/user/nodeActor");
        //public static readonly ActorMetaData LeaderActor = new ActorMetaData("leader", "akka.tcp://ClientActorSystem@localhost:8090/user/leader");
    }

    public class ActorMetaData
    {
        public ActorMetaData(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; private set; }

        public string Path { get; private set; }
    }
}
