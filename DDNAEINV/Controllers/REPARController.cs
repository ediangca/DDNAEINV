using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;



namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class REPARController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ApplicationDBContext dBContext1;

        public REPARController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
            this.dBContext1 = dBContext;

        }
        // localhost:port/api/REPAR
        [HttpGet]
        public IActionResult List()
        {

            var repar = dBContext.ListOfREPar.ToList()
                   .OrderByDescending(x => x.Date_Created) 
                   .ToList();

            return Ok(repar);
        }
        // localhost:port/api/REPAR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<RePARVw> Search(string key)
        {
            return dBContext.ListOfREPar.Where(x => x.parNo.Contains(key) ||
            x.lgu.Contains(key) || x.fund.ToString().Contains(key) ||
            x.received.ToString().Contains(key) || x.issued.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }
        // GET: api/items/generateID
        [HttpGet("generateID/{parNo}")]
        public async Task<ActionResult<string>> GenerateREPARID(string parNo)
        {
            // SQL query to execute the function
            var sqlQuery = "SELECT dbo.GenerateREPARID(@parNo) AS GenREPARID";

            // SQL parameter for the type
            var param = new SqlParameter("@parNo", parNo);

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
                    new { message = "REPAR No could not be generated.!" });
            }
        }

        // localhost:port/api/REPAR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateReparRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "REPAR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateREPARID(@parNo) AS GenREPARID";

                // SQL parameter for the type
                var param = new SqlParameter("@parNo", details.parNo);

                // Execute the query and get the result
                string REPARNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(param);

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    REPARNo = scalarResult + "".ToString();
                }

                if (!string.IsNullOrEmpty(REPARNo))
                {
                    var repar = new RePAR
                    {
                        REPARNo = REPARNo,
                        parNo = details.parNo,
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
                    await dBContext.REPARS.AddAsync(repar);
                    await dBContext.SaveChangesAsync();

                    var existingItems = await dBContext.PARItems
                                                       .Where(x => x.PARNo == details.parNo)
                                                       .ToListAsync();


                    foreach (var updatedItem in updatedItems)
                    {
                        // Find if the updated item exists in the existing items
                        var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                        if (existingItem != null)
                        {

                            // Update the existing item's fields with the updated data
                            existingItem.reparFlag = true;
                            existingItem.REPARNo = REPARNo;

                            //Store into Property History
                            var propertyCards = new PropertyCard
                            {
                                REF = "PTR",
                                REFNoFrom = details.parNo,
                                REFNoTo = REPARNo,
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
                        details = repar,
                        items = existingItems
                    });
                }
                else
                {
                    return BadRequest(
                        new { message = "REPAR No could not be generated.!" });
                }


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/PAR/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id)
        {
            // Find the REPAR by id asynchronously
            var repar = await dBContext.ListOfREPar
                                       .Where(x => x.REPARNo == id)
                                       .FirstOrDefaultAsync();

            if (repar == null)
                return NotFound(new { message = "REPAR not Found!" });

            // Find the REPAR Item by id asynchronously
            var reparItem = await dBContext.PARItems
                                           .Where(x => x.REPARNo == id)
                                           .ToListAsync();

            if (reparItem == null || !reparItem.Any())
                return NotFound(new { message = "REPAR items not Found!" });

            return Ok(new
            {
                details = repar,
                parItems = reparItem,
            });
        }

        // localhost:port/api/REPAR/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateReparRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            // Find the PAR by id
            var repar = await dBContext.REPARS.FindAsync(id);

            if (repar == null)
                return NotFound(new { message = "REPAR not found." });

            try
            {

                // Update the repar properties
                repar.parNo = details.parNo;
                repar.ttype = details.ttype;
                repar.otype = details.otype;
                repar.reason = details.reason;
                repar.receivedBy = details.receivedBy;
                repar.issuedBy = details.issuedBy;
                repar.approvedBy = details.approvedBy;
                repar.createdBy = details.createdBy;
                repar.Date_Updated = DateTime.Now;

                // Save REPAR changes
                await dBContext.SaveChangesAsync();

                // Fetch existing REPAR items by REPAR No
                var reparItems = await dBContext.PARItems.Where(x => x.REPARNo == id).ToListAsync();

                // Fetch existing CARD by PAR No
                var existingProperties = await dBContext1.PropertyCards
                                                   .Where(x => x.REF == "PTR" && x.REFNoTo == id)
                                                   .ToListAsync();

                // Nullify REPARNo and update reparFlag for old items
                foreach (var reparItem in reparItems)
                {
                    // Update the repar properties
                    reparItem.REPARNo = null;
                    reparItem.reparFlag = false;

                }

                // Fetch existing items by PAR No
                var existingItems = await dBContext.PARItems.ToListAsync();


                var propertyToAdd = new List<PropertyCard>();

                // Update existing items or prepare to add new ones
                foreach (var updatedItem in updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {
                        // Update the existing item's fields with the updated data
                        existingItem.reparFlag = true;
                        existingItem.REPARNo = id;
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
                            REF = "PTR",
                            REFNoFrom = updatedItem.PARNo,
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


                // Save all changes to the database at once
                await dBContext.SaveChangesAsync();
                await dBContext1.SaveChangesAsync();

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


        // localhost:port/api/REPAR/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the PAR by id
            var repar = dBContext.REPARS.Find(id);

            if (repar == null)
                return NotFound(new { message = "REPAR not found." });

            try
            {

                // Update the property postFlag
                repar.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(par);
                return Ok(new
                {
                    message = "REPAR # " + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
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
            // Check if the REPAR is already posted
            var PARExist = await dBContext.REPARS.FirstOrDefaultAsync(x => x.REPARNo == id && x.postFlag == true);

            if (PARExist != null)
                return BadRequest(new { message = "REPAR already posted!" });

            // Find the REPAR by id
            var repar = await dBContext.REPARS.FirstOrDefaultAsync(x => x.REPARNo == id);

            if (repar == null)
                return NotFound(new { message = "REPAR not found." });

            // Remove the REPAR
            dBContext.REPARS.Remove(repar);
            await dBContext.SaveChangesAsync();  // Ensure save is async

            // Fetch existing PAR items by REPAR No
            var reparItems = await dBContext.PARItems
                                               .Where(x => x.REPARNo == id)
                                               .ToListAsync();

            // Nullify REPARNo and update reparFlag
            foreach (var item in reparItems)
            {
                item.REPARNo = null;
                item.reparFlag = false;
                var cardExist = await dBContext1.PropertyCards.FirstOrDefaultAsync(x => x.REF == "PTR" && x.PropertyNo == item.PropertyNo);

                if (cardExist != null)
                {
                    dBContext1.PropertyCards.Remove(cardExist);
                    await dBContext1.SaveChangesAsync();  
                }
            }

            // Save changes if any items were updated
            if (reparItems.Count > 0)
            {
                await dBContext.SaveChangesAsync();  // Use async save
            }

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }


        // localhost:port/api/REPAR/Transfer
        [HttpPost]
        [Route("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] CreateReparRequest request)
        {
            var details = request.Details;
            var updatedItems = request.UpdatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "PTR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateREPARID(@parNo) AS GenPTRID";

                // SQL parameter for the type
                var param = new SqlParameter("@parNo", (details.parNo + "").ToString());

                // Execute the query and get the result
                string ptrNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(param);

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    ptrNo = scalarResult + "".ToString();
                }

                if (!string.IsNullOrEmpty(ptrNo))
                {
                    var ptr = new RePAR
                    {
                        REPARNo = ptrNo,
                        parNo = details.parNo,
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
                    await dBContext.REPARS.AddAsync(ptr);
                    await dBContext.SaveChangesAsync();

                    //return Ok(optr);
                    //return Ok(new
                    //{
                    //    message = "Successfully Saved!"
                    //});

                    // Fetch existing items by PAR No
                    var existingItems = await dBContext.PARItems
                                                       .Where(x => x.PARNo == details.parNo)
                                                       .ToListAsync();


                    foreach (var updatedItem in updatedItems)
                    {
                        // Find if the updated item exists in the existing items
                        var existingItem = existingItems.FirstOrDefault(x => x.PARINO == updatedItem.PARINO);

                        if (existingItem != null)
                        {
                            var oldPTRNo = existingItem.REPARNo;

                            // Update the existing item's fields with the updated data
                            existingItem.reparFlag = true;
                            existingItem.REPARNo = ptrNo;
                            // Update other fields as necessary

                            var propertyCards = new PropertyCard
                            {
                                REF = "PTR",
                                REFNoFrom = oldPTRNo,
                                REFNoTo = ptrNo,
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
                        details = ptr,
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

public class CreateReparRequest
{
    public RePARDto Details { get; set; }
    public List<ParItemDto> UpdatedItems { get; set; }
}
