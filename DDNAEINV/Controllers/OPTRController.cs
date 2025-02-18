using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPTRController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;

        public OPTRController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;

        }
        // localhost:port/api/OPTR
        [HttpGet]
        public IActionResult List()
        {

            var optr = dBContext.ListOfOPTR.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(optr);
        }
        // localhost:port/api/OPTR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<OPTRVw> Search(string key)
        {
            return dBContext.ListOfOPTR.Where(x => x.OPTRNo.Contains(key) ||
            x.itemSource.Contains(key) || x.ownership.ToString().Contains(key) ||
            x.received.ToString().Contains(key) || x.issued.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }
        // GET: api/items/generateID
        [HttpGet("generateID/{oprNo}")]
        public async Task<ActionResult<string>> GenerateOPTRID(string oprNo)
        {
            // SQL query to execute the function
            var sqlQuery = "SELECT dbo.GenerateOPTRID(@oprNo) AS GenOPTRID";

            // SQL parameter for the type
            var param = new SqlParameter("@oprNo", oprNo);

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
                    new { message = "OPTR No could not be generated.!" });
            }
        }

        // localhost:port/api/OPTR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateOPTRRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "OPTR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateOPTRID(@oprNo) AS GenOPTRID";

                // SQL parameter for the type
                //var param = new SqlParameter("@oprNo", details.oprNo+"");
                var param = new SqlParameter("@oprNo", (details.oprNo + "").ToString());

                // Execute the query and get the result
                string OPTRNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(param);

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    OPTRNo = scalarResult + "".ToString();
                }

                if (!string.IsNullOrEmpty(OPTRNo))
                {
                    var optr = new OPTR
                    {
                        OPTRNo = OPTRNo,
                        oprNo = details.oprNo,
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
                    await dBContext.OPTRS.AddAsync(optr);
                    await dBContext.SaveChangesAsync();

                    //return Ok(optr);
                    //return Ok(new
                    //{
                    //    message = "Successfully Saved!"
                    //});

                    // Fetch existing items by OPR No
                    var existingItems = await dBContext.OPRItems
                                                       .Where(x => x.oprNo == details.oprNo)
                                                       .ToListAsync();


                    foreach (var updatedItem in updatedItems)
                    {
                        // Find if the updated item exists in the existing items
                        var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                        if (existingItem != null)
                        {

                            // Update the existing item's fields with the updated data
                            existingItem.optrFlag = true;
                            existingItem.OPTRNo = OPTRNo;
                            // Update other fields as necessary


                            var propertyCards = new PropertyCard
                            {
                                Ref = "OPTR",
                                REFNoFrom = details.oprNo.ToString(),
                                REFNoTo = OPTRNo,
                                itemNo = existingItem.OPRINO,
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
                        details = optr,
                        items = existingItems
                    });
                }
                else
                {
                    return BadRequest(
                        new { message = "OPTR No could not be generated.!" });
                }


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/OPTR/Create/
        /**
        [HttpPost]
        [Route("RECreate")]
        public async Task<IActionResult> RECreate([FromBody] CreateOPTRRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "OPTR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateOPTRID(@oprNo) AS GenOPTRID";

                // SQL parameter for the type
                //var param = new SqlParameter("@oprNo", details.oprNo+"");
                var param = new SqlParameter("@oprNo", (details.oprNo + "").ToString());

                // Execute the query and get the result
                string OPTRNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(param);

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    OPTRNo = scalarResult + "".ToString();
                }

                if (!string.IsNullOrEmpty(OPTRNo))
                {
                    var optr = new OPTR
                    {
                        OPTRNo = OPTRNo,
                        oprNo = details.oprNo,
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
                    await dBContext.OPTRS.AddAsync(optr);
                    await dBContext.SaveChangesAsync();

                    //return Ok(optr);
                    //return Ok(new
                    //{
                    //    message = "Successfully Saved!"
                    //});

                    // Fetch existing items by OPR No
                    var existingItems = await dBContext.OPRItems
                                                       .Where(x => x.OPTRNo == details.OPTRNo)
                                                       .ToListAsync();

                    foreach (var updatedItem in updatedItems)
                    {
                        // Find if the updated item exists in the existing items
                        var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                        if (existingItem != null)
                        {

                            // Update the existing item's fields with the updated data
                            //existingItem.optrFlag = true;
                            existingItem.OPTRNo = OPTRNo;
                            // Update other fields as necessary
                        }
                    }

                    await dBContext.SaveChangesAsync();
                    return Ok(new
                    {
                        message = "Successfully Saved!",
                        details = optr,
                        items = existingItems
                    });
                }
                else
                {
                    return BadRequest(
                        new { message = "OPTR No could not be generated.!" });
                }


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }
        */


        // localhost:port/api/OPR/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id)
        {
            // Find the OPTR by id asynchronously
            var optr = await dBContext.ListOfOPTR
                                       .Where(x => x.OPTRNo == id)
                                       .FirstOrDefaultAsync();

            if (optr == null)
                return NotFound(new { message = "OPTR not Found!" });

            // Find the OPTR Item by id asynchronously
            var optrItem = await dBContext.OPRItems
                                           .Where(x => x.OPTRNo == id)
                                           .ToListAsync();

            if (optrItem == null || !optrItem.Any())
                return NotFound(new { message = "OPTR items not Found!" });

            return Ok(new
            {
                details = optr,
                parItems = optrItem,
            });
        }

        // localhost:port/api/OPTR/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateOPTRRequest request)
        {
            // Find the OPR by id
            var optr = await dBContext.OPTRS.FindAsync(id);

            if (optr == null)
                return NotFound(new { message = "OPTR not found." });

            OPTRDto details = request.Details;

            try
            {

                // Update the optr properties
                optr.oprNo = details.oprNo;
                optr.ttype = details.ttype;
                optr.otype = details.otype;
                optr.reason = details.reason;
                optr.receivedBy = details.receivedBy;
                optr.issuedBy = details.issuedBy;
                optr.approvedBy = details.approvedBy;
                optr.createdBy = details.createdBy;
                optr.Date_Updated = DateTime.Now;

                // Save OPTR changes
                await dBContext.SaveChangesAsync();

                // Fetch existing OPTR items by OPTR No
                var optrItems = await dBContext.OPRItems.Where(x => x.OPTRNo == id).ToListAsync();

                // Nullify OPTRNo and update optrFlag for old items
                foreach (var optrItem in optrItems)
                {
                    // Update the optr properties
                    optrItem.OPTRNo = null;
                    optrItem.optrFlag = false;
                }


                // Fetch existing items by OPR No
                var existingItems = await dBContext.OPRItems.ToListAsync();

                // Update existing items or poptre to add new ones
                foreach (var updatedItem in request.UpdatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {
                        // Update the existing item's fields with the updated data
                        existingItem.optrFlag = true;
                        existingItem.OPTRNo = id;
                        // Update other fields as necessary from updatedItem
                        existingItem.Brand = updatedItem.Brand;
                        existingItem.Model = updatedItem.Model;
                        existingItem.Description = updatedItem.Description;
                        existingItem.SerialNo = updatedItem.SerialNo;
                        existingItem.Amount = updatedItem.Amount;
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


        // localhost:port/api/OPTR/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the OPR by id
            var optr = dBContext.OPTRS.Find(id);

            if (optr == null)
                return NotFound(new { message = "OPTR not found." });

            try
            {

                // Update the property postFlag
                optr.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "OPTR # 000" + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        /**
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            // Check if the OPTR is already posted
            var OPRExist = await dBContext.OPTRS.FirstOrDefaultAsync(x => x.OPTRNo == id && x.postFlag == true);

            if (OPRExist != null)
                return BadRequest(new { message = "OPTR already posted!" });

            // Find the OPTR by id
            var optr = await dBContext.OPTRS.FirstOrDefaultAsync(x => x.OPTRNo == id);

            if (optr == null)
                return NotFound(new { message = "OPTR not found." });

            // Remove the OPTR
            dBContext.OPTRS.Remove(optr);
            await dBContext.SaveChangesAsync();  // Ensure save is async

            // Fetch existing OPR items by OPTR No
            var optrItems = await dBContext.OPRItems
                                               .Where(x => x.OPTRNo == id)
                                               .ToListAsync();

            // Nullify OPTRNo and update optrFlag
            foreach (var optrItem in optrItems)
            {
                optrItem.OPTRNo = null;
                optrItem.optrFlag = false;
            }

            // Save changes if any items were updated
            if (optrItems.Count > 0)
            {
                await dBContext.SaveChangesAsync();  // Use async save
            }

            var propertyCards = await dBContext.PropertyCards
                                               .Where(x => x.Ref == "OPTR" && x.RefNo == id)
                                               .ToListAsync();

            if (propertyCards.Count > 0)
            {
                dBContext.RemoveRange(propertyCards);
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }
        */
        
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            // Check if the OPTR is already posted
            var OPRExist = await dBContext.OPTRS.FirstOrDefaultAsync(x => x.OPTRNo == id && x.postFlag == true);
            if (OPRExist != null)
                return BadRequest(new { message = "OPTR already posted!" });

            // Find the OPTR by id
            var optr = await dBContext.OPTRS.FindAsync(id);
            if (optr == null)
                return NotFound(new { message = "OPTR not found." });

            // Fetch existing OPR items by OPTR No
            var optrItems = await dBContext.OPRItems.Where(x => x.OPTRNo == id).ToListAsync();

            // Update related OPR items (nullify OPTRNo and reset optrFlag)
            foreach (var optrItem in optrItems)
            {
                optrItem.OPTRNo = null;
                optrItem.optrFlag = false;
            }

            // Remove related property cards
            //var propertyCards = await dBContext.PropertyCards.Where(x => x.Ref == "OPTR" && x.RefNo == id).ToListAsync();
            //if (propertyCards.Any())
            //{
            //    dBContext.PropertyCards.RemoveRange(propertyCards);
            //}

            // Save changes for updated OPR items and removed property cards
            await dBContext.SaveChangesAsync();

            // Now, remove the OPTR record
            dBContext.OPTRS.Remove(optr);
            await dBContext.SaveChangesAsync();

            return Ok(new { message = "Successfully Removed" });
        }


        // localhost:port/api/OPTR/Transfer
        [HttpPost]
        [Route("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] CreateOPTRRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "OPTR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateOPTRID(@oprNo) AS GenOPTRID";

                // SQL parameter for the type
                var param = new SqlParameter("@oprNo", (details.oprNo+"").ToString());

                // Execute the query and get the result
                string OPTRNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(param);

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    OPTRNo = scalarResult + "".ToString();
                }

                if (!string.IsNullOrEmpty(OPTRNo))
                {
                    var optr = new OPTR
                    {
                        OPTRNo = OPTRNo,
                        oprNo = details.oprNo,
                        ttype = details.ttype,
                        otype = details.otype,
                        reason = details.reason,
                        receivedBy = details.receivedBy,
                        issuedBy = details.issuedBy,
                        approvedBy = details.approvedBy,
                        //postFlag = false,
                        //voidFlag = false,
                        createdBy = details.createdBy,
                        Date_Created = DateTime.Now,
                        Date_Updated = DateTime.Now
                    };
                    // Save changes to the database
                    await dBContext.OPTRS.AddAsync(optr);
                    await dBContext.SaveChangesAsync();

                    //return Ok(optr);
                    //return Ok(new
                    //{
                    //    message = "Successfully Saved!"
                    //});

                    // Fetch existing items by OPR No
                    var existingItems = await dBContext.OPRItems
                                                       .Where(x => x.oprNo == details.oprNo)
                                                       .ToListAsync();


                    foreach (var updatedItem in updatedItems)
                    {
                        // Find if the updated item exists in the existing items
                        var existingItem = existingItems.FirstOrDefault(x => x.OPRINO == updatedItem.OPRINO);
                        
                        if (existingItem != null)
                        {
                            var oldOPTRNo = existingItem.OPTRNo;

                            // Update the existing item's fields with the updated data
                            existingItem.optrFlag = true;
                            existingItem.OPTRNo = OPTRNo;
                            // Update other fields as necessary

                            var propertyCards = new PropertyCard
                            {
                                Ref = "OPTR",
                                REFNoFrom = oldOPTRNo,
                                REFNoTo = OPTRNo,
                                itemNo = existingItem.OPRINO,
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
                        details = optr,
                        items = existingItems
                    });
                }
                else
                {
                    return BadRequest(
                        new { message = "OPTR No could not be generated.!" });
                }


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }
   
    }
}

public class CreateOPTRRequest
{
    public OPTRDto Details { get; set; }
    public List<OPRItemDto> UpdatedItems { get; set; }
}
