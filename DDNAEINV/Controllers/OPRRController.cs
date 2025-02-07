using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DDNAEINV.Model.Views;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPRRController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public OPRRController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/OPRR
        [HttpGet]
        public IActionResult List()
        {

            var opr = dBContext.ListOfOPRR.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(opr);
        }
        // localhost:port/api/OPRR/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<OPRRVw> Search(string key)
        {
            return dBContext.ListOfOPRR.Where(x => x.OPRRNo.Contains(key) ||
            x.issued!.ToString().Contains(key) || x.issuedBy!.ToString().Contains(key) ||
            x.received!.ToString().Contains(key) || x.receivedBy!.ToString().Contains(key) ||
            x.approved!.ToString().Contains(key) || x.approvedBy!.ToString().Contains(key) ||
            x.created.ToString().Contains(key) || x.Date_Created.ToString().Contains(key) || x.Date_Updated.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/OPRR/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateOPRRRequest request)
        {
            var details = request.Details;
            var items = request.updatedItems;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "OPRR is Invalid!" });


            try
            {
                var sqlQuery = "SELECT dbo.GenerateOPRRID() AS GenOPRRID";


                // Execute the query and get the result
                string OPRRNo;
                using (var command = dBContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlQuery;

                    await dBContext.Database.OpenConnectionAsync();

                    var scalarResult = await command.ExecuteScalarAsync();
                    OPRRNo = scalarResult + "".ToString();
                }

                if (string.IsNullOrEmpty(OPRRNo))
                {
                    return BadRequest(new { message = "Failed to Generate OPRR ID!" });
                }
                details.OPRRNo = OPRRNo;

                    var oprr = new OPRR
                {
                    OPRRNo = details.OPRRNo,
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
                await dBContext.OPRRS.AddAsync(oprr);
                await dBContext.SaveChangesAsync();


                var existingItems = await dBContext.OPRItems.ToListAsync();


                foreach (var updatedItem in items)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {

                        // Update the existing item's fields with the updated data
                        existingItem.oprrFlag = true;
                        existingItem.OPRRNo = details.OPRRNo;
                        // Update other fields as necessary
                    }
                }

                await dBContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Successfully Saved!",
                    details = oprr,
                    items = items
                });


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while saving the data.", error = ex.Message });
            }
        }

        // localhost:port/api/OPRR/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id)
        {
            // Find the OPRR by id asynchronously
            var oprr = await dBContext.ListOfOPRR
                                       .Where(x => x.OPRRNo == id)
                                       .FirstOrDefaultAsync();

            if (oprr == null)
                return NotFound(new { message = "OPRR not Found!" });

            // Find the OPRR Item by id asynchronously
            var oprrItems = await dBContext.OPRItems
                                           .Where(x => x.OPRRNo == id)
                                           .ToListAsync();

            if (oprrItems == null || !oprrItems.Any())
                return NotFound(new { message = "OPRR items not Found!" });

            return Ok(new
            {
                details = oprr,
                oprrItems = oprrItems,
            });
        }

        // localhost:port/api/OPRR/Update/
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateOPRRRequest request)
        {
            // Find the OPRR by id
            var oprr = await dBContext.OPRRS.FindAsync(id);

            if (oprr == null)
                return NotFound(new { message = "OPRR not found." });

            var details = request.Details;

            try
            {

                // Update the OPRR properties
                oprr.rtype = details.rtype;
                oprr.otype = details.otype;
                oprr.issuedBy = details.issuedBy;
                oprr.receivedBy = details.receivedBy;
                oprr.approvedBy = details.approvedBy;
                oprr.createdBy = details.createdBy;
                oprr.Date_Updated = DateTime.Now;

                // Save OPRR changes
                await dBContext.SaveChangesAsync();

                // Fetch existing OPR items by OPRR No
                var opritems = await dBContext.OPRItems.Where(x => x.OPRRNo == id).ToListAsync();

                // Nullify OPRRNo and update oprrFlag for old items
                foreach (var oprItem in opritems)
                {
                    // Update the OPRR properties
                    oprItem.oprrFlag = false;
                    oprItem.OPRRNo = null;
                }

                var existingItems = await dBContext.OPRItems.ToListAsync();

                foreach (var updatedItem in request.updatedItems)
                {
                    // Find if the updated item exists in the existing items
                    var existingItem = existingItems.FirstOrDefault(x => x.PropertyNo == updatedItem.PropertyNo);

                    if (existingItem != null)
                    {

                        // Update the existing item's fields with the updated data
                        existingItem.oprrFlag = true;
                        existingItem.OPRRNo = details.OPRRNo;
                        // Update other fields as necessary
                    }
                }

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


        // localhost:port/api/OPRR/Update/
        [HttpPut]
        [Route("Post")]
        public IActionResult Post(string id, [FromBody] bool postVal)
        {
            // Find the PAR by id
            var opr = dBContext.OPRRS.Find(id);

            if (opr == null)
                return NotFound(new { message = "OPRR not found." });

            try
            {

                // Update the property postFlag
                opr.postFlag = postVal;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(opr);
                return Ok(new
                {
                    message = "OPRR # 000" + id + " " + (postVal ? "Successfully Posted!" : "Successfully Unposted!")
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
            // Check if the OPRRS is already posted
            var OPRRExist = await dBContext.OPRRS.FirstOrDefaultAsync(x => x.OPRRNo == id && x.postFlag == true);

            if (OPRRExist != null)
                return BadRequest(new { message = "OPRR already posted!" });

            // Find the OPRRS by id
            var oprr = await dBContext.OPRRS.FirstOrDefaultAsync(x => x.OPRRNo == id);

            if (oprr == null)
                return NotFound(new { message = "OPRR not found." });

            // Remove the OPRRS
            dBContext.OPRRS.Remove(oprr);
            await dBContext.SaveChangesAsync();  // Ensure save is async

            // Fetch existing OPR items by OPR No
            var oprItems = await dBContext.OPRItems
                                               .Where(x => x.OPRRNo == id)
                                               .ToListAsync();

            // Nullify OPRR No and update oprrFlag
            foreach (var oprrItem in oprItems)
            {
                oprrItem.OPRRNo = null;
                oprrItem.oprrFlag = false;
            }

            // Save changes if any items were updated
            if (oprItems.Count > 0)
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

public class CreateOPRRRequest
{
    public OPRR Details { get; set; }
    public List<OPRItemDto> updatedItems { get; set; }
}
