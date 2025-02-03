using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {

        private readonly ApplicationDBContext dBContext;

        public SectionController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }


        // localhost:port/api/Sections
        [HttpGet]
        public IActionResult List()
        {

            var sections = dBContext.ListOfSection.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(sections);
        }

        // localhost:port/api/Section/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<SectionsVw> Search(string key)
        {
            return dBContext.ListOfSection.Where(x => x.SectionName.Contains(key) || x.DepartmentName.Contains(key) || x.Date_Created.ToString().Contains(key));
        }

        // localhost:port/api/Section/getSectionsByDepID/
        [HttpGet]
        [Route("getSectionsByDepID")]
        public IQueryable<SectionsVw> getSectionsByDepID(int? id = null)
        {
            return dBContext.ListOfSection.Where(x => x.DepID == id);
        }

        // localhost:port/api/Section/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] SectionDto details)
        {
            if (details == null)
                return BadRequest(new { message = "Please fill required fields!" });

            var sectionExist = await dBContext.Sections.FirstOrDefaultAsync(x => x.DepID == details.DepID && x.SectionName == details.SectionName);

            if (sectionExist != null)
                return BadRequest(new { message = "Section already exist!" });

            var section = new Section
            {
                SectionName = details.SectionName,
                DepID = details.DepID,
                Date_Created = DateTime.Now,
                Date_Updated = DateTime.Now

            };
            // Save changes to the database
            await dBContext.Sections.AddAsync(section);
            await dBContext.SaveChangesAsync();

            //return Ok(section);
            return Ok(new
            {
                message = "Successfully Saved!"
            });
        }

        // localhost:port/api/Section/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var section = dBContext.Sections.Find(id);
            if (section == null)
                return BadRequest(new { message = "Section not Found!" });

            return Ok(section);
        }


        // localhost:port/api/Section/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] SectionDto details)
        {
            // Find the Section by id
            var section = dBContext.Sections.Find(id);

            if (section == null)
                return BadRequest(new { message = "Section not Found!" });

            try
            {
                var sectExist = dBContext.Sections.FirstOrDefault(x => x.DepID != id && x.SectionName == details.SectionName);

                if (sectExist != null)
                    return BadRequest(new { message = "Section already exist!" });

                // Update the properties
                section.SectionName = details.SectionName;
                section.DepID = details.DepID;
                section.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(section);
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

        // localhost:port/api/Section/Update/
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {

            // Find the branch by id
            var section = dBContext.Sections.Find(id);

            if (section == null)
                return BadRequest(new { message = "Section not Found!" });

            section = dBContext.Sections.Where(x => x.DepID == id).First();

            dBContext.Sections.Remove(section);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }

    }
}
