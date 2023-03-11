namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Items;

    public class ItemsController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public ItemsController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories
                .ProjectTo<CreateItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(categories);
        }

        [HttpPost]
        public IActionResult Create(CreateItemInputModel model)
        {
            var item = _mapper.Map<Item>(model);

            _context.Items.Add(item);

            _context.SaveChanges();

            return RedirectToAction(nameof(this.All), "Items");
        }

        [HttpGet]
        public IActionResult All()
        {
            var items = _context.Items
                .ProjectTo<ItemsAllViewModels>(_mapper.ConfigurationProvider)
                .ToList();

            return View(items);
        }
    }
}
