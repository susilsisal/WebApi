using ApiHost.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiHost.Controllers
{
    public class CustomerLocationController : ControllerBase
    {

        [HttpPost]
        [Route("api/add-customer-location")]
        public IActionResult AddFruit([FromBody] CustomerLocationsInsert model)
        {
            DbHelper.ExecuteStoredProcedureAsync("usp_Insert_CustomerLocations", model);

            return Ok();
        }


    }


    public class CustomerLocationsInsert
    {
        public int customer_id { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public string address { get; set; }
        public string label { get; set; }
        public DateTime created_at { get; set; }
        public string Nearest_landmark { get; set; }
        public string Remarks { get; set; }
    }
}
