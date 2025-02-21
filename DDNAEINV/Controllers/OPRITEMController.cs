using DDNAEINV.Data;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPRITEMController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        public OPRITEMController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        // localhost:port/api/OPRITEM
        [HttpGet]
        public IActionResult List()
        {

            var opritems = dBContext.OPRItems.ToList();

            return Ok(opritems);
        }
        // localhost:port/api/OPRITEM
        [HttpGet]
        [Route("posted")]
        public IActionResult PosteOPRItems()
        {
            var opritems = dBContext.OPRItems
                .Where(pi => pi.oprrFlag == false &&
                (pi.optrFlag == true && dBContext.OPTRS.Any(p => p.OPTRNo == pi.OPTRNo && p.postFlag == true))
                || (pi.optrFlag == false && dBContext.OPRS.Any(p => p.oprNo == pi.oprNo && p.postFlag == true))).ToList();


            //var opritems = dBContext.OPRItems
            //    .Where(pi => dBContext.OPRS.Any(p => p.oprNo == pi.oprNo && p.postFlag == true)) 
            //    //|| dBContext.REPARS.Any(r => r.REPARNo == pi.REPARNo && r.postFlag == true))
            //    .ToList();

            return Ok(opritems);
        }

        // localhost:port/api/OPRITEM/Active/Search
        [HttpGet]
        [Route("posted/Search")]
        public IQueryable<OPRItem> SearchActiveOPRItems(string key)
        {
            return dBContext.OPRItems
                .Where(pi => pi.oprrFlag == false &&
                (pi.optrFlag == true && dBContext.OPTRS.Any(p => p.OPTRNo == pi.OPTRNo && p.postFlag == true))
                || (pi.optrFlag == false && dBContext.OPRS.Any(p => p.oprNo == pi.oprNo && p.postFlag == true))
                          && (pi.Brand.Contains(key) || pi.Model.ToString().Contains(key) ||
            pi.Description.Contains(key) || pi.SerialNo.ToString().Contains(key) ||
            pi.PropertyNo.ToString().Contains(key) || pi.QRCode.ToString().Contains(key) || pi.Date_Acquired.ToString().Contains(key)));
        }


        // localhost:port/api/OPRITEM/SearchByOPRNO/
        [HttpGet]
        [Route("SearchByOPRNO")]
        public IQueryable<OPRItem> SearchByOPRNO(int oprNo, string key)
        {
            return dBContext.OPRItems.Where(x => x.oprNo == oprNo && (
            x.Brand.Contains(key) || x.Model.ToString().Contains(key) ||
            x.Description.Contains(key) || x.SerialNo.ToString().Contains(key) ||
            x.PropertyNo.ToString().Contains(key) || x.QRCode.ToString().Contains(key) || x.Date_Acquired.ToString().Contains(key)));
        }

        // localhost:port/api/OPRITEM/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<OPRItem> Search(string key)
        {
            return dBContext.OPRItems.Where(x =>
            x.Brand.Contains(key) || x.Model.ToString().Contains(key) ||
            x.Description.Contains(key) || x.SerialNo.ToString().Contains(key) ||
            x.PropertyNo.ToString().Contains(key) || x.QRCode.ToString().Contains(key) || x.Date_Acquired.ToString().Contains(key));
        }


        // localhost:port/api/OPRITEM/Search/
        [HttpGet]
        [Route("ScanUnique")]
        public IQueryable<OPRItem> ScanUnique(string key)
        {
            return dBContext.OPRItems.Where(x => x.SerialNo.ToString().Equals(key) ||
            x.PropertyNo.ToString().Equals(key) || x.QRCode.ToString().Equals(key));
        }

        // localhost:port/api/OPRITEM/Search/
        [HttpGet]
        [Route("ScanExistingUnique")]
        public IQueryable<OPRItem> ScanExistingUnique(int oprINo, string key)
        {
            return dBContext.OPRItems.Where(x => x.OPRINO != oprINo && (x.SerialNo.ToString().Equals(key) ||
            x.PropertyNo.ToString().Equals(key) || x.QRCode.ToString().Equals(key)));
        }

        // localhost:port/api/OPRITEM/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] List<OPRItem> details)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "OPRITEM is Invalid!" });

            foreach (var item in details)
            {
                var oprExist = await dBContext.OPRItems.FirstOrDefaultAsync(x => x.oprNo == item.oprNo);
                if (oprExist != null)
                    return NotFound(new { message = $"OPR #000{item.oprNo} is not Found!" });

                var oprItemExist = await dBContext.OPRItems.FirstOrDefaultAsync(x => x.PropertyNo == item.PropertyNo);
                if (oprItemExist != null)
                    return BadRequest(new { message = $"Property #{item.PropertyNo} already exists!" });

                // Perform additional validation if necessary

                // Add the item to the context
                dBContext.OPRItems.Add(item);
            }

            try
            {
                // Save changes to the database
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


        // localhost:port/api/OPRITEM/OPRNO/{id}
        [HttpGet("OPRNO/{oprNo}")]
        public async Task<IActionResult> RetrieveByOPRNO(int oprNo)
        {

            if (oprNo != 0)
            {
                var items = await dBContext.OPRItems
                                            .Where(x => x.oprNo == oprNo)
                                            .ToListAsync();

                if (items == null || items.Count == 0)
                {
                    return NotFound(new { message = "No items found for the OPR #000" + oprNo });
                }

                return Ok(items);
            }

            return BadRequest(new { message = "OPR No is required." });
        }

        // localhost:port/api/OPRITEM/OPTRNO/{id}
        [HttpGet("OPTRNO/{optrNo}")]
        public async Task<IActionResult> RetrieveByOPTRNO(string optrNo)
        {

            if (optrNo != null)
            {
                var items = await dBContext.OPRItems
                                            .Where(x => x.OPTRNo == optrNo)
                                            .ToListAsync();

                if (items == null || items.Count == 0)
                {
                    return NotFound(new { message = "No items found for the OPTR #000" + optrNo });
                }

                return Ok(items);
            }

            return BadRequest(new { message = "OPTR No is required." });
        }

        // localhost:port/api/OPRITEM/QRCode/{id}
        [HttpGet("QRCode/{qrcode}")]
        public async Task<IActionResult> RetrieveByQRCode(string qrcode)
        {

            if (string.IsNullOrEmpty(qrcode))
            {
                return BadRequest(new { message = "QR Code is required." });
            }

            var items = await dBContext.OPRItems
                                        .Where(x => x.QRCode == qrcode)
                                        .ToListAsync();

            if (items == null || items.Count == 0)
            {
                return NotFound(new { message = "No items found for the QR Code " + qrcode });
            }

            return Ok(items);
        }


        // localhost:port/api/OPRITEM/{id}
        [HttpGet("{oprINo}")]
        public async Task<IActionResult> Retrieve(int oprINo)
        {

            var items = await dBContext.OPRItems
                                        .Where(x => x.OPRINO == oprINo)
                                        .ToListAsync();

            return Ok(items);
        }

        // localhost:port/api/OPRITEM/Update
        [HttpPut]
        [Route("UpdateByID")]
        public async Task<IActionResult> UpdateByID(int opriNo, [FromBody] OPRItemDto items)
        {
            var oprItems = dBContext.OPRItems.Find(opriNo);


            if (oprItems == null)
                return BadRequest(new { message = "OPR No and items are required." });

            try
            {


                // Update the existing item's fields with the updated data
                oprItems.IID = items.IID;
                oprItems.Brand = items.Brand;
                oprItems.Model = items.Model;
                oprItems.Description = items.Description;
                oprItems.SerialNo = items.SerialNo;
                oprItems.PropertyNo = items.PropertyNo;
                oprItems.QRCode = items.QRCode;
                oprItems.Unit = items.Unit;
                oprItems.Amount = items.Amount;
                oprItems.Date_Acquired = items.Date_Acquired;

                // Save all changes (updates and additions)
                await dBContext.SaveChangesAsync();
                return Ok(new
                {
                    message = "Successfully Updated!"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the data.", error = ex.Message });
            }
        }


        // localhost:port/api/OPRITEM/Update
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(int oprNo, [FromBody] List<OPRItemDto> updatedItems)
        {
            if (oprNo == null || oprNo == 0 || updatedItems == null || updatedItems.Count == 0)
            {
                return BadRequest(new { message = "OPR No and updated items are required." });
            }

            // Fetch existing items by OPR No
            var existingItems = await dBContext.OPRItems
                                               .Where(x => x.oprNo == oprNo)
                                               .ToListAsync();

            // Prepare a list to keep track of items that need to be added
            var itemsToAdd = new List<OPRItem>();

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

                    var oprItemExist = await dBContext.OPRItems.FirstOrDefaultAsync(x => x.PropertyNo == updatedItem.PropertyNo);
                    if (oprItemExist != null)
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
            }

            try
            {
                // Save all changes (updates and additions)
                await dBContext.SaveChangesAsync();
                return Ok(new
                {
                    message = "Successfully Updated!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the data.", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateExisting")]
        public async Task<IActionResult> UpdateExisting(int oprNo, [FromBody] List<OPRItemDto> updatedItems)
        {

            //Debug.Write("LIST OF OPRITEM" + updatedItems);

            if (oprNo == 0 || updatedItems == null || updatedItems.Count == 0)
            {
                return BadRequest(new { message = "OPR No and updated items are required." });
            }

            // Fetch existing items by OPR No
            var existingItems = await dBContext.OPRItems
                                               .Where(x => x.oprNo == oprNo)
                                               .ToListAsync();

            // Delete existing items
            if (existingItems.Count > 0)
            {
                dBContext.OPRItems.RemoveRange(existingItems);
            }

            // Add new items
            foreach (var newItem in updatedItems)
            {
                var newOPRItems = new OPRItem
                {
                    oprNo = newItem.oprNo,
                    IID = newItem.IID,
                    Brand = newItem.Brand,
                    Model = newItem.Model,
                    Description = newItem.Description,
                    SerialNo = newItem.SerialNo,
                    PropertyNo = newItem.PropertyNo,
                    QRCode = newItem.QRCode,
                    Unit = newItem.Unit,
                    Amount = newItem.Amount,
                    Date_Acquired = newItem.Date_Acquired,
                };

                await dBContext.OPRItems.AddAsync(newOPRItems);
            }

            try
            {
                // Save all changes (deletions and additions)
                await dBContext.SaveChangesAsync();
                return Ok(new { message = "Items successfully updated!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the data.", error = ex.Message });
            }
        }




        // localhost:port/api/OPRITEM/Delete
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int oprNo)
        {
            if (oprNo == 0)
            {
                return BadRequest(new { message = "OPR No is required." });
            }

            var itemsToDelete = await dBContext.OPRItems.Where(x => x.oprNo == oprNo).ToListAsync();

            if (itemsToDelete == null || itemsToDelete.Count == 0)
            {
                return NotFound(new { message = "No items found for the given OPR No." });
            }

            dBContext.OPRItems.RemoveRange(itemsToDelete);

            try
            {
                await dBContext.SaveChangesAsync();
                return Ok(new { message = "Successfully Removed" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while deleting the data.", error = ex.Message });
            }
        }

    }
}
