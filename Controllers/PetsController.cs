using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pets_web_api;

[ApiController]
[Route("api/pets")]
public class PetsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<PetsController> _logger;

    public PetsController(
        ApplicationDbContext dbContext,
        IMapper mapper,
        ILogger<PetsController> logger
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<PetDTO>>> Get()
    {
        try
        {
            var pets = await _dbContext.Pets.ToListAsync();
            return _mapper.Map<List<PetDTO>>(pets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting pets.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id:int}", Name = "getPet")]
    public async Task<ActionResult<PetDTO>> Get(int id)
    {
        try
        {
            var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

            if (pet == null)
            {
                return NotFound();
            }

            return _mapper.Map<PetDTO>(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting a pet");
            return StatusCode(500, "An error occurred while processing your request.");
        }
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
        try
        {
            var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

            if (pet == null)
            {
                return NotFound();
            }

            pet = _mapper.Map(updatePetDTO, pet);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating a pet.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<PatchPetDTO> jsonPatchDocument)
    {
        try
        {
            var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

            if (pet == null)
            {
                return NotFound();
            }

            var patchPetDto = _mapper.Map<PatchPetDTO>(pet);
            jsonPatchDocument.ApplyTo(patchPetDto, ModelState);

            var isValid = TryValidateModel(patchPetDto);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(patchPetDto, pet);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating a pet.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var petExists = await _dbContext.Pets.AnyAsync(x => x.Id == id);

            if (!petExists)
            {
                return NotFound();
            }

            _dbContext.Remove(new Pet() { Id = id });
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting a pet.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
