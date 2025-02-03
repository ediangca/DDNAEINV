using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PARController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public PARController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/PAR
        [HttpGet]
        public IActionResult List()
        {

            var par = dBContext.ListOfPar.ToList()
                   .OrderByDescending(x => x.Date_Created).ToList();

            return Ok(par);
        }

        // localhost:port/api/PAR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ParVw> Search(string key)
        {
            return dBContext.ListOfPar.Where(x => x.parNo.Contains(key) ||
            x.lgu.Contains(key) || x.fund.ToString().Contains(key) ||
            x.received.ToString().Contains(key) || x.issued.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/PAR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ParDto details)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { message = "PAR is Invalid!" });

            var parExist = await dBContext.PARS.FirstOrDefaultAsync(x => x.parNo == details.parNo);

            if (parExist != null)
                return BadRequest(new { message = "PAR already exist!" });


            try
            {

                var par = new Par
                {
                    parNo = details.parNo,
                    lgu = details.lgu,
                    fund = details.fund,
                    receivedBy = details.receivedBy,
                    issuedBy = details.issuedBy,
                    postFlag = details.postFlag,
                    voidFlag = details.voidFlag,
                    createdBy = details.createdBy,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                // Save changes to the database
                await dBContext.PARS.AddAsync(par);
                await dBContext.SaveChangesAsync();

                //return Ok(par);
                return Ok(new
                {
                    message = "Successfully Saved!"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/PAR/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(string id)
        {

            // Find the PAR by id
            var par = dBContext.ListOfPar.Where(x => x.parNo == id);
            if (par == null)
                return NotFound(new { message = "PAR not Found!" });

            return Ok(par);
        }


        // localhost:port/api/PAR/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(string id, [FromBody] ParDto details)
        {
            // Find the PAR by id
            var par = dBContext.PARS.Find(id);

            if (par == null)
                return NotFound(new { message = "PAR not found." });

            try
            {
                var PARExist = dBContext.PARS.FirstOrDefault(x => x.parNo != id && x.parNo == details.parNo);

                if (PARExist != null)
                    return BadRequest(new { message = "PAR already exist!" });

                // Update the properties
                par.parNo = details.parNo;
                par.lgu = details.lgu;
                par.fund = details.fund;
                par.receivedBy = details.receivedBy;
                par.issuedBy = details.issuedBy;
                par.postFlag = details.postFlag;
                par.voidFlag = details.voidFlag;
                par.createdBy = details.createdBy;
                par.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "Successfully Updated!"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/PAR/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the PAR by id
            var par = dBContext.PARS.Find(id);

            if (par == null)
                return NotFound(new { message = "PAR not found." });

            try
            {

                // Update the property postFlag
                par.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "PAR # " + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }


        // localhost:port/api/PAR/Delete
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {

            var PARExist = await dBContext.PARS.FirstOrDefaultAsync(x => x.parNo == id && x.postFlag == true);

            if (PARExist != null) { return BadRequest(new { message = "PAR already posted!" }); }
                
            // Find the PAR by id
            var par = dBContext.PARS.Find(id);

            if (par == null) { return NotFound(new { message = "PAR not found." }); }
              
            par = await dBContext.PARS.Where(x => x.parNo == id).FirstAsync();

            dBContext.PARS.Remove(par);
            dBContext.SaveChanges();

            // Fetch existing items by PAR No
            var existingItems = await dBContext.PARItems
                                               .Where(x => x.PARNo == id)
                                               .ToListAsync();

            // Delete existing items
            if (existingItems.Count > 0)
            {
                dBContext.PARItems.RemoveRange(existingItems);
                dBContext.SaveChanges();
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }



    }
}
