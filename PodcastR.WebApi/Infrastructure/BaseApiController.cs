using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using PodcastR.ApiCore.Models;

namespace PodcastR.WebApi.Infrastructure
{
    public class BaseApiController: ApiController
    {
        private ApplicationDbContext _dbContext;
        private ApplicationUserManager _userManager;

        public BaseApiController()
        {
        }

        public BaseApiController(ApplicationDbContext context, ApplicationUserManager manager)
        {
            DbContext = context;
            UserManager = manager;
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? Request.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
                DbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}