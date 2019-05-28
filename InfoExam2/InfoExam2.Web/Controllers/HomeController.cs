using System;
using System.Linq;
using System.Net.Mime;
using InfoExam2.Core;
using InfoExam2.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace InfoExam2.Web.Controllers
{
	[Route("[controller]/[action]")]
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Authentificate()
		{
			return View("Login");
		}

//		[Authorize("Admin")]
		public IActionResult GetDishes()
		{
//			if (User == null || User.Claims.FirstOrDefault(x => x.Type == "admin")?.Value != bool.TrueString)
//				return View("Error");

			var dishes = _context.Dishes.ToArray();
			return Ok(dishes);
		}

		public IActionResult AddDish(string name, double cost, string description)
		{
//			if (User == null || User.Claims.FirstOrDefault(x => x.Type == "admin")?.Value != bool.TrueString)
//				return View("Error");
//
//			if (!double.TryParse(cost, out var result))
//				return View("Error");

			_context.Dishes.Add(new Dish
			{
				Name = name,
				Cost = cost,
				Description = description
			});
			_context.SaveChanges();
			return Ok();
		}

		public IActionResult RestarauntList()
		{
			return Ok(_context.Restaraunts.ToArray());
		}

		public IActionResult GetDishesFromRestaraunt(string restName)
		{
			var rest = _context.Restaraunts.FirstOrDefault(x => x.Name == restName) ??
			           throw new Exception("No such resta");

			var queryable = _context.Dishes.Where(x=>x.RestId == rest.Id);
			return View("RestDishes", queryable);
		}

		public IActionResult AddDishToRestaraunt(string dishName, string restName)
		{
			var rest = _context.Restaraunts.FirstOrDefault(x => x.Name == restName) ??
			           throw new Exception("No such resta");
			var dish = _context.Dishes.FirstOrDefault(x => x.Name == dishName) ??
			           throw new Exception("No such dish!");
			dish.RestId = rest.Id;
			_context.SaveChanges();
			return Ok();
		}
		
		public IActionResult DeleteDishFromRestaraunt(string dishName)
		{
			var dish = _context.Dishes.FirstOrDefault(x => x.Name == dishName)??
			           throw new Exception("No such dish");
			dish.RestId = 0;
			_context.SaveChanges();
			return Ok();
		}

		public IActionResult AddItem(Dish dish)
		{
			var user = new User();//_context.Users.FirstOrDefault(x => x.Login ==);
			var currentOrder = user.CurrentOrder == 0 ? new Order() : _context.Orders.Find(user.CurrentOrder);
			_context.SaveChanges();
			var orderItem = new OrderItem()
			{
				Dish = dish,
				OrderId =  currentOrder.Id
			};
			var orderItems = _context.OrderItems.Where(x => x.OrderId == currentOrder.Id).ToArray();
			foreach (var item in orderItems)
			{
				if (item.Dish.RestId != dish.RestId)
					_context.OrderItems.Remove(item);
			}
			_context.OrderItems.Add(orderItem);
			_context.SaveChanges();
			return Ok();
		}

		public IActionResult FinalizeOrder(string code)
		{
			var user = new User();
			var orderItems = _context.OrderItems.Where(x => x.OrderId == user.CurrentOrder).ToArray();
			var sum = orderItems.Sum(x => x.Dish.Cost);
			if (!string.IsNullOrEmpty(code))
			{
				var promoCode = _context.PromoCodes.FirstOrDefault(x => x.Code == code);
				if (promoCode != null)
					sum *= (1 - promoCode.Discount);
			}
			return View("Index", new {Sum = sum, ItemsList = orderItems}); //todo
		}
	}
}