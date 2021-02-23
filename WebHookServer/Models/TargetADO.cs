using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebHookServer.Models
{
    public class TargetADO
    { 
        private string targetCollectionUrl;
        private string sourceCollectionUrl;
        private Logger logger;
        private readonly TfsConnection targetTfsConnection;
        private readonly TfsConnection sourceTfsConnection;
        private readonly WorkItemTrackingHttpClient targetWorkItemTrackingHttpClient;
        private readonly WorkItemTrackingHttpClient sourceWorkItemTrackingHttpClient;
        private string workItemType;
        public TargetADO(string sourceUrl, string targetUrl, string logFile, string workItem = null)
        {
            workItemType = workItem;
            logger = new Logger(logFile);
            sourceCollectionUrl = sourceUrl;
            targetCollectionUrl = targetUrl;
            sourceTfsConnection = GetAzureAccess(sourceCollectionUrl);
            targetTfsConnection = GetAzureAccess(targetCollectionUrl);
            sourceWorkItemTrackingHttpClient = sourceTfsConnection.GetClient<WorkItemTrackingHttpClient>();
            targetWorkItemTrackingHttpClient = targetTfsConnection.GetClient<WorkItemTrackingHttpClient>();
        }
        private TfsConnection GetAzureAccess(string CollectionUrl)
        {
            try
            {
                logger.WriteLog("Trying to GET Connection to Azure Devops: " + CollectionUrl, LogLevel.Inform);
                TfsConnection tfsConnection = new TfsTeamProjectCollection(new Uri(CollectionUrl));
                try
                {
                    tfsConnection.EnsureAuthenticated();
                    logger.WriteLog("Connected !!", LogLevel.Attention);
                }
                catch (Exception e)
                {
                    logger.WriteLog("Exception : " + e.Message, LogLevel.Critical);
                    throw;
                }
                return tfsConnection;
            }
            catch (Exception ex)
            {
                logger.WriteLog(message: "Error Try Get TFS URI " + "\n" + ex.InnerException, LogLevel.Critical);
                return null;
            }
        }
        public bool CreateNewWorkItem(JToken obj)
        {
            Dictionary<string, JToken> model = PharseModel(obj);
            var x = model["id"];
            WorkItem newWorkItem = GetWorkItemFromSource(int.Parse(x.ToString()));
            string projectName = newWorkItem.Fields["System.TeamProject"].ToString();
            string workItemType = newWorkItem.Fields["System.WorkItemType"].ToString();
            JsonPatchDocument patchDocument = CreatePatchDoc(newWorkItem);
            try
            {
                WorkItem newBug = targetWorkItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, workItemType, bypassRules: true).Result;
            }
            catch (Exception ex)
            {
                logger.WriteLogAllExceptionError(ex);
            }

            return true;
        }
        private WorkItem GetWorkItemFromSource(int id)
        {
            try
            {
                WorkItem sourceWorkItem = sourceWorkItemTrackingHttpClient.GetWorkItemAsync(75, null, null, WorkItemExpand.All).Result;
                return sourceWorkItem;
            }
            catch(Exception ex)
            {
                logger.WriteLogAllExceptionError(ex);
                return null;
            }  
        }
        public bool UpdateWorkItem(JToken obj)
        {
            Dictionary<string, JToken> model = PharseModel(obj);
            var x = model["id"];
            WorkItem newWorkItem = GetWorkItemFromSource(int.Parse(x.ToString()));
            JsonPatchDocument patchDocument = CreatePatchDoc(newWorkItem);
            WorkItem returnedWI = targetWorkItemTrackingHttpClient.UpdateWorkItemAsync(document: patchDocument, id: 1).Result;
            return true;
        }
        public bool DeleteWorkItem(int id)
        {
            return true;
        }
        public bool RestoreWorkItem(int id)
        {
            return true;
        }
        public bool PushCode()
        {
            return true;
        }
        private Dictionary<string, JToken> PharseModel(JToken WorkItem)
        {
            JProperty WorkItemFields = WorkItem.Children<JProperty>().FirstOrDefault(x => x.Name == "resource");
            JObject WorkItemFieldsValues = (JObject)WorkItemFields.Value;
            Dictionary<string, JToken> FieldsValues = new Dictionary<string, JToken>();
            FieldsValues.Add("id", WorkItemFieldsValues.Children<JProperty>().FirstOrDefault(x => x.Name == "id").Value);
            FieldsValues.Add("rev", WorkItemFieldsValues.Children<JProperty>().FirstOrDefault(x => x.Name == "rev").Value);
            FieldsValues.Add("fields", WorkItemFieldsValues.Children<JProperty>().FirstOrDefault(x => x.Name == "fields").Value);
            FieldsValues.Add("_links", WorkItemFieldsValues.Children<JProperty>().FirstOrDefault(x => x.Name == "_links").Value);
            FieldsValues.Add("url", WorkItemFieldsValues.Children<JProperty>().FirstOrDefault(x => x.Name == "url").Value);
            return FieldsValues;
        }
        private JsonPatchDocument CreatePatchDoc(WorkItem newWorkItem)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            foreach (KeyValuePair<string, object> field in newWorkItem.Fields)
            {
                if (field.Key.ToLower() == "System.Id".ToLower())
                    continue;
                try
                {
                    patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/"+field.Key,
                        Value = field.Value
                    });
                }
                catch (Exception ex)
                {
                    logger.WriteLogAllExceptionError(ex);
                }
            }
            return patchDocument;
        }
    }
}
public enum actionType
{
    New,
    Update,
    Delete,
    Restore
}