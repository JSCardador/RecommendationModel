class Program
{
    static void Main(string[] args)
    {

        var recommendationService = new RecommendationService();

        // Ruta al dataset
        string dataPath = "tmdb_5000_movies.csv";

        var contentData = recommendationService.LoadContentData(dataPath);

        // Generar interacciones ficticias para entrenamiento
        var interactions = contentData.Select(contentRecord => new ContentInteraction
        {
            UserId = 1, 
            ContentId = (uint)contentRecord.Id,
            Label = contentRecord.VoteAverage
        }).ToList();

        Console.WriteLine("Datos de prueba: \n" + string.Join("\n", interactions.Select(i => $"Usuario: {i.UserId}, Contenido: {i.ContentId}, Calificación: {i.Label}")));

        recommendationService.TrainModel(interactions);

        float score = recommendationService.PredictScore(1, (uint)contentData.First().Id);
        Console.WriteLine($"Predicted Score: {score} ");
      

        var contentList = contentData.Select(c => new ContentRecord
        {
            Id = c.Id,
            Title = c.Title
        }).ToList().OrderBy(c => recommendationService.PredictScore(1, (uint)c.Id));

        Console.WriteLine("Top 5 recomendaciones:" + string.Join("\n", contentList.Take(5).Select(c => $"{c.Title} - Score: {recommendationService.PredictScore(1, (uint)c.Id)}")));
    }

}
