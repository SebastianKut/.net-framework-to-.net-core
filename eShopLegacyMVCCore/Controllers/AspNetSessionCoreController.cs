using eShopLegacy.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace eShopLegacyMVCCore.Controllers
{
    public class AspNetSessionCoreController : Controller
    {
        // GET: AspNetCoreSession
        public ActionResult Index()
        {
            var model = ((System.Web.HttpContext)HttpContext).Session["DemoItem"];
            return View(model);
        }

        // POST: AspNetCoreSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SessionDemoModel demoModel)
        {
            ((System.Web.HttpContext)HttpContext).Session["DemoItem"] = demoModel;
            return View(demoModel);
        }
    }
}
