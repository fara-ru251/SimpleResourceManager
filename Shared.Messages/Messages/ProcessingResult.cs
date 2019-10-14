﻿using Shared.Messages.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages.Messages
{
    public class ProcessingResult
    {
        public ProcessingResult(TimeSpan interval, int? exitCode = null, string output = null)
        {
            this.ExitCode = exitCode;
            this.Output = output;
            this.TimeSpan = interval;
        }

        public TimeSpan TimeSpan { get; set; }
        public string Output { get; private set; }
        public int? ExitCode { get; private set; }
    }
}
