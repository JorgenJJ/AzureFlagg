using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Reflection.Metadata.Ecma335;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Core;

namespace Flags.Models
{
    public class Quiz
    {
        public static SecretClientOptions options = new SecretClientOptions()
        {
            Retry = {
                Delay = TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };
        public SecretClient keyClient = new SecretClient(new Uri("https://toukerkeyvault.vault.azure.net/"), new DefaultAzureCredential(), options);

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "attempts")]
        public int Attempts { get; set; }

        [JsonProperty(PropertyName = "completed")]
        public int Completed { get; set; }

        [JsonProperty(PropertyName = "average")]
        public float Average { get; set; }

        [JsonProperty(PropertyName = "countries")]
        public string[] Countries { get; set; }

        public string[] GetFullName(string abr) {
            //KeyVaultSecret secret = keyClient.GetSecret("CosmosPrimaryKey");

            var databaseUri = "https://toukerdb.documents.azure.com:443/";
            var primaryKey = "QR9M7wX2mZyCh0eCMZc8WcI3Mug0bkDVwS9Fi2lxMfmHJ6745aaUHS6O6WepgV01hUKBT471845jdglwDuLg5A==";
            var databaseName = "flagdatabase";
            var containerName = "Countries";


            DocumentClient client = new DocumentClient(new Uri(databaseUri), primaryKey);

            Country country = client.CreateDocumentQuery<Country>(
                UriFactory.CreateDocumentCollectionUri(databaseName, containerName))
                .Where(c => c.Abreviation == abr).AsEnumerable().First();

            return country.Names;
        }

        public string[] GetRandomOrder() {
            Random rng = new Random();

            string[] shuffledList = Countries;

            int n = Countries.Count();
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                string temp = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = temp;
            }

            return shuffledList;
        }

        public List<Country> GetAllCountries()
        {
            //KeyVaultSecret secret = keyClient.GetSecret("CosmosPrimaryKey");

            var databaseUri = "https://toukerdb.documents.azure.com:443/";
            string primaryKey = "QR9M7wX2mZyCh0eCMZc8WcI3Mug0bkDVwS9Fi2lxMfmHJ6745aaUHS6O6WepgV01hUKBT471845jdglwDuLg5A==";
            var databaseName = "flagdatabase";
            var containerName = "Countries";

            DocumentClient client = new DocumentClient(new Uri(databaseUri), primaryKey);
            FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            var countries = client.CreateDocumentQuery<Country>(
                UriFactory.CreateDocumentCollectionUri(databaseName, containerName), 
                "SELECT * FROM Countries c ORDER BY c.abreviation", DefaultOptions)
                .AsEnumerable().ToList();

            return countries;
        }
    }
}