using Akka.Actor;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaClient.Actors
{
    public class ObsoleteProcessActor : ReceiveActor
    {
        #region Messages To Handle

        public class Start
        {

        }

        public class Cancel
        {

        }

        public class Completed
        {
            public Completed(int exitCode)
            {
                ExitCode = exitCode;
            }
            public int ExitCode { get; private set; }
        }

        public class CheckForExit
        {

        }

        #endregion

        private readonly ProcessInfo _processInfo;
        private readonly Func<Process> _processGenerator;
        private Process _process;
        private readonly ICancelable _cancelRepeating;
        private bool _processExited;

        public ObsoleteProcessActor(ProcessInfo processInfo, Func<Process> processGenerator)
        {
            _processInfo = processInfo;
            _processGenerator = processGenerator;
            _cancelRepeating = new Cancelable(Context.System.Scheduler);
            _processExited = false;
            //FIRST ATTEMPT - FAIL
            //Receive<Start>(start =>
            //{
            //    //HandleProcessStart();
            //});

            Receive<Start>(start => RunProcess());


            Receive<CheckForExit>(check => CheckExistence());
            //BLOCKING UNTIL THE TASK<T> IS RETURNED
            //ReceiveAsync<Start>(async start => await RunProcessAsync());

            //MAY BE WRONG OR NOT
            //Receive<Start>(start => RunProcessAsync().PipeTo(Self));


            Receive<Cancel>(cancel =>
            {
                // cancel soon;
            });

            //SENDS FROM ITSELF
            Receive<Completed>(cancel =>
            {
                Context.System.Terminate();
                //Context.ActorSelection("akka://ClientActorSystem/user/nodeActor").Tell(new NodeActor.ShutDown());
            });
        }


        #region Individual Message Type Handlers

        //TEST_METHOD NOT WORKING
        //private void HandleProcessStart()
        //{

        //}


        private void RunProcess()
        {
            _process.Start();



            //string output = _process.StandardOutput.ReadToEnd();


            //Thread.Sleep(1000);
            //if (_process.HasExited)
            //{
            //    Console.WriteLine("HasExited says true");
            //}

            //сомнительно только для оконных приложений
            //_process.WaitForExit();


            //Console.WriteLine(output);
        }

        private void CheckExistence()
        {
            if (_process.HasExited)
            {
                if (_processExited == true)
                {
                    return;
                }

                var process_result = new Models.ProcessResult();
                process_result.SetProcessResult(_process.ExitTime - _process.StartTime, true, _process.ExitCode);

                Context.Parent.Tell(new ProcessCoordinatorActor.ProcessComplete(_processInfo._requiredCores, process_result));
                _processExited = true;
                _cancelRepeating.Cancel();
                Self.Tell(PoisonPill.Instance);
            }
        }




        #endregion

        #region Actor lifecycle methods

        protected override void PreStart()
        {
            _process = _processGenerator();
            _process.StartInfo = new ProcessStartInfo(_processInfo._exePath) { UseShellExecute = true };//{ UseShellExecute = false,CreateNoWindow = false, RedirectStandardOutput = true };
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(2000), Self, new CheckForExit(), Self, _cancelRepeating);

            base.PreStart();
        }


        protected override void PostStop()
        {
            try
            {

                //when added scheduler, stop it here
                _process.Close();
                _process.Dispose();
                //_cancelRepeating.Cancel();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                base.PostStop();
            }
        }

        #endregion





        #region Asynchronous Process Methods

        /*
         * нужно использовать HasExited для проверки процесса
         * использовать Scheduler для того чтобы каждые N мс проверять состояние процесса и отправлять самому себе сообщение, при удачи сделать cancel 
         */

        //private Task<Completed> RunProcessAsync()
        //{



        //    var tcs = new TaskCompletionSource<Completed>();


        //    using (Process myProcess = new Process())
        //    {
        //        myProcess.StartInfo = new ProcessStartInfo(_processInfo._exePath) { UseShellExecute = false, RedirectStandardOutput = true };

        //        myProcess.EnableRaisingEvents = true;
        //        myProcess.Exited += (sender,args) => 
        //        {
        //            tcs.SetResult(new Completed(myProcess.ExitCode));
        //        };
        //        myProcess.Start();

        //        string output = myProcess.StandardOutput.ReadToEnd();


        //        //сомнительно
        //        myProcess.WaitForExit();


        //        Console.WriteLine(output);


        //    }
        //    return tcs.Task;
        //}


        //private void MyProcess_Exited(object sender, EventArgs e)
        //{
        //    Self.Tell(new Completed());
        //}


        #endregion

    }
}
