using DDNAEINV.Data;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Linq;


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

        [HttpGet]
        [Route("Search")]
        public IQueryable<PropertyCardDetailsVw> Search(string key)
        {
            return dBContext.PropertyCardDetails.Where(x => x.REF.Contains(key) ||
            x.RefNoFrom.ToString().Contains(key) || x.RefNoTo.ToString().Contains(key) ||
            x.PropertyNo.ToString().Contains(key) || x.Description.ToString().Contains(key) ||
            x.Issued.ToString().Contains(key) || x.Received.ToString().Contains(key) ||
            x.Approved.ToString().Contains(key) || x.Date_Created.ToString().Contains(key))
                .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/PropertyCard/PropertyList/
        [HttpGet]
        [Route("PropertyCardList")]
        public IQueryable<PropertyDetailsVw> PropertyList(string category, string key)
        {
            IQueryable<PropertyDetailsVw> properties;

            Debug.Print("Category >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> " + category);

            switch (category)
            {
                case "Above50k":

                    properties = dBContext.ListOfProperty.Where(x =>
                    x.REF == "PAR" &&
                    (x.Description.ToString().ToLower().Contains(key) || x.PropertyNo.ToString().ToLower().Contains(key) || x.QRCode.ToString().ToLower().Contains(key)));

                    break;
                case "Below50k":

                    properties = dBContext.ListOfProperty.Where(x =>
                    x.REF == "ICS" &&
                    (x.Description.ToString().ToLower().Contains(key) || x.PropertyNo.ToString().ToLower().Contains(key) || x.QRCode.ToString().ToLower().Contains(key)));

                    break;

                case "Others":

                    properties = dBContext.ListOfProperty.Where(x =>
                    x.REF == "OPR" &&
                    (x.Description.ToString().ToLower().Contains(key) || x.PropertyNo.ToString().ToLower().Contains(key) || x.QRCode.ToString().ToLower().Contains(key)));

                    break;
                default:
                    properties = dBContext.ListOfProperty.Where(x =>
                    x.Description.ToString().ToLower().Contains(key) || x.PropertyNo.ToString().ToLower().Contains(key) || x.QRCode.ToString().ToLower().Contains(key));
                    break;
            }

            return properties;

        }


        // localhost:port/api/PropertyCard/PropertyOwnerList/
        [HttpGet]
        [Route("PropertyCardOwnerList")]
        //public IQueryable<PropertyCardDetailsVw> PropertyCardListOwner(string key)
        //{
        //    //return dBContext.PropertyCardDetails.Where(x => x.ReceivedBy.ToString().ToLower().Contains(key) || x.Received.ToString().ToLower().Contains(key));
        //    return dBContext.PropertyCardDetails.Where(x => (x.ReceivedBy ?? "").ToLower().Contains(key) || (x.Received ?? "").ToLower().Contains(key)).GroupBy(x => x.Received);
        //}
        public List<object> PropertyCardListOwner(string key)
        {
            var list = dBContext.PropertyCardDetails
                .Where(x => (x.ReceivedBy ?? "").ToLower().Contains(key) || (x.Received ?? "").ToLower().Contains(key))
                .AsEnumerable() // Convert to LINQ-to-Objects before GroupBy
                .GroupBy(x => x.Received)
                .Select(group => new
                {
                    Received = group.Key,
                    Items = group.ToList() // List of PropertyCardDetailsVw
                })
                .ToList();

            return list.Cast<object>().ToList(); // Ensure return type matches
        }


        // localhost:port/api/PropertyCard/PropertyOwnerList/
        [HttpGet]
        [Route("PropertyOwnerList")]
        public IQueryable<ListofPropertiesByOwnerVw> PropertyListOwner(string key)
        {
            return dBContext.ListOfPropertyOwner.Where(x => x.AccountID.ToString().ToLower().Contains(key) || x.AccountName.ToString().ToLower().Contains(key));

        }


        // localhost:port/api/PropertyCard/SearchByCategoryAndID/
        [HttpGet]
        [Route("SearchByCategoryAndID")]
        public IQueryable<PropertyCardDetailsVw> SearchByModuleAndID(string category, string key)
        {
            switch (category.ToLower())
            {
                case "Above50k":

                    return dBContext.PropertyCardDetails.Where(x =>
                    (x.REF.Equals("PAR") || x.REF.Equals("PTR") || x.REF.Equals("PRS")) &&
                    (x.PropertyNo.ToString().ToLower().Equals(key) || x.QRCode.ToString().ToLower().Equals(key))).OrderBy(x => x.Date_Created);

                case "Below50k":

                    return dBContext.PropertyCardDetails.Where(x =>
                    (x.REF.Equals("ICS") || x.REF.Equals("ITR") || x.REF.Equals("RRSEP")) &&
                    (x.PropertyNo.ToString().ToLower().Equals(key) || x.QRCode.ToString().ToLower().Equals(key))).OrderBy(x => x.Date_Created);


                case "Others":

                    return dBContext.PropertyCardDetails.Where(x =>
                    (x.REF.Equals("ICS") || x.REF.Equals("ITR") || x.REF.Equals("RRSEP")) &&
                    (x.PropertyNo.ToString().ToLower().Equals(key) || x.QRCode.ToString().ToLower().Equals(key))).OrderBy(x => x.Date_Created);


                default:


                    return dBContext.PropertyCardDetails.Where(x =>
                    x.PropertyNo.ToString().ToLower().Equals(key) || x.QRCode.ToString().ToLower().Equals(key));
            }

        }
        // localhost:port/api/PropertyCard/SearchByLogAccount/
        [HttpGet]
        [Route("SearchByLogAccount")]
        public IQueryable<PropertyCardDetailsVw> SearchByLogAccount(string accountID)
        {
            //x.IssuedBy == accountID ||
            return dBContext.PropertyCardDetails.Where(x => x.ReceivedBy == accountID || x.IssuedBy == accountID);

        }

        // localhost:port/api/PropertyCard/SearchByAccount/
        [HttpGet]
        [Route("SearchByAccount")]
        public IQueryable<ListOfPropertiesVw> SearchByAccount(string accountID)
        {
            return dBContext.ListOfProperties.Where(x => x.Received == accountID);

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
                    REF = details.REF,
                    REFNoFrom = details.REFNoFrom,
                    REFNoTo = details.REFNoTo,
                    PropertyNo = details.PropertyNo,
                    IssuedBy = details.IssuedBy,
                    ReceivedBy = details.ReceivedBy,
                    ApprovedBy = details.ApprovedBy,
                    CreatedBy = details.CreatedBy,
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
