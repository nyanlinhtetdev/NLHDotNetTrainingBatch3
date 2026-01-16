using Microsoft.AspNetCore.Mvc;

namespace NLHDotNetTrainingBatch3.ThemeMvcApp.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
