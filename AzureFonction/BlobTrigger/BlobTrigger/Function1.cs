using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobTrigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([BlobTrigger("input-container/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=devoir2storageaccount1;AccountKey=CB0zBAZO5eVQVoDEGKFmjJFMdVrYORyXxIb0u56rJb8s2GrYGIk3114Riz5fjD1QU+Cq9A79RUAG+AStIDP8ug==;EndpointSuffix=core.windows.net")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
