using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data.Entity;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RRSEPController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;


        public RRSEPController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;

        }
        // localhost:port/api/RRSEP
        [HttpGet]
        public IActionResult List()
        {

            var repar = dBContext.ListOfRRSEP.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(repar);
        }
        // localhost:port/api/RRSEP/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<RRSEPVw> Search(string key)
        {
            return dBContext.ListOfRRSEP.Where(x => x.RRSEPNo.Contains(key) ||
            x.issued!.ToString().Contains(key) || x.issuedBy!.ToString().Contains(key) ||
            x.received!.ToString().Contains(key) || x.receivedBy!.ToString().Contains(key) ||
            x.approved!.ToString().Contains(key) || x.approvedBy!.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/RRSEP/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateRRSEPRequest request)
        {
            var details = request.Details;
            var items = request.updatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "RRSEP is Invalid!" });


            try
            {
                var rrsep = new RRSEP
                {
                    RRSEPNo = details.RRSEPNo,
                    entityName = details.entityName,
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
                await dBContext.RRSEPS.AddAsync(rrsep);
                await dBContext.SaveChangesAsync();


                var existingItems = await dBContext.ICSItems.ToListAsync();


                foreach (var updatedItem in items)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {

                        // Update the existing item's fields with the updated data
                        existingItem.rrsepFlag = true;
                        existingItem.RRSEPNo = details.RRSEPNo;


                        //Store into Property History
                        var propertyCards = new PropertyCard
                        {
                            Ref = "PRS",
                            REFNoFrom = existingItem.itrFlag != null ? existingItem.ITRNo : existingItem.ICSNo,
                            REFNoTo = details.RRSEPNo,
                            itemNo = existingItem.ICSItemNo,
                            propertyNo = existingItem.PropertyNo,
                            issuedBy = details.issuedBy,
                            receivedBy = details.receivedBy,
                            approvedBy = details.approvedBy,
                            createdBy = details.createdBy,
                            Date_Created = DateTime.Now,
                        };

                        await dBContext1.PropertyCards.AddAsync(propertyCards);
                        await dBContext1.SaveChangesAsync();
                    }
                }

                await dBContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Successfully Saved!",
                    details = rrsep,
                    items = items
                });


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/RRSEP/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id)
        {
            // Find the PRS by id asynchronously
            var rrsep = await dBContext.ListOfRRSEP
                                       .Where(x => x.RRSEPNo == id)
                                       .FirstOrDefaultAsync();

            if (rrsep == null)
                return NotFound(new { message = "RRSEP not Found!" });

            // Find the PRS Item by id asynchronously
            var icsItems = await dBContext.ICSItems
                                           .Where(x => x.RRSEPNo == id)
                                           .ToListAsync();

            if (icsItems == null || !icsItems.Any())
                return NotFound(new { message = "RRSEP items not Found!" });

            return Ok(new
            {
                details = rrsep,
                prsItems = icsItems,
            });
        }

        // localhost:port/api/RRSEP/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateRRSEPRequest request)
        {
            // Find the RRSEP by id
            var prs = await dBContext.RRSEPS.FindAsync(id);

            if (prs == null)
                return NotFound(new { message = "RRSEP not found." });

            var details = request.Details;

            try
            {

                // Update the RRSEP properties
                prs.entityName = details.entityName;
                prs.rtype = details.rtype;
                prs.otype = details.otype;
                prs.issuedBy = details.issuedBy;
                prs.receivedBy = details.receivedBy;
                prs.approvedBy = details.approvedBy;
                prs.createdBy = details.createdBy;
                prs.Date_Updated = DateTime.Now;

                // Save RRSEP changes
                await dBContext.SaveChangesAsync();

                // Fetch existing RRSEP items by PRS No
                var rrsepItems = await dBContext.ICSItems.Where(x => x.RRSEPNo == id).ToListAsync();

                // Nullify RRSEPNo and update prsFlag for old items
                foreach (var item in rrsepItems)
                {
                    item.rrsepFlag = false;
                    item.RRSEPNo = null;
                }

                var existingItems = await dBContext.ICSItems.ToListAsync();

                foreach (var updatedItem in request.updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {
                        // Update the existing item's fields with the updated data
                        existingItem.rrsepFlag = true;
                        existingItem.RRSEPNo = details.RRSEPNo;
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


        // localhost:port/api/RRSEP/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the PAR by id
            var rrsep = dBContext.RRSEPS.Find(id);

            if (rrsep == null)
                return NotFound(new { message = "RRSEP not found." });

            try
            {

                // Update the property postFlag
                rrsep.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(prs);
                return Ok(new
                {
                    message = "RRSEP # " + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
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
            var RRSEPExist = await dBContext.RRSEPS.FirstOrDefaultAsync(x => x.RRSEPNo == id && x.postFlag == true);

            if (RRSEPExist != null)
                return BadRequest(new { message = "RRSEP already posted!" });

            // Find the RRSEP by id
            var rrsep = await dBContext.RRSEPS.FirstOrDefaultAsync(x => x.RRSEPNo == id);

            if (rrsep == null)
                return NotFound(new { message = "RRSEP not found." });

            // Remove the RRSEP
            dBContext.RRSEPS.Remove(rrsep);
            await dBContext.SaveChangesAsync();  // Ensure save is async

            // Fetch existing ICS items by PRS No
            var prsItems = await dBContext.ICSItems
                                               .Where(x => x.RRSEPNo == id)
                                               .ToListAsync();

            // Nullify PRS No and update prsFlag
            foreach (var reparItem in prsItems)
            {
                reparItem.RRSEPNo = null;
                reparItem.rrsepFlag = false;
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

public class CreateRRSEPRequest
{
    public RRSEP Details { get; set; }
    public List<ICSItemDto> updatedItems { get; set; }
}
