using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace tema6_revisiones
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string SUBSCRIPTION_KEY = "PON AQUI TU CLAVE DE LA HERRAMIENTA DE REVISION";
            const string REVIEWS_ENDPOINT = "PON AQUI EL ENDPOINT DE LA HERRAMIENTA DE REVISION";
            const string IMAGEN = "https://moderatorsampleimages.blob.core.windows.net/samples/sample1.jpg";
            const string TEAM = "PON AQUI EL IDENTIFICADOR DE TU EQUIPO DE REVISION";
            
            ContentModeratorClient client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = REVIEWS_ENDPOINT };

            //Preparamos la revisión
            List<CreateReviewBodyItemMetadataItem> metadata = new List<CreateReviewBodyItemMetadataItem>(new CreateReviewBodyItemMetadataItem[]
                    { 
                        new CreateReviewBodyItemMetadataItem("a", "false"), 
                        new CreateReviewBodyItemMetadataItem("r", "true"), 
                        new CreateReviewBodyItemMetadataItem("m", "false") 
                    });
            
            CreateReviewBodyItem revision = new CreateReviewBodyItem("image",IMAGEN,"sample1.jpg",null,metadata);
            
            //Ejemplo de revisión de texto
            //CreateReviewBodyItem revision = new CreateReviewBodyItem("text","Texto a revisar","ID de la revisión,null,metadata");
            
            //Invocamos el método de la API para crear la revisión
            var listaIdRevision = await client.Reviews.CreateReviewsAsync("application/json",TEAM,new List<CreateReviewBodyItem> {revision});

            //Esperamos a que se valide por el revisor humano en el portal
            Console.WriteLine();
            Console.WriteLine("Realizar la validación manual de la revisión en el portal de Content Moderator.");
            Console.WriteLine("Después, presionar una tecla para continuar.");
            Console.ReadKey();

            //Invocamos el método de la API para consultar la revisión
            Review infoRevision = await client.Reviews.GetReviewAsync(TEAM, listaIdRevision[0]);

            //Si la revisión se ha completado, mostramos las etiquetas asigandas por el revisor
            if (infoRevision.Status == "Complete")
            {
                foreach (var tag in infoRevision.ReviewerResultTags)
                {
                    Console.WriteLine($"{tag.Key}={tag.Value}");
                }
            }
        }
    }
}
