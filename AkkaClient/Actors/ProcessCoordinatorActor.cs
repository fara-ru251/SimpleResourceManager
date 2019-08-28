using Akka.Actor;
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
        /// <summary>
        /// To run process
        /// </summary>
        public class ProcessRun
        {
            public ProcessRun(ProcessInfo processInfo)
            {
                _processInfo = processInfo;
            }

            public ProcessInfo _processInfo { get; private set; }
        }

        /// <summary>
        /// Indicates that process completed
        /// </summary>
        public class ProcessComplete
        {
            public ProcessComplete(int releasedProcesses)
            {
                ReleasedProcesses = releasedProcesses;
            }
            public int ReleasedProcesses;
        }
        
        #endregion


        private IList<IActorRef> _processActors;

        public ProcessCoordinatorActor()
        {
            _processActors = new List<IActorRef>();


            //приходит с главного актора (Node)
            Receive<ProcessRun>(process =>
            {
                //Props processorActorProps = Props.Create<ProcessActor>(process._processInfo);
                Props processorActorProps = Props.Create(() => new ProcessActor(process._processInfo, () => new Process()));
                var processActor = Context.ActorOf(processorActorProps);

                _processActors.Add(processActor);
                processActor.Tell(new ProcessActor.Start());
            });

            //приходит с дочернего актора
            Receive<ProcessComplete>(processStop => 
            {
                _processActors.Remove(Sender);


                //WARNING
                Context.ActorSelection(ActorPaths.NodeActor.Path).Tell(processStop);
            });
        }

    }
}
