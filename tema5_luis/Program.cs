using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;


namespace tema5_luis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string PREDICTION_KEY = "PONER AQUI LA CLAVE DEL RECURSO DE PREDICCION DE LUIS";
            const string ENDPOINT = "PONER AQUI LA URL DEL ENDPOINT DE PREDICCION DE LUIS";
            
            Guid APPID = new Guid("PONER AQUI EL IDENTIFICADOR DE LA APLICACION DE LUIS");
            string slot = "Staging"; //Staging o Production

            LUISRuntimeClient client = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(PREDICTION_KEY)) { Endpoint = ENDPOINT };

            while (true)
            {
                //Realizamos la petición a la API
                Console.Write("Introduce la orden: ");
                string pregunta = Console.ReadLine();
                PredictionRequest peticion = new PredictionRequest { Query = pregunta };

                PredictionResponse prediccion = await client.Prediction.GetSlotPredictionAsync(APPID, slot, peticion);
                
                //Procesamos el resultado
                Console.WriteLine($"Acción: {prediccion.Prediction.TopIntent}");
                foreach (var entidad in prediccion.Prediction.Entities)
                {
                    Console.WriteLine($"{entidad.Key}: {JsonConvert.DeserializeObject<List<string>>(entidad.Value.ToString())[0]}");                
                }
                Console.WriteLine();
            }
        }
    }
}
