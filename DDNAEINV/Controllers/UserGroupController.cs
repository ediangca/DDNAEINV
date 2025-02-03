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
    public class UserGroupController : ControllerBase
    {

        private readonly ApplicationDBContext dBContext;


        public UserGroupController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // localhost:port/api/UserGroup
        [HttpGet]
        public IActionResult List(string role)
        {
            var userGroups = dBContext.UserGroups.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();
            //Descending Order By Date_Created
            //var userGroups = dBContext.UserGroups.OrderByDescending(x => x.Date_Created).ToList();

            if (!role.Equals("System Administrator") && !role.Equals("*"))
                userGroups = dBContext.UserGroups.Where(x => !x.UserGroupName.Equals("System Administrator")
                && !x.UserGroupName.Equals("Test") && !x.UserGroupName.Equals("System Generated")).ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(userGroups);
        }

        // localhost:port/api/UserGroup/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<UserGroup> Search(string key)
        {
            return dBContext.UserGroups.Where(x => x.UserGroupName.Contains(key) || x.Notes.Contains(key) || x.Date_Created.ToString().Contains(key));
        }

        // localhost:port/api/UserGroup/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] UserGroupDto details)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "User Group is Invalid!" });

            var usergroup = await dBContext.UserGroups.FirstOrDefaultAsync(x => x.UserGroupName == details.UserGroupName);

            if (usergroup != null)
                return BadRequest(new { message = "User Group already exist!" });


            try
            {
                Debug.WriteLine("UserGroupDto: " + details.ToString());
                var userGroup = new UserGroup
                {
                    UserGroupName = details.UserGroupName,
                    Notes = details.Notes,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                Debug.WriteLine("UserGroup: " + userGroup.ToString());
                // Save changes to the database
                await dBContext.UserGroups.AddAsync(userGroup);
                await dBContext.SaveChangesAsync();

                //return Ok(userGroup);
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

        // localhost:port/api/UserGroup/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var userGroup = dBContext.UserGroups.Find(id);
            if (userGroup == null)
                return NotFound(new { message = "User Group not exist!" });

            return Ok(userGroup);
        }

        // localhost:port/api/UserGroup/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] UserGroupDto details)
        {
            // Find the User Group by id

            var userGroup = dBContext.UserGroups.Find(id);

            if (userGroup == null)
                return BadRequest(new { message = "User Group not Found!" });

            try
            {
                var userGroupExist = dBContext.UserGroups.FirstOrDefault(x => x.UGID != id && x.UserGroupName == details.UserGroupName);

                if (userGroupExist != null)
                    return BadRequest(new { message = "User Group already exist!" });

                // Update the properties
                userGroup.UserGroupName = details.UserGroupName;
                userGroup.Notes = details.Notes;
                userGroup.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(userGroup);
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

        // localhost:port/api/UserGroup/Delete
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {

            // Find the branch by id
            var userGroup = dBContext.UserGroups.Find(id);

            if (userGroup == null)
                return BadRequest(new { message = "User Group not Found!" });


            var isUserGroupUsed = dBContext.UserAccounts.FirstOrDefault(x => x.UGID == id);
            if (isUserGroupUsed != null)
                return BadRequest(new { message = "Unable to remove User Group that has been already assigned to User!" });

            dBContext.UserGroups.Remove(userGroup);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }

    }
}
