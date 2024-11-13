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

    public void LoadModel(string modelPath)
    {
        // Cargar el modelo entrenado desde el archivo .zip
        using var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        _model = _mlContext.Model.Load(stream, out var schema);

        // Crear el PredictionEngine a partir del modelo cargado
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ContentInteraction, Prediction>(_model);
    }

    public float PredictScore(int userId, int contentId)
    {
        if (_predictionEngine == null)
            throw new InvalidOperationException("El modelo no está cargado. Asegúrate de cargar el modelo antes de predecir.");

        // Realizar una predicción
        var prediction = _predictionEngine.Predict(new ContentInteraction { UserId = (uint)userId, ContentId = (uint)contentId });
        return prediction.Score;
    }

    public List<ContentRecord> LoadContentData(string dataPath)
    {
        using (var reader = new StreamReader(dataPath))
        using (var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null, // Ignora validación de encabezados
            MissingFieldFound = null // Ignora campos faltantes
        }))
        {
            return csv.GetRecords<ContentRecord>().ToList();
        }

    }

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

    public float PredictScore(uint userId, uint contentId)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<ContentInteraction, Prediction>(_model);
        var prediction = predictionEngine.Predict(new ContentInteraction { UserId = userId, ContentId = contentId });
        return prediction.Score;
    }

    public class Prediction
    {
        public float Score { get; set; }
    }
}