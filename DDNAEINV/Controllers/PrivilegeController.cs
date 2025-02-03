using DDNAEINV.Data;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivilegeController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public PrivilegeController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        // localhost:port/api/Privilege
        [HttpGet]
        public IActionResult List()
        {
            var privilege = dBContext.ListOfPrivilege.ToList();

            return Ok(privilege);
        }


        [HttpGet]
        [Route("Modules")]
        public IActionResult ModuleList()
        {
            var module = dBContext.Modules.ToList();

            return Ok(module);
        }

        // localhost:port/api/Privilege/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<PrivilegeVw> Search(string key)
        {
            return dBContext.ListOfPrivilege.Where(x => x.ModuleName.Contains(key) || x.UserGroupName.Contains(key));
        }


        // localhost:port/api/Privilege/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] List<Privilege> privileges)
        {
            if (privileges == null || !privileges.Any())
            {
                return BadRequest(new { success = false, message = "No privileges to process." });
            }

            foreach (var privilege in privileges)
            {
                var existingPrivilege = dBContext.Privileges
                    .FirstOrDefault(p => p.UGID == privilege.UGID && p.MID == privilege.MID);

                if (existingPrivilege != null)
                {
                    // Update existing privilege
                    existingPrivilege.isActive = privilege.isActive;
                    existingPrivilege.C = privilege.C;
                    existingPrivilege.R = privilege.R;
                    existingPrivilege.U = privilege.U;
                    existingPrivilege.D = privilege.D;
                    existingPrivilege.POST = privilege.POST;
                    existingPrivilege.UNPOST = privilege.UNPOST;
                }
                else
                {
                    // Add new privilege
                    dBContext.Privileges.Add(privilege);
                }
            }

            dBContext.SaveChanges();
            return Ok(new { success = true });
        }

        // localhost:port/api/Privilege/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var privilege = dBContext.Privileges.Find(id);
            if (privilege == null)
                return NotFound(new { message = "Privilege not exist!" });

            return Ok(privilege);
        }


        // localhost:port/api/Privilege/UGID/{ugid}
        [HttpGet("UGID/{ugid}")]
        //[Route("{id:int}")]
        public IActionResult RetrieveByUGID(int ugid)
        {

            var privilege = dBContext.ListOfPrivilege.Where(x => x.UGID == ugid).ToList();

            if (privilege == null)
                return NotFound(new { message = "Privilege not exist!" });

            return Ok(privilege);
        }


        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int ugid, [FromBody] List<Privilege> privileges)
        {
            // Validate input
            if (privileges == null || !privileges.Any())
            {
                return BadRequest(new { success = false, message = "Invalid or empty privilege list." });
            }

            try
            {
                foreach (var privilege in privileges)
                {
                    // Validate UGID consistency
                    if (privilege.UGID != ugid)
                    {
                        return BadRequest(new { success = false, message = "UGID mismatch between request and privileges." });
                    }

                    // Check if the privilege exists
                    var existingPrivilege = dBContext.Privileges
                        .FirstOrDefault(p => p.UGID == privilege.UGID && p.MID == privilege.MID);

                    if (existingPrivilege != null)
                    {
                        // Update existing privilege
                        existingPrivilege.isActive = privilege.isActive;
                        existingPrivilege.C = privilege.C;
                        existingPrivilege.R = privilege.R;
                        existingPrivilege.U = privilege.U;
                        existingPrivilege.D = privilege.D;
                        existingPrivilege.POST = privilege.POST;
                        existingPrivilege.UNPOST = privilege.UNPOST;

                        dBContext.Privileges.Update(existingPrivilege);
                    }
                    else
                    {
                        // Add new privilege if not found
                        dBContext.Privileges.Add(privilege);
                    }
                }

                // Save changes to the database
                dBContext.SaveChanges();
                return Ok(new { success = true, message = "Privileges updated successfully." });
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, new { success = false, message = "An error occurred while saving the data.", error = ex.Message });
            }
        }


        // localhost:port/api/Privilege/Delete
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {

            // Find the Privilege by id
            var privilege = dBContext.Privileges.Find(id);

            if (privilege == null)
                return BadRequest(new { message = "Privilege not Found!" });


            var isprivilegeUsed = dBContext.UserAccounts.FirstOrDefault(x => x.UGID == id);
            if (isprivilegeUsed != null)
                return BadRequest(new { message = "Unable to remove Privilege that has been already assigned to User Group!" });

            dBContext.Privileges.Remove(privilege);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }


    }
}
