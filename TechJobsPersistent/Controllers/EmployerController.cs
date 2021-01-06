﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechJobsPersistent.Controllers
{
    public class EmployerController : Controller
    {
        private JobDbContext context;

        public EmployerController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Employer> employers = context.Employers.ToList();
            return View(employers);
        }

        public IActionResult Add()
        {
            AddEmployerViewModel addEmployerViewModel = new AddEmployerViewModel();

            return View(addEmployerViewModel);
        }

        [HttpPost]
        public IActionResult ProcessAddEmployerForm(AddEmployerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                string name = viewModel.Name;
                string location = viewModel.Location;

                List<Employer> existingItems = context.Employers
                    .Where(js => js.Name == name)
                    .Where(js => js.Location == location)
                    .ToList();

                if (existingItems.Count == 0)
                {
                    Employer employer = new Employer
                    {
                        Name = name,
                        Location = location
                    };
                    context.Employers.Add(employer);
                    context.SaveChanges();
                }

                return Redirect("/Home/Detail/" + name);
            }

            return View(viewModel);
        }
        
        public IActionResult About(int id)
        {
            List<Employer> employers = context.Employers
            .Where(js => js.Id == id)
            .Include(js => js.Name)
            .Include(js => js.Location)
            .ToList();

            return View(employers);
        }
    }
}
