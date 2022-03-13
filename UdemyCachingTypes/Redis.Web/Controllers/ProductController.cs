using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Redis.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Web.Controllers
{
    // ****************************** IDistributedCache ********************************

    // IDistributedCache için Microsoft.Extensions.Caching.StackExchangeRedis kütüphanesi eklenmelidir.
    // Not : String veri tipinde redise key-value çifti kaydetmek istersek  IDistributedCache yeterli olur
    // Asenkron metodlarda kullanılabilir
    // Tavisye : ComplexType lar Byte dizisi olarak değilse, json olarak redise kaydedilmelidir.
    // Not : Redis server default olarak bellek dolduğu zaman en az kullanılanı siler
    public class ProductController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

       
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(10);


            // String veri kaydet
            _distributedCache.SetString("car", "BMW", cacheEntryOptions);

            // Asenkron olarak String veri kaydet
            await _distributedCache.SetStringAsync("city", "Bilecik", cacheEntryOptions);

            // Json serialize olarak ComplexType kaydet
            Product product = new Product()
            {
                Id = 1,
                Name = "Kalem - 1",
                Price = 100
            };
            string jsonProduct = JsonConvert.SerializeObject(product);
            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheEntryOptions);   // product:1 kullanarak veri kaydedersen klasör yapısında gösterir


            // Byte dizisi olarak ComplexType kaydet
            Product product2 = new Product()
            {
                Id = 2,
                Name = "Kalem - 2",
                Price = 200
            };
            string jsonProduct2 = JsonConvert.SerializeObject(product2);
            Byte[] byteProduct2 = Encoding.UTF8.GetBytes(jsonProduct2);
             await _distributedCache.SetAsync("product:2", byteProduct2, cacheEntryOptions);   // product:1 kullanarak veri kaydedersen klasör yapısında gösterir


            return View();
        }

        public IActionResult Show()
        {
            string car = _distributedCache.GetString("car");

            string city = _distributedCache.GetString("city");

            string jsonProduct1 = _distributedCache.GetString("product:1");
            Product product1 = JsonConvert.DeserializeObject<Product>(jsonProduct1);

            Byte[] byteProduct2 = _distributedCache.Get("product:2");
            string jsonProduct2 = Encoding.UTF8.GetString(byteProduct2);
            Product product2 = JsonConvert.DeserializeObject<Product>(jsonProduct2);


            ViewBag.Car = car;
            ViewBag.City = city;
            ViewBag.Product = product1;
            ViewBag.Product2 = product2;


            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("Sehir");
            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/car.jpg");
            Byte[] imageByte = System.IO.File.ReadAllBytes(path);
            _distributedCache.Set("resim", imageByte);
            return View();
        }

        public async Task<IActionResult> ImageUrl()
        {
            Byte[] imageByte = await _distributedCache.GetAsync("resim");

            return File(imageByte,"image/jpg");
        }
    }
}
