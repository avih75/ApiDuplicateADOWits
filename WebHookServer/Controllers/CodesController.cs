using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using WebHookServer.Models;

namespace WebHookServer.Controllers
{
    public class CodesController : ApiController
    {
        string sourceUrl = ConfigurationManager.AppSettings["SourceUrl"];
        string targetUrl = ConfigurationManager.AppSettings["TargetUrl"];
        string loggerFullNameAndPath = ConfigurationManager.AppSettings["loggerFullNameAndPath"];
        // POST api/Codes
        public void Post([FromBody] JObject ChangeSet)
        { 
            TargetADO ADO = new TargetADO(sourceUrl, targetUrl,  loggerFullNameAndPath);
            //JProperty WorkItemFields = ChangeSet.Children<JProperty>().FirstOrDefault(x => x.Name == "resource");
            ADO.PushCode();
        }
    }
}
