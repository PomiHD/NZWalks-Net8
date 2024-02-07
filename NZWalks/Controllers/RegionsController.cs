using Microsoft.AspNetCore.Mvc;
using NZWalks.Data;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Models.Repositories;

namespace NZWalks.Controllers;

//https:localhost:7103/api/regions
[Route("api/[controller]")]
[ApiController]
public class RegionsController : ControllerBase
{
    private readonly NzWalksDbContext _dbContext;
    private readonly IRegionRepository _regionRepository;

    public RegionsController(NzWalksDbContext dbContext, IRegionRepository regionRepository)
    {
        _dbContext = dbContext;
        _regionRepository = regionRepository;
    }

    //Get all regions
    // Get: https://localhost:7103/api/regions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var regions = await _regionRepository.GetAllAsync();

        return Ok(regions);
    }

    //Get region by Id
    // Get: https://localhost:7103/api/regions/{id}
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // var regions = dbContext.Regions.Find(id);
        // Get Region Domain Model From Database
        var region = await _regionRepository.GetByIdAsync(id);
        // Check if region is null before accessing its properties
        if (region == null) return NotFound();

        //Since region is not null, we can safely map it to RegionDto,
        //Map Region Domain Model to Region Dto
        var regionDto = new RegionDto
        {
            Id = region.Id,
            Code = region.Code,
            Name = region.Name,
            RegionImageUrl = null
        };
        return Ok(regionDto);
    }

    //POST to create new region
    //POST: https://localhost:7031/api/regions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
        //Map or convert DTO to Domain Model
        var regionDomainModel = new Region
        {
            Code = addRegionRequestDto.Code,
            Name = addRegionRequestDto.Name,
            RegionImageUrl = addRegionRequestDto.RegionImageUrl
        };

        // Use Domain Model to creat Region
        regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);

        //Map Domain Model back to DTO
        var regionDto = new RegionDto
        {
            Id = regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl
        };
        return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
    }

    // Update region
    // PUT: https://localhost/7031/api/regions/{id}
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
    {
        //Map DTO to Domain Model
        var regionDomainModel = new Region
        {
            Id = id,
            Code = updateRegionDto.Code,
            Name = updateRegionDto.Name,
            RegionImageUrl = updateRegionDto.RegionImageUrl
        };
        // Check if region exist
        regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);
        if (regionDomainModel == null) return NotFound();

        //Convert Domain Model to DTO 
        var regionDto = new RegionDto
        {
            Id = regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl
        };
        return Ok(regionDto);
    }

    // Delete region
    // DELETE: https://localhost/7031/api/regions/{id}
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var regionDomainModel = await _regionRepository.DeleteAsync(id);
        if (regionDomainModel == null) return NotFound();
        
        //Return the deleted region
        //Map Domain Model to DTO
        var regionDto = new RegionDto
        {
            Id = regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl
        };
        return Ok(regionDto);
    }
}