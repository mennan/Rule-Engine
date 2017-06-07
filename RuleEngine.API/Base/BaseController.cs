using Microsoft.AspNetCore.Mvc;
using RuleEngine.Data;

namespace RuleEngine.API.Base
{
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        protected RuleEngineDb Db { get; set; }

        public BaseController()
        {
            Db = new RuleEngineDb();
        }
    }
}
