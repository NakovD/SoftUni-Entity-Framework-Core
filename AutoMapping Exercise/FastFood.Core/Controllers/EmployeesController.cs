namespace FastFood.Core.Controllers
{
    using System;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Employees;

    public class EmployeesController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public EmployeesController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var positions = _context.Positions
                .ProjectTo<RegisterEmployeeViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(positions);
        }

        [HttpPost]
        public IActionResult Register(RegisterEmployeeInputModel model)
        {
            var employee = _mapper.Map<Employee>(model);

            _context.Employees.Add(employee);

            _context.SaveChanges();

            return RedirectToAction(nameof(this.All), "Employees");

        }

        public IActionResult All()
        {
            var employees = _context.Employees
                .ProjectTo<EmployeesAllViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(employees);
        }
    }
}
