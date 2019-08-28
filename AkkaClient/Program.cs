using Akka.Actor;
using AkkaClient.Actors;
using System;
using System.Diagnostics;
using System.IO;
using Akka.Routing;
using Akka.Configuration;

namespace AkkaClient
{
    class Program
    {
        public static ActorSystem ClientActorSystem;


        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
            akka {  
                actor {
                    provider = remote
                }
                serialization-bindings {
                    ""System.Object"" = wire
                }
                remote {
                    dot-netty.tcp {
		                port = 0
		                hostname = localhost
                    }
                }
            }
            ");


            ClientActorSystem = ActorSystem.Create("ClientActorSystem", config);


            var processCoordinatorActor = ClientActorSystem.ActorOf(Props.Create(() => new ProcessCoordinatorActor()), "coordinatorActor");

            var nodeActor = ClientActorSystem.ActorOf(Props.Create(() => new NodeActor(processCoordinatorActor)), "nodeActor");

            nodeActor.Tell("registerYourself");


            //example
            //string path = @"D:\app_for_sabina\OBK\OBKApplications\00da66d1-f9a1-4366-af95-f49b090ffac7.txt"; //@"C:\MinGW\Новая\a.exe";

            //nodeActor.Tell(new NodeActor.ProcessDispatch(new ProcessInfo(2, new Param(Path.GetDirectoryName(path)), path, "test_task")));

            //wait until terminate is affected
            ClientActorSystem.WhenTerminated.Wait();
        }

    }
}
