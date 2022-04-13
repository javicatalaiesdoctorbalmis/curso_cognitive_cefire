using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace tema4_translator_speech
{
    class Program
    {
        const string SPEECH_SUBSCRIPTION_KEY = "PONER_LA_CLAVE_SPEECH_AQUI";
        const string SPEECH_REGION = "PONER_LA_REGION_SPEECH_AQUI";
        const string TRANSLATE_SUBSCRIPTION_KEY = "PONER_LA_CLAVE_TRANSLATE_AQUI";
        const string TRANSLATE_ENDPOINT = "PONER_EL_ENDPOINT_TRANSLATE_AQUI";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Di la frase a traducir...");
            string textoEscuchado = await VozATexto();
            Console.WriteLine($"Texto escuchado:{textoEscuchado}");
            
            string textoTraducido = await Traducir(textoEscuchado);
            Console.WriteLine($"Texto traducido:{textoTraducido}");
            
            await TextoAVoz(textoTraducido);
        }
        
        public static async Task<string> VozATexto()
        {
            //Creamos las configuraciones necesarias
            var speechConfig = SpeechConfig.FromSubscription(SPEECH_SUBSCRIPTION_KEY, SPEECH_REGION);
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            //Si queremos reconocer texto en un fichero de audio en lugar de en el micrófono usaremos la línea siguiente
            //using var audioConfig = AudioConfig.FromWavFileInput("PathToFile.wav");
            
            //Creamos el cliente
            using var cliente = new SpeechRecognizer(speechConfig, "es-ES", audioConfig);
            
            //Realizamos la llamada a la API
            SpeechRecognitionResult resultado = await cliente.RecognizeOnceAsync();

            //Devolvemos el resultado
            return resultado.Text;
        }

        public static async Task TextoAVoz(string texto)
        {
            //Creamos las configuraciones necesarias
            var speechConfig = SpeechConfig.FromSubscription(SPEECH_SUBSCRIPTION_KEY, SPEECH_REGION);
            //Si queremos enviar la voz a un fichero de audio en lugar de a los altavoces crearemos un audioConfig
            //using var audioConfig = AudioConfig.FromWavFileOutput("path/to/write/file.wav");

            //Creamos el cliente                        
            using var cliente = new SpeechSynthesizer(speechConfig); 
            //Si enviamos a un fichero, añadimos el audioConfig
            //using var cliente = new SpeechSynthesizer(speechConfig, audioConfig);
            
            //Realizamos la llamada a la API
            await cliente.SpeakTextAsync(texto);
        }

        public static async Task<string> Traducir(string texto)
        {
            //Ruta de la petición con los parámetros (versión, from y to)
            string ruta = "/translate?api-version=3.0&from=es&to=en";
            object[] body = new object[] { new { Text = texto } };
            string requestBody = JsonConvert.SerializeObject(body);
    
            using (var cliente = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                //Preparamos la petición
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(TRANSLATE_ENDPOINT + ruta);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", TRANSLATE_SUBSCRIPTION_KEY);
                        
                //Realizamos la petición
                HttpResponseMessage response = cliente.Send(request);
                
                //Recuperamos la traducción
                string resultadoJson = await response.Content.ReadAsStringAsync();
                List<document> documentos = JsonConvert.DeserializeObject<List<document>>(resultadoJson);
                return documentos[0].translations[0].text;
            }
        }
    }
    
    public class translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }

    public class document
    {
        public List<translation> translations { get; set; }
    }


}
