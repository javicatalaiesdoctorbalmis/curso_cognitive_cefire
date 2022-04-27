using Azure;
using Azure.AI.Language.QuestionAnswering;
using System;

namespace tema5_qa
{
    class Program
    {
        static void Main(string[] args)
        {
            const string KEY = "PON AQUI TU CLAVE DEL RECURSO DE LENGUAJE";
            const string ENDPOINT = "PON AQUI TU URL DEL RECURSO DE LENGUAJE";
            const string PROJECT_NAME = "PON AQUI EL NOMBRE DE TU PROYECTO EN LANGUAGE STUDIO";
            const string DEPLOYMENT_NAME = "production";

            QuestionAnsweringClient cliente = new QuestionAnsweringClient(new Uri(ENDPOINT), new AzureKeyCredential(KEY));
            QuestionAnsweringProject proyecto = new QuestionAnsweringProject(PROJECT_NAME, DEPLOYMENT_NAME);

            //Bucle de conversación
            while (true)
            {
                //Leemos la pregunta
                Console.Write("Escribe tu pregunta:");
                string pregunta = Console.ReadLine();

                //Invocamos a la API para obtener la respuesta
                Response<AnswersResult> respuesta = cliente.GetAnswers(pregunta, proyecto);
                
                //Mostramos la respuesta
                Console.WriteLine("Respuesta: {0}\n", respuesta.Value.Answers[0].Answer);
            }

        }
    }
}
