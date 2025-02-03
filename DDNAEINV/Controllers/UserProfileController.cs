using DDNAEINV.Data;
using DDNAEINV.Helper;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public UserProfileController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        // localhost:port/api/UserProfile
        [HttpGet]
        public IActionResult List()
        {

            var userProfile = dBContext.ListOfProfile.ToList()
                   .OrderByDescending(x => x.Date_Created)
                   .ToList();

            return Ok(userProfile);
        }

        // localhost:port/api/UserProfile/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<UserProfileVw> Search(string key)
        {
            key = key.ToLower();
            return dBContext.ListOfProfile.Where(x => x.FullName.ToLower().Contains(key) ||
                    x.Branch.ToLower().Contains(key) ||
                    x.Department.ToLower().Contains(key) ||
                    x.Section.ToLower().Contains(key) ||
                    x.Position.ToLower().Contains(key) ||
                    x.UserID.ToLower().Contains(key) ||
                    x.Date_Created.ToString().Contains(key));
        }

        // localhost:port/api/UserProfile/SearchByUserID/
        [HttpGet]
        [Route("SearchByUserID")]
        public IQueryable<UserProfileVw> SearchByUserID(string UserID)
        {
            return dBContext.ListOfProfile.Where(x => x.UserID == UserID);
        }

        // localhost:port/api/UserProfile/Create/
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] UserProfileDto details)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { message = "User Profile is Invalid!" });


            var userProfileExist =  dBContext.UserProfiles.FirstOrDefault(x => (x.Lastname + x.Firstname) == (details.Lastname + details.Firstname));

            if (userProfileExist != null)
                return Conflict(new { message = "Profile already exist!" });


            try
            {

                var userProfile = new UserProfile
                {
                    Lastname = details.Lastname,
                    Firstname = details.Firstname,
                    Middlename = details.Middlename,
                    Sex = details.Sex,
                    BranchID = details.BranchID,
                    DepID = details.DepID,
                    SecID = details.SecID,
                    PositionID = details.PositionID,
                    UserID = details.UserID,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                // Save changes to the database
                dBContext.UserProfiles.Add(userProfile);
                dBContext.SaveChanges();

                //return Ok(userProfile);
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

        // localhost:port/api/UserProfile/Create/GenrateAccount
        [HttpPost]
        [Route("Create/GenratedAccount")]
        public async Task<IActionResult> CreateGenratedAccount([FromBody] UserProfileDto details)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { message = "User Profile is Invalid!" });

            var id = dBContext.GeneratedAccID.ToArray();

            if (id.Length < 1)
            {
                Debug.WriteLine("Error in Generating Account ID.");
                return BadRequest(new { message = "Failed to generate account ID." });
                //return BadRequest(500, new { message = "Failed to generate account ID." });
            }

            string AccID = id[0].NewUserID;
            string username = (details.Lastname.Substring(0, 1) + details.Firstname.Substring(0)).ToLower();

            var hasher = new PasswordHasher();
            Debug.WriteLine("HashPassword: " + hasher.HashPassword(username));


            var userAccount = new UserAccount
            {
                UserID = AccID,
                UserName = username,
                Password = hasher.HashPassword(username),
                UGID = 3,
                Date_Created = DateTime.Now,
                Date_Updated = DateTime.Now

            };
            // Save changes to the userAccounts
            await dBContext.UserAccounts.AddAsync(userAccount);
            await dBContext.SaveChangesAsync();


            var userProfileExist = dBContext.UserProfiles.FirstOrDefault(x => (x.Lastname + x.Firstname) == (details.Lastname + details.Firstname));

            if (userProfileExist != null)
                return BadRequest(new { message = "Profile already exist!" });


            try
            {

                var userProfile = new UserProfile
                {
                    Lastname = details.Lastname,
                    Firstname = details.Firstname,
                    Middlename = details.Middlename,
                    Sex = details.Sex,
                    BranchID = details.BranchID,
                    DepID = details.DepID,
                    SecID = details.SecID,
                    PositionID = details.PositionID,
                    UserID = AccID,
                    Date_Created = DateTime.Now,
                    Date_Updated = DateTime.Now

                };
                // Save changes to the database
                dBContext.UserProfiles.Add(userProfile);
                dBContext.SaveChanges();

                //return Ok(userProfile);
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

        // localhost:port/api/UserProfile/{id}
        [HttpGet("{id}")]
        //[Route("{id:int}")]
        public IActionResult Retrieve(int id)
        {

            var userProfile = dBContext.UserProfiles.Find(id);
            if (userProfile == null)
                return NotFound(new { message = "userProfile not Found!" });

            return Ok(userProfile);
        }

        // localhost:port/api/UserProfile/{id}
        [HttpGet("UserID/{id}")]
        //[Route("{id:int}")]
        public IActionResult RetrieveByUserID(string id)
        {

            var userProfile = dBContext.UserProfiles.Where(x => x.UserID == id);
            if (userProfile == null)
                return NotFound(new { message = "User Profile not Found!" });

            return Ok(userProfile);
        }

        // localhost:port/api/UserProfile/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(int id, [FromBody] UserProfileDto details)
        {
            // Find the User Account by id
            var userProfile = dBContext.UserProfiles.Find(id);

            if (userProfile == null)
                return NotFound(new { message = "User Profile not found." });

            try
            {
                var userProfileExist = dBContext.UserProfiles.FirstOrDefault(x => x.ProfileID != id && (x.Lastname + x.Firstname) == (details.Lastname + details.Firstname));

                if (userProfileExist != null)
                    return BadRequest(new { message = "User Profile already exist!" });

                // Update the properties
                userProfile.Lastname = details.Lastname;
                userProfile.Firstname = details.Firstname;
                userProfile.Middlename = details.Middlename;
                userProfile.Sex = details.Sex;
                userProfile.BranchID = details.BranchID;
                userProfile.DepID = details.DepID;
                userProfile.SecID = details.SecID;
                userProfile.PositionID = details.PositionID;
                userProfile.UserID = details.UserID;
                userProfile.Date_Updated = DateTime.Now;

                // Save changes to the database
                dBContext.SaveChanges();

                //return Ok(userProfile);
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

            // Find the User Account by id
            var userProfile = dBContext.UserProfiles.Find(id);

            if (userProfile == null)
                return NotFound(new { message = "User Profile not found." });

            userProfile = dBContext.UserProfiles.Where(x => x.ProfileID == id).First();

            dBContext.UserProfiles.Remove(userProfile);
            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Successfully Removed"
            });
        }



    }
}
