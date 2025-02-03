using DDNAEINV.Data;
using DDNAEINV.Model.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        private readonly ApplicationDBContext dBContext;

        public ReportController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;

        }

        [HttpGet("GetItemsByQRCode")]
        public async Task<IActionResult> GetItemsByQRCode(string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
                return BadRequest("QR Code is required.");

            try
            {
                var result = await FetchItemsByQRCodeAsync(qrCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        private async Task<List<Dictionary<string, object>>> FetchItemsByQRCodeAsync(string qrCode)
        {
            var itemsList = new List<Dictionary<string, object>>();

            // Define stored procedure and parameter
            var qrCodeParam = new SqlParameter("@QRCode", qrCode);

            // Execute raw SQL with FromSqlInterpolated
            await using var command = dBContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = "[dbo].[GetItemsByQRCode]";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(qrCodeParam);

            // Open connection if closed
            if (command.Connection.State == ConnectionState.Closed)
                await command.Connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            // Map data to a list of dictionary
            while (await reader.ReadAsync())
            {
                var item = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    item[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                }
                itemsList.Add(item);
            }

            return itemsList;
        }

        [HttpGet]
        [Route("Offices")]
        public IActionResult ListOfOfficeCensus(string module)
        {
            try
            {
                IQueryable<object> result = Enumerable.Empty<object>().AsQueryable();

                switch (module.ToLower())
                {
                    case "par":
                        result = dBContext.ListPAROffices.OrderBy(x => x.Office);
                        break;
                    case "ptr":
                        result = dBContext.ListREPAROffices.OrderBy(x => x.Office);
                        break;
                    case "prs":
                        result = dBContext.ListPRSOffices.OrderBy(x => x.Office);
                        break;
                    case "ics":
                        result = dBContext.ListICSOffices.OrderBy(x => x.Office);
                        break;
                    case "itr":
                        result = dBContext.ListITROffices.OrderBy(x => x.Office);
                        break;
                    case "rrsep":
                        result = dBContext.ListRRSEPOffices.OrderBy(x => x.Office);
                        break;
                    case "sopa5":
                        result = dBContext.ListofAbove50KOffices.OrderBy(x => x.Office);
                        break;
                    case "sopb5":
                        result = dBContext.ListofBelow50KOffices.OrderBy(x => x.Office);
                        break;
                    default:
                        // Return an empty result when no matching module is found
                        return Ok(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the request", Details = ex.Message });
            }
        }


        // localhost:port/api/Report/PAR/{office}
        [HttpGet]
        [Route("PAR")]
        public IQueryable<PARItemsDetailsVw> ListOfPARByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfPARByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfPARByOffice;
        }

        // localhost:port/api/Report/PTR/{office}
        [HttpGet]
        [Route("PTR")]
        public IQueryable<REPARItemsDetailsVw> ListOfREPARByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfREPARByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfREPARByOffice;
        }

        // localhost:port/api/Report/PRS/{office}
        [HttpGet]
        [Route("PRS")]
        public IQueryable<PRSItemsDetailsVw> ListOfPRSByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfPRSByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfPRSByOffice;
        }


        // localhost:port/api/Report/ICS/{office}
        [HttpGet]
        [Route("ICS")]
        public IQueryable<ICSItemsDetailsVw> ListOfICSByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfICSByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfICSByOffice;
        }

        // localhost:port/api/Report/ITR/{office}
        [HttpGet]
        [Route("ITR")]
        public IQueryable<ITRItemsDetailsVw> ListOfITRByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfITRByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfITRByOffice;
        }

        // localhost:port/api/Report/RRSEP/{office}
        [HttpGet]
        [Route("RRSEP")]
        public IQueryable<RRSEPItemsDetailsVw> ListOfRRSEPByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfRRSEPByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfRRSEPByOffice;
        }

        // localhost:port/api/Report/SOPA5/{office}
        [HttpGet]
        [Route("SOPA5")]
        public IQueryable<SummaryItemsA50kDetailsVw> ListOfA50KItemsByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfAbove50KByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfAbove50KByOffice;
        }
        // localhost:port/api/Report/SOPB5/{office}
        [HttpGet]
        [Route("SOPB5")]
        public IQueryable<SummaryItemsB50kDetailsVw> ListOfB50KItemsByOffice(string? office)
        {
            if (office != null)
            {
                return dBContext.ListOfBelow50KByOffice.Where(x => x.ReceivedByOffice == office);
            }
            return dBContext.ListOfBelow50KByOffice;
        }
    }


}
