using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pets_web_api;

[ApiController]
[Route("api/pets")]
public class PetsController : MyCustomControllerBase<PetsController>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<PetsController> _logger;

    public PetsController(
        ApplicationDbContext dbContext,
        IMapper mapper,
        ILogger<PetsController> logger
    )
        : base(dbContext, mapper, logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<PetDTO>>> Get()
    {
        return await Get<Pet, PetDTO>();
    }

    [HttpGet("{id:int}", Name = "getPet")]
    public async Task<ActionResult<PetDTO>> Get(int id)
    {
        return await Get<Pet, PetDTO>(id);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreatePetDTO createPetDTO)
    {
        try
        {
            var pet = _mapper.Map<Pet>(createPetDTO);

            pet.CreationDate = DateTime.Now;

            _dbContext.Add(pet);
            await _dbContext.SaveChangesAsync();

            var petDto = _mapper.Map<PetDTO>(pet);

            return CreatedAtRoute("getPet", new { id = pet.Id }, petDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a pet");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put([FromBody] CreatePetDTO updatePetDTO, int id)
    {
        return await Put<Pet, CreatePetDTO>(id, updatePetDTO);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<PatchPetDTO> jsonPatchDocument)
    {
        return await Patch<Pet, PatchPetDTO>(id, jsonPatchDocument);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        return await Delete<Pet>(id);
    }
}
