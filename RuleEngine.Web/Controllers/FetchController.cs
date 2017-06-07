using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RuleEngine.Web.Models;

namespace RuleEngine.Web.Controllers
{
    public class FetchController : Controller
    {
        public async Task<string> Index([FromBody] FetchModel value)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(value.Url);
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }
    }
}
