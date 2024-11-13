class Program
{
    static void Main(string[] args)
    {

        var recommendationService = new RecommendationService();

        // Ruta del archivo de datos (cambia esto según tu archivo)
        string dataPath = "tmdb_5000_movies.csv";

        // Cargar datos
        var contentData = recommendationService.LoadContentData(dataPath);

        // Generar interacciones ficticias para entrenamiento (ajusta con datos reales más adelante)
        var interactions = contentData.Select(contentRecord => new ContentInteraction
        {
            UserId = 1, // Ejemplo: Usuario de prueba
            ContentId = (uint)contentRecord.Id,
            Label = contentRecord.VoteAverage
        }).ToList();

        // Imprimir datos de prueba
        Console.WriteLine("Datos de prueba: \n" + string.Join("\n", interactions.Select(i => $"Usuario: {i.UserId}, Contenido: {i.ContentId}, Calificación: {i.Label}")));

        // Entrenar el modelo
        recommendationService.TrainModel(interactions);

        // Hacer una predicción de prueba
        float score = recommendationService.PredictScore(1, (uint)contentData.First().Id);
        Console.WriteLine($"Predicted Score: {score} ");
      

        // Lista de Contenidos
        var contentList = contentData.Select(c => new ContentRecord
        {
            Id = c.Id,
            Title = c.Title
        }).ToList().OrderBy(c => recommendationService.PredictScore(1, (uint)c.Id));

        // Imprimir las 5 recomendaciones principales con sus títulos y puntuaciones
        Console.WriteLine("Top 5 recomendaciones:" + string.Join("\n", contentList.Take(5).Select(c => $"{c.Title} - Score: {recommendationService.PredictScore(1, (uint)c.Id)}")));

    }

}