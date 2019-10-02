using Akka.Actor;
using AkkaClient.Models;
using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AkkaClient.Actors
{
    public class AsyncProcessActor : ReceiveActor
    {
        #region Messages

        public class Start
        {
        }


        #endregion

        private readonly ProcessInfo _processInfo;
        private readonly Func<Process> _processGenerator;
        private Process _process; // assigning value on starting of actor

        public AsyncProcessActor(ProcessInfo processInfo, Func<Process> processGenerator)
        {
            _processGenerator = processGenerator;
            _processInfo = processInfo;


            Waiting();

        }

        private void Waiting()
        {
            Receive<Start>(start => 
            {
                Become(Working);
                ExecuteShellCommandAsync().PipeTo(Self);
            });
        }

        private void Working()
        {

            //приходит с самого себя
            Receive<ProcessResult>(result =>
            {
                //TODO
                Context.Parent.Tell(new ProcessCoordinatorActor.ProcessComplete(_processInfo._requiredCores, result));

                //destruct yourself
                Self.Tell(PoisonPill.Instance);


            });
        }


        #region Actor Lifecycle Methods

        protected override void PreStart()
        {
            _process = _processGenerator();


            _process.StartInfo = new ProcessStartInfo(_processInfo._exePath)
            {
                Arguments = _processInfo._params.Arguments, // dafault value = NULL
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = new FileInfo(_processInfo._exePath).Directory.FullName
            };

            base.PreStart();
        }

        protected override void PostStop()
        {
            if (_process != null)
            {
                _process.Close();
                _process.Dispose();
            }

            base.PostStop();
        }
        #endregion


        #region Asynchronous Process Methods

        private async Task<ProcessResult> ExecuteShellCommandAsync()
        {
            var result = new ProcessResult();

            using (_process)
            {
                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();


                //надо вынести в метод
                _process.OutputDataReceived += (s, e) =>
                {
                    // The output stream has been closed i.e. the process has terminated
                    if (e.Data == null)
                    {
                        outputCloseEvent.SetResult(true);
                    }
                    else
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                //this will be too in a method
                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                _process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        errorCloseEvent.SetResult(true);
                    }
                    else
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                bool isStarted;
                
                try
                {
                    isStarted = _process.Start();
                }
                catch (Exception error)
                {
                    result.SetProcessResult(completed: true, exitCode: -1, output: error.Message);
                    isStarted = false;
                    Console.WriteLine("---------------Started----------------");
                    Console.WriteLine(error.Message);
                    Console.WriteLine("======================================");
                    Console.WriteLine(error.InnerException != null ? error.InnerException.Message : string.Empty);
                    Console.WriteLine("======================================");
                    Console.WriteLine(error.StackTrace);
                    Console.WriteLine("---------------Finished---------------");
                }



                if (isStarted)
                {
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();

                    var time_out = (int)TimeSpan.FromMinutes(_processInfo.Timeout).TotalMilliseconds;


                    var waitForExit = WaitForExitAsync(_process, time_out);


                    //waiting for termination of all tasks
                    var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);


                    if (await Task.WhenAny(Task.Delay(time_out), processTask) == processTask && waitForExit.Result)
                    {
                        result.SetProcessResult(completed: true, exitCode: _process.ExitCode, output: outputBuilder.ToString());


                        //if process exit code other than zero => means error
                        if (_process.ExitCode != 0)
                        {
                            result.SetProcessResult(output: $"{outputBuilder}{errorBuilder}");
                        }
                    }
                    else
                    {
                        try
                        {
                            result.SetProcessResult(completed: false, output: "Process hung, so was killed by timeout");

                            _process.Kill();
                        }
                        catch
                        {
                            //nothing to do
                        }
                    }
                }
            }

            return result;
        }

        //method used by "ExecuteShellCommand"
        private Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            //MAX timeout 596 hours => 596 * 60 minutes
            return Task.Run(() => process.WaitForExit(timeout));
        }


        #endregion

    }
}
