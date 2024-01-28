using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Models.Domain;

namespace NZWalks.Controllers
{
    //https:localhost:7103/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        //Get all regions
        // Get: https://localhost:7103/api/regions
        [HttpGet]
        public IActionResult GetAll()
        {
            var regions = new List<Region>
            {
                new Region()
                {
                    Id = Guid.NewGuid(),
                    Name = "Auckland Region",
                    Code = "AKL",
                    RegionImageUrl = "blob:https://photos.onedrive.com/05b0d0e3-b31c-4892-ab1d-05ac53f89b32"
                },
                new Region()
                {
                    Id = Guid.NewGuid(),
                    Name = "Wellington  Region",
                    Code = "WLG",
                    RegionImageUrl = "blob:https://photos.onedrive.com/05b0d0e3-b31c-4892-ab1d-05ac53f89b32"
                }
            };
            return Ok(regions);
        }
    }
}
