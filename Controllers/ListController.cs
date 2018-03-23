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

namespace ListClientMVC.Controllers
{
    public class ListController : Controller
    {
        //private string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkZTaW11RnJGTm9DMHNKWEdtdjEzbk5aY2VEYyIsImtpZCI6IkZTaW11RnJGTm9DMHNKWEdtdjEzbk5aY2VEYyJ9.eyJhdWQiOiIzZWZjNmJkOS04MjFjLTQzM2MtOGY1MC02ZGJjMWVlZDU1ZjAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNTIxODE3NjI5LCJuYmYiOjE1MjE4MTc2MjksImV4cCI6MTUyMTgyMTUyOSwiYWNyIjoiMSIsImFpbyI6IlkyTmdZTWk3cERUblhXWlFEOGZWS08rQUNBdldTVzRocHVsMXJtZFVxempGNi9OdmNBRUEiLCJhbXIiOlsicHdkIiwibWZhIl0sImFwcGlkIjoiNGUyYWMxOWItZDliMi00MTlhLTg0ODItMDY0MTBmMzBlZTIxIiwiYXBwaWRhY3IiOiIxIiwiZmFtaWx5X25hbWUiOiJIYW5zZW4iLCJnaXZlbl9uYW1lIjoiTWljaGFlbCIsImluX2NvcnAiOiJ0cnVlIiwiaXBhZGRyIjoiMTY3LjIyMC4xNDguNDIiLCJuYW1lIjoiTWljaGFlbCBIYW5zZW4iLCJvaWQiOiIwZDk0OTA4Yy05MDYxLTQzNTEtYjlkNy0wYmZhMWEwODA1ZTkiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMTI0NTI1MDk1LTcwODI1OTYzNy0xNTQzMTE5MDIxLTE3MjY1MTciLCJzY3AiOiJMaXN0QXBpLlJlYWRXcml0ZSIsInN1YiI6Ik5scjJVcVlWNElLRWRjZDJCbjBSY3lHbkYyN2hVWGN4NkZuVGgtMFd5Z0kiLCJ0aWQiOiI3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJ1bmlxdWVfbmFtZSI6Im1paGFuc2VuQG1pY3Jvc29mdC5jb20iLCJ1cG4iOiJtaWhhbnNlbkBtaWNyb3NvZnQuY29tIiwidXRpIjoiZGlsbi1QQU9vRW1FSEJKS1JZRUdBQSIsInZlciI6IjEuMCJ9.J5DlNJcNprgApQAgbGf1qx1-pil2cIp9z2tWE01B2D4UBdry57lEp1STfVvmjO8fOPvDOvukCeoNU0myiJtk-rsUj311HxkL6p4HgHmTqVT1LDsxkrFdWI9vnDkX8GF6RofsKub5PZXu4r-S5SjTVTYl6oDts3dH1ewrxPY9ikhH7YxCpiFOYPkJH4vJH5okqbeCYvEvvMnGh4zM4D8nkZLzY5AuR_PpU07zNAKbOCTT9cLPSgJGCxmReYjCrSRFsJ7w57PTd9WOCf_uv5Gv04koY4l7o8_CKygVmYjMw3kFKPF55GuL9UR3G23sPljJ_HYqaGx1h53b2Zu4zRog-A";
 
        public async Task<IActionResult> Index()
        {
            string accessToken = Request.Headers["x-ms-token-aad-access-token"];

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://listapi.azurewebsites.net/api/list");

            var cont = await response.Content.ReadAsStringAsync();

            List<ListItem> items = new List<ListItem>();

            List<ListItem> returnItems = JsonConvert.DeserializeObject< List<ListItem> >(cont);
            if (returnItems != null) {
                items = returnItems;
            }

            ViewData["AccessToken"] = accessToken;

            return View(items);
        }

        public async Task<IActionResult> Delete(int id)
        {
            string accessToken = Request.Headers["x-ms-token-aad-access-token"];

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.DeleteAsync("https://listapi.azurewebsites.net/api/list/" + id.ToString());

            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> New(string description)
        {
            string accessToken = Request.Headers["x-ms-token-aad-access-token"];

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
            string accessToken = Request.Headers["x-ms-token-aad-access-token"];

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
