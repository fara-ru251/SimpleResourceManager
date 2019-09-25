using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using ManagerAPI.UI.Models.ActorProviders;
using ManagerAPI.UI.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages.Models;

namespace ManagerAPI.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        //private readonly IActorRef _leaderActor;


        //public TasksController(LeaderActorProvider leaderActorProvider)
        //{
        //    _leaderActor = leaderActorProvider();
        //}

        private readonly IActorRef _leaderActor = SystemActors.LeaderActor;

        //public async Task<IActionResult> Post([FromBody] ProcessInfo info)
        [HttpPost]
        public async Task<IActionResult> Post(int cores, string path, string name, int timeout)
        {
            //Debug.WriteLine(string.Format("{0} {1} {2}",info.TaskName,info._requiredCores, info.Timeout));

            var result = await _leaderActor.Ask<Guid>(new LeaderActor.StashForPending(new ProcessInfo(cores, path, name, param: new Param(Path.GetDirectoryName(path)))));
            return Ok(result);
        }

        /*
         * {
              "_requiredCores": 1,
              "_params": {
                "directory": "D:\Akka.NET_Testing\cpu-z_1.90-en"
              },
              "_exePath": "D:\Akka.NET_Testing\cpu-z_1.90-en\cpuz_x64.exe",
              "TaskName": "Task1",
              "Timeout": 10
            }
         */

    }
}