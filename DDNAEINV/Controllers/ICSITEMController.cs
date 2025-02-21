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
    public class ICSITEMController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public ICSITEMController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        // localhost:port/api/PARITEM
        [HttpGet]
        public IActionResult List()
        {

            var icsItems = dBContext.ICSItems.ToList();

            return Ok(icsItems);
        }

        // localhost:port/api/ICSITEM/posted/
        [HttpGet]
        [Route("posted")]
        public IActionResult PostedICSItem()
        {
            var icsItems = dBContext.ICSItems
                .Where(pi => pi.rrsepFlag == false && 
                (pi.itrFlag == true && dBContext.ITRS.Any(p => p.ITRNo == pi.ITRNo && p.postFlag == true))
                || (pi.itrFlag == false && dBContext.ICSS.Any(p => p.ICSNo == pi.ICSNo && p.postFlag == true))).ToList();

            //var icsItems = dBContext.ICSItems
            //    .Where(pi => dBContext.ICSS.Any(p => p.ICSNo == pi.ICSNo && p.postFlag == true) ||
            //                 dBContext.ITRS.Any(r => r.ITRNo == pi.ITRNo && r.postFlag == true))
            //    .ToList();

            return Ok(icsItems);
        }

        // localhost:port/api/ICSITEM/Active/Search
        [HttpGet]
        [Route("posted/Search")]
        public IQueryable<ICSItem> SearchActiveICSItems(string key)
        {

            return dBContext.ICSItems
                .Where(pi => pi.rrsepFlag == false &&
                (pi.itrFlag == true && dBContext.ITRS.Any(p => p.ITRNo == pi.ITRNo && p.postFlag == true))
                || (pi.itrFlag == false && dBContext.ICSS.Any(p => p.ICSNo == pi.ICSNo && p.postFlag == true))
                          && (pi.Brand.Contains(key) || pi.Model.ToString().Contains(key) ||
            pi.Description.Contains(key) || pi.SerialNo.ToString().Contains(key) ||
            pi.PropertyNo.ToString().Contains(key) || pi.QRCode.ToString().Contains(key) ||
            pi.Date_Acquired.ToString().Contains(key)));

            //return dBContext.ICSItems
            //    .Where(pi => (dBContext.ICSS.Any(p => p.ICSNo == pi.ICSNo && p.postFlag == true) ||
            //                 dBContext.ITRS.Any(r => r.ITRNo == pi.ITRNo && r.postFlag == true))
            //              && (pi.Brand.Contains(key) || pi.Model.ToString().Contains(key) ||
            //pi.Description.Contains(key) || pi.SerialNo.ToString().Contains(key) ||
            //pi.PropertyNo.ToString().Contains(key) || pi.QRCode.ToString().Contains(key) ||
            //pi.Date_Acquired.ToString().Contains(key)));
        }


        // localhost:port/api/ICSITEM/SearchByICSNO/
        [HttpGet]
        [Route("SearchByPARNO")]
        public IQueryable<ICSItem> SearchByPARNO(string icsNo, string key)
        {
            return dBContext.ICSItems.Where(x => x.ICSNo == icsNo && x.Description.Contains(key));
        }

        // localhost:port/api/ICSITEM/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ICSItem> Search(string key)
        {
            return dBContext.ICSItems.Where(x => x.Description.Contains(key) || x.ICSNo.Contains(key));
        }


        // localhost:port/api/ICSITEM/Search/
        [HttpGet]
        [Route("ScanUnique")]
        public IQueryable<ICSItem> ScanUnique(string key)
        {
            return dBContext.ICSItems.Where(x => (!x.SerialNo.ToString().Contains("n/a") && x.SerialNo.ToString().Equals(key)) ||
            (!x.PropertyNo.ToString().Contains("n/a") && x.PropertyNo.ToString().Equals(key)) || (!x.QRCode.ToString().Contains("n/a") && x.QRCode.ToString().Equals(key)));
        }

        // localhost:port/api/ICSITEM/Search/
        [HttpGet]
        [Route("ScanExistingUnique")]
        public IQueryable<ICSItem> ScanExistingUnique(int ICSItemNo, string key)
        {
            return dBContext.ICSItems.Where(x => x.ICSItemNo != ICSItemNo &&
            (x.PropertyNo.ToString().Equals(key) || x.SerialNo.ToString().Equals(key) || x.QRCode.ToString().Equals(key)));
        }

        // localhost:port/api/ICSITEM/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] List<ICSItem> details)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "ICSITEM is Invalid!" });

            foreach (var item in details)
            {
                var parExist = await dBContext.ICSItems.FirstOrDefaultAsync(x => x.ICSNo == item.ICSNo);
                if (parExist != null)
                    return NotFound(new { message = $"ICS #{item.ICSNo} is not Found!" });

                // Add the item to the context
                dBContext.ICSItems.Add(item);
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


        // localhost:port/api/ICSITEM/ICSNO/{id}
        [HttpGet("ICSNO/{icsNo}")]
        public async Task<IActionResult> RetrieveByICSNO(string icsNo)
        {

            if (string.IsNullOrEmpty(icsNo))
            {
                return BadRequest(new { message = "ICS No is required." });
            }

            var items = await dBContext.ICSItems.Where(x => x.ICSNo == icsNo)
                                        .ToListAsync();

            if (items == null || items.Count == 0)
                return BadRequest(new { message = "No items found for the ICS # " + icsNo });

            return Ok(items);
        }


        // localhost:port/api/ICSITEM/ITRNO/{id}
        [HttpGet("ITRNO/{id}")]
        public async Task<IActionResult> RetrieveByITRNO(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ITR No is required." });
            }

            // Find the ITR Item by id asynchronously
            var itrItems = await dBContext.ICSItems
                                           .Where(x => x.ITRNo == id)
                                           .ToListAsync();


            if (itrItems == null || itrItems.Count == 0)
                return BadRequest(new { message = "No items found for the ITR # " + id });

            return Ok(itrItems);
        }

        // localhost:port/api/PARITEM/{id}
        [HttpGet("{icsItemNo}")]
        public async Task<IActionResult> Retrieve(int parINo)
        {

            var items = await dBContext.PARItems
                                        .Where(x => x.PARINO == parINo)
                                        .ToListAsync();

            return Ok(items);
        }


        // localhost:port/api/ICSITEM/QRCode/{id}
        [HttpGet("QRCode/{qrcode}")]
        public async Task<IActionResult> RetrieveByQRCode(string qrcode)
        {

            if (string.IsNullOrEmpty(qrcode))
            {
                return BadRequest(new { message = "QR Code is required." });
            }

            var items = await dBContext.ICSItems
                                        .Where(x => x.QRCode == qrcode)
                                        .ToListAsync();

            if (items == null || items.Count == 0)
            {
                return NotFound(new { message = "No items found for the QR Code " + qrcode });
            }

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
        public async Task<IActionResult> Delete(string icsNo)
        {
            if (string.IsNullOrEmpty(icsNo))
            {
                return BadRequest(new { message = "ICS No is required." });
            }

            var itemsToDelete = await dBContext.ICSItems.Where(x => x.ICSNo == icsNo).ToListAsync();

            if (itemsToDelete == null || itemsToDelete.Count == 0)
            {
                return NotFound(new { message = "No items found for the given PAR No." });
            }

            dBContext.ICSItems.RemoveRange(itemsToDelete);

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
