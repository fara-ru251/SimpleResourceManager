using Akka.Actor;
using Shared.Messages.Messages;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AkkaClient.Actors
{
    public class NodeActor : ReceiveActor
    {
        #region Messages


        //IN SHARED
        //public class ProcessDispatch
        //{
        //    public ProcessDispatch(ProcessInfo processInfo)
        //    {
        //        _processInfo = processInfo;
        //    }

        //    public ProcessInfo _processInfo;
        //}

        public class ShutDown
        {

        }

        //public class NodeFinishedJob
        //{
        //    public NodeFinishedJob(int _releasedProcesses, string _actorPath)
        //    {
        //        ReleasedProcesses = _releasedProcesses;
        //        ActorPath = _actorPath;
        //    }

        //    public int ReleasedProcesses;
        //    public string ActorPath;
        //}

        #endregion


        private readonly IActorRef _processCoordinatorActor;
        //private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://ServerActorSystem@localhost:8090/user/leader"); //leaderActor
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://ServerActorSystem@192.168.154.23:8090/user/leader"); //leaderActor

        public NodeActor(IActorRef processCoordinatorActor)
        {
            _processCoordinatorActor = processCoordinatorActor;



            Receive<ProcessDispatch>(dispatch =>
            {
                _processCoordinatorActor.Tell(new ProcessCoordinatorActor.ProcessRun(dispatch._processInfo.Key, dispatch._processInfo.Value));
            });


            //Receive<ShutDown>(shutdown => 
            //{
            //    Context.System.Terminate();
            //});

            Receive<ProcessCoordinatorActor.SendToNode>(node =>
            {
                //WARNING
                //Context.ActorSelection("leaderPath").Tell(new NodeFinishedJob(processComplete.ReleasedProcesses, Self.Path.ToString()));
                Console.WriteLine($"Saying to leader that process {node._key} complete...");

                var processingResult = new ProcessingResult(node._processComplete.ProcessResult.ExitCode, node._processComplete.ProcessResult.Output);
                _server.Tell(new NodeFinishedJob(node._processComplete.ReleasedProcesses, Self, node._key, processingResult));
            });


            Receive<string>(str => str == "registerYourself", register =>
            {
                Console.WriteLine("Connecting to server....");


                _server.Tell(new RegisterNodeMessage(Self, Environment.ProcessorCount / 2), Self);
            });

            //TODO improve graceful shutdown
            Receive<ShutDown>(shutdown => 
            {
                Context.System.Terminate();
            });
        }
    }
}
