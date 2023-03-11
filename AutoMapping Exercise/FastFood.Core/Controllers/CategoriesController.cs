namespace FastFood.Core.Controllers
{
    using System;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryInputModel model)
        {
            var mappedModel = _mapper.Map<Category>(model);

            _context.Categories.Add(mappedModel);

            _context.SaveChanges();

            return RedirectToAction(nameof(this.All), "Categories");
        }

        [HttpGet]
        public IActionResult All()
        {
            var result = _context
                .Categories
                .ProjectTo<CategoryAllViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(result);
        }
    }
}
