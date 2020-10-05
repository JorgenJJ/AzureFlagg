using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Flags.Models;
using Flags.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Flags.Controllers
{
    public class ItemController : Controller
    {
        private readonly ICosmosDBService _cosmosDBService;
        public ItemController(ICosmosDBService cosmosDbService)
        {
            _cosmosDBService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _cosmosDBService.GetItemsAsync("SELECT * FROM c"));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,Name,Attempts,Completed,Average,Countries")] Quiz item)
        {
            if (ModelState.IsValid)
            {
                item.Id = Guid.NewGuid().ToString();
                await _cosmosDBService.AddItemAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,Name,Attempts,Completed,Average,Countries")] Quiz item)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDBService.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Quiz item = await _cosmosDBService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Quiz item = await _cosmosDBService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDBService.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDBService.GetItemAsync(id));
        }

        [ActionName("Play")]
        public async Task<ActionResult> PlayQuiz(string id)
        {
            if (id == null)
                return BadRequest();

            Quiz item = await _cosmosDBService.GetItemAsync(id);
            if (item == null)
                return NotFound();


            return View(item);
        }

        public void Test()
        {
            Console.WriteLine("Testtest");
        }

        [HttpPost]
        public ActionResult getNames(string abr)
        {
            abr = "dk";
            var databaseUri = "https://toukerdb.documents.azure.com:443/";
            var primaryKey = "QR9M7wX2mZyCh0eCMZc8WcI3Mug0bkDVwS9Fi2lxMfmHJ6745aaUHS6O6WepgV01hUKBT471845jdglwDuLg5A==";
            var databaseName = "flagdatabase";
            var containerName = "Countries";


            DocumentClient client = new DocumentClient(new Uri(databaseUri), primaryKey);

            Country country = client.CreateDocumentQuery<Country>(
                UriFactory.CreateDocumentCollectionUri(databaseName, containerName))
                .Where(c => c.Abreviation == abr).AsEnumerable().First();

            return View(Json(country.Names));
        }

        [HttpPost]
        public ActionResult getNames2()
        {
            string abr = "se";
            Quiz item = _cosmosDBService.GetItemAsync('SELECT c.names FROM c WHERE c.abreviation = "dk"');
        }
    }
}
