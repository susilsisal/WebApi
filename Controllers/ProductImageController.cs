using ApiHost.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {


        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("No image uploaded");

            const long maxSize = 2 * 1024 * 1024;

            if (request.Image.Length > maxSize)
                return BadRequest("Image size must be 2 MB or less");

            using var ms = new MemoryStream();
            await request.Image.CopyToAsync(ms);
            var imageData = ms.ToArray();

            var parameters = new
            {
                ProductId = request.ProductId,
                ImageData = imageData,
                FileName = request.Image.FileName,
                ContentType = request.Image.ContentType
            };

            //using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = @"INSERT INTO ProductImages (ProductId, ImageData, FileName, ContentType)
                VALUES (@ProductId, @ImageData, @FileName, @ContentType)";
            await DbHelper.ExecuteAsync(sql, parameters);

            return Ok("Image uploaded successfully");
        }


        [HttpGet("get-image/{productId}")]
        public async Task<IActionResult> GetImage(int productId)
        {
            var sql = @"SELECT TOP 1 ImageData, FileName, ContentType 
                FROM ProductImages 
                WHERE ProductId = @ProductId";

            var result = (await DbHelper.QueryAsync<ProductImage>(sql, new { ProductId = productId }))
                    .FirstOrDefault();

            if (result == null || result.ImageData == null)
                return NotFound("Image not found");

            return File((byte[])result.ImageData, (string)result.ContentType, (string)result.FileName);
        }



    }
}
