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
    public class ItemGroupController : ControllerBase
    {

        private readonly ApplicationDBContext dBContext;


        public ItemGroupController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // localhost:port/api/ItemGroup
        [HttpGet]
        public IActionResult List()
        {

            var itemGroups = dBContext.ItemGroups.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(itemGroups);
        }

        // localhost:port/api/ItemGroup/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<ItemGroup> Search(string key)
        {
            return dBContext.ItemGroups.Where(x => x.ItemGroupName.Contains(key) || x.Notes.Contains(key) || x.Date_Created.ToString().Contains(key));
        }

        // localhost:port/api/ItemGroup/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ItemGroupDto details)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var itemgroupExist = await dBContext.ItemGroups.FirstOrDefaultAsync(x => x.ItemGroupName == details.ItemGroupName);

            if (itemgroupExist != null)
                return BadRequest(new { message = "Item Group already exist!" });


            try
            {
                Debug.WriteLine("ItemGroupDto: " + details.ToString());
                var itemGroup = new ItemGroup
                {
                    ItemGroupName = details.ItemGroupName,
                    Notes = details.Notes,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                Debug.WriteLine("ItemGroup: " + itemGroup.ToString());
                // Save changes to the database
                await dBContext.ItemGroups.AddAsync(itemGroup);
                await dBContext.SaveChangesAsync();

                //return Ok(itemGroup);
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

        // localhost:port/api/ItemGroup/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var itemGroup = dBContext.ItemGroups.Find(id);
            if (itemGroup == null)
                return BadRequest(new { message = "Item Group not exist!" });

            return Ok(itemGroup);
        }

        // localhost:port/api/ItemGroup/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] ItemGroupDto details)
        {
            // Find the Item Group by id

            var itemGroup = dBContext.ItemGroups.Find(id);

            if (itemGroup == null)
                return BadRequest(new { message = "Item Group not Found!" });

            try
            {
                Debug.WriteLine(id + "&&" + details.ItemGroupName);
                var itemGroupExist = dBContext.ItemGroups.FirstOrDefault(x => x.IGID != id && x.ItemGroupName == details.ItemGroupName);

                if (itemGroupExist != null)
                    return BadRequest(new { message = "Item Group already exist!" });

                //// Update the properties
                itemGroup.ItemGroupName = details.ItemGroupName;
                itemGroup.Notes = details.Notes;
                itemGroup.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(itemGroup);
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
        public IActionResult Delete(int id)
        {

            // Find the branch by id
            var itemGroup = dBContext.ItemGroups.Find(id);

            if (itemGroup == null)
                return BadRequest(new { message = "Item Group not Found!" });


            var isItemGroupUsed = dBContext.Items.FirstOrDefault(x => x.IGID == id);
            if (isItemGroupUsed != null)
                return BadRequest(new { message = "Unable to remove Item Group that has been already assigned to Item!" });

            dBContext.ItemGroups.Remove(itemGroup);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }

    }
}
