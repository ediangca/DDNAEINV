using Azure.Core;
using DDNAEINV.Data;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ICSController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;

        public ICSController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;

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
        public async Task<IActionResult> Create([FromBody] ICSDetails request)
        {

            var i = request.Details;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "ICS is Invalid!" });

            var icsExist = await dBContext.ICSS.FirstOrDefaultAsync(x => x.ICSNo == i.ICSNo);

            if (icsExist != null)
                return BadRequest(new { message = "ICS No. already exist!" });


            try
            {
                var ics = new ICS
                {
                    ICSNo = i.ICSNo,
                    entityName = i.entityName,
                    fund = i.fund,
                    receivedBy = i.receivedBy,
                    issuedBy = i.issuedBy,
                    postFlag = false,
                    voidFlag = false,
                    createdBy = i.createdBy,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now
                };

                // Save changes to the database
                await dBContext.ICSS.AddAsync(ics);
                await dBContext.SaveChangesAsync();

                await dBContext.ICSItems.AddRangeAsync(request.icsItems);
                await dBContext.SaveChangesAsync();

                var existingItems = await dBContext1.ICSItems
                                                    .Where(x => x.ICSNo == i.ICSNo)
                                                    .ToListAsync();

                foreach (var item in request.icsItems)
                {
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == item.PropertyNo);

                    if (existingItem != null)
                    {
                        var propertyCards = new PropertyCard
                        {
                            REF = "ICS",
                            REFNoFrom = i.ICSNo,
                            PropertyNo = existingItem.PropertyNo,
                            IssuedBy = i.issuedBy,
                            ReceivedBy = i.receivedBy,
                            CreatedBy = i.createdBy,
                            Date_Created = DateTime.Now,
                        };
                        await dBContext1.PropertyCards.AddAsync(propertyCards);
                        await dBContext1.SaveChangesAsync();
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


        // localhost:port/api/ICS/Search/
        [HttpGet]
        [Route("ICSNo")]
        public async Task<IActionResult> SearchByICSNo(string key)
        {
            // Check if any records match the given key
            bool isExist = await dBContext.ListOfICS
                .AnyAsync(x => x.ICSNo == key);

            return Ok(isExist); // Returns true or false
        }


        // localhost:port/api/ICS/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] ICSDetails request)
        {
            var details = request.Details;
            var updatedItems = request.icsItems;

            // Find the ICS by ID
            var ics = await dBContext.ICSS.FindAsync(id);

            if (ics == null)
                return NotFound(new { message = "ICS not found." });

            try
            {
                // Check if ICS with the same ICSNo exists
                var icsExist = await dBContext.ICSS.FirstOrDefaultAsync(x => x.ICSNo != id && x.ICSNo == details.ICSNo);

                if (icsExist != null)
                    return BadRequest(new { message = "ICS already exists!" });

                // Update ICS properties
                ics.entityName = details.entityName;
                ics.fund = details.fund;
                ics.receivedBy = details.receivedBy;
                ics.issuedBy = details.issuedBy;
                ics.createdBy = details.createdBy;
                ics.Date_Updated = DateTime.Now;

                // Save changes to the database
                await dBContext.SaveChangesAsync();

                // Retrieve existing ICS items for the given ICSNo
                var existingItems = await dBContext.ICSItems
                                               .Where(x => x.ICSNo == details.ICSNo)
                                               .ToListAsync();
                // Fetch existing CARD by PAR No
                var existingProperties = await dBContext1.PropertyCards
                                                   .Where(x => x.REF == "ICS" && x.REFNoFrom == id)
                                                   .ToListAsync();

                // Prepare a list to track new items to be added
                var itemsToAdd = new List<ICSItem>();
                var propertyToAdd = new List<PropertyCard>();

                foreach (var updatedItem in updatedItems)
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


                    // Find if the updated item exists in the existing property
                    var existingProperty = existingProperties.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);
                    if (existingProperty != null)
                    {

                        existingProperty.PropertyNo = updatedItem.PropertyNo;
                        existingProperty.IssuedBy = details.issuedBy;
                        existingProperty.ReceivedBy = details.receivedBy;
                        existingProperty.CreatedBy = details.createdBy;
                        existingProperty.Date_Created = DateTime.Now;
                    }
                    else
                    {
                        Debug.Print("TO ADD PROPERTY CARD " + updatedItem.PropertyNo);

                        //var cardItemExist = await dBContext1.PropertyCards.FirstOrDefaultAsync(x => x.PropertyNo == updatedItem.PropertyNo);
                        //if (cardItemExist != null)
                        //    return BadRequest(new { message = $"Property #{updatedItem.PropertyNo} already exists!" });

                        var propertyCard = new PropertyCard
                        {
                            REF = "ICS",
                            REFNoFrom = updatedItem.ICSNo,
                            PropertyNo = updatedItem.PropertyNo,
                            IssuedBy = details.issuedBy,
                            ReceivedBy = details.receivedBy,
                            CreatedBy = details.createdBy,
                            Date_Created = DateTime.Now,
                        };
                        propertyToAdd.Add(propertyCard);


                    }
                }

                // Identify the items that need to be deleted (those in existingItems but not in updatedItems)
                var propertyNosInUpdatedItems = updatedItems.Select(i => i.ICSItemNo).ToHashSet();
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

                await dBContext.SaveChangesAsync();
                await dBContext1.SaveChangesAsync();

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

                var cardExist = await dBContext1.PropertyCards.Where(x => x.REF == "ICS" && x.REFNoFrom == id).ToListAsync();

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

public class ICSDetails
{
    public ICS Details { get; set; }
    public List<ICSItem> icsItems { get; set; }
}
