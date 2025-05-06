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
