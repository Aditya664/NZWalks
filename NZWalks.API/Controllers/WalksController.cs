﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IWalkRepository repository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository repository,IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        // GET: ALL WALKS
        // GET: https://localhost:portnumber/api/walks?filterOn=Name&filterQuary=Track
        [HttpGet]
        [Authorize(Roles = "Reader")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WalkDto[]))]
        public async Task<IActionResult> GetAllWalks([FromQuery] string? filterOn, [FromQuery] string? filterQuary,
            [FromQuery] string? sortBy, [FromQuery] bool? isAssending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var walksDomain = await repository.GetAllWalksAsync(filterOn, filterQuary, sortBy, isAssending ?? true,pageNumber,pageSize);
            return Ok(mapper.Map<List<WalkDto>>(walksDomain));
        }

        // POST: POST NEW WALK
        // POST: https://localhost:portnumber/api/walks
        [HttpPost]
        [Authorize(Roles = "Writer")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WalkDto))]
        [ValidateModel]
        public async Task<IActionResult> CreateWalk([FromBody] AddWalkDto addWalkDto)
        {
            var walkDomainModel = mapper.Map<Walk>(addWalkDto);
            await repository.CreateWalkAsync(walkDomainModel);
            var walkDto = mapper.Map<WalkDto>(walkDomainModel);
            return CreatedAtAction(nameof(GetWalkById), new { id = walkDto.Id }, walkDto);
        }

        // GET: ALL Walk BY ID
        // GET: https://localhost:portnumber/api/walks/08dd2694-27f9-4997-b8bc-5020631574c0
        [HttpGet]
        [Authorize(Roles = "Reader")]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WalkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWalkById([FromRoute] Guid id)
        {
            var walkDomain = await repository.GetWalkByIdAsync(id);
            if (walkDomain == null)
            {
                return NotFound();
            }
            var walkDto = mapper.Map<WalkDto>(walkDomain);
            return Ok(walkDto);
        }

        [HttpPut]
        [Authorize(Roles = "Writer")]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateWalkDto updateDto)
        {
            var regionWalkMode = mapper.Map<Walk>(updateDto);
            regionWalkMode = await repository.UpdateWalkAsync(id, regionWalkMode);
            if (regionWalkMode == null)
            {
                return NotFound();
            }
            regionWalkMode.DifficultyId = updateDto.DifficultyId;
            regionWalkMode.WalkImageUrl = updateDto.WalkImageUrl;
            regionWalkMode.Name = updateDto.Name;
            regionWalkMode.Description = updateDto.Description;
            regionWalkMode.LengthInKm = updateDto.LengthInKm;
            regionWalkMode.RegionId = updateDto.RegionId;
            var regionDto = mapper.Map<WalkDto>(regionWalkMode);
            return Ok(regionDto);

        }
    }
}
