using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Web.WebPages;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {


        private readonly ApplicationDBContext dBContext;


        public PositionController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // localhost:port/api/Position
        [HttpGet]
        public IActionResult List()
        {

            //var positions = dBContext.Positions.OrderByDescending(x => x.Date_Created).ToList(); //Descending Order By Date_Created
            var positions = dBContext.Positions.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(positions);
        }

        // localhost:port/api/Position/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<Position> Search(string key)
        {
            return dBContext.Positions.Where(x => x.PositionName.Contains(key));
        }

        // localhost:port/api/Position/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PositionDto details)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Position is Invalid!" });

            var positionExist = await dBContext.Positions.FirstOrDefaultAsync(x => x.PositionName == details.PositionName);

            if (positionExist != null)
                return BadRequest(new { message = "Position already exist!" });


            try
            {
                var position = new Position
                {
                    PositionName = details.PositionName,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                // Save changes to the database
                await dBContext.Positions.AddAsync(position);
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

        // localhost:port/api/Position/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var position = dBContext.UserGroups.Find(id);
            if (position == null)
                return BadRequest(new { message = "Position not Found!" });

            return Ok(position);
        }

        // localhost:port/api/Position/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] PositionDto details)
        {
            // Find the Position by id

            var position = dBContext.Positions.Find(id);

            if (position == null)
                return BadRequest(new { message = "Position not Found!" });

            try
            {
                var positionExist = dBContext.Positions.FirstOrDefault(x => x.PositionID != id && x.PositionName == details.PositionName);

                if (positionExist != null)
                    return BadRequest(new { message = "Position already exist!" });

                // Update the properties
                position.PositionName = details.PositionName;
                position.Date_Updated = DateTime.Now;

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

        // localhost:port/api/Position/Delete
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {

            // Find the Position by id
            var position = dBContext.Positions.Find(id);

            if (position == null)
                return BadRequest(new { message = "Position not Found!" });


            var positionUsed = dBContext.UserProfiles.FirstOrDefault(x => x.PositionID == id);
            if (positionUsed != null)
                return BadRequest(new { message = "Unable to remove Position that has been already assigned to User!" });

            dBContext.Positions.Remove(position);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }
    }
}
