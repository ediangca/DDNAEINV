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
    public class PropertyCardController : ControllerBase
    {

        private readonly ApplicationDBContext dBContext;

        public PropertyCardController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }



        // localhost:port/api/PropertyCard
        [HttpGet]
        public IActionResult List()
        {

            var paritems = dBContext.PARItems.ToList();

            return Ok(paritems);
        }

        // localhost:port/api/PropertyCard/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<PropertyCardDetailsVw> Search(string key)
        {
            return dBContext.PropertyCardDetails.Where(x => x.REF.Contains(key) || 
            x.RefNoFrom.ToString().Contains(key) ||x.RefNoTo.ToString().Contains(key) || 
            x.PropertyNo.ToString().Contains(key) || x.Description.ToString().Contains(key) || 
            x.Issued.ToString().Contains(key) || x.Received.ToString().Contains(key) || 
            x.Approved.ToString().Contains(key) || x.Date_Created.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }


        // localhost:port/api/PropertyCard/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PropertyCard details)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Property Cards is Invalid!" });

            var cardExist = await dBContext.PropertyCards.FirstOrDefaultAsync(x => x.PCNo == details.PCNo);

            if (cardExist != null)
                return BadRequest(new { message = "Carrd already exist!" });


            try
            {

                var propertyCard = new PropertyCard
                {
                    Ref = details.Ref,
                    REFNoFrom = details.REFNoFrom,
                    REFNoTo = details.REFNoTo,
                    itemNo = details.itemNo,
                    propertyNo = details.propertyNo,
                    issuedBy = details.issuedBy,
                    receivedBy = details.receivedBy,
                    approvedBy = details.approvedBy,
                    createdBy = details.createdBy,
                    Date_Created = DateTime.Now

                };
                // Save changes to the database
                await dBContext.PropertyCards.AddAsync(propertyCard);
                await dBContext.SaveChangesAsync();

                //return Ok(par);
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


        // localhost:port/api/PropertyCard/{id}
        [HttpGet("{id}")]
        public IActionResult Retrieve(int id)
        {

            // Find the PAR by id
            var par = dBContext.PropertyCardDetails.Where(x => x.PCNo == id);
            if (par == null)
                return NotFound(new { message = "Property Card not Found!" });

            return Ok(par);
        }

        // localhost:port/api/PropertyCard/{id}
        [HttpGet("REF/{id}")]
        public IActionResult RetrieveByRef(string id)
        {

            // Find the PAR by id
            var par = dBContext.PropertyCardDetails.Where(x => x.REF == id);
            if (par == null)
                return NotFound(new { message = "Property Card not Found!" });

            return Ok(par);
        }


        // localhost:port/api/PropertyCard/{id}
        [HttpGet("Property/{id}")]
        public IActionResult RetrieveByProperty(string id)
        {

            // Find the PAR by id
            var par = dBContext.PropertyCardDetails.Where(x => x.PropertyNo == id);
            if (par == null)
                return NotFound(new { message = "Property Card not Found!" });

            return Ok(par);
        }


    }
}
