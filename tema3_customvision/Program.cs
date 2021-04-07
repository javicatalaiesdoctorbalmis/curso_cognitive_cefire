using System;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace tema3_customvision
{
    class Program
    {
        static void Main(string[] args)
        {
            const string PREDICTION_KEY = "PONER AQUI LA CLAVE";
            const string ENDPOINT = "PONER AQUI LA URL DEL ENDPOINT";
            const string PROJECT_ID = "PONER AQUI EL ID DEL PROYECTO";
            const string PUBLISHED_NAME = "PONER AQUI EL NOMBRE DE LA ITERACION";
            const string IMAGE_BASE_URL = "https://moderatorsampleimages.blob.core.windows.net/samples";

            CustomVisionPredictionClient client = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(PREDICTION_KEY))
            {
                Endpoint = ENDPOINT
            };
           ImageUrl imagen = new ImageUrl($"{IMAGE_BASE_URL}/sample8.png");

           //Invocamos el método de la API para clasificar la imagen
           ImagePrediction resultado = client.ClassifyImageUrl(new Guid(PROJECT_ID),PUBLISHED_NAME , imagen);

            //Procesamos el resultado
            foreach (var c in resultado.Predictions)
            {
                Console.WriteLine($"\t{c.TagName}:{c.Probability}");
            }
        }
    }
}
