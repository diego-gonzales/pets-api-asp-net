using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace pets_web_api;

[ApiController]
[Route("api/pets")]
public class PetsController : ControllerBase
{
    private readonly ILogger<PetsController> _logger;
    private readonly IPetRepository _petRepository;

    public PetsController(IPetRepository petRepository, ILogger<PetsController> logger)
    {
        _logger = logger;
        _petRepository = petRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<PetDTO>>> Get()
    {
        try
        {
            return await _petRepository.GetPets();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting pets");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id:int}", Name = "getPet")]
    public async Task<ActionResult<PetDTO>> Get(int id)
    {
        try
        {
            var petDto = await _petRepository.GetPet(id);

            if (petDto == null)
            {
                return NotFound();
            }

            return petDto;
        }
        catch (System.Exception ex)
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
            var petDto = await _petRepository.CreatePet(createPetDTO);

            return CreatedAtRoute("getPet", new { id = petDto.Id }, petDto);
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
            await _petRepository.UpdatePet(id, updatePetDTO);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating a pet");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<PatchPetDTO> jsonPatchDocument)
    {
        try
        {
            await _petRepository.PatchPet(id, jsonPatchDocument);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while patching a pet");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _petRepository.RemovePet(id);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting a pet");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
