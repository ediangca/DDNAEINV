using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Entity;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PARController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;

        public PARController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;

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
        /**
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

        */

        // localhost:port/api/PAR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateParRequest request)
        {
            var p = request.Details;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "PAR is Invalid!" });

            var parExist = await dBContext.PARS.FirstOrDefaultAsync(x => x.parNo == p.parNo);

            if (parExist != null)
                return BadRequest(new { message = "PAR No. already exist!" });


            try
            {
                var par = new Par
                {
                    parNo = p.parNo,
                    lgu = p.lgu,
                    fund = p.fund,
                    receivedBy = p.receivedBy,
                    issuedBy = p.issuedBy,
                    postFlag = p.postFlag,
                    voidFlag = p.voidFlag,
                    createdBy = p.createdBy,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now
                };


                // Save changes to the database
                await dBContext.PARS.AddAsync(par);
                await dBContext.SaveChangesAsync();

                await dBContext.PARItems.AddRangeAsync(request.parItems);
                await dBContext.SaveChangesAsync();

                var existingItems = await dBContext.PARItems
                                                    .Where(x => x.PARNo == p.parNo)
                                                    .ToListAsync();

                foreach (var item in request.parItems)
                {
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == item.PropertyNo);

                    if (existingItem != null)
                    {
                        var propertyCards = new PropertyCard
                        {
                            Ref = "PAR",
                            REFNoFrom = p.parNo,
                            itemNo = existingItem.PARINO,
                            propertyNo = existingItem.PropertyNo,
                            issuedBy = p.issuedBy,
                            receivedBy = p.receivedBy,
                            createdBy = p.createdBy,
                            Date_Created = DateTime.Now,
                        };
                        await dBContext.PropertyCards.AddAsync(propertyCards);
                        await dBContext.SaveChangesAsync();
                    }
                }

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

        // localhost:port/api/PAR/Search/
        [HttpGet]
        [Route("PARNo")]
        public async Task<IActionResult> SearchByPARNo(string key)
        {
            // Check if any records match the given key
            bool isExist = await dBContext.ListOfPar
                .AnyAsync(x => x.parNo == key);

            return Ok(isExist); // Returns true or false
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

                var cardExist = dBContext1.PropertyCards.Where(x => x.Ref == "PAR" && x.REFNoFrom == id).ToList();

                if (cardExist.Count > 0)
                {
                    dBContext1.PropertyCards.RemoveRange(cardExist);
                    dBContext1.SaveChanges();
                }

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

                var cardExist = await dBContext1.PropertyCards.Where(x => x.Ref == "PAR" && x.REFNoFrom == id).ToListAsync();

                if (cardExist.Count > 0)
                {
                    dBContext1.PropertyCards.RemoveRange(cardExist);
                    dBContext1.SaveChanges();
                }
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }



    }
}



public class CreateParRequest
{
    public ParDto Details { get; set; }
    public List<ParItem> parItems { get; set; }
}
