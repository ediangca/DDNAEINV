using Azure.Core;
using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPRController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        public OPRController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/OPR
        [HttpGet]
        public IActionResult List()
        {

            var opr = dBContext.ListOfOPR.ToList()
                   .OrderByDescending(x => x.Date_Created).ToList();

            return Ok(opr);
        }

        // localhost:port/api/OPR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<OPRVw> Search(string key)
        {
            return dBContext.ListOfOPR.Where(x => x.itemSource.Contains(key) || x.ownership.ToString().Contains(key) ||
            x.received.ToString().Contains(key) || x.issued.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) ||
            x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/OPR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateOPRRequest request)
        {
            var o = request.Details;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "OPR is Invalid!" });

            var isExist = await dBContext.OPRS.FirstOrDefaultAsync(x => x.issuedBy == o.issuedBy
            && x.itemSource == o.itemSource);

            if (isExist != null)
                return BadRequest(new { message = "OPR already exist!" });


            try
            {

                var opr = new OPR
                {
                    itemSource = o.itemSource,
                    ownership = o.ownership,
                    receivedBy = o.receivedBy,
                    issuedBy = o.issuedBy,
                    postFlag = o.postFlag,
                    voidFlag = o.voidFlag,
                    createdBy = o.createdBy,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now
                };

                // Save changes to the database
                await dBContext.OPRS.AddAsync(opr);
                await dBContext.SaveChangesAsync();


                var existingItems = await dBContext.OPRItems
                                                    .Where(x => x.oprNo == o.oprNo)
                                                    .ToListAsync();

                foreach (var item in request.oprItems)
                {
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == item.PropertyNo);

                    if (existingItem != null)
                    {
                        var propertyCards = new PropertyCard
                        {
                            Ref = "OPR",
                            REFNoFrom = o.oprNo.ToString(),
                            itemNo = existingItem.OPRINO,
                            propertyNo = existingItem.PropertyNo,
                            issuedBy = o.issuedBy,
                            receivedBy = o.receivedBy,
                            createdBy = o.createdBy,
                            Date_Created = DateTime.Now,
                        };
                        await dBContext.PropertyCards.AddAsync(propertyCards);
                        await dBContext.SaveChangesAsync();
                    }
                }

                //return Ok(opr);
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

        // localhost:port/api/OPR/{id}
        [HttpGet("{id}")]
        public IActionResult Retrieve(int id)
        {

            // Find the OPR by id
            var opr = dBContext.ListOfOPR.Where(x => x.oprNo == id);
            if (opr == null)
                return NotFound(new { message = "OPR not Found!" });

            return Ok(opr);
        }


        // localhost:port/api/OPR/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] OPRDto details)
        {
            // Find the PAR by id
            var opr = dBContext.OPRS.Find(id);

            if (opr == null)
                return NotFound(new { message = "OPR not found." });

            try
            {
                var PARExist = dBContext.OPRS.FirstOrDefault(x => x.oprNo != id && x.oprNo == details.oprNo);

                if (PARExist != null)
                    return BadRequest(new { message = "OPR already exist!" });

                // Update the properties
                opr.itemSource = details.itemSource;
                opr.ownership = details.ownership;
                opr.receivedBy = details.receivedBy;
                opr.issuedBy = details.issuedBy;
                opr.postFlag = details.postFlag;
                opr.voidFlag = details.voidFlag;
                opr.Date_Updated = DateTime.Now;
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

        // localhost:port/api/OPR/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(int id, [FromBody] bool postVal)
        {
            // Find the OPR by id
            var par = dBContext.OPRS.Find(id);

            if (par == null)
                return NotFound(new { message = "OPR not found." });

            try
            {

                // Update the property postFlag
                par.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "OPR #000" + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }


        // localhost:port/api/OPR/Delete
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {

            var PARExist = await dBContext.OPRS.FirstOrDefaultAsync(x => x.oprNo == id && x.postFlag == true);

            if (PARExist != null) { return BadRequest(new { message = "PAR already posted!" }); }

            // Find the PAR by id
            var opr = dBContext.OPRS.Find(id);

            if (opr == null) { return NotFound(new { message = "OPR not found." }); }

            opr = await dBContext.OPRS.Where(x => x.oprNo == id).FirstAsync();

            dBContext.OPRS.Remove(opr);
            dBContext.SaveChanges();

            // Fetch existing items by OPR No
            var existingItems = await dBContext.OPRItems
                                               .Where(x => x.OPRINO == id)
                                               .ToListAsync();

            // Delete existing items
            if (existingItems.Count > 0)
            {
                dBContext.OPRItems.RemoveRange(existingItems);
                dBContext.SaveChanges();
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }


    }



}
public class CreateOPRRequest
{
    public OPRDto Details { get; set; }
    public List<OPRItem> oprItems { get; set; }
}
