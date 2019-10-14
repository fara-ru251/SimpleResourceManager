using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaClient.Models
{
    public class ProcessResult
    {
        public bool Completed { get; private set; }
        public int? ExitCode { get; private set; }
        public string Output { get; private set; }
        public TimeSpan TimeSpan { get; private set; }


        public ProcessResult()
        {
            this.Completed = false;
            this.ExitCode = default(int?);
            this.Output = default(string);
            this.TimeSpan = TimeSpan.Zero;
        }


        public void SetProcessResult(TimeSpan interval, bool? completed = null, int? exitCode = null, string output = null)
        {
            this.TimeSpan = interval;
            this.Completed = completed ?? this.Completed;
            this.ExitCode = exitCode ?? this.ExitCode;
            this.Output = output ?? this.Output;
        }
    }
}
