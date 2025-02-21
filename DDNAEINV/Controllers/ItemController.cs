using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DDNAEINV.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : Controller
    {
        private readonly ApplicationDBContext dBContext;


        public ItemController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // localhost:port/api/Item
        [HttpGet]
        public IActionResult List()
        {

            var itemGroups = dBContext.ListOfItem.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(itemGroups);
        }


        // localhost:port/api/Item/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ItemVw> Search(string key)
        {
            return dBContext.ListOfItem.Where(x => x.Description.Contains(key) || x.ItemGroupName.Contains(key));
        }

        // GET: api/items/generateID
        [HttpGet("generateID/{type}")]
        public async Task<ActionResult<string>> GenerateItemID(string type)
        {
            // SQL query to execute the function
            var sqlQuery = "SELECT dbo.GenerateItemID(@type) AS GenItemID";

            // SQL parameter for the type
            var param = new SqlParameter("@type", type);

            // Execute the query and get the result
            string result;
            using (var command = dBContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.Parameters.Add(param);

                await dBContext.Database.OpenConnectionAsync();

                var scalarResult = await command.ExecuteScalarAsync();
                result = scalarResult+"".ToString();
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
                return NotFound("Item ID could not be generated.");
            }
        }





        // localhost:port/api/Item/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ItemDto details)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var itemExist = await dBContext.Items.FirstOrDefaultAsync(x => x.Description == details.Description);

            if (itemExist != null)
                return BadRequest(new { message = "Item already exist!" });


            try
            {
                Debug.WriteLine("ItemDto: " + details.ToString());
                var item = new Item
                {
                    IID = details.IID,
                    IGID = details.IGID,
                    Description = details.Description,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                Debug.WriteLine("Item: " + item.ToString());
                // Save changes to the database
                await dBContext.Items.AddAsync(item);
                await dBContext.SaveChangesAsync();

                //return Ok(item);
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

        // localhost:port/api/Item/{id}
        [HttpGet("{id}")]
        //[Route("{id:string}")]
        public IActionResult Retrieve(string id)
        {

            var itemGroup = dBContext.Items.Find(id);
            if (itemGroup == null)
                return BadRequest(new { message = "Item not exist!" });

            return Ok(itemGroup);
        }

        // localhost:port/api/Item/{description}
        [HttpGet("Description/")]
        //[Route("{id:int}")]
        public IActionResult RetrieveByDescription(string description)
        {
            var item = dBContext.Items.FirstOrDefault(x => x.Description == description);
            if (item == null)
                return BadRequest(new { message = "Item not exist!" });

            return Ok(item);
        }

        // localhost:port/api/Item/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(string id, [FromBody] ItemDto details)
        {
            // Find the Item Group by id

            var item = dBContext.Items.Find(id);

            if (item == null)
                return BadRequest(new { message = "Item not Found!" });

            try
            {
                Debug.WriteLine(id + "&&" + details.Description);
                var itemExist = dBContext.Items.FirstOrDefault(x => x.IID != id && x.Description == details.Description);

                if (itemExist != null)
                    return BadRequest(new { message = "Item already exist!" });

                //// Update the properties
                item.IGID = details.IGID;
                item.Description = details.Description;
                item.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(item);
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

        // localhost:port/api/ItemGroup/Delete
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(string id)
        {

            // Find the branch by id
            var item = dBContext.Items.Find(id);

            if (item == null)
                return BadRequest(new { message = "Item not Found!" });

            var isItemUsed = dBContext.PARItems.FirstOrDefault(x => x.IID == id);
            if (isItemUsed != null)
                return BadRequest(new { message = "Unable to remove Item that has been already added in Issuances!" });

            dBContext.Items.Remove(item);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }


    }
}
