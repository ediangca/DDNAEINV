using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PARITEMController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;

        public PARITEMController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;
        }
        // localhost:port/api/PARITEM
        [HttpGet]
        public IActionResult List()
        {

            var paritems = dBContext.PARItems.ToList();

            return Ok(paritems);
        }
        // localhost:port/api/PARITEM
        [HttpGet]
        [Route("posted")]
        public IActionResult PostedPARItems()
        {

            var paritems = dBContext.PARItems
                .Where(pi => dBContext.PARS.Any(p => p.parNo == pi.PARNo && p.postFlag == true) ||
                             dBContext.REPARS.Any(r => r.REPARNo == pi.REPARNo && r.postFlag == true))
                .ToList();

            return Ok(paritems);
        }

        // localhost:port/api/PARITEM/Active/Search
        [HttpGet]
        [Route("posted/Search")]
        public IQueryable<ParItem> SearchActivePARItems(string key)
        {
            return dBContext.PARItems
                .Where(pi => (dBContext.PARS.Any(p => p.parNo == pi.PARNo && p.postFlag == true) ||
                             dBContext.REPARS.Any(r => r.REPARNo == pi.REPARNo && r.postFlag == true))
                          && (pi.Brand.Contains(key) || pi.Model.ToString().Contains(key) ||
            pi.Description.Contains(key) || pi.SerialNo.ToString().Contains(key) ||
            pi.PropertyNo.ToString().Contains(key) || pi.QRCode.ToString().Contains(key) || pi.Date_Acquired.ToString().Contains(key)));
        }


        // localhost:port/api/PARITEM/SearchByPARNO/
        [HttpGet]
        [Route("SearchByPARNO")]
        public IQueryable<ParItem> SearchByPARNO(string parNo, string key)
        {
            return dBContext.PARItems.Where(x => x.PARNo == parNo && (
            x.Brand.Contains(key) || x.Model.ToString().Contains(key) ||
            x.Description.Contains(key) || x.SerialNo.ToString().Contains(key) ||
            x.PropertyNo.ToString().Contains(key) || x.QRCode.ToString().Contains(key) || x.Date_Acquired.ToString().Contains(key)));
        }

        // localhost:port/api/PARITEM/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ParItem> Search(string key)
        {
            return dBContext.PARItems.Where(x =>
            x.Brand.Contains(key) || x.Model.ToString().Contains(key) ||
            x.Description.Contains(key) || x.SerialNo.ToString().Contains(key) ||
            x.PropertyNo.ToString().Contains(key) || x.QRCode.ToString().Contains(key) || x.Date_Acquired.ToString().Contains(key));
        }


        // localhost:port/api/PARITEM/Search/
        [HttpGet]
        [Route("ScanUnique")]
        public IQueryable<ParItem> ScanUnique(string key)
        {
            return dBContext.PARItems.Where(x => x.SerialNo.ToString().Equals(key) ||
            x.PropertyNo.ToString().Equals(key) || x.QRCode.ToString().Equals(key));
        }

        // localhost:port/api/PARITEM/Search/
        [HttpGet]
        [Route("ScanExistingUnique")]
        public IQueryable<ParItem> ScanExistingUnique(int parino, string key)
        {
            return dBContext.PARItems.Where(x => x.PARINO != parino && (x.SerialNo.ToString().Equals(key) ||
            x.PropertyNo.ToString().Equals(key) || x.QRCode.ToString().Equals(key)));
        }

        // localhost:port/api/PARITEM/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] List<ParItem> details)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "PARITEM is Invalid!" });

            foreach (var item in details)
            {
                var parExist = await dBContext.PARItems.FirstOrDefaultAsync(x => x.PARNo == item.PARNo);
                if (parExist != null)
                    return NotFound(new { message = $"PAR #{item.PARNo} is not Found!" });

                var parItemExist = await dBContext.PARItems.FirstOrDefaultAsync(x => x.PropertyNo == item.PropertyNo);
                if (parItemExist != null)
                    return BadRequest(new { message = $"Property #{item.PropertyNo} already exists!" });

                // Perform additional validation if necessary

                // Add the item to the context
                dBContext.PARItems.Add(item);

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


        // localhost:port/api/PARITEM/PARNO/{id}
        [HttpGet("PARNO/{parNo}")]
        public async Task<IActionResult> RetrieveByPARNO(string parNo)
        {

            if (string.IsNullOrEmpty(parNo))
            {
                return BadRequest(new { message = "PAR No is required." });
            }

            var items = await dBContext.PARItems
                                        .Where(x => x.PARNo == parNo)
                                        .ToListAsync();

            if (items == null || items.Count == 0)
            {
                return NotFound(new { message = "No items found for the PAR # " + parNo });
            }

            return Ok(items);
        }


        // localhost:port/api/PARITEM/QRCode/{id}
        [HttpGet("QRCode/{qrcode}")]
        public async Task<IActionResult> RetrieveByQRCode(string qrcode)
        {

            if (string.IsNullOrEmpty(qrcode))
            {
                return BadRequest(new { message = "QR Code is required." });
            }

            var items = await dBContext.PARItems
                                        .Where(x => x.QRCode == qrcode)
                                        .ToListAsync();

            if (items == null || items.Count == 0)
            {
                return NotFound(new { message = "No items found for the QR Code " + qrcode });
            }

            return Ok(items);
        }


        // localhost:port/api/PARITEM/{id}
        [HttpGet("{parINo}")]
        public async Task<IActionResult> Retrieve(int parINo)
        {

            var items = await dBContext.PARItems
                                        .Where(x => x.PARINO == parINo)
                                        .ToListAsync();

            return Ok(items);
        }

        // localhost:port/api/PARITEM/Update
        [HttpPut]
        [Route("UpdateByID")]
        public async Task<IActionResult> UpdateByID(int pariNo, [FromBody] ParItemDto items)
        {
            var parItems = dBContext.PARItems.Find(pariNo);


            if (parItems == null)
                return BadRequest(new { message = "PAR No and items are required." });

            try
            {


                // Update the existing item's fields with the updated data
                parItems.IID = items.IID;
                parItems.Brand = items.Brand;
                parItems.Model = items.Model;
                parItems.Description = items.Description;
                parItems.SerialNo = items.SerialNo;
                parItems.PropertyNo = items.PropertyNo;
                parItems.QRCode = items.QRCode;
                parItems.Unit = items.Unit;
                parItems.Amount = items.Amount;
                parItems.Date_Acquired = items.Date_Acquired;

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


        // localhost:port/api/PARITEM/Update
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string parNo, [FromBody] List<ParItemDto> updatedItems)
        {
            if (string.IsNullOrEmpty(parNo) || updatedItems == null || updatedItems.Count == 0)
            {
                return BadRequest(new { message = "PAR No and updated items are required." });
            }

            // Fetch existing items by PAR No
            var existingItems = await dBContext.PARItems
                                               .Where(x => x.PARNo == parNo)
                                               .ToListAsync();

            // Prepare a list to keep track of items that need to be added
            var itemsToAdd = new List<ParItem>();

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

                    var parItemExist = await dBContext.PARItems.FirstOrDefaultAsync(x => x.PropertyNo == updatedItem.PropertyNo);
                    if (parItemExist != null)
                        return BadRequest(new { message = $"Property #{updatedItem.PropertyNo} already exists!" });

                    // If the item doesn't exist, add it to the list of items to add
                    var newParItems = new ParItem
                    {
                        PARINO = null,
                        PARNo = updatedItem.PARNo,
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
                    itemsToAdd.Add(newParItems);
                }
            }
            // Identify the items that need to be deleted (those in existingItems but not in updatedItems)
            var propertyNosInUpdatedItems = updatedItems.Select(i => i.PropertyNo).ToHashSet();
            var itemsToDelete = existingItems.Where(e => !propertyNosInUpdatedItems.Contains(e.PropertyNo)).ToList();

            // Remove the items that are not in updatedItems
            if (itemsToDelete.Count > 0)
            {
                dBContext.PARItems.RemoveRange(itemsToDelete);
            }
            // Add the new items to the context
            if (itemsToAdd.Count > 0)
            {
                await dBContext.PARItems.AddRangeAsync(itemsToAdd);
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
        public async Task<IActionResult> UpdateExisting(string parNo, [FromBody] List<ParItemDto> updatedItems)
        {

            //Debug.Write("LIST OF PARITEM" + updatedItems);

            if (string.IsNullOrEmpty(parNo) || updatedItems == null || updatedItems.Count == 0)
            {
                return BadRequest(new { message = "PAR No and updated items are required." });
            }

            // Fetch existing items by PAR No
            var existingItems = await dBContext.PARItems
                                               .Where(x => x.PARNo == parNo)
                                               .ToListAsync();

            // Delete existing items
            if (existingItems.Count > 0)
            {
                dBContext.PARItems.RemoveRange(existingItems);
            }

            // Add new items
            foreach (var newItem in updatedItems)
            {
                var newParItems = new ParItem
                {
                    PARNo = newItem.PARNo,
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

                Debug.WriteLine("LIST OF PARITEM : " + newItem.PARNo + ", " +
                    newItem.IID + ", " +
                    newItem.Brand + ", " +
                    newItem.Model + ", " +
                    newItem.Description + ", " +
                    newItem.SerialNo + ", " +
                    newItem.PropertyNo + ", " +
                    newItem.QRCode + ", " +
                    newItem.Unit + ", " +
                    newItem.Amount + ", " +
                    newItem.Date_Acquired);

                await dBContext.PARItems.AddAsync(newParItems);
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




        // localhost:port/api/PARITEM/Delete
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string parNo)
        {
            if (string.IsNullOrEmpty(parNo))
            {
                return BadRequest(new { message = "PAR No is required." });
            }

            var itemsToDelete = await dBContext.PARItems.Where(x => x.PARNo == parNo).ToListAsync();

            if (itemsToDelete == null || itemsToDelete.Count == 0)
            {
                return NotFound(new { message = "No items found for the given PAR No." });
            }

            dBContext.PARItems.RemoveRange(itemsToDelete);

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
