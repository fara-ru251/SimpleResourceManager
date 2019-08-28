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

        /// <summary>
        /// To register self in "ParentNode"
        /// </summary>
        
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
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://ClientActorSystem@localhost:8090/user/leader"); //leaderActor


        public NodeActor(IActorRef processCoordinatorActor)
        {
            _processCoordinatorActor = processCoordinatorActor;


            //var sel = Context.System.ActorSelection("");
            //sel.Tell(new RegisterNodeMessage() { NodeActor = Self, Cores = Environment.ProcessorCount - 1 });

            Receive<ProcessDispatch>(dispatch =>
            {
                _processCoordinatorActor.Tell(new ProcessCoordinatorActor.ProcessRun(dispatch._processInfo));
            });


            //Receive<ShutDown>(shutdown => 
            //{
            //    Context.System.Terminate();
            //});

            Receive<ProcessCoordinatorActor.ProcessComplete>(processComplete =>
            {
                //WARNING
                //Context.ActorSelection("leaderPath").Tell(new NodeFinishedJob(processComplete.ReleasedProcesses, Self.Path.ToString()));
                Console.WriteLine("Saying to leader that process complete...");

                _server.Tell(new NodeFinishedJob(processComplete.ReleasedProcesses, Self));
            });


            Receive<string>(str => str == "registerYourself", register =>
            {
                Console.WriteLine("Connecting to server....");


                _server.Tell(new RegisterNodeMessage(Self, Environment.ProcessorCount / 2), Self);
            });
        }
    }
}
