using Akka.Actor;
using Akka.Configuration;
using CoreAkkaServer.Actors;
using CoreAkkaServer.Models;
using Shared.Messages.Models;
using System;
using System.IO;

namespace CoreAkkaServer
{
    class Program
    {
        public static ActorSystem ServerActorSystem; //server, but namespace is same as in client
        //THIS IS SERVER

        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
    akka {  
            #log-config-on-start = on
            #stdout-loglevel = DEBUG
            #loglevel = DEBUG
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        
                #debug {  
                #  receive = on 
                #  autoreceive = on
                #  lifecycle = on
                #  event-stream = on
                #  unhandled = on
                #}
                #deployment {
                #    /nodeActor {
                #        router = round-robin-pool
                #        #nr-of-instances = 5
                #        remote = ""akka.tcp://ClientActorSystem@localhost:8080""
                #    }
                #}
            }
            serialization-bindings {
                ""System.Object"" = wire
            }
            remote {
                dot-netty.tcp {
		            port = 8090
		            hostname = localhost
                }
            }
        }
        ");



            ServerActorSystem = ActorSystem.Create("ClientActorSystem", config); //name remains as in the client

            var leaderActor = ServerActorSystem.ActorOf(Props.Create(() => new LeaderActor()), "leader");


            int cnt = 1;

            while (true)
            {

                string str = Console.ReadLine();

                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }


                if (str == "exit")
                {
                    break;
                }


                //IMPROVE
                leaderActor.Tell(new LeaderActor.CanAcceptJob(new ProcessInfo(1, new Param(Path.GetDirectoryName(str)), str, $"task_{cnt++}")));

            }

            ServerActorSystem.Terminate();

        }
    }
}
