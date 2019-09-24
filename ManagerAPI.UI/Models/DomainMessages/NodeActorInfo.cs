using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerAPI.UI.Models.DomainMessages
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

        public int AvailableProcesses
        {
            get { return TotalProcesses - CurrentProcesses; }
        }


        //initial
        public NodeActorInfo(IActorRef _actorPath, double _totalCores)
        {
            this.ActorPath = _actorPath;
            this.TotalCores = _totalCores;
            this.TotalProcesses = (int)_totalCores * 2; // two processes per one core

            this.InProcessCores = 0;
            this.CurrentProcesses = 0;
        }




        public void IncrementCoreAndProcess(double _coreDelta = 1, int _processDelta = 1)
        {
            if (this.AvailableCores + _coreDelta <= this.TotalCores && this.AvailableProcesses + _processDelta <= this.TotalProcesses)
            {
                this.InProcessCores -= _coreDelta;
                this.CurrentProcesses -= _processDelta;
                return;
            }


            Console.WriteLine("SHOULD NOT HAPPEN");
        }


        public void DecrementCoreAndProcess(double _coreDelta = 1, int _processDelta = 1)
        {
            if (this.AvailableCores - _coreDelta >= 0.0 && this.AvailableProcesses - _processDelta >= 0)
            {
                this.InProcessCores += _coreDelta;
                this.CurrentProcesses += _processDelta;
                return;
            }

            Console.WriteLine("SHOULD NOT HAPPEN");
        }

        #region Old Code
        //no need
        //private NodeActorInfo(IActorRef _actorPath, double _totalCores, double _inProcessCores = 0, int _currentProcesses = 0)
        //{
        //    this.ActorPath = _actorPath;
        //    this.TotalCores = _totalCores;
        //    this.TotalProcesses = (int)_totalCores * 2; // two processes per one core

        //    this.InProcessCores = _inProcessCores;
        //    this.CurrentProcesses = _currentProcesses;
        //}


        //public NodeActorInfo IncrementCoreAndProcess(double _coreDelta = 1, int _processDelta = 1)
        //{
        //    return Copy(_inProcessCores: this.InProcessCores + _coreDelta, _currentProcesses: this.CurrentProcesses + _processDelta);
        //}


        //public NodeActorInfo DecrementCoreAndProcess(double _coreDelta = 1, int _processDelta = 1)
        //{
        //    return Copy(_inProcessCores: this.InProcessCores - _coreDelta, _currentProcesses: this.CurrentProcesses - _processDelta);
        //}



        ////unused
        //private NodeActorInfo Copy(double? _inProcessCores, int? _currentProcesses)
        //{
        //    return new NodeActorInfo(ActorPath, TotalCores, 
        //        _inProcessCores : _inProcessCores ?? InProcessCores,
        //        _currentProcesses: _currentProcesses.HasValue ? _currentProcesses.Value : CurrentProcesses
        //        );
        //}
        #endregion
    }
}
