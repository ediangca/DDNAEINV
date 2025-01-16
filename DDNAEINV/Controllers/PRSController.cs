using DDNAEINV.Data;
using DDNAEINV.Helper;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PRSController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public PRSController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/PRS
        [HttpGet]
        public IActionResult List()
        {

            var prs = dBContext.ListOfPRS.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(prs);
        }
        // localhost:port/api/PRS/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<PRSVw> Search(string key)
        {
            return dBContext.ListOfPRS.Where(x => x.PRSNo.Contains(key) ||
            x.issued!.ToString().Contains(key) || x.issuedBy!.ToString().Contains(key) ||
            x.received!.ToString().Contains(key) || x.receivedBy!.ToString().Contains(key) ||
            x.approved!.ToString().Contains(key) || x.approvedBy!.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/PRS/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreatePRSRequest request)
        {
            var details = request.Details;
            var items = request.updatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "PRS is Invalid!" });


            try
            {
                var prs = new PRS
                {
                    PRSNo = details.PRSNo,
                    rtype = details.rtype,
                    otype = details.otype,
                    issuedBy = details.issuedBy,
                    receivedBy = details.receivedBy,
                    approvedBy = details.approvedBy,
                    postFlag = details.postFlag,
                    voidFlag = details.voidFlag,
                    createdBy = details.createdBy,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now
                };

                // Save changes to the database
                await dBContext.PRSS.AddAsync(prs);
                await dBContext.SaveChangesAsync();


                var existingItems = await dBContext.PARItems.ToListAsync();


                foreach (var updatedItem in items)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {

                        // Update the existing item's fields with the updated data
                        existingItem.prsFlag = true;
                        existingItem.PRSNo = details.PRSNo;
                        // Update other fields as necessary
                    }
                }

                await dBContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Successfully Saved!",
                    details = prs,
                    items = items
                });


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/PRS/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id)
        {
            // Find the PRS by id asynchronously
            var prs = await dBContext.ListOfPRS
                                       .Where(x => x.PRSNo == id)
                                       .FirstOrDefaultAsync();

            if (prs == null)
                return NotFound(new { message = "PRS not Found!" });

            // Find the PRS Item by id asynchronously
            var prsItems = await dBContext.PARItems
                                           .Where(x => x.PRSNo == id)
                                           .ToListAsync();

            if (prsItems == null || !prsItems.Any())
                return NotFound(new { message = "PRS items not Found!" });

            return Ok(new
            {
                details = prs,
                prsItems = prsItems,
            });
        }

        // localhost:port/api/PRS/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] CreatePRSRequest request)
        {
            // Find the PRS by id
            var prs = await dBContext.PRSS.FindAsync(id);

            if (prs == null)
                return NotFound(new { message = "PRS not found." });

            var details = request.Details;

            try
            {

                // Update the PRS properties
                prs.rtype = details.rtype;
                prs.otype = details.otype;
                prs.issuedBy = details.issuedBy;
                prs.receivedBy = details.receivedBy;
                prs.approvedBy = details.approvedBy;
                prs.createdBy = details.createdBy;
                prs.Date_Updated = DateTime.Now;

                // Save PRS changes
                await dBContext.SaveChangesAsync();

                // Fetch existing PRS items by PRS No
                var prsItems = await dBContext.PARItems.Where(x => x.PRSNo == id).ToListAsync();

                // Nullify PRSNo and update prsFlag for old items
                foreach (var prsItem in prsItems)
                {
                    // Update the prs properties
                    prsItem.prsFlag = false;
                    prsItem.PRSNo = null;
                }

                var existingItems = await dBContext.PARItems.ToListAsync();

                foreach (var updatedItem in request.updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {

                        // Update the existing item's fields with the updated data
                        existingItem.prsFlag = true;
                        existingItem.PRSNo = details.PRSNo;
                        // Update other fields as necessary
                    }
                }

                await dBContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Successfully Updated!"
                });
            }
            catch (Exception ex)
            {
                // Log the exception (optional: use a logging framework)
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }


        // localhost:port/api/PRS/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the PAR by id
            var prs = dBContext.PRSS.Find(id);

            if (prs == null)
                return NotFound(new { message = "PRS not found." });

            try
            {

                // Update the property postFlag
                prs.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(prs);
                return Ok(new
                {
                    message = "PRS # " + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            // Check if the PRS is already posted
            var PRSExist = await dBContext.PRSS.FirstOrDefaultAsync(x => x.PRSNo == id && x.postFlag == true);

            if (PRSExist != null)
                return BadRequest(new { message = "PRS already posted!" });

            // Find the PRS by id
            var prs = await dBContext.PRSS.FirstOrDefaultAsync(x => x.PRSNo == id);

            if (prs == null)
                return NotFound(new { message = "PRS not found." });

            // Remove the PRS
            dBContext.PRSS.Remove(prs);
            await dBContext.SaveChangesAsync();  // Ensure save is async

            // Fetch existing PAR items by PRS No
            var prsItems = await dBContext.PARItems
                                               .Where(x => x.PRSNo == id)
                                               .ToListAsync();

            // Nullify PRS No and update prsFlag
            foreach (var reparItem in prsItems)
            {
                reparItem.PRSNo = null;
                reparItem.prsFlag = false;
            }

            // Save changes if any items were updated
            if (prsItems.Count > 0)
            {
                await dBContext.SaveChangesAsync();  // Use async save
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }

    }
}

public class CreatePRSRequest
{
    public PRS Details { get; set; }
    public List<ParItemDto> updatedItems { get; set; }
}
