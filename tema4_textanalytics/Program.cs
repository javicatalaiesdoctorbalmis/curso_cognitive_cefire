using Azure;
using System;
using System.Globalization;
using Azure.AI.TextAnalytics;

namespace tema4_textanalytics
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SUBSCRIPTION_KEY = "PONER_LA_CLAVE_AQUI";
            const string ENDPOINT = "PONER_EL_ENDPOINT_AQUI";
            const string TEXTO = "El viaje por Francia del verano pasado fue genial. Conocimos muchos sitios"+ 
            "(el que más nos gustó fue la Provenza) y a mucha gente. Lo único malo fue el tiempo,"+ 
            " bastante lluvioso (sobre todo en Normandía). Muchas gracias a nuestro guía, Pierre Martin.";

            TextAnalyticsClient client = new TextAnalyticsClient(new Uri(ENDPOINT), new AzureKeyCredential(SUBSCRIPTION_KEY));

            /////////////////////////
            //Análisis de opinión (Text Analytics-Sentiment)
            /////////////////////////
            Console.WriteLine("---Análisis de opinión---");

            //Invocamos el método de la API para el análisis de la opinión
            DocumentSentiment opinion = client.AnalyzeSentiment(TEXTO,"es");

            //Procesamos el resultado
            Console.WriteLine($"Opinión general del documento: {opinion.Sentiment}\n");

            foreach (SentenceSentiment oracion in opinion.Sentences)
            {
                Console.WriteLine($"\tOración: \"{oracion.Text}\"");
                Console.WriteLine($"\tOpinión de la oración: {oracion.Sentiment}\n");
            }

            /////////////////////////
            //Frases clave (Text Analytics-Key Phrases)
            /////////////////////////
            Console.WriteLine("---Frases clave---");

            //Invocamos el método de la API para extraer frases clave
            KeyPhraseCollection frases = client.ExtractKeyPhrases(TEXTO,"es");

            //Procesamos el resultado
            foreach (string frase in frases)
            {
                Console.WriteLine(frase);
            }

            /////////////////////////
            //Entidades con nombre (Text Analytics-Named Entity Recognition)
            /////////////////////////
            Console.WriteLine("---Entidades con nombre---");

            //Invocamos el método de la API para extraer las entidades
            CategorizedEntityCollection entidades = client.RecognizeEntities(TEXTO,"es");
  
            //Procesamos el resultado
            foreach (CategorizedEntity entidad in entidades)
            {
                Console.WriteLine($"{entidad.Text} - Categoría:{entidad.Category}, Subcategoría:{entidad.SubCategory}");
            }


        }
    }
}
