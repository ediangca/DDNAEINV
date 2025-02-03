using DDNAEINV.Data;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ICSController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public ICSController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/PAR
        [HttpGet]
        public IActionResult List()
        {

            var par = dBContext.ListOfICS.ToList()
                   .OrderByDescending(x => x.Date_Created).ToList();

            return Ok(par);
        }
        // localhost:port/api/PAR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ICSVw> Search(string key)
        {
            return dBContext.ListOfICS.Where(x => x.ICSNo.Contains(key) ||
            x.entityName.Contains(key) || x.fund.ToString().Contains(key) ||
            x.received.ToString().Contains(key) || x.issued.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/ICS/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ICSDetails icsDetatils)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { message = "ICS is Invalid!" });

            var icsExist = await dBContext.ICSS.FirstOrDefaultAsync(x => x.ICSNo == icsDetatils.Details.ICSNo);

            if (icsExist != null)
                return BadRequest(new { message = "ICS already exist!" });


            try
            {
                var ics = new ICS
                {
                    ICSNo = icsDetatils.Details.ICSNo,
                    entityName = icsDetatils.Details.entityName,
                    fund = icsDetatils.Details.fund,
                    receivedBy = icsDetatils.Details.receivedBy,
                    issuedBy = icsDetatils.Details.issuedBy,
                    postFlag = false,
                    voidFlag = false,
                    createdBy = icsDetatils.Details.createdBy,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now
                };

                // Save changes to the database
                await dBContext.ICSS.AddAsync(ics);
                await dBContext.SaveChangesAsync();

                await dBContext.ICSItems.AddRangeAsync(icsDetatils.icsItems);

                await dBContext.SaveChangesAsync();
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

        // localhost:port/api/ICS/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(string id)
        {

            // Find the PAR by id
            var ics = dBContext.ListOfICS.Where(x => x.ICSNo == id);

            if (ics == null)
                return NotFound(new { message = "ICS not Found!" });

            return Ok(ics);
        }

        // localhost:port/api/ICS/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string icsNo, [FromBody] ICSDetails icsDetails)
        {
            // Find the ICS by ID
            var ics = await dBContext.ICSS.FindAsync(icsNo);

            if (ics == null)
                return NotFound(new { message = "ICS not found." });

            try
            {
                // Check if ICS with the same ICSNo exists
                var icsExist = await dBContext.ICSS.FirstOrDefaultAsync(x => x.ICSNo != icsNo && x.ICSNo == icsDetails.Details.ICSNo);

                if (icsExist != null)
                    return BadRequest(new { message = "ICS already exists!" });

                // Update ICS properties
                ics.entityName = icsDetails.Details.entityName;
                ics.fund = icsDetails.Details.fund;
                ics.receivedBy = icsDetails.Details.receivedBy;
                ics.issuedBy = icsDetails.Details.issuedBy;
                ics.createdBy = icsDetails.Details.createdBy;
                ics.Date_Updated = DateTime.Now;

                // Save changes to the database
                await dBContext.SaveChangesAsync();

                // Retrieve existing ICS items for the given ICSNo
                var existingItems = await dBContext.ICSItems
                                               .Where(x => x.ICSNo == icsDetails.Details.ICSNo)
                                               .ToListAsync();

                // Prepare a list to track new items to be added
                var itemsToAdd = new List<ICSItem>();

                foreach (var updatedItem in icsDetails.icsItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.ICSItemNo == updatedItem.ICSItemNo);

                    if (existingItem != null)
                    {
                        // Update the existing item's fields with the updated data
                        existingItem.IID = updatedItem.IID;
                        existingItem.Model = updatedItem.Model;
                        existingItem.Brand = updatedItem.Brand;
                        existingItem.Description = updatedItem.Description;
                        existingItem.SerialNo = updatedItem.SerialNo;
                        existingItem.PropertyNo = updatedItem.PropertyNo;
                        existingItem.QRCode = updatedItem.QRCode;
                        existingItem.Unit = updatedItem.Unit;
                        existingItem.Amount = updatedItem.Amount;
                        existingItem.Qty = updatedItem.Qty;
                        existingItem.EUL = updatedItem.EUL;
                    }
                    else
                    {
                        // If the item doesn't exist, add it to the list of items
                        var newICSItem = new ICSItem
                        {
                            ICSNo = updatedItem.ICSNo,
                            IID = updatedItem.IID,
                            Model = updatedItem.Model,
                            Brand = updatedItem.Brand,
                            Description = updatedItem.Description,
                            SerialNo = updatedItem.SerialNo,
                            PropertyNo = updatedItem.PropertyNo,
                            QRCode = updatedItem.QRCode,
                            Unit = updatedItem.Unit,
                            Amount = updatedItem.Amount,
                            Qty = updatedItem.Qty,
                            EUL = updatedItem.EUL,
                        };
                        itemsToAdd.Add(newICSItem);
                    }
                }

                // Identify the items that need to be deleted (those in existingItems but not in updatedItems)
                var propertyNosInUpdatedItems = icsDetails.icsItems.Select(i => i.ICSItemNo).ToHashSet();
                var itemsToDelete = existingItems.Where(e => !propertyNosInUpdatedItems.Contains(e.ICSItemNo)).ToList();

                // Remove the items that are not in updatedItems
                if (itemsToDelete.Count > 0)
                {
                    dBContext.ICSItems.RemoveRange(itemsToDelete);
                }
                // Add new ICS items if there are any
                if (itemsToAdd.Count > 0)
                {
                    await dBContext.ICSItems.AddRangeAsync(itemsToAdd);
                }

                // Save all changes (updates and additions)
                await dBContext.SaveChangesAsync();

                return Ok(new { message = "Successfully Updated!" });
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
            var par = dBContext.ICSS.Find(id);

            if (par == null)
                return NotFound(new { message = "ICS not found." });

            try
            {

                // Update the property postFlag
                par.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "ICS # " + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
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

            var ICSExist = await dBContext.ICSS.FirstOrDefaultAsync(x => x.ICSNo == id && x.postFlag == true);

            if (ICSExist != null) { return BadRequest(new { message = "ICS already posted!" }); }
                
            // Find the PAR by id
            var ics = dBContext.ICSS.Find(id);

            if (ics == null) { return NotFound(new { message = "ICS not found." }); }
               

            ics = await dBContext.ICSS.Where(x => x.ICSNo == id).FirstAsync();

            dBContext.ICSS.Remove(ics);
            dBContext.SaveChanges();

            // Fetch existing items by PAR No
            var existingItems = await dBContext.ICSItems
                                               .Where(x => x.ICSNo == id)
                                               .ToListAsync();

            // Delete existing items
            if (existingItems.Count > 0)
            {
                dBContext.ICSItems.RemoveRange(existingItems);
                dBContext.SaveChanges();
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }



    }
}

public class ICSDetails
{
    public ICS Details { get; set; }
    public List<ICSItem> icsItems { get; set; }
}
