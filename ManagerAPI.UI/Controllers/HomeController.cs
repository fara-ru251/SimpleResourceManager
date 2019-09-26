using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using ManagerAPI.UI.Models.Domain;
using ManagerAPI.UI.Models.MVCModels;
using Microsoft.AspNetCore.Mvc;

namespace ManagerAPI.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActorRef _leaderActor = SystemActors.LeaderActor;

        public async Task<IActionResult> Index()
        {
            var jobs = await _leaderActor.Ask<IEnumerable<AllJobs>>(new LeaderActor.GetAllJobs());

            return View(jobs);
        }
    }
}