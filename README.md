# RecommendationModel

Este proyecto es un sistema de recomendación de contenido creado con .NET y ML.NET. El modelo de recomendación utiliza la técnica de **Matrix Factorization** para analizar interacciones de usuarios con contenido, y luego predecir una puntuación de interés en otros contenidos. Este proyecto está pensado para mostrar el proceso de creación y entrenamiento de un modelo de recomendación.


## Estructura del proyecto

- **ContentRecord**: Representa los registros de contenido en el dataset, con información como el título, fecha de lanzamiento y calificación promedio.
- **ContentInteraction**: Define la interacción entre usuarios y contenido, usada para entrenar el modelo de recomendación.
- **RecommendationService**: Carga los datos, entrena el modelo y realiza predicciones.

## DataSet
Se usa un DataSet extrado de [Kaggle](https://www.kaggle.com/datasets/tmdb/tmdb-movie-metadata), cuenta con 5000 peliculas direntes. 
El archivo CSV y el modelo generado se encuentran en el repositorio. 

El modelo muestra el **error cuadrático medio (RMSE)** y el **error absoluto medio (MAE)** después de entrenar. Esto permite evaluar la precisión del modelo de recomendación.
