using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace tema3_computervision
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string SUBSCRIPTION_KEY = "PONER AQUI LA CLAVE";
            const string ENDPOINT = "PONER AQUI LA URL DEL ENDPOINT";
            const string IMAGE_BASE_URL = "https://moderatorsampleimages.blob.core.windows.net/samples";

            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };

            /////////////////////////
            //Análisis de imagen (ComputerVision-Analyze Image)
            /////////////////////////
            Console.WriteLine("---Análisis de imagen---");

            //Definimos la lista de características y detalles que queremos obtener de la imagen
            List<VisualFeatureTypes?> caracteristicas = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Objects
            };

            List<Details?> detalles = new List<Details?>()
            {
                Details.Celebrities
            };
            
            //Invocamos el método de la API para el análisis de la imagen
            ImageAnalysis resultado = await client.AnalyzeImageAsync(
                url: $"{IMAGE_BASE_URL}/sample1.jpg",
                visualFeatures: caracteristicas,
                details: detalles,
                language:"es"
                );
            
            //Procesamos el resultado
            //Descripción
            Console.WriteLine($"Descripción:{resultado.Description.Captions[0].Text}"); 
            
            //Categorías
            Console.WriteLine("Categorías:");  
            foreach (Category categoria in resultado.Categories)
            {
                Console.WriteLine($"\t{categoria.Name} ({categoria.Score})");
            };

            //Etiquetas
            Console.WriteLine("Etiquetas:");  
            foreach (ImageTag etiqueta in resultado.Tags)
            {
                Console.WriteLine($"\t{etiqueta.Name} ({etiqueta.Confidence})");
            };

            //Contenido para adultos
            Console.WriteLine($"¿Contenido para adultos? {resultado.Adult.IsAdultContent}");
            Console.WriteLine($"¿Contenido subido de tono? {resultado.Adult.IsRacyContent}");
            Console.WriteLine($"¿Contenido sangriento? {resultado.Adult.IsGoryContent}");

            //Objetos encontrados
            Console.WriteLine("Objetos:");  
            foreach (DetectedObject objeto in resultado.Objects)
            {
                Console.WriteLine($"\t{objeto.ObjectProperty} ({objeto.Rectangle.W},{objeto.Rectangle.H},{objeto.Rectangle.X},{objeto.Rectangle.Y})");
            };

            //Famosos
            Console.WriteLine("Famosos:");  
            foreach (Category categoria in resultado.Categories)
            {
                if (categoria.Detail?.Celebrities != null)
                {
                    foreach (CelebritiesModel famoso in categoria.Detail.Celebrities)
                    {
                        Console.WriteLine($"\t{famoso.Name}");
                    }
                }
            }

            /////////////////////////
            //Obtención de miniatura (ComputerVision-Get Thumbnail)
            /////////////////////////
            Console.WriteLine("---Obtención de miniatura---");

            //Invocamos el método de la API para obtener la miniatura
            Stream miniatura = await client.GenerateThumbnailAsync(100, 100, $"{IMAGE_BASE_URL}/sample6.png", true);

            //Almacenamos el stream del resultado en un fichero local            
            using (Stream file = File.Create("./miniatura.jpg")) { miniatura.CopyTo(file); } 
            Console.WriteLine($"Generado el fichero miniatura.jpg");

            /////////////////////////
            //OCR (ComputerVision-Read)
            /////////////////////////
            Console.WriteLine("---OCR---");

            //Invocamos el método de la API para solicitar la operación de lectura 
            ReadHeaders operacionOCR = await client.ReadAsync($"{IMAGE_BASE_URL}/sample2.jpg", language: "es");

            //Obtenemos el identificador de la operaciónd e lectura
            string localizador = operacionOCR.OperationLocation;
            string idOperacion = localizador.Substring(localizador.Length - 36);

            //Esperamos a que la operación de lectura acabe    
            ReadOperationResult resultadoOCR;
            while (true)
            {
                await Task.Delay(1000);
                resultadoOCR = await client.GetReadResultAsync(Guid.Parse(idOperacion));
                if (resultadoOCR.Status == OperationStatusCodes.Succeeded) { break; }
            }

            //Procesamos el resultado
            Console.WriteLine("Texto encontrado:");
            foreach (ReadResult pagina in resultadoOCR.AnalyzeResult.ReadResults)
            {
                foreach (Line linea in pagina.Lines)
                {
                    Console.WriteLine($"\t{linea.Text}");
                }
            }
        }
    }
}
