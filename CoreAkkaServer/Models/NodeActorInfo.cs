using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreAkkaServer.Models
{
    public class NodeActorInfo
    {

        public IActorRef ActorPath { get; private set; }

        public double TotalCores { get; private set; }
        public double InProcessCores { get; private set; } = 0;

        public int TotalProcesses { get; private set; }
        public int CurrentProcesses { get; private set; } = 0;

        public bool IsCoresAvailable
        {
            get { return TotalCores - InProcessCores > 0; }
        }

        public double AvailableCores
        {
            get { return TotalCores - InProcessCores; }
        }

        //initial
        public  NodeActorInfo(IActorRef _actorPath, double _totalCores)
        {
            this.ActorPath = _actorPath;
            this.TotalCores = _totalCores;
            this.TotalProcesses = (int)_totalCores * 2; // two processes per one core

            this.InProcessCores = 0;
            this.CurrentProcesses = 0;
        }



        private NodeActorInfo(IActorRef _actorPath, double _totalCores, double _inProcessCores = 0, int _currentProcesses = 0)
        {
            this.ActorPath = _actorPath;
            this.TotalCores = _totalCores;
            this.TotalProcesses = (int)_totalCores * 2; // two processes per one core

            this.InProcessCores = _inProcessCores;
            this.CurrentProcesses = _currentProcesses;
        }


        public NodeActorInfo IncrementCoreAndProcess(double _coreDelta = 1, int _processDelta = 1)
        {
            return Copy(_inProcessCores: this.InProcessCores + _coreDelta, _currentProcesses: this.CurrentProcesses + _processDelta);
        }


        public NodeActorInfo DecrementCoreAndProcess(double _coreDelta = 1, int _processDelta = 1)
        {
            return Copy(_inProcessCores: this.InProcessCores - _coreDelta, _currentProcesses: this.CurrentProcesses - _processDelta);
        }


        private NodeActorInfo Copy(double? _inProcessCores, int? _currentProcesses)
        {
            return new NodeActorInfo(ActorPath, TotalCores, 
                _inProcessCores : _inProcessCores ?? InProcessCores,
                _currentProcesses: _currentProcesses.HasValue ? _currentProcesses.Value : CurrentProcesses
                );
        }
    }
}
