using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

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
                            REF = "RRSEP",
                            REFNoFrom = existingItem.itrFlag == true ? existingItem.ITRNo : existingItem.ICSNo,
                            REFNoTo = details.RRSEPNo,
                            PropertyNo = existingItem.PropertyNo,
                            IssuedBy = details.issuedBy,
                            ReceivedBy = details.receivedBy,
                            ApprovedBy = details.approvedBy,
                            CreatedBy = details.createdBy,
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
            var updatedItems = request.updatedItems;

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

                // Fetch existing REPAR items by RRSEP No
                var rrsepItems = await dBContext.ICSItems.Where(x => x.RRSEPNo == id).ToListAsync();

                // Fetch existing CARD by RRSEP No
                var existingProperties = await dBContext1.PropertyCards
                                                   .Where(x => x.REF == "RRSEP" && x.REFNoTo == id)
                                                   .ToListAsync();

                // Nullify REPARNo and update reparFlag for old items
                foreach (var rrsepItem in rrsepItems)
                {
                    // Update the repar properties
                    rrsepItem.RRSEPNo = null;
                    rrsepItem.rrsepFlag = false;

                }

                // Fetch existing items by RRSEP No
                var existingItems = await dBContext.ICSItems.ToListAsync();


                var propertyToAdd = new List<PropertyCard>();

                // Update existing items or prepare to add new ones
                foreach (var updatedItem in updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {
                        // Update the existing item's fields with the updated data
                        existingItem.rrsepFlag = true;
                        existingItem.RRSEPNo = id;
                        // Update other fields as necessary from updatedItem
                        existingItem.Brand = updatedItem.Brand;
                        existingItem.Model = updatedItem.Model;
                        existingItem.Description = updatedItem.Description;
                        existingItem.SerialNo = updatedItem.SerialNo;
                        existingItem.Amount = updatedItem.Amount;

                    }

                    // Find if the updated item exists in the existing property
                    var existingProperty = existingProperties.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);
                    if (existingProperty != null)
                    {

                        existingProperty.PropertyNo = updatedItem.PropertyNo;
                        existingProperty.REFNoFrom = existingItem.itrFlag == true ? existingItem.ITRNo : existingItem.ICSNo;
                        existingProperty.IssuedBy = details.issuedBy;
                        existingProperty.ReceivedBy = details.receivedBy;
                        existingProperty.ApprovedBy = details.approvedBy;
                        existingProperty.CreatedBy = details.createdBy;
                        existingProperty.Date_Created = DateTime.Now;
                    }
                    else
                    {
                        Debug.Print("TO ADD PROPERTY CARD " + updatedItem.PropertyNo);

                        var propertyCard = new PropertyCard
                        {
                            REF = "RRSEP",
                            REFNoFrom = existingItem.itrFlag == true ? existingItem.ITRNo : existingItem.ICSNo,
                            REFNoTo = id,
                            PropertyNo = updatedItem.PropertyNo,
                            IssuedBy = details.issuedBy,
                            ReceivedBy = details.receivedBy,
                            ApprovedBy = details.approvedBy,
                            CreatedBy = details.createdBy,
                            Date_Created = DateTime.Now,
                        };
                        propertyToAdd.Add(propertyCard);

                    }

                }
                var propertyNosInUpdatedItems1 = updatedItems.Select(i => i.PropertyNo).ToHashSet();
                var itemsToDelete1 = existingProperties.Where(e => !propertyNosInUpdatedItems1.Contains(e.PropertyNo)).ToList();

                if (itemsToDelete1.Count > 0)
                {
                    dBContext1.PropertyCards.RemoveRange(itemsToDelete1);
                }

                if (propertyToAdd.Count > 0)
                {
                    await dBContext1.PropertyCards.AddRangeAsync(propertyToAdd);
                }


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
            var rrsepItems = await dBContext.ICSItems
                                               .Where(x => x.RRSEPNo == id)
                                               .ToListAsync();

            // Nullify PRS No and update prsFlag
            foreach (var item in rrsepItems)
            {

                var cardExist = await dBContext1.PropertyCards.FirstOrDefaultAsync(x => x.REF == "RRSEP" && x.PropertyNo == item.PropertyNo);

                if (cardExist != null)
                {
                    dBContext1.PropertyCards.Remove(cardExist);
                    await dBContext1.SaveChangesAsync();  // Ensure save is async
                }
                // Update the RRSEP properties
                item.RRSEPNo = null;
                item.rrsepFlag = false;
            }

            // Save changes if any items were updated
            if (rrsepItems.Count > 0)
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
