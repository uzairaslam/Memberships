﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Memberships.Extensions;
using Memberships.Models;
using Microsoft.AspNet.Identity;

namespace Memberships.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var userId = Request.IsAuthenticated ? HttpContext.User.Identity.GetUserId() : null; 
            var model = new List<ThumbnailAreaModel>();
            model.Add(new ThumbnailAreaModel
            {
                Title = "Area Title",
                Thumbnails = new List<ThumbnailModel>()
            });
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}