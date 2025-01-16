using DDNAEINV.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CencusController : ControllerBase
    {

        private readonly ApplicationDBContext dBContext;

        public CencusController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // localhost:port/api/Branch
        [HttpGet]
        public IActionResult List()
        {

            var cencus = dBContext.Cencus.ToList();

            return Ok(cencus);
        }
        [HttpGet]
        [Route("ActivityLog")]
        public IActionResult ActivityLog()
        {

            var cencus = dBContext.ListOfActivity.OrderByDescending(x => x.ActivityDate).ToList();

            return Ok(cencus);
        }
    }
}
