using System;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFonction1
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run(
        [QueueTrigger("myqueue-items", Connection = "ConnexionString")] QueueMessage message)
        {
            // Récupérer le nom du fichier à partir du message
            string originalName = message.MessageText;
            string modifiedName = $"modified_{originalName}";

            _logger.LogInformation($"Processing blob: {originalName}, new name: {modifiedName}");

            // Connexion au compte de stockage
            string connectionString = Environment.GetEnvironmentVariable("Blob_ConnectionString");
            string inputContainerName = "blobinput";
            string outputContainerName = "bloboutput";

            // Créer les clients de blob pour accéder aux conteneurs source et destination
            var blobServiceClient = new BlobServiceClient(connectionString);

            var inputContainerClient = blobServiceClient.GetBlobContainerClient(inputContainerName);
            var outputContainerClient = blobServiceClient.GetBlobContainerClient(outputContainerName);

            // Obtenir les références des blobs source et destination
            var sourceBlobClient = inputContainerClient.GetBlobClient(originalName);
            var destinationBlobClient = outputContainerClient.GetBlobClient(modifiedName);

            // Copier le blob source dans le blob destination
            if (await sourceBlobClient.ExistsAsync())
            {
                await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
                _logger.LogInformation($"Blob '{originalName}' copié en '{modifiedName}' dans le conteneur '{outputContainerName}'.");
            }
            else
            {
                _logger.LogError($"Le blob source '{originalName}' n'existe pas dans le conteneur '{inputContainerName}'.");
            }
        }
    }
}
