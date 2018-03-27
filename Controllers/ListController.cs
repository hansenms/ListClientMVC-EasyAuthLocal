using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ListClientMVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using ListClientMVC.Services;

namespace ListClientMVC.Controllers
{
    public class ListController : Controller
    { 

        private IEasyAuthProxy _easyAuthProxy;

        public ListController(IEasyAuthProxy easyproxy) {
            _easyAuthProxy = easyproxy;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = _easyAuthProxy.Headers["x-ms-token-aad-access-token"];

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://listapi.azurewebsites.net/api/list");

            var cont = await response.Content.ReadAsStringAsync();

            List<ListItem> items = new List<ListItem>();

            List<ListItem> returnItems = JsonConvert.DeserializeObject< List<ListItem> >(cont);
            if (returnItems != null) {
                items = returnItems;
            }

            return View(items);
        }

        public async Task<IActionResult> Delete(int id)
        {
            string accessToken = _easyAuthProxy.Headers["x-ms-token-aad-access-token"];

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.DeleteAsync("https://listapi.azurewebsites.net/api/list/" + id.ToString());

            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> New(string description)
        {
            string accessToken = _easyAuthProxy.Headers["x-ms-token-aad-access-token"];

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ListItem item = new ListItem();
            item.Description = description;

            var stringContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://listapi.azurewebsites.net/api/list/", stringContent);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ToggleComplete(int id)
        {
            string accessToken = _easyAuthProxy.Headers["x-ms-token-aad-access-token"];

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://listapi.azurewebsites.net/api/list/" + id.ToString());
            var cont = await response.Content.ReadAsStringAsync();
            ListItem item = JsonConvert.DeserializeObject<ListItem>(cont);

            //Toggle
            item.Completed = !item.Completed;

            var stringContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            response = await client.PutAsync ("https://listapi.azurewebsites.net/api/list/" + id.ToString(), stringContent);

            return RedirectToAction("Index");
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
