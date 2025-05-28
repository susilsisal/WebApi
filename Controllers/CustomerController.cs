using ApiHost.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiHost.Controllers
{
    public class CustomerController : ControllerBase
    {

        [HttpPost]
        [Route("api/add-customers")]
        public IActionResult AddFruit([FromBody] usp_Insert_Customers model)
        {
            DbHelper.ExecuteStoredProcedureAsync("usp_Insert_Customers", model);

            return Ok();
        }
        [HttpPost]
        [Route("api/add-customers-location")]
        public async Task<IActionResult> AddCustomerLocation([FromBody] Usp_Insert_Customer_Location_ProcParams model)
        {
            try
            {
                await DbHelper.CallPostgresProcedureAsync("usp_insert_customer_location_proc", model);
                return Ok(new { message = "Customer location added successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging in place
                // _logger.LogError(ex, "Error inserting customer location");

                return StatusCode(500, new
                {
                    error = "An error occurred while adding customer location.",
                    details = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/get-customer-location")]
        public async Task<IActionResult> GetCustomerLocation(int c_id)
        {
            try
            {
                var list =await DbHelper.QueryPostgresFunctionAsync<CusotmerLocation>("usp_get_customerlocation", new { c_id});
                return Ok(list);
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging in place
                // _logger.LogError(ex, "Error inserting customer location");

                return StatusCode(500, new
                {
                    error = "",
                    details = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("api/get-customers")]
        public async Task<IActionResult> GetCustomers( int id = 0)
        {
            var customers = await DbHelper.QueryPostgresFunctionAsync<Customers>("usp_getcustomers", new {id});
            return Ok(customers.ToList());
        }
        [HttpPost]
        [Route("api/check-user")]
        public async Task<IActionResult> CheckUser([FromBody] usp_CheckUsers model)
        {
            var user = await DbHelper.QueryStoredProcedureAsync<Customers>("usp_CheckUsers", model);
            return Ok(user.ToList());
        }

    [HttpPost]
        [Route("api/update-user_phone")]
        public async Task<IActionResult> UpdateUserPhone([FromBody] UpdatePhone model)
        {
            await DbHelper.ExecuteStoredProcedureAsync("usp_UpdatePhone", model);
            return Ok();
        }

    }

    public class UpdatePhone
    {
        public int? CustomerID { get; set; }
        public string? Email { get; set; }
        public string NewPhone { get; set; }
    }


    public class usp_CheckUsers
    {
        public string email { get; set; }
    }


    public class Usp_Insert_Customer_Location_ProcParams
    {
        public int P_Customer_Id { get; set; }
        public decimal P_Latitude { get; set; }
        public decimal P_Longitude { get; set; }
        public string P_Address { get; set; }
        public string P_Label { get; set; }
        public DateTime P_Created_At { get; set; }
        public string P_Nearest_Landmark { get; set; }
        public string P_Remarks { get; set; }
    }

    public class CusotmerLocation
    {

public	int	location_id	{ get; set; }
public	int	customer_id	{ get; set; }
public	float	latitude	{ get; set; }
public	float	longitude	{ get; set; }
public	string	address	{ get; set; }
public	string	label	{ get; set; }
public	DateTime	created_at	{ get; set; }
public	string	nearest_landmark	{ get; set; }
public	string	remarks	{ get; set; }
    }
    public class usp_Insert_Customers
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public decimal primary_latitude { get; set; }
        public decimal primary_longitude { get; set; }
        public string primary_address { get; set; }
        public DateTime created_at { get; set; }
    }
}
