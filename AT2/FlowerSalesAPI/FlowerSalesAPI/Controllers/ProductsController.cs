using Amazon.Runtime.Internal;
using FlowerSalesAPI.Models;
using FlowerSalesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace FlowerSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly FlowerService _flowerService;

        public ProductsController(FlowerService flowerService) =>
            _flowerService = flowerService;

        [HttpGet]
        public async Task<List<Product>> GetAll([FromQuery] QueryParameters queryParameters)
        {
            var products = await _flowerService.GetAsync();

            var pagesProducts = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size)
                .ToList();
            
            return pagesProducts;
        } 

        [Route("filternames")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetNamesAsync(string name, string sortBy, string sortOrder)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(sortBy) || string.IsNullOrEmpty(sortOrder))
            {
                return BadRequest();
            }

            var productNames = await _flowerService.GetNamesAsync(name, sortBy, sortOrder);

            if (productNames == null)
            {
                return NotFound();
            }

            return Ok(productNames);
        }

        [Route("filterstores")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetStoresAsync(string store, string sortBy, string sortOrder)
        {
            if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(sortBy) || string.IsNullOrEmpty(sortOrder))
            {
                return BadRequest();
            }

            var productStores = await _flowerService.GetStores(store, sortBy, sortOrder);

            if (productStores == null)
            {
                return NotFound();
            }
            return Ok(productStores);            
        }

        [Route("filterprices")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetPricesAsync(double minPrice, double maxPrice)
        {
            if (minPrice >= 0 && maxPrice >= 0 && minPrice <= maxPrice)
            {
                var productPrices = await _flowerService.GetPrices(minPrice, maxPrice);

                if (productPrices == null)
                {
                    return NotFound();
                }

                return Ok(productPrices);
            }
            else
            {
                return BadRequest();
            }            
        }            

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Product>> GetID(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var product = await _flowerService.GetAsync(id);

                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            else 
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(Product newProduct)
        {
            await _flowerService.CreateAsync(newProduct);

            return CreatedAtAction(nameof(GetAll), new { id = (newProduct.Id) }, newProduct);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Product updatedProduct)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var product = await _flowerService.GetAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                updatedProduct.Id = product.Id;

                await _flowerService.UpdateAsync(id, updatedProduct);

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var product = await _flowerService.GetAsync(id);

                if (product is null)
                {
                    return NotFound();
                }

                await _flowerService.RemoveAsync(id);

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
