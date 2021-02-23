using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using WebHookServer.Models;

namespace WebHookServer.Controllers
{
    public class WorkItemsController : ApiController
    {
        string sourceUrl = ConfigurationManager.AppSettings["SourceUrl"];  
        string targetUrl = ConfigurationManager.AppSettings["TargetUrl"];
        string loggerFullNameAndPath = ConfigurationManager.AppSettings["loggerFullNameAndPath"];       
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        // GET api/WorkItems/5
        public string Get(int id)
        {
            return "value";
        }
        // POST api/WorkItems
        [HttpPost]
        public void NewWorkItem([FromBody] JObject WorkItem)
        {
            TargetADO ADO = new TargetADO(sourceUrl, targetUrl, loggerFullNameAndPath);
            ADO.CreateNewWorkItem(WorkItem);
        }
        // POST api/WorkItems
        [HttpPost]
        public void UpdateWorkItem([FromBody] JObject WorkItem)
        {
            TargetADO ADO = new TargetADO(sourceUrl, targetUrl,  loggerFullNameAndPath);
            ADO.UpdateWorkItem(WorkItem);
        }
        // PUT api/WorkItems/5
        public void Put(int id, [FromBody] string value)
        {
        }
        // DELETE api/WorkItems/5
        public void Delete(int id)
        {
        }
    }
}
