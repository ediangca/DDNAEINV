using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public DepartmentController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }


        // localhost:port/api/Department
        [HttpGet]
        public IActionResult List()
        {

            var departments = dBContext.ListOfDeparment.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .Take(10)
                   .ToList();

            return Ok(departments);
        }

        // localhost:port/api/Department/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<DepartmentsVw> Search(string key)
        {
            return dBContext.ListOfDeparment.Where(x => x.DepartmentName.Contains(key) || x.BranchName.Contains(key) || x.Date_Created.ToString().Contains(key));
        }


        // localhost:port/api/Department/getDepartmentsByCompanyID/
        [HttpGet]
        [Route("getDepartmentsByCompanyID")]
        public IQueryable<DepartmentsVw> getDepartmentsByCompanyID(int? id = null)
        {
            return dBContext.ListOfDeparment.Where(x => x.BranchID == id);
        }



        // localhost:port/api/Department/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] DepartmentDto details)
        {
            if (details == null)
                return BadRequest(new { message = "Please fill required fields!" });

            var departmentExist = await dBContext.Departments.FirstOrDefaultAsync(x => x.BranchID == details.BranchID && x.DepartmentName == details.DepartmentName);

            if (departmentExist != null)
                return BadRequest(new { message = "Department already exist!" });


            var department = new Department
            {
                DepartmentName = details.DepartmentName,
                BranchID = details.BranchID,
                Date_Created = DateTime.Now,
                Date_Updated = DateTime.Now

            };
            // Save changes to the database
            await dBContext.Departments.AddAsync(department);
            await dBContext.SaveChangesAsync();

            //return Ok(department);
            return Ok(new
            {
                message = "Successfully Saved!"
            });
        }

        // localhost:port/api/Department/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var department = dBContext.Departments.Find(id);
            if (department == null)
                return BadRequest(new { message = "Department not Found!" });

            return Ok(department);
        }


        // localhost:port/api/Department/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, DepartmentDto details)
        {
            // Find the Department by id
            var department = dBContext.Departments.Find(id);

            if (department == null)
                return BadRequest(new { message = "Department not Found!" });

            try
            {
                var deptExist = dBContext.Departments.FirstOrDefault(x => x.DepID != id && x.DepartmentName == details.DepartmentName);

                if (deptExist != null)
                    return BadRequest(new { message = "Department already exist!" });

                // Update the properties
                department.DepartmentName = details.DepartmentName;
                department.BranchID = details.BranchID;
                department.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(department);
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


        // localhost:port/api/Department/Delete/
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {

            // Find the branch by id
            var department = dBContext.Departments.Find(id);

            if (department == null)
                return BadRequest(new { message = "Department not Found!" });

            department = dBContext.Departments.Where(x => x.DepID == id).First();

            var isDepartmentUsed = dBContext.UserProfiles.FirstOrDefault(x => x.BranchID == id);
            if (isDepartmentUsed != null)
                return BadRequest(new { message = "Unable to remove Department that has been already assigned to User!" });

            dBContext.Departments.Remove(department);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }


    }
}
