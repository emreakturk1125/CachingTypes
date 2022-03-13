using Microsoft.AspNetCore.Mvc;
using Redis2.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis2.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _database;
        private string listKey = "names";

        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _database = _redisService.GetDb(0);
        }

        public IActionResult Index()
        {
            List<string> nameList = new List<string>();
            if (_database.KeyExists(listKey))
            {
                _database.ListRange(listKey).ToList().ForEach(x =>
                {
                    nameList.Add(x.ToString());
                });
            }
            return View(nameList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            _database.ListRightPush(listKey, name);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(string name)
        {
            _database.ListRemoveAsync(listKey, name).Wait();
            return RedirectToAction("Index");

        }

        public IActionResult RemoveFirstItem()
        {
            _database.ListLeftPop(listKey);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveLastItem()
        {
            _database.ListRightPop(listKey);
            return RedirectToAction("Index");
        }
    }
}
