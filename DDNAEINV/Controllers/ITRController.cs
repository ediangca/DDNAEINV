using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Views;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ITRController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public ITRController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/ITR
        [HttpGet]
        public IActionResult List()
        {
            var itr = dBContext.ListOfITR
                   .OrderByDescending(x => x.Date_Created) // Replace SomeProperty with the column to sort by
                   .Take(10)
                   .ToList();

            return Ok(itr);
        }
        // localhost:port/api/ITR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ITRVw> Search(string key)
        {
            return dBContext.ListOfITR.Where(x => x.icsNo.Contains(key) ||
            x.entityName.Contains(key) || x.fund.ToString().Contains(key) ||
            x.otype.Contains(key) || x.reason.ToString().Contains(key) ||
            x.approved.ToString().Contains(key)  || x.received.ToString().Contains(key) || x.issued.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created)
                .Take(10);
        }
        // GET: api/ICS/generateID
        [HttpGet("generateITRID/{icsNo}")]
        public async Task<ActionResult<string>> GenerateITRID(string icsNo)
        {
            // SQL query to execute the function
            var sqlQuery = "SELECT dbo.GenerateITRID(@icsNo) AS GenITRID";

            // SQL parameter for the type
            var param = new SqlParameter("@icsNo", icsNo);

            // Execute the query and get the result
            string result;
            using (var command = dBContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.Parameters.Add(param);

                await dBContext.Database.OpenConnectionAsync();

                var scalarResult = await command.ExecuteScalarAsync();
                result = scalarResult + "".ToString();
            }

            if (!string.IsNullOrEmpty(result))
            {
                return Ok(new
                {
                    id = result
                });
            }
            else
            {
                return BadRequest(
                    new { message = "ITR ID could not be generated.!" });
            }
        }

        // localhost:port/api/REPAR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateITRRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "ITR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateITRID(@icsNo) AS GenITRID";

                // SQL parameter for the type
                var param = new SqlParameter("@icsNo", details.icsNo);

                // Execute the query and get the result
                string ITRNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(param);

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    ITRNo = scalarResult + "".ToString();
                }

                if (!string.IsNullOrEmpty(ITRNo))
                {
                    var itr = new ITR
                    {
                        ITRNo = ITRNo,
                        icsNo = details.icsNo,
                        ttype = details.ttype,
                        otype = details.otype,
                        reason = details.reason,
                        receivedBy = details.receivedBy,
                        issuedBy = details.issuedBy,
                        approvedBy = details.approvedBy,
                        postFlag = details.postFlag,
                        voidFlag = details.voidFlag,
                        createdBy = details.createdBy,
                        Date_Created = DateTime.Now,
                        Date_Updated = DateTime.Now
                    };

                    // Save changes to the database
                    await dBContext.ITRS.AddAsync(itr);
                    await dBContext.SaveChangesAsync();

                    // Fetch existing items by ICS No
                    var existingItems = await dBContext.ICSItems
                                                       .Where(x => x.ICSNo == details.icsNo)
                                                       .ToListAsync();


                    foreach (var updatedItem in updatedItems)
                    {
                        // Find if the updated item exists in the existing items
                        var existingItem = existingItems.FirstOrDefault(x => x.ICSItemNo == updatedItem.ICSItemNo);

                        if (existingItem != null)
                        {

                            // Update the existing item's fields with the updated data
                            existingItem.itrFlag = true;
                            existingItem.ITRNo = ITRNo;
                            // Update other fields as necessary
                        }
                    }

                    await dBContext.SaveChangesAsync();
                    return Ok(new
                    {
                        message = "Successfully Saved!",
                        details = itr,
                        items = existingItems
                    });
                }
                else
                {
                    return BadRequest(
                        new { message = "ITR ID could not be generated.!" });
                }


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/ITR/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id)
        {
            // Find the ITR by id asynchronously
            var itr = await dBContext.ListOfITR
                                       .Where(x => x.ITRNo == id)
                                       .FirstOrDefaultAsync();

            if (itr == null)
                return NotFound(new { message = "ITR not Found!" });

            // Find the ITR Item by id asynchronously
            var itrItem = await dBContext.ICSItems
                                           .Where(x => x.ITRNo == id)
                                           .ToListAsync();

            if (itrItem == null || !itrItem.Any())
                return NotFound(new { message = "ITR items not Found!" });

            return Ok(new
            {
                details = itr,
                itrItems = itrItem,
            });
        }


        // localhost:port/api/ITR/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateITRRequest request)
        {
            // Find the PAR by id
            var itr = await dBContext.ITRS.FindAsync(id);

            if (itr == null)
                return NotFound(new { message = "ITR not found." });

            ITRDto details = request.Details;

            try
            {

                // Update the repar properties
                itr.icsNo = details.icsNo;
                itr.ttype = details.ttype;
                itr.otype = details.otype;
                itr.reason = details.reason;
                itr.receivedBy = details.receivedBy;
                //itr.issuedBy = details.issuedBy;
                itr.approvedBy = details.approvedBy;
                itr.createdBy = details.createdBy;
                itr.Date_Updated = DateTime.Now;

                // Save REPAR changes
                await dBContext.SaveChangesAsync();

                // Fetch existing ICS items by ITRNO No
                var itrItems = await dBContext.ICSItems.Where(x => x.ITRNo == id).ToListAsync();

                // Nullify REPARNo and update reparFlag for old items
                foreach (var itrItem in itrItems)
                {
                    // Update the repar properties
                    itrItem.ITRNo = null;
                    itrItem.itrFlag = false;
                }

                await dBContext.SaveChangesAsync();

                var updatedItems = request.UpdatedItems;

                // Fetch existing items by ICS No
                var existingItems = await dBContext.ICSItems.ToListAsync();

                // Update existing items or prepare to add new ones
                foreach (var updatedItem in updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.ICSItemNo == updatedItem.ICSItemNo);

                    if (existingItem != null)
                    {
                        // Update the existing item's fields with the updated data
                        existingItem.itrFlag = true;
                        existingItem.ITRNo = id;
                        // Update other fields as necessary from updatedItem
                        existingItem.Description = updatedItem.Description;
                        existingItem.Qty = updatedItem.Qty;
                        existingItem.Amount = updatedItem.Amount;
                        existingItem.EUL = updatedItem.EUL;
                    }
                }

                // Save all changes to the database at once
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


        // localhost:port/api/ITR/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the PAR by id
            var itr = dBContext.ITRS.Find(id);

            if (itr == null)
                return NotFound(new { message = "ITR not found." });

            try
            {

                // Update the property postFlag
                itr.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "ITR # " + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
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
            // Check if the ITR is already posted
            var ITRExist = await dBContext.ITRS.FirstOrDefaultAsync(x => x.ITRNo == id && x.postFlag == true);

            if (ITRExist != null)
                return BadRequest(new { message = "ITR already posted!" });

            // Find the ITR by id
            var itr = await dBContext.ITRS.FirstOrDefaultAsync(x => x.ITRNo == id);

            if (itr == null)
                return NotFound(new { message = "ITR not found." });

            // Remove the ITR
            dBContext.ITRS.Remove(itr);
            await dBContext.SaveChangesAsync();  // Ensure save is async

            // Fetch existing ICS items by ITR No
            var itrItems = await dBContext.ICSItems
                                               .Where(x => x.ITRNo == id)
                                               .ToListAsync();

            // Nullify ITRNo and update reparFlag
            foreach (var reparItem in itrItems)
            {
                reparItem.ITRNo = null;
                reparItem.itrFlag = false;
            }

            // Save changes if any items were updated
            if (itrItems.Count > 0)
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

public class CreateITRRequest
{
    public ITRDto Details { get; set; }
    public List<ICSItemDto> UpdatedItems { get; set; }
}
