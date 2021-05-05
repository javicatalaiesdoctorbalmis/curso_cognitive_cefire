using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bing.WebSearch;
using Microsoft.Bing.WebSearch.Models;

namespace tema7_websearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string SUBSCRIPTION_KEY = "PON AQUI TU CLAVE DE BING SEARCH";
            const string BUSQUEDA = "Zion Wiliamson";

            WebSearchClient client = new WebSearchClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY));
            
            //Así filtrariamos el tipo de resultado, pero se produce una excepción si se indican varios valores
            //IList<string> filtros = new List<string>() {"news"};
            
            //Invocamos el método de la API para la búsqueda web
            SearchResponse webData = await client.Web.SearchAsync(
                query: BUSQUEDA, count:5, market:"es-ES", setLang:"es");
                        
            //Procesamos el resultado

            //Contexto
            Console.WriteLine($"Consulta realizada: {webData.QueryContext.AlteredQuery}\nConsulta original: {webData.QueryContext.AlterationOverrideQuery}\n");

            //Páginas
            Console.WriteLine("\n-------- Páginas web -----\n");
            if (webData?.WebPages?.Value?.Count > 0)
            {
                Console.WriteLine($"Número de páginas encontradas:{webData.WebPages.Value.Count}\n");
                foreach (WebPage pagina in webData.WebPages.Value)
                {
                    Console.WriteLine($"{pagina.Name}\n{pagina.Url}\n{pagina.Snippet}\n");
                }
            }
            else
            {
                Console.WriteLine("No se han encontrado resultados de páginas web.\n");
            }

            //Imágenes
            Console.WriteLine("\n-------- Imágenes -----\n");
            if (webData?.Images?.Value?.Count > 0)
            {
                Console.WriteLine($"Número de imágenes encontradas:{webData.Images.Value.Count}\n");
                foreach (ImageObject imagen in webData.Images.Value)
                {
                    Console.WriteLine($"{imagen.Name}\n{imagen.ContentUrl}\n{imagen.HostPageUrl}\n{imagen.ThumbnailUrl}\n");
                    
                }
            }
            else
            {
                Console.WriteLine("No se han encontrado resultados de imágenes. \n");
            }

            //Vídeos
            Console.WriteLine("\n-------- Vídeos -----\n");
            if (webData?.Videos?.Value?.Count > 0)
            {
                Console.WriteLine($"Número de vídeos encontrados:{webData.Videos.Value.Count}\n");
                foreach (VideoObject video in webData.Videos.Value)
                {
                    Console.WriteLine($"{video.Name}\n{video.ContentUrl}\n{video.HostPageUrl}\n{video.ThumbnailUrl}\n{video.MotionThumbnailUrl}\n{video.EmbedHtml}\n{video.ViewCount}\n");
                    
                }
            }
            else
            {
                Console.WriteLine("No se han encontrado resultados de videos.\n");
            }

            //Noticias
            Console.WriteLine("\n-------- Noticias -----\n");
            if (webData?.News?.Value?.Count > 0)
            {
                Console.WriteLine($"Número de noticias encontradas:{webData.News.Value.Count}\n");
                foreach (NewsArticle noticia in webData.News.Value)
                {
                    Console.WriteLine($"{noticia.Name}\n{noticia.Description}\n{noticia.Url}\n{noticia.ThumbnailUrl}\n");
                    
                }
            }
            else
            {
                Console.WriteLine("No se han encontrado resultados de noticias.\n");
            }

            //Búsquedas relacionadas
            Console.WriteLine("\n-------- Búsquedas relacionadas -----\n");
            if (webData?.RelatedSearches?.Value?.Count > 0)
            {
                Console.WriteLine($"Número de búsquedas relacionadas encontradas:{webData.RelatedSearches.Value.Count}\n");
                foreach (Query busqueda in webData.RelatedSearches.Value)
                {
                    Console.WriteLine($"{busqueda.Text}\n{busqueda.WebSearchUrl}\n");
                    
                }
            }
            else
            {
                Console.WriteLine("No se han encontrado búsquedas relacionadas.\n");
            }
        }
    }
}
