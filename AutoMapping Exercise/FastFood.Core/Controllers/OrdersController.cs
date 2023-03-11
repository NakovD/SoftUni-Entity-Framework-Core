namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Orders;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = _context.Items.Select(x => x.Id).ToList(),
                Employees = _context.Employees.Select(x => x.Id).ToList(),
            };

            return View(viewOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        {
            var order = _mapper.Map<Order>(model);

            _context.Orders.Add(order);

            _context.SaveChanges();

            var orderId = order?.Id;

            if (orderId == null) return RedirectToAction("Error", "Home");

            var orderItem = _mapper.Map<OrderItem>(model);

            orderItem.OrderId = (int)orderId;

            _context.OrderItems.Add(orderItem);

            _context.SaveChanges();

            return RedirectToAction(nameof(this.All), "Orders");
        }

        [HttpGet]
        public IActionResult All()
        {
            var orders = _context.Orders
                .ProjectTo<OrderAllViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(orders);
        }
    }
}
