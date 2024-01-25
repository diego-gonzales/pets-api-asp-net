using AutoMapper;

namespace pets_web_api;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Pet, PetDTO>();
        CreateMap<CreatePetDTO, Pet>();
        CreateMap<PatchPetDTO, Pet>().ReverseMap();
    }
}
