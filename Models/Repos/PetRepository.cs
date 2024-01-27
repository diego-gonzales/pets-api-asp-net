using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pets_web_api;

public class PetRepository : IPetRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public PetRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<PetDTO>> GetPets()
    {
        var pets = await _dbContext.Pets.ToListAsync();
        return _mapper.Map<List<PetDTO>>(pets);
    }

    public async Task<PetDTO> GetPet(int id)
    {
        var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

        if (pet == null)
        {
            throw new KeyNotFoundException();
        }

        return _mapper.Map<PetDTO>(pet);
    }

    public async Task<PetDTO> CreatePet(CreatePetDTO createPetDTO)
    {
        var pet = _mapper.Map<Pet>(createPetDTO);

        pet.CreationDate = DateTime.Now;

        _dbContext.Add(pet);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<PetDTO>(pet);
    }

    public async Task UpdatePet(int id, CreatePetDTO updatePetDTO)
    {
        var petExist = await GetPet(id);

        var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

        pet = _mapper.Map(updatePetDTO, pet);

        await _dbContext.SaveChangesAsync();
    }

    public async Task PatchPet(int id, JsonPatchDocument<PatchPetDTO> jsonPatchDocument)
    {
        var petExist = await GetPet(id);

        var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

        var patchPetDTO = _mapper.Map<PatchPetDTO>(pet);

        jsonPatchDocument.ApplyTo(patchPetDTO);

        pet = _mapper.Map(patchPetDTO, pet);

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemovePet(int id)
    {
        var petExist = await GetPet(id);

        var pet = await _dbContext.Pets.FirstOrDefaultAsync(x => x.Id == id);

        _dbContext.Remove(pet);
        await _dbContext.SaveChangesAsync();
    }
}
