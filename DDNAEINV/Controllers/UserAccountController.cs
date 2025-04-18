using DDNAEINV.Data;
using DDNAEINV.Helper;
using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public UserAccountController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }
        //[Authorize]
        // localhost:port/api/UserAccount
        [HttpGet]
        public IActionResult List()
        {
            var userAccounts = dBContext.ListOfUserAccount
                            .OrderByDescending(ua => ua.Date_Created)
                            .ToList();

            return Ok(userAccounts);
        }

        // localhost:port/api/UserAccount/Search/
        [HttpGet]
        [Route("Search")]
        public IQueryable<UserAccountsVw> Search(string key)
        {
            return dBContext.ListOfUserAccount.Where(x => x.UserID.Contains(key) || x.UserName.Contains(key) ||
            x.Fullname.Contains(key) || x.UserGroupName.Contains(key) || x.Date_Created.ToString().Contains(key))
                            .OrderByDescending(x => x.Date_Created);
        }

        // localhost:port/api/GetAccountID/Search/
        [HttpGet]
        [Route("GetAccountbyUsername")]
        public IQueryable<UserAccountsVw> GetAccountIDbyUsername(string username)
        {
            return dBContext.ListOfUserAccount.Where(x => x.UserName == username);
        }

        [HttpGet("generateUserId")]
        public string GetAccID()
        {
            var id = dBContext.GeneratedAccID.ToArray();

            if (id.Length < 1)
            {
                Debug.WriteLine("Error in Generating Account ID.");
            }


            return id[0].NewUserID;
        }

        // localhost:port/api/UserAccount/Create/
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] UserAccountDto details)
        {

            var hasher = new PasswordHasher();
            Debug.WriteLine("HashPassword: " + hasher.HashPassword(details.Password));

            if (details == null)
                return BadRequest(new { message = "Please fill required fields!" });

            var account = await dBContext.UserAccounts.FirstOrDefaultAsync(x => x.UserName == details.UserName);

            if (account != null)
                return BadRequest(new { message = "Username already exist!" });


            var userAccount = new UserAccount
            {
                UserID = GetAccID(),
                UserName = details.UserName,
                Password = hasher.HashPassword(details.Password),
                UGID = details.UGID,
                Date_Created = DateTime.Now,
                Date_Updated = DateTime.Now

            };
            // Save changes to the database
            await dBContext.UserAccounts.AddAsync(userAccount);
            await dBContext.SaveChangesAsync();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Successfully Saved!"
            });
        }

        // localhost:port/api/UserAccount/{id}
        [HttpGet("{id}")]
        public IActionResult Retrieve(string id)
        {

            var userAccount = dBContext.UserAccounts.Find(id);
            if (userAccount == null)
                return BadRequest(new { message = "UserAccount not Found!" });

            return Ok(userAccount);
        }

        // localhost:port/api/UserAccount/Update/
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(string id, UserAccountDto details)
        {
            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);


            if (userAccount == null)
                return BadRequest(new { message = "Please fill required fields!" });

            var account = dBContext.UserAccounts.FirstOrDefault(x => x.UserID != id && x.UserName == details.UserName);

            if (account != null)
                return BadRequest(new { message = "Username already exist!" });

            // Update the properties
            userAccount.UserName = details.UserName;
            //userAccount.Password = details.Password;
            userAccount.UGID = details.UGID;
            userAccount.Date_Updated = DateTime.Now;

            // Save changes to the database
            dBContext.SaveChanges();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Successfully Saved!"
            });
        }

        // localhost:port/api/UserAccount/Verification/
        [HttpPut("Update/Verification")]
        public IActionResult UpdateVerification(string id)
        {
            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);


            if (userAccount == null)
                return BadRequest(new { message = "Please fill required fields!" });

            // Update the properties
            userAccount.isVerified = !(userAccount.isVerified);

            // Save changes to the database
            dBContext.SaveChanges();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Verification status updated!"
            });
        }

        // localhost:port/api/UserAccount/Update/UserGroup/
        [HttpPut("Update/UserGroup")]
        public IActionResult UpdateUserGroup(string id, int UGID)
        {
            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);

            if (userAccount == null)
                return BadRequest(new { message = "UserAccount not Found!" });

            // Update the properties
            userAccount.UGID = UGID;
            userAccount.Date_Updated = DateTime.Now;

            // Save changes to the database
            dBContext.SaveChanges();

            return Ok(userAccount);
        }
        // localhost:port/api/UserAccount/Update/Password/
        [HttpPut]
        [Route("Update/ForgetPassword")]
        public IActionResult ForgetPassword(string id, ChangePassDto details)
        {

            if (details.OldPassword == null)
                return BadRequest(new { message = "Please fill Old Password!" });

            var hasher = new PasswordHasher();
            Debug.WriteLine("HashPassword: " + hasher.HashPassword(details.NewPassword));

            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);


            if (userAccount == null)
                return BadRequest(new { message = "Please fill required fields!" });


            if (!hasher.VerifyPassword(userAccount.Password, details.OldPassword))
                return BadRequest(new { message = "Invalid Old Password!" });

            // Update the properties
            userAccount.Password = hasher.HashPassword(details.NewPassword);

            //userAccount.Password = details.Password;

            userAccount.Date_Updated = DateTime.Now;

            // Save changes to the database
            dBContext.SaveChanges();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Password Successfully Changed!"
            });
        }

        // localhost:port/api/UserAccount/Update/Password/
        [HttpPut]
        [Route("Update/Password")]
        public IActionResult UpdatePassword(string id, ChangePassDto details)
        {

            var hasher = new PasswordHasher();
            Debug.WriteLine("HashPassword: " + hasher.HashPassword(details.NewPassword));

            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);


            if (userAccount == null)
                return BadRequest(new { message = "Please fill required fields!" });


            //if (!hasher.VerifyPassword(userAccount.Password, details.OldPassword))
            //    return BadRequest(new { message = "Invalid Old Password!" });

            // Update the properties
            userAccount.Password = hasher.HashPassword(details.NewPassword);

            //userAccount.Password = details.Password;

            userAccount.Date_Updated = DateTime.Now;

            // Save changes to the database
            dBContext.SaveChanges();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Password Successfully Changed!"
            });
        }

        [HttpPut("Update/Leave")]
        public IActionResult UpdateLeave(string id)
        {
            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);


            if (userAccount == null)
                return BadRequest(new { message = "No User Account Found!" });

            // Update the properties
            userAccount.isLeave = !(userAccount.isLeave);

            // Save changes to the database
            dBContext.SaveChanges();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Leave status updated!"
            });
        }

        // localhost:port/api/UserGroup/Delete
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(string id)
        {

            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);

            if (userAccount == null)
                return NotFound("User Account not found.");


            var isUserUsed = dBContext.PARS.FirstOrDefault(x => x.receivedBy == id || x.issuedBy == id || x.createdBy == id);
            if (isUserUsed != null)
                return BadRequest(new { message = "Unable to remove User that has been already assigned!" });


            dBContext.UserAccounts.Remove(userAccount);
            dBContext.SaveChanges();

            var userProfile = dBContext.UserProfiles.FirstOrDefault(x => x.UserID == id);

            if (userProfile != null)
                dBContext.UserProfiles.Remove(userProfile);
                dBContext.SaveChanges();



            return Ok(new
            {
                message = "Successfully Removed"
            });
        }


        [HttpGet("leave/{UserID}")]
        public IActionResult RetrieveLeave(string UserID)
        {

            var leave = dBContext.ListOfLeave.FirstOrDefault(x => x.UserID == UserID);
            if (leave == null)
                return BadRequest(new { message = "No Leave Found!" });

            return Ok(leave);
        }


        [HttpPut("status/Update")]
        public IActionResult UpdateStatus(string id)
        {
            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);


            if (userAccount == null)
                return BadRequest(new { message = "User Account not Found!" });

            // Update the properties
            userAccount.isLeave = !(userAccount.isLeave);

            // Save changes to the database
            dBContext.SaveChanges();

            //return Ok(userAccount);
            return Ok(new
            {
                message = "Status successfully Active!"
            });
        }

        [HttpPut]
        [Route("leave/update")]
        public IActionResult leave(string id, Leave leave)
        {


            // Find the User Account by id
            var userAccount = dBContext.UserAccounts.Find(id);
            if (userAccount == null)
                return BadRequest(new { message = "User Account not Found!" });


            var hasLeaveAccount = dBContext.Leaves.FirstOrDefault(x => x.UserID == id);

            if (hasLeaveAccount != null)
            {
                // Save changes to the database
                //UserAccount
                userAccount.isLeave = true;
                //Leave
                hasLeaveAccount.Remarks = leave.Remarks;
                hasLeaveAccount.CareOfUserID = leave.CareOfUserID;
                hasLeaveAccount.Date_Created = DateTime.Now;
            }
            else
            {
                userAccount.isLeave = true;
                // Create Leave
                dBContext.Leaves.Add(leave);
            }


            dBContext.SaveChanges();

            return Ok(new
            {
                message = "Status successfully Inactive!"
            });
        }

    }
}
