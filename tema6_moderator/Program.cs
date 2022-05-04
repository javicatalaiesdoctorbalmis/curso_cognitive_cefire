using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace tema6_moderator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string SUBSCRIPTION_KEY = "PON AQUI TU CLAVE DE CONTENT MODERATOR";
            const string ENDPOINT = "PON AQUI TU ENDPOINT DE CONTENT MODERATOR";
            const string IMAGEN = "https://moderatorsampleimages.blob.core.windows.net/samples/sample1.jpg";
            const string TEXTO = @"La direción IP deel serviDor es 176.34.21.23 (para cualquier poblema escribir al fucking master
                                   del Administrador: admin@acme.com o buscarlo en Facebook)";
            
            ContentModeratorClient client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };

            /////////////////////////
            //Moderación de texto (Text-Screen)
            /////////////////////////
            Console.WriteLine("---Moderación de texto---");
            Console.WriteLine($"Texto original:{TEXTO}"); 

            //Creamos la lista de términos
            string listaTerminosId = await CrearListaTerminos(client);


            //Preparamos el texto para el procesado
            string textoSinSaltos = TEXTO.Replace(Environment.NewLine, " ");
            byte[] textoBytes = Encoding.UTF8.GetBytes(textoSinSaltos);
            MemoryStream stream = new MemoryStream(textoBytes);
            
            //Invocamos el método de la API para el análisis del texto
            Screen resultadoTexto = await client.TextModeration.ScreenTextAsync("text/plain", stream, "spa", true, true, listaTerminosId, false);

            //Procesamos el resultado
            //Texto autocorregido
            Console.WriteLine($"Texto autocorregido:{resultadoTexto.AutoCorrectedText}"); 

            //Información personal
            Console.WriteLine($"Información personal");
            if (resultadoTexto.PII != null)
            {
                foreach (Email correo in resultadoTexto.PII.Email)
                {
                    Console.WriteLine($"\tCorreo electrónico:{correo.Text}"); 
                }
                foreach (IPA direccionIP in resultadoTexto.PII.IPA)
                {
                    Console.WriteLine($"\tDirección IP:{direccionIP.Text}"); 
                } 
            } 

            //Términos encontrados (palabras soeces o coincidencias en una lista)
            Console.WriteLine($"Términos encontrados");
            if (resultadoTexto.Terms != null)
            {
                foreach (DetectedTerms termino in resultadoTexto.Terms)
                {
                    Console.WriteLine($"\t{termino.Term}"); 
                }
            }

            //Eliminamos la lista de términos
            await EliminarListaTerminos(client,listaTerminosId);

            /////////////////////////
            //Moderación de imagen (Image-Evaluate)
            /////////////////////////
            Console.WriteLine("\n---Moderación de imagen---");

            BodyModel imagenUrl = new BodyModel("URL", IMAGEN);
            
            //Invocamos el método de la API para el análisis de la imagen
            Evaluate resultadoImagen = await client.ImageModeration.EvaluateUrlInputAsync("application/json", imagenUrl);

            //Procesamos el resultado
            Console.WriteLine($"Contenido adultos:{resultadoImagen.IsImageAdultClassified} ({resultadoImagen.AdultClassificationScore})");             
            Console.WriteLine($"Contenido sugerente:{resultadoImagen.IsImageRacyClassified} ({resultadoImagen.RacyClassificationScore})");             

            /////////////////////////
            //Comprobación de imagen en lista (Image-Match)
            /////////////////////////
            Console.WriteLine("\n---Comprobación de imagen en lista---");

            //Creamos la lista de imágenes
            string listaImagenesId = await CrearListaImagenes(client);

            //Invocamos el método de la API para la coprobación de la imagen en la lista
            MatchResponse resultadoComprobacion = await client.ImageModeration.MatchUrlInputAsync("application/json", imagenUrl);

            //Procesamos el resultado
            if (resultadoComprobacion.IsMatch.Value)
            {
                foreach (Match comprobacion in resultadoComprobacion.Matches)
                {
                    Console.WriteLine($"Descripción: {comprobacion.Label} - Etiqueta:{comprobacion.Tags[0]}");             
                }
            }

            //Eliminamos la lista de imágenes
            await EliminarListaImagenes(client,listaImagenesId);
        }


        ////// Métodos de gestión de listas
        static async Task<string> CrearListaTerminos(ContentModeratorClient client)
        {            
            //Creamos la lista
            TermList lista = await client.ListManagementTermLists.CreateAsync("application/json", new Body("palabras_prohibidas"));
            string listaId = lista.Id.ToString();

            //Añadimos los términos
            await client.ListManagementTerm.AddTermAsync(listaId, "Facebook", "spa");
            await client.ListManagementTerm.AddTermAsync(listaId, "Instagram", "spa");

            //Refrescamos el índice de la lista
            await client.ListManagementTermLists.RefreshIndexMethodAsync(listaId,"spa");

            return listaId;
        }
        static async Task EliminarListaTerminos(ContentModeratorClient client, string listaId)
        {            
            //Eliminamos la lista
            await client.ListManagementTermLists.DeleteAsync(listaId);            
        }


        static async Task<string> CrearListaImagenes(ContentModeratorClient client)
        {            
            //Creamos la lista
            ImageList lista = await client.ListManagementImageLists.CreateAsync("application/json", new Body("imagenes_prohibidas"));
            string listaId = lista.Id.ToString();

            //Añadimos las imágenes
            BodyModel imagenUrl = new BodyModel("URL", "https://moderatorsampleimages.blob.core.windows.net/samples/sample1.jpg");
            await client.ListManagementImage.AddImageUrlInputAsync(listaId,"application/json",imagenUrl,101,"Grupo de gente en el lago");
            imagenUrl = new BodyModel("URL", "https://moderatorsampleimages.blob.core.windows.net/samples/sample3.png");
            await client.ListManagementImage.AddImageUrlInputAsync(listaId,"application/json",imagenUrl,101,"Mujer en bikini");
                        
            //Refrescamos el índice de la lista
            await client.ListManagementImageLists.RefreshIndexMethodAsync(listaId);

            return listaId;
        }

        static async Task EliminarListaImagenes(ContentModeratorClient client, string listaId)
        {            
            //Eliminamos la lista
            await client.ListManagementImageLists.DeleteAsync(listaId);            
        }

    }
}
