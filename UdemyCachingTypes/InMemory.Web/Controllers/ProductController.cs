using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemory.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            #region 1. yol

            // key'e ait değer yoksa oluşturur

            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("date")))
            //{
            //    _memoryCache.Set<string>("date", DateTime.Now.ToString());
            //} 

            #endregion

            #region 2. yol
            //// değer varsa true döner ve dataCache'e  değeri set'ler, yoksa false döner
            //if (!_memoryCache.TryGetValue("date", out string dateCache))
            //{
            //    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            //    options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            //    _memoryCache.Set<string>("date", DateTime.Now.ToString(), options);
            //}
            //string cacheValue = dateCache; 
            #endregion

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            //options.SlidingExpiration = TimeSpan.FromSeconds(10);
            options.Priority = CacheItemPriority.High;
            options.RegisterPostEvictionCallback((key, value, reason, state) => {

                _memoryCache.Set("callback", $"{key}->{value} => sebep: {reason}");
            
            });
            _memoryCache.Set<string>("date", DateTime.Now.ToString(), options);

            return View();
        }

        public IActionResult Show()
        {
            #region 3. yol

            //// key:date olan değer varsa alır yoksa oluşturur.
            //_memoryCache.GetOrCreate<string>("date", entry =>
            // {
            //     //entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            //     ///entry.AbsoluteExpiration = TimeSpan.FromMinutes(5);
            //     return DateTime.Now.ToString();
            // });
            // 
            //    ViewBag.Date =  _memoryCache.Get<string>("date");


            #endregion

            _memoryCache.TryGetValue("date", out string dateCache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.Date = dateCache;
            ViewBag.Callback = callback;

            return View();
        }
    }
}
