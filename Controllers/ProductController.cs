using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using UdemyRedisCalisma.Models;

namespace UdemyRedisCalisma.Controllers
{
    public class ProductController : Controller
    {
        readonly IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            //1dkya kadar 10snden ömrünü 6 kez arttırabiliriz. topla süre geçemez. 1 dk sonunda memoryden düşecektir.  
            //cacheOptions.SlidingExpiration = TimeSpan.FromSeconds(10);
            //10sn içerisinde erişirsem sürekli +10sn artacak.
            cacheOptions.Priority = CacheItemPriority.High;
            //memory dolduğunda hangisi silinecek ona karar vermek için priority.
            // low normal high silecek. neverremove asla silinmeyecek. hepsini böyle yaparsak yeni bir memory ye gelirse exception fırlatır.
            cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key}->{value}=> sebep: {reason}");
            });
            //niye silindiğini göstermek için bu method kullanılır


            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), cacheOptions);
            //ömrü 30sn olacak. verdiğimiz data kadar

            Product p = new Product { Id = 1, Name = "Kalem", Price = 200 };
            _memoryCache.Set<Product>("Product:1", p);
            _memoryCache.Set<double>("money", 100.99);



            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue("zaman", out string zamancache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.zaman = zamancache;
            ViewBag.callback = callback;

            ViewBag.product = _memoryCache.Get<Product>("Product:1");
            return View();

        }
    }
}



//index
////1.yol
//if (String.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
//{
//    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
//}


//show

//_memoryCache.GetOrCreate<string>("zaman",entry =>                   //var mı diye bak yoksa oluştur varsa getir. getorcreate
//{

//    return DateTime.Now.ToString();
//});