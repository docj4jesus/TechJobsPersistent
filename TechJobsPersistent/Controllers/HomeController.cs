﻿using System;
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
            AddJobViewModel addJobViewModel = new AddJobViewModel();

            return View(addJobViewModel);
        }

        public IActionResult ProcessAddJobForm(AddJobViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                string name = viewModel.Name;
                int employerId = viewModel.EmployerId;

                List<Job> existingItems = context.Jobs
                    .Where(js => js.Name == name)
                    .Where(js => js.EmployerId == employerId)
                    .ToList();

                if (existingItems.Count == 0)
                {
                    Job job = new Job
                    {
                        Name = name,
                        EmployerId = employerId
                    };
                    context.Jobs.Add(job);
                    context.SaveChanges();
                }

                return Redirect("/Home/Detail/" + name);
            }

            return View(viewModel);
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
