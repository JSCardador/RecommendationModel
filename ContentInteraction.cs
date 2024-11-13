using Microsoft.ML.Data;

public class ContentInteraction
{
    [KeyType(count: 100000)]
    public uint UserId { get; set; }
    [KeyType(count: 100000)]
    public uint ContentId { get; set; }
    public float Label { get; set; } // Este será el rating o interés del usuario
}