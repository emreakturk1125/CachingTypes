using Microsoft.AspNetCore.Mvc;
using Redis2.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis2.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly RedisService _redisService;

        protected readonly IDatabase db;

        public BaseController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(1);
        }
    }
}
