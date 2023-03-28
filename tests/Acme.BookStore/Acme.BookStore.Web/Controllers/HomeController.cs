using Microsoft.AspNetCore.Mvc;

namespace Acme.BookStore.Web.Controllers
{
    public class HomeController : BookStoreControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}