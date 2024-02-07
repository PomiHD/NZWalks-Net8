using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly IRegionRepository _regionRepository;


    public RegionsController(NzWalksDbContext dbContext, IRegionRepository regionRepository,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _regionRepository = regionRepository;
        _mapper = mapper;
    }

    //Get all regions
    // Get: https://localhost:7103/api/regions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Get all regions from the database - Domain Model
        var regionsDomain = await _regionRepository.GetAllAsync();

        //Map Domain Model to DTO
        // var regionsDto = new List<RegionDto>();
        // foreach (var regionDomain in regionsDomain)
        //     regionsDto.Add(new RegionDto
        //     {
        //         Id = regionDomain.Id,
        //         Code = regionDomain.Code,
        //         Name = regionDomain.Name,
        //         RegionImageUrl = regionDomain.RegionImageUrl
        //     });

        //Map Domain Model to DTO
        var regionsDto = _mapper.Map<List<RegionDto>>(regionsDomain);
        //Return DTO
        return Ok(regionsDto);
    }

    //Get region by Id
    // Get: https://localhost:7103/api/regions/{id}
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // var region = dbContext.Regions.Find(id);
        // Get Region Domain Model From Database
        var regionDomain = await _regionRepository.GetByIdAsync(id);
        // Check if region is null before accessing its properties
        if (regionDomain == null) return NotFound();

        //Since region is not null, we can safely map it to RegionDto,
        //Map Region Domain Model to Region Dto
        // var regionDto = new RegionDto
        // {
        //     Id = regionDomain.Id,
        //     Code = regionDomain.Code,
        //     Name = regionDomain.Name,
        //     RegionImageUrl = regionDomain.RegionImageUrl
        // };

        return Ok(_mapper.Map<RegionDto>(regionDomain));
    }

    //POST to create new region
    //POST: https://localhost:7031/api/regions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
        //Map or convert DTO to Domain Model
        var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);
        // var regionDomainModel = new Region
        // {
        //     Code = addRegionRequestDto.Code,
        //     Name = addRegionRequestDto.Name,
        //     RegionImageUrl = addRegionRequestDto.RegionImageUrl
        // };

        // Use Domain Model to creat Region
        regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);

        //Map Domain Model back to DTO
        var regionDto = _mapper.Map<RegionDto>(regionDomainModel);
        // var regionDto = new RegionDto
        // {
        //     Id = regionDomainModel.Id,
        //     Code = regionDomainModel.Code,
        //     Name = regionDomainModel.Name,
        //     RegionImageUrl = regionDomainModel.RegionImageUrl
        // };

        return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
    }

    // Update region
    // PUT: https://localhost/7031/api/regions/{id}
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id,
        [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
    {
        //Map DTO to Domain Model
        var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);
        // var regionDomainModel = new Region
        // {
        //     Id = id,
        //     Code = updateRegionRequestDto.Code,
        //     Name = updateRegionRequestDto.Name,
        //     RegionImageUrl = updateRegionRequestDto.RegionImageUrl
        // };

        // Check if region exist
        regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);
        if (regionDomainModel == null) return NotFound();

        //Convert Domain Model to DTO 
        var regionDto = _mapper.Map<RegionDto>(regionDomainModel);
        // var regionDto = new RegionDto
        // {
        //     Id = regionDomainModel.Id,
        //     Code = regionDomainModel.Code,
        //     Name = regionDomainModel.Name,
        //     RegionImageUrl = regionDomainModel.RegionImageUrl
        // };
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
        var regionDto = _mapper.Map<RegionDto>(regionDomainModel);
        // var regionDto = new RegionDto
        // {
        //     Id = regionDomainModel.Id,
        //     Code = regionDomainModel.Code,
        //     Name = regionDomainModel.Name,
        //     RegionImageUrl = regionDomainModel.RegionImageUrl
        // };
        return Ok(regionDto);
    }
}