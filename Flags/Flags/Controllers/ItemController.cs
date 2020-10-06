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

        [ActionName("Results")]
        public async Task<ActionResult> ShowResults(string id)
        {
            Console.WriteLine("GET {0}", id);
            if (id == null)
                return BadRequest();

            Quiz item = await _cosmosDBService.GetItemAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        [ActionName("Results")]
        public async Task<ActionResult> PostResults([Bind("Id,Name,Attempts,Completed,Average,Countries")] Quiz item, [Bind("CorrectAnswers")] int correctAnswers)
        {
            Console.WriteLine("POST {0} and {1} and {2}", item.Id, item.Average, correctAnswers);

            if (item.Id == null)
                return BadRequest();

            int quizLength = item.Countries.Length;

            float newAverage = (float)correctAnswers / (float)quizLength * 100;
            string shortened = newAverage.ToString("0.0");
            newAverage = float.Parse(shortened);

            Console.WriteLine(shortened);

            item.Attempts++;
            item.Average = item.Average + ((newAverage - item.Average) / item.Attempts);
            if (correctAnswers == quizLength) item.Completed++;

            await _cosmosDBService.UpdateItemAsync(item.Id, item);

            return View(item);
        }
    }
}
