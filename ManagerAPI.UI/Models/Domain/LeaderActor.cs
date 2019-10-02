using Akka.Actor;
using ManagerAPI.UI.Models.DomainMessages;
using ManagerAPI.UI.Models.MVCModels;
using Shared.Messages.Messages;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerAPI.UI.Models.Domain
{
    public class LeaderActor : ReceiveActor /*, IWithUnboundedStash*/
    {

        #region Messages

        public class CanAcceptJob
        {
            public CanAcceptJob(KeyValuePair<Guid, ProcessInfo> kvp)
            {
                _keyValuePair = kvp;
            }

            public KeyValuePair<Guid, ProcessInfo> _keyValuePair { get; private set; }
        }

        public class DispatchTo
        {
            public DispatchTo(KeyValuePair<Guid, ProcessInfo> kvp, IActorRef actorPath)
            {
                _keyValuePair = kvp;
                _actorPath = actorPath;
            }

            public IActorRef _actorPath { get; private set; }
            public KeyValuePair<Guid, ProcessInfo> _keyValuePair { get; private set; }
        }

        public class StashForPending
        {
            public StashForPending(ProcessInfo processInfo)
            {
                _processInfo = processInfo;
            }
            public ProcessInfo _processInfo { get; private set; }
        }

        public class CheckForPending
        {
            private CheckForPending() { }

            public static CheckForPending Instance { get; } = new CheckForPending();
        }

        public class GetAllJobs { }

        #endregion

        public Dictionary<Guid, ProcessInfo> _pendingJobs { get; private set; }
        private HashSet<NodeActorInfo> _nodeInfoList { get; set; }
        public Dictionary<Guid, ProcessInfo> _runningJobs { get; private set; }
        public Dictionary<Guid, ProcessingResult> _finishedJobs { get; private set; }
        private ICancelable _cancelPending { get; set; }

        //public IStash Stash { get; set; }

        public LeaderActor()
        {
            _nodeInfoList = new HashSet<NodeActorInfo>();
            _pendingJobs = new Dictionary<Guid, ProcessInfo>();
            _runningJobs = new Dictionary<Guid, ProcessInfo>();
            _finishedJobs = new Dictionary<Guid, ProcessingResult>();
            _cancelPending = new Cancelable(Context.System.Scheduler);
            //maybe we do not need any state than this one (for a while)
            Ready();
        }

        private void Ready()
        {
            Receive<GetAllJobs>(jobs => 
            {
                var result = new List<AllJobs>();

                foreach (var kvp in _pendingJobs)
                {
                    result.Add(new AllJobs(kvp.Key,"Pending"));
                }

                foreach (var kvp in _runningJobs)
                {
                    result.Add(new AllJobs(kvp.Key, "Running"));
                }

                foreach (var kvp in _finishedJobs)
                {
                    result.Add(new AllJobs(kvp.Key, "Finished", result: kvp.Value));
                }

                Sender.Tell(result);
            });



            //when node is up, & is likely getting to join
            Receive<RegisterNodeMessage>(register =>
            {

                Debug.WriteLine($"Connection established with {register.NodeActor.Path}");
                //TODO Remove from "_nodeInfoList"
                _nodeInfoList.Add(new NodeActorInfo(Sender, register.Cores));
            });

            //via "CheckForPending" method
            Receive<CanAcceptJob>(CantAccept, job =>
            {
                //when message not accepted stash here
                //prepending to stack this message

                _pendingJobs.Add(job._keyValuePair.Key, job._keyValuePair.Value);
                //Stash.Stash();
            });

            //приходит с самого себя
            //when REALLY BEGIN TO RUN
            Receive<DispatchTo>(dispatch =>
            {
                //NOT RUN, have to do checking op.

                if (dispatch._actorPath == ActorRefs.Nobody)
                {
                    Console.WriteLine("Nobody to sent");
                    return;
                }
                
                Context.ActorSelection(dispatch._actorPath.Path).Tell(new ProcessDispatch(dispatch._keyValuePair));
            });


            //comes from node actor
            Receive<NodeFinishedJob>(finishedJob =>
            {
                foreach (var nodeInfo in _nodeInfoList)
                {
                    if (nodeInfo.ActorPath.Path == finishedJob.ActorPath.Path)
                    {
                        Debug.WriteLine($"Confirm that process was done by actor: {nodeInfo.ActorPath.Path}");
                        nodeInfo.IncrementCoreAndProcess(_coreDelta: (finishedJob.ReleasedProcesses / 2.0), _processDelta: finishedJob.ReleasedProcesses);


                        _runningJobs.Remove(finishedJob.Guid);
                        _finishedJobs.Add(finishedJob.Guid, finishedJob.Result);
                        
                        //next message processed
                        //Stash.Unstash();
                        return;
                    }
                }
            });


            Receive<StashForPending>(stash =>
            {
                var guid = Guid.NewGuid();
                _pendingJobs.Add(guid, stash._processInfo);

                Sender.Tell(guid);
            });

            //via Scheduler
            Receive<CheckForPending>(check => 
            {
                if (_pendingJobs.Count == 0)
                {
                    return;
                }

                var job = _pendingJobs.First();

                _pendingJobs.Remove(job.Key);

                Self.Tell(new CanAcceptJob(job));


            });

        }


        #region Predicate Method

        /// <summary>
        /// Inversely working method
        /// </summary>
        /// <param name="job">ProcessInfo that passed</param>
        /// <returns></returns>
        private bool CantAccept(CanAcceptJob job)
        {
            if (_nodeInfoList.Count == 0)
            {
                return true;
            }

            foreach (var nodeInfo in _nodeInfoList)
            {
                //gets the first node that fits our requirements
                //easy way 
                if ((nodeInfo.AvailableCores - (job._keyValuePair.Value._requiredCores / 2.0)) >= 0.0)
                {
                    //WARNING PLACE!!!
                    nodeInfo.DecrementCoreAndProcess(_coreDelta: (job._keyValuePair.Value._requiredCores / 2.0), _processDelta: job._keyValuePair.Value._requiredCores);

                    _runningJobs.Add(job._keyValuePair.Key, job._keyValuePair.Value);
                    //self tell to dispath to one of available nodes
                    Self.Tell(new DispatchTo(job._keyValuePair, nodeInfo.ActorPath));

                    return false;
                }
            }


            return true;
        }


        #endregion

        #region Actor LifeCycle Methods

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2),
                        Self, CheckForPending.Instance, Self, _cancelPending);

            base.PreStart();
        }

        protected override void PostStop()
        {
            _cancelPending.Cancel();
            base.PostStop();
        }

        #endregion



    }
}
