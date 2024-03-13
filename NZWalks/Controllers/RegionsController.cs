using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.CustomActionFilters;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Models.Repositories;

namespace NZWalks.Controllers;

//https:localhost:7103/api/Regions
[Route("api/[controller]")]
[ApiController]
public class RegionsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger<RegionsController> _logger;
    private readonly IRegionRepository _regionRepository;

    public RegionsController(
        IRegionRepository regionRepository,
        IMapper mapper,
        ILogger<RegionsController> logger
    )
    {
        _regionRepository = regionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    //Get all regions
    // Get: https://localhost:7103/api/Regions
    [HttpGet]
    // [Authorize(Roles = "Reader, Writer")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            throw new Exception("This is an custom exception");
            // _logger.LogInformation("Getting all regions");
            // _logger.LogWarning("This is a warning");
            // _logger.LogError("This is an error");

            // Get all regions from the database - Domain Model
            var regionsDomain = await _regionRepository.GetAllAsync();

            //Map Domain Model to DTO
            var regionsDto = _mapper.Map<List<RegionDto>>(regionsDomain);
            //Return DTO
            _logger.LogInformation(
                $"Returning all regions: {JsonSerializer.Serialize(regionsDomain)}"
            );
            return Ok(regionsDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    //Get region by Id
    // Get: https://localhost:7103/api/Regions/{id}
    [HttpGet]
    [Route("{id:guid}")]
    // [Authorize(Roles = "Reader, Writer")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // Get Region Domain Model From Database
        var regionDomain = await _regionRepository.GetByIdAsync(id);
        // Check if region is null before accessing its properties
        if (regionDomain == null)
            return NotFound();

        return Ok(_mapper.Map<RegionDto>(regionDomain));
    }

    //POST to create new region
    //POST: https://localhost:7103/api/Regions
    [HttpPost]
    [ValidateModel]
    // [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
        //Map or convert DTO to Domain Model
        var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);

        // Use Domain Model to creat Region
        regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);

        //Map Domain Model back to DTO
        var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

        return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
    }

    // Update region
    // PUT: https://localhost/7103/api/Regions/{id}
    [HttpPut]
    [Route("{id:guid}")]
    [ValidateModel]
    // [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateRegionRequestDto updateRegionRequestDto
    )
    {
        //Map DTO to Domain Model
        var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);

        // Check if region exist
        regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);
        if (regionDomainModel == null)
            return NotFound();

        //Convert Domain Model to DTO
        var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

        return Ok(regionDto);
    }

    // Delete region
    // DELETE: https://localhost/7103/api/Regions/{id}
    [HttpDelete]
    [Route("{id:guid}")]
    // [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var regionDomainModel = await _regionRepository.DeleteAsync(id);
        if (regionDomainModel == null)
            return NotFound();

        //Return the deleted region
        //Map Domain Model to DTO
        var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

        return Ok(regionDto);
    }
}
