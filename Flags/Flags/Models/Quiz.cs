using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Flags.Models
{
    public class Quiz
    {

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

        private int counter = 0;
        public int Counter { get { return counter; } set { counter += value; } }

        public string[] getFullName(string abr) {
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

        public string[] getRandomOrder() {
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
    }
}