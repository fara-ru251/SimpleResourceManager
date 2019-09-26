using Akka.Actor;
using AkkaClient.Models;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AkkaClient.Actors
{
    public class ProcessCoordinatorActor : ReceiveActor
    {
        #region Messages

        public class ProcessRun
        {
            public ProcessRun(Guid key, ProcessInfo processInfo)
            {
                this.KeyGuid = key;
                _processInfo = processInfo;
            }

            public Guid KeyGuid { get; set; }
            public ProcessInfo _processInfo { get; private set; }
        }

        public class ProcessComplete
        {
            public ProcessComplete(int releasedProcesses, ProcessResult processResult)
            {
                ReleasedProcesses = releasedProcesses;
                ProcessResult = processResult;
            }
            public int ReleasedProcesses { get; private set; }
            public ProcessResult ProcessResult { get; private set; }
        }

        public class SendToNode
        {
            public SendToNode(ProcessComplete processComplete, Guid guid)
            {
                _processComplete = processComplete;
                _key = guid;
            }
            public ProcessComplete _processComplete { get; private set; }
            public Guid _key { get; private set; }
        }

        #endregion


        private Dictionary<IActorRef, Guid> _processActors;

        public ProcessCoordinatorActor()
        {
            _processActors = new Dictionary<IActorRef, Guid>();


            //приходит с главного актора (Node) 
            Receive<ProcessRun>(process =>
            {
                //Props processorActorProps = Props.Create<ProcessActor>(process._processInfo);

                Props processorActorProps = Props.Create(() => new AsyncProcessActor(process._processInfo, () => new Process()));
                var processActor = Context.ActorOf(processorActorProps);

                _processActors.Add(processActor, process.KeyGuid);
                processActor.Tell(new AsyncProcessActor.Start());
            });

            //приходит с дочернего актора
            Receive<ProcessComplete>(processStop =>
            {
                _processActors.Remove(Sender, out Guid key);


                //WARNING
                Context.ActorSelection(ActorPaths.NodeActor.Path).Tell(new SendToNode(processStop, key));
            });
        }

    }
}
