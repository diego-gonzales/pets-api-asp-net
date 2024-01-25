namespace pets_web_api;

public class PetDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Breed { get; set; }
    public string Color { get; set; }
    public int Age { get; set; }
    public float Weight { get; set; }
    public DateTime CreationDate { get; set; }
}
