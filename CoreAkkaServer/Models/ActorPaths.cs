using System;
using System.Collections.Generic;
using System.Text;

namespace CoreAkkaServer.Models
{
    public static class ActorPaths
    {
        //SERVER
        public static readonly ActorMetaData NodeActor = new ActorMetaData("nodeActor", "akka.tcp://ClientActorSystem@localhost:9000/user/nodeActor");
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
