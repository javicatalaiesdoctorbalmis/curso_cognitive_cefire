using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace prueba_face
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string SUBSCRIPTION_KEY = "268eb38e97024633854cbdeedad05016";
            const string ENDPOINT = "https://cursocefireface.cognitiveservices.azure.com/";
            const string IMAGE_BASE_URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";
 
            FaceClient client = new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };
        
            /////////////////////////
            //Detección (Face-Detect)
            /////////////////////////
            Console.WriteLine("---Detección de caras---");

            //Definimos la lista de atributos que queremos obtener de cada cara
            List<FaceAttributeType> atributos = new List<FaceAttributeType> 
            {
                FaceAttributeType.Age,FaceAttributeType.Gender
            };
            
            //Invocamos el método de la API para la detección
            IList<DetectedFace> carasDetect = await client.Face.DetectWithUrlAsync(
                $"{IMAGE_BASE_URL}"+"detection5.jpg",
                returnFaceAttributes: atributos,
                returnFaceId:true
                ); 

            //Procesamos el resultado
            foreach (DetectedFace cara in carasDetect)
            {
                Console.WriteLine($"{cara.FaceId}-{cara.FaceAttributes.Gender}-{cara.FaceAttributes.Age}");
            };


            /////////////////////////
            //Verificación (Face-Verify)
            /////////////////////////
            Console.WriteLine("\n---Verificación de caras---");

            //Invocamos el método de la API para la detección para cada una de las imágenes a comparar
            IList<DetectedFace> carasVerify1 = await client.Face.DetectWithUrlAsync($"{IMAGE_BASE_URL}"+"Family1-Dad1.jpg",returnFaceId:true);
            IList<DetectedFace> carasVerify2 = await client.Face.DetectWithUrlAsync($"{IMAGE_BASE_URL}"+"Family1-Dad2.jpg",returnFaceId:true);

            //Invocamos al método de la API para verificar si se trata de la mimsa persona
            VerifyResult verificacion = await client.Face.VerifyFaceToFaceAsync(carasVerify1[0].FaceId.Value, carasVerify2[0].FaceId.Value);

            //Procesamos el resultado
            if (verificacion.IsIdentical)
                Console.WriteLine($"Las dos caras pertenecen a la misma persona (confianza de la verificación:{verificacion.Confidence})");
            else
                Console.WriteLine($"Las dos caras no pertenecen a la misma persona (confianza de la verificación:{verificacion.Confidence})");
            
            
            /////////////////////////
            //Identificación (Face-Indetify)
            /////////////////////////
            Console.WriteLine("\n---Identificación de caras---");

            //Creamos el grupo (lo borramos previamente si existe)
            string idGrupo = "familia";
            try
            {
                    await client.PersonGroup.DeleteAsync(idGrupo);
            }
            catch{}
            await client.PersonGroup.CreateAsync(idGrupo,"Familia");

            //Creamos dos personas en el grupo
            Person padre = await client.PersonGroupPerson.CreateAsync(idGrupo,"Padre");
            Person madre = await client.PersonGroupPerson.CreateAsync(idGrupo,"Madre");

            //Asociamos dos imágenes a cada una de las dos personas
            await client.PersonGroupPerson.AddFaceFromUrlAsync(idGrupo,padre.PersonId,$"{IMAGE_BASE_URL}"+"Family1-Dad1.jpg");
            await client.PersonGroupPerson.AddFaceFromUrlAsync(idGrupo,padre.PersonId,$"{IMAGE_BASE_URL}"+"Family1-Dad2.jpg");
            await client.PersonGroupPerson.AddFaceFromUrlAsync(idGrupo,madre.PersonId,$"{IMAGE_BASE_URL}"+"Family1-Mom1.jpg");
            await client.PersonGroupPerson.AddFaceFromUrlAsync(idGrupo,madre.PersonId,$"{IMAGE_BASE_URL}"+"Family1-Mom2.jpg");

            //Entrenamos el grupo
            await client.PersonGroup.TrainAsync(idGrupo);
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(idGrupo);
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }

            //Detectamos las caras de una nueva imagen y las añadimos a una  lista
            IList<DetectedFace> carasIdentify = await client.Face.DetectWithUrlAsync($"{IMAGE_BASE_URL}"+"identification1.jpg",returnFaceId:true);
            List<Guid> listaCarasDetectadas = new List<Guid>();
            foreach (DetectedFace cara in carasIdentify) 
            { 
                listaCarasDetectadas.Add(cara.FaceId.Value); 
            };

            //Invocamos el método de la API para identificar las caras detectadas dentro de nuestro grupo.
            IList<IdentifyResult> resultadosIdentify = await client.Face.IdentifyAsync(listaCarasDetectadas, idGrupo);

            //Procesamos el resultado
            foreach (IdentifyResult resultado in resultadosIdentify)
            {
                if (resultado.Candidates.Count > 0)
                {
                    Person person = await client.PersonGroupPerson.GetAsync(idGrupo, resultado.Candidates[0].PersonId);
                    Console.WriteLine($"{person.Name} se ha encontrado en la imagen"); 
                }
            }
         }
    }
}
