using Azure.Core;
using DDNAEINV.Data;
using DDNAEINV.Model.Details;
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
    public class OPRController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;
        public OPRController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;

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

                //await dBContext.OPRItems.AddRangeAsync(request.oprItems);
                //await dBContext.SaveChangesAsync();


                //var existingItems = await dBContext1.OPRItems
                //                                    .Where(x => x.oprNo == o.oprNo)
                //                                    .ToListAsync();

                //foreach (var item in request.oprItems)
                //{
                //    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == item.PropertyNo);

                //    if (existingItem != null)
                //    {
                //        var propertyCards = new PropertyCard
                //        {
                //            REF = "OPR",
                //            REFNoFrom = o.oprNo.ToString(),
                //            PropertyNo = existingItem.PropertyNo,
                //            IssuedBy = o.issuedBy,
                //            ReceivedBy = o.receivedBy,
                //            CreatedBy = o.createdBy,
                //            Date_Created = DateTime.Now,
                //        };
                //        await dBContext.PropertyCards.AddAsync(propertyCards);
                //        await dBContext.SaveChangesAsync();
                //    }
                //}

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
        /*
         public IActionResult Update(int id, [FromBody] OPRDto details)
        {
            // Find the OPR by id
            var opr = dBContext.OPRS.Find(id);

            if (opr == null)
                return NotFound(new { message = "OPR not found." });

            try
            {
                var exist = dBContext.OPRS.FirstOrDefault(x => x.oprNo != id && x.oprNo == details.oprNo);

                if (exist != null)
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
        */

        public async Task<IActionResult> Update(int id, [FromBody] CreateOPRRequest request)
            {

            var details = request.Details;
            var updatedItems = request.oprItems;

            if (updatedItems == null || updatedItems.Count == 0)
            {
                return BadRequest(new { message = "OPR No and updated items are required." });
            }

            // Find the OPR by id
            var par = await dBContext.OPRS.FindAsync(id);

            if (par == null)
                return NotFound(new { message = "OPR not found." });

            try
            {
                var OPRExist = await dBContext.OPRS.FirstOrDefaultAsync(x => x.oprNo != id && x.oprNo == details.oprNo);

                if (OPRExist != null)
                    return BadRequest(new { message = "OPR already exist!" });

                // Update the properties
                par.oprNo = id;
                par.itemSource = details.itemSource;
                par.ownership = details.ownership;
                par.receivedBy = details.receivedBy;
                par.issuedBy = details.issuedBy;
                par.postFlag = details.postFlag;
                par.voidFlag = details.voidFlag;
                par.createdBy = details.createdBy;
                par.Date_Updated = DateTime.Now;

                // Save changes to the database
                await dBContext.SaveChangesAsync();

                // Fetch existing items by OPR No
                var existingItems = await dBContext.OPRItems
                                                   .Where(x => x.oprNo == id)
                                                   .ToListAsync();
                // Fetch existing CARD by OPR No
                var existingProperties = await dBContext1.PropertyCards
                                                   .Where(x => x.REF == "OPR" && x.REFNoFrom == id.ToString())
                                                   .ToListAsync();

                Debug.Print("PROPERTIES " + "REF: OPR" + " OPR NO: " + id);
                Debug.Print("TO ADD PROPERTY CARD " + existingProperties.Count);

                // Prepare a list to keep track of items that need to be added
                var itemsToAdd = new List<OPRItem>();
                var propertyToAdd = new List<PropertyCard>();

                foreach (var updatedItem in updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);


                    if (existingItem != null)
                    {

                        // Update the existing item's fields with the updated data
                        existingItem.IID = updatedItem.IID;
                        existingItem.Brand = updatedItem.Brand;
                        existingItem.Model = updatedItem.Model;
                        existingItem.Description = updatedItem.Description;
                        existingItem.SerialNo = updatedItem.SerialNo;
                        existingItem.PropertyNo = updatedItem.PropertyNo;
                        existingItem.QRCode = updatedItem.QRCode;
                        existingItem.Unit = updatedItem.Unit;
                        existingItem.Amount = updatedItem.Amount;
                        existingItem.Date_Acquired = updatedItem.Date_Acquired;
                        // Update other fields as necessary

                    }
                    else
                    {

                        var OPRItemExist = await dBContext.OPRItems.FirstOrDefaultAsync(x => x.PropertyNo == updatedItem.PropertyNo);
                        if (OPRItemExist != null)
                            return BadRequest(new { message = $"Property #{updatedItem.PropertyNo} already exists!" });

                        // If the item doesn't exist, add it to the list of items to add
                        var newOPRItems = new OPRItem
                        {
                            OPRINO = null,
                            oprNo = updatedItem.oprNo,
                            IID = updatedItem.IID,
                            Brand = updatedItem.Brand,
                            Model = updatedItem.Model,
                            Description = updatedItem.Description,
                            SerialNo = updatedItem.SerialNo,
                            PropertyNo = updatedItem.PropertyNo,
                            QRCode = updatedItem.QRCode,
                            Unit = updatedItem.Unit,
                            Amount = updatedItem.Amount,
                            Date_Acquired = updatedItem.Date_Acquired,
                        };
                        itemsToAdd.Add(newOPRItems);

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
                        //    return BadRequest(new { message = $"Property Card #{updatedItem.PropertyNo} already exists!" });

                        var propertyCard = new PropertyCard
                        {
                            REF = "OPR",
                            REFNoFrom = updatedItem.oprNo.ToString(),
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
                var propertyNosInUpdatedItems = updatedItems.Select(i => i.PropertyNo).ToHashSet();
                var itemsToDelete = existingItems.Where(e => !propertyNosInUpdatedItems.Contains(e.PropertyNo)).ToList();

                // Remove the items that are not in updatedItems
                if (itemsToDelete.Count > 0)
                {
                    dBContext.OPRItems.RemoveRange(itemsToDelete);
                }
                // Add the new items to the context
                if (itemsToAdd.Count > 0)
                {
                    await dBContext.OPRItems.AddRangeAsync(itemsToAdd);
                    // Add the new items to the context
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

            var exist = await dBContext.OPRS.FirstOrDefaultAsync(x => x.oprNo == id && x.postFlag == true);

            if (exist != null) { return BadRequest(new { message = "OPR already posted!" }); }

            // Find the OPR by id
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

                var cardExist = await dBContext1.PropertyCards.Where(x => x.REF == "OPR" && x.REFNoFrom == id.ToString()).ToListAsync();

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
public class CreateOPRRequest
{
    public OPRDto Details { get; set; }
    public List<OPRItem> oprItems { get; set; }
}
