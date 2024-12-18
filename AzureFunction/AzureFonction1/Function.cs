using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFonction1
{
    public class Function
    {
        private readonly ILogger<Function> _logger;

        public Function(ILogger<Function> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function))]
        public async Task Run(
            [BlobTrigger("blobintput/{name}", Connection = "Blob_ConnectionString")] Stream stream,
            string name)
        {
            // Lire le contenu du blob
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            // Envoyer le nom du fichier à la file d'attente
            string queueName = "myqueue-items";
            string connectionString = Environment.GetEnvironmentVariable("Blob_ConnectionString");

            QueueClient queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                await queueClient.SendMessageAsync(name);
                _logger.LogInformation($"Nom du fichier '{name}' envoyé dans la file d'attente.");
            }
            else
            {
                _logger.LogError("La file d'attente spécifiée n'existe pas.");
            }
        }
    }
}
