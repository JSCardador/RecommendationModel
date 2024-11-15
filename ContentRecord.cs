using CsvHelper.Configuration.Attributes;

/// <summary>
/// Representa um registro de contenidos del Dataset.
/// </summary>
public class ContentRecord
{
    [Name("id")]
    public int Id { get; set; }

    [Name("title")]
    public string Title { get; set; }

    [Name("overview")]
    public string Overview { get; set; }

    [Name("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [Name("vote_average")]
    public float VoteAverage { get; set; }
}