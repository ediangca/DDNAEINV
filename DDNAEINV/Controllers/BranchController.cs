using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public BranchController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // localhost:port/api/Branch
        [HttpGet]
        public IActionResult List()
        {

            var branches = dBContext.ListOfBranch.ToList()
                   .OrderByDescending(x => x.Date_Created) 
                   .ToList();

            return Ok(branches);
        }

        // localhost:port/api/Branch/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<Branch> Search(string key)
        {
            return dBContext.Branches.Where(x => x.BranchName.Contains(key) || x.Type.Contains(key) || x.Date_Created.ToString().Contains(key)
            || x.Date_Updated.ToString().Contains(key));
        }


        // localhost:port/api/Branch/SearchByType/
        [HttpGet]
        [Route("SearchByType")]
        public IQueryable<string> SearchByType(string key)
        {
            var distinctTypes = dBContext.Branches
                .Where(b => b.Type.Contains(key))
                .GroupBy(b => b.Type)
                .Select(g => g.Key);

            return distinctTypes;
        }


        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] BranchDto details)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var branchExist = await dBContext.Branches.FirstOrDefaultAsync(x => x.BranchName == details.BranchName);

            if (branchExist != null)
                return BadRequest(new { message = "Company/Branch already exist!" });

            try
            {
                var branch = new Branch
                {
                    BranchName = details.BranchName,
                    Type = details.Type,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                Debug.WriteLine("Branch: " + branch.ToString());
                // Save changes to the database
                await dBContext.Branches.AddAsync(branch);
                await dBContext.SaveChangesAsync();

                //return Ok(branch);
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

        // localhost:port/api/Branch/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var branches = dBContext.Branches.Find(id);
            if (branches == null)
                return BadRequest(new { message = "Company/Branch not exist!" });

            return Ok(branches);
        }

        // localhost:port/api/Branch/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] BranchDto details)
        {
            // Find the branch by id
            var branch = dBContext.Branches.Find(id);

            if (branch == null)
                return BadRequest(new { message = "Company/Branch not Found!" });

            try
            {
                var branchExist = dBContext.Branches.FirstOrDefault(x => x.BranchID != id && x.BranchName == details.BranchName);

                if (branchExist != null)
                    return BadRequest(new { message = "Company/Branch already exist!" });

                // Update the properties
                branch.BranchName = details.BranchName;
                branch.Type = details.Type;
                branch.Date_Updated = DateTime.Now;

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

        // localhost:port/api/Branch/Delete/
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {

            // Find the branch by id
            var branch = dBContext.Branches.Find(id);

            if (branch == null)
                return BadRequest(new { message = "Company/Branch not Found!" });

            branch = dBContext.Branches.Where(x => x.BranchID == id).First();

            var isBranchUsed = dBContext.UserProfiles.FirstOrDefault(x => x.BranchID == id);
            if (isBranchUsed != null)
                return BadRequest(new { message = "Unable to remove Company/Branch that has been already assigned to User!" });

            dBContext.Branches.Remove(branch);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }

    }
}
