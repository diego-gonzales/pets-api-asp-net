using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace pets_web_api;

public interface IPetRepository
{
    Task<List<PetDTO>> GetPets();
    Task<PetDTO> GetPet(int id);
    Task<PetDTO> CreatePet(CreatePetDTO createPetDTO);
    Task UpdatePet(int id, CreatePetDTO updatePetDTO);
    Task PatchPet(int id, JsonPatchDocument<PatchPetDTO> jsonPatchDocument);
    Task RemovePet(int id);
}
