﻿using Microsoft.Data.SqlClient;

namespace ApiHost.Controllers
{
    using ApiHost.Models;
    using Microsoft.AspNetCore.Mvc;

    public class FruitsController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddFruit([FromBody] usp_Insert_productsParams model)
        {
            DbHelper.ExecuteStoredProcedureAsync("usp_Insert_products", model);

            return Ok();
        }



        [HttpGet]
        [Route("api/get-active-fruits")]
        public async Task<IActionResult> GetActiveFruits(int product_id=0)
        {
            try
            {
                var fruits = await DbHelper.QueryStoredProcedureAsync<dynamic>("usp_GetActiveFruits", new { product_id });
                return Ok(fruits.ToList());
            }
            catch (SqlException ex)
            {
                // Log error here (e.g., using a logger)
                return StatusCode(500, new { error = "Database error occurred.", details = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other unexpected errors
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
    }



    public class usp_Insert_productsParams
    {
        public string name { get; set; }
        public string description { get; set; }
        public decimal price_per_kg { get; set; }
        public DateTime created_at { get; set; }
        public string status { get; set; }
        public int categoryid { get; set; }
    }
