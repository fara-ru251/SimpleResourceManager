using Akka.Actor;
using CoreAkkaServer.Models;
//using CoreAkkaServer.Models;
using Shared.Messages.Messages;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreAkkaServer.Actors
{
    public class LeaderActor : ReceiveActor, IWithUnboundedStash
    {

        #region Messages

        public class CanAcceptJob
        {
            public CanAcceptJob(ProcessInfo processInfo)
            {
                _processInfo = processInfo;
            }

            public ProcessInfo _processInfo;
        }

        public class DispatchTo
        {
            public DispatchTo(ProcessInfo processInfo, IActorRef actorPath)
            {
                _processInfo = processInfo;
                _actorPath = actorPath;
            }

            public IActorRef _actorPath;
            public ProcessInfo _processInfo;
        }

       

        #endregion


        public HashSet<NodeActorInfo> _nodeInfoList;

        public IStash Stash { get; set; }
        

        public LeaderActor()
        {
            _nodeInfoList = new HashSet<NodeActorInfo>();


            //may be we don't need any state than this one (for a while)
            Ready();
        }

        private void Ready()
        {

            //when node is up
            Receive<RegisterNodeMessage>(register => 
            {

                Console.WriteLine("Connection established");
                _nodeInfoList.Add(new NodeActorInfo(Sender, register.Cores));
            });


            //С КОНСОЛИ (ВВОЖУ Я)
            Receive<CanAcceptJob>(CantAccept, job => 
            {
                //when message not accepted stash here
                //prepending to stack this message
                Stash.Stash();
            });

            //приходит с самого себя
            Receive<DispatchTo>(dispatch => 
            {
                //NOT RUN, have to do checking op.
                //опасно
                Context.ActorSelection(dispatch._actorPath.Path).Tell(new ProcessDispatch(dispatch._processInfo));
            });


            //приходит с дочернего узла
            Receive<NodeFinishedJob>(finishedJob => 
            {
                foreach (var nodeInfo in _nodeInfoList)
                {
                    if (nodeInfo.ActorPath == finishedJob.ActorPath)
                    {
                        Console.WriteLine($"Confirm that process is done by actor: {nodeInfo.ActorPath.Path}");
                        nodeInfo.IncrementCoreAndProcess(_coreDelta: (finishedJob.ReleasedProcesses / 2.0), _processDelta: finishedJob.ReleasedProcesses);

                        //next message processed
                        Stash.Unstash();
                        return;
                    }
                }
            });
        }


        #region Predicate Method

        /// <summary>
        /// Inversely working method
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private bool CantAccept(CanAcceptJob job)
        {
            if (_nodeInfoList.Count == 0)
            {
                return false;
            }

            foreach (var nodeInfo in _nodeInfoList)
            {
                //gets the first node that fits our requirements
                //easy way
                if ((nodeInfo.AvailableCores - (job._processInfo._requiredCores / 2.0)) >= 0.0)
                {
                    //WARNING PLACE!!!
                    nodeInfo.DecrementCoreAndProcess(_coreDelta: (job._processInfo._requiredCores / 2.0), _processDelta: job._processInfo._requiredCores);
                    //self tell to dispath to one of available nodes
                    Self.Tell(new DispatchTo(job._processInfo, nodeInfo.ActorPath));

                    return false;
                }
            }


            return true;
        }


        #endregion

        #region Actor LifeCycle Methods


        #endregion



    }
}
