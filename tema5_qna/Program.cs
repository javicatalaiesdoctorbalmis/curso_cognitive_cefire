using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

namespace tema5_qna
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string ENDPOINT_KEY = "PONER AQUI LA CLAVE DEL ENDPOINT";
            const string ENDPOINT = "PONER AQUI LA URL DEL ENDPOINT";
            const string KBID = "PONER AQUI EL IDENTIFICADOR DE LA BASE DE CONOCIMIENTO";

            QnAMakerRuntimeClient runtimeClient = new QnAMakerRuntimeClient(new EndpointKeyServiceClientCredentials(ENDPOINT_KEY)) { RuntimeEndpoint = ENDPOINT };

            //Bucle de conversación
            while (true)
            {
                //Leemos la pregunta
                Console.Write("Escribe tu pregunta:");
                string pregunta = Console.ReadLine();

                //Invocamos a la API para obtener la respuesta
                QnASearchResultList respuesta = await runtimeClient.Runtime.GenerateAnswerAsync(
                    KBID, new QueryDTO { Question = pregunta });
                
                //Mostramos la respuesta
                Console.WriteLine("Respuesta: {0}\n", respuesta.Answers[0].Answer);
                
                //Si la respuesta incluye prompts los procesamos
                if (respuesta.Answers[0].Context.Prompts.Count > 0)
                {
                    Console.WriteLine("Temas relacionados:");
                    foreach (PromptDTO prompt in respuesta.Answers[0].Context.Prompts)
                    {
                        Console.WriteLine($"{prompt.DisplayText} ({prompt.QnaId})");
                    }
                    Console.WriteLine("Si quieres saber más indica el número entre paréntesis (Enter para seguir con la conversación)");
                    string idPregunta = Console.ReadLine();
                    if (idPregunta != "")
                    {
                        respuesta = await runtimeClient.Runtime.GenerateAnswerAsync(KBID, new QueryDTO { QnaId = idPregunta});
                        Console.WriteLine("{0}\n", respuesta.Answers[0].Answer);
                    }
                }
            }
        }
    }
}
