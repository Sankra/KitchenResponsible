﻿using System;
using System.Linq;
using System.Threading.Tasks;
using KitchenResponsibleService.Clients;
using KitchenResponsibleService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace KitchenResponsibleService.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        readonly KitchenService kitchenService;
        readonly IMemoryCache memoryCache;
        readonly ComicsClient comicsClient;

        public HomeController(KitchenService kitchenService, IMemoryCache memoryCache, ComicsClient comicsClient)
        {
            this.kitchenService = kitchenService;
            this.memoryCache = memoryCache;
            this.comicsClient = comicsClient;
        }

        public async Task<IActionResult> Index()
        {
#if DEBUG
            return await GetWebsite();
#else
            return await GetWebsiteFromCacheIfFilled();
#endif
            // TODO: Show/Hide button is ridiculously ugly, half transparent last row or something must be better
            // TODO: iPad app to enable proper fullscreen and service discovery
        }

        async Task<IActionResult> GetWebsiteFromCacheIfFilled() {
			if (!memoryCache.TryGetValue(Keys.KitchenResponsibleWebsite, out ViewResult view))
			{
                view = await GetWebsite();
				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetAbsoluteExpiration(TimeSpan.FromMinutes(59));

				memoryCache.Set(Keys.KitchenResponsibleWebsite, view, cacheEntryOptions);
			}

            return view;
        }

        async Task<ViewResult> GetWebsite() {
			var weeksAndResponsiblesTask = kitchenService.GetWeeksAndResponsibles();
			var getLatestComicTask = comicsClient.GetLatestComicAsync();
			await Task.WhenAll(weeksAndResponsiblesTask, getLatestComicTask);
			var weeksAndResponsibles = weeksAndResponsiblesTask.Result;
			ViewData["WeeksAndResponsibles"] = weeksAndResponsibles.ToList();
            ViewData["LatestComic"] = getLatestComicTask.Result;
			return View();
        } 
    }
}
