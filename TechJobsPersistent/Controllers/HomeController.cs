using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechJobsPersistent.Controllers
{
    public class HomeController : Controller
    {
        private JobDbContext context;

        public HomeController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();

            return View(jobs);
        }

        [HttpGet("/Add")]
        public IActionResult AddJob()
        {
            List<Employer> employers = context.Employers.ToList();
            List<Skill> possibleSkills = context.Skills.ToList();
            
            AddJobViewModel addJobViewModel = new AddJobViewModel(employers, possibleSkills);

            return View(addJobViewModel);
        }

        [HttpPost]
        public IActionResult ProcessAddJobForm(AddJobViewModel addJobViewModel, string[] selectedSkills)
        {

            if (ModelState.IsValid)
            {
                Job newJob = new Job
                {
                    Name = addJobViewModel.Name,
                    EmployerId = addJobViewModel.EmployerId
                };

                foreach (var skill in selectedSkills)
                {
                    int jobId = newJob.Id;
                    int skillId = int.Parse(skill);

                    List<JobSkill> existingItems = context.JobSkills
                        .Where(js => js.JobId == jobId)
                        .Where(js => js.SkillId == skillId)
                        .ToList();

                    if (existingItems.Count == 0)
                    {
                        JobSkill jobSkill = new JobSkill
                        {
                            JobId = jobId,
                            SkillId = skillId,
                            Job = newJob,
                            Skill = context.Skills.Find(skillId)
                        };

                        context.JobSkills.Add(jobSkill);

                    }
                }

                context.Jobs.Add(newJob);
                context.SaveChanges();

                return Redirect("/Job");
            }

                return View(addJobViewModel);
            
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }
    }
}
