using System.Globalization;
using Microsoft.ML;
using Microsoft.ML.Trainers;

public class RecommendationService
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private PredictionEngine<ContentInteraction, Prediction> _predictionEngine;

    public RecommendationService()
    {
        _mlContext = new MLContext();
    }

    /// <summary>
    /// Carga un modelo de recomendación desde un archivo.
    /// El modelo debe haber sido guardado previamente con el método Save del objeto MLContext.
    /// </summary>
    /// <param name="modelPath"></param>
    public void LoadModel(string modelPath)
    {
        using var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        _model = _mlContext.Model.Load(stream, out var schema);

        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ContentInteraction, Prediction>(_model);
    }


    /// <summary>
    /// Carga los datos de contenidos desde un archivo CSV. 
    /// </summary>
    /// <param name="dataPath"></param>
    /// <returns></returns>
    public List<ContentRecord> LoadContentData(string dataPath)
    {
        using (var reader = new StreamReader(dataPath))
        using (var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null, 
            MissingFieldFound = null 
        }))
        {
            return csv.GetRecords<ContentRecord>().ToList();
        }
    }


    /// <summary>
    /// Entrena un modelo de recomendación basado en interacciones de usuario con contenidos.
    /// El modelo se entrena utilizando el algoritmo de Factorización de Matrices.
    /// </summary>
    /// <param name="interactions"></param>
    public void TrainModel(IEnumerable<ContentInteraction> interactions)
    {
        var data = _mlContext.Data.LoadFromEnumerable(interactions);

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = nameof(ContentInteraction.UserId),
            MatrixRowIndexColumnName = nameof(ContentInteraction.ContentId),
            LabelColumnName = nameof(ContentInteraction.Label),
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var pipeline = _mlContext.Recommendation().Trainers.MatrixFactorization(options);
        _model = pipeline.Fit(data);
        _mlContext.Model.Save(_model, data.Schema, "model.zip");
    }


    /// <summary>
    /// Predice la puntuación de un usuario para un contenido específico basado en el modelo entrenado.
    /// Crear un nuevo PredictionEngine cada vez que se realice una predicción es ineficiente.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="contentId"></param>
    /// <returns></returns>
    public float PredictScore(uint userId, uint contentId)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<ContentInteraction, Prediction>(_model);
        var prediction = predictionEngine.Predict(new ContentInteraction { UserId = userId, ContentId = contentId });
        return prediction.Score;
    }


    /// <summary>
    /// Predice la puntuación de un usuario para un contenido específico basado en el modelo entrenado.
    /// Utiliza el PredictionEngine pre-creado para mejorar la eficiencia.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="contentId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public float PredictScore(int userId, int contentId)
    {
        if (_predictionEngine == null)
            throw new InvalidOperationException("El modelo no está cargado. Asegúrate de cargar el modelo antes de predecir.");

        var prediction = _predictionEngine.Predict(new ContentInteraction { UserId = (uint)userId, ContentId = (uint)contentId });
        return prediction.Score;
    }

    public class Prediction
    {
        public float Score { get; set; }
    }
}
