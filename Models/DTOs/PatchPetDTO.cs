using System.ComponentModel.DataAnnotations;

namespace pets_web_api;

public class PatchPetDTO
{
    [Required]
    public string Name { get; set; }
    public string Breed { get; set; }
    public string Color { get; set; }

    [Range(0, 25)]
    public int Age { get; set; }
    public float Weight { get; set; }
}
