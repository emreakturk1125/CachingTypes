using Microsoft.AspNetCore.Mvc;
using Redis2.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis2.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _database;

        // Metodlarda aynı db kullanılacaksa Constructor'da global bir değişken tanımlayabilisin, aksi durumda yani farklı db'ler  için metod içinde ayrı ayrı tanımlanmalıdır
        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _database = _redisService.GetDb(0);   // redisteki "db0" database'ini al demek
        }

        public IActionResult Index()
        {
            //var db = _redisService.GetDb(1);    
            _database.StringSet("name", "Fatih Çakıroğlu");
            _database.StringSet("ziyaretci", 200);
            return View();
        }

        public IActionResult Show()
        {
            var name = _database.StringGet("name");

            var name2 = _database.StringGetAsync("name").Result;     //  Asenkron metodu kullanırken "async" ve "await" key'lerini kullanmak istemiyorsan  ".Result" olarak data çekmelisin

            _database.StringIncrement("ziyaretci", 1);

            _database.StringIncrementAsync("ziyaretci", 1).Wait();   // Asenkron metodu kullanmak istersen

            var ziyaretci = _database.StringGet("ziyaretci");

            if (name.HasValue)
            {
                ViewBag.Name = name.ToString();
            }
            return View();
        }
    }
}
