using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api.V1;
using Microsoft.Rest;
using Microsoft.PowerBI.Api.V1.Models;

namespace ConsoleApplication39
{
    class Program
    {

        static string accesskey = "KJixsmmw+NGNOtjDZTLOMxi8nilQJ+EzvXDRKVLBnP5HFp7G45RYHRIO23ViXY8iE/zNVHROCmcaOIFr6a2vmQ==";
        static string workspaceCollectionName = "cisdemo";
        static string workspaceId = "79c71931-de23-49a8-a3c8-b79d2192fe0b";
        static void Main(string[] args)
        {

            var credentials = new TokenCredentials(accesskey, "AppKey");

            // Instantiate your Power BI client passing in the required credentials
            var client = new PowerBIClient(credentials);

            // Override the api endpoint base URL.  Default value is https://api.powerbi.com
            client.BaseUri = new Uri("https://api.powerbi.com");

            //create a dataset and get datasetkey
            string datasetID = CreateDatasets(workspaceCollectionName, workspaceId, client);

            string data = @"{  ""rows"":  
                                [
                                    { ""id"": 1, ""name"": ""Tom""},                                
                                    { ""id"": 2, ""name"": ""Jerry""}
                                ]
                            }";

            object dataObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Object>(data.ToString());

            PostRows(workspaceCollectionName, workspaceId, datasetID, "testtable", dataObj, client);

        }


        static void PostRows(string workspaceCollectionName, string workspaceId, string datasetid, string tableName, object datajson, PowerBIClient client)
        {
            //public static object PostRows(this IDatasets operations, string collectionName, string workspaceId, string datasetKey, string tableName, object requestMessage);
            var response = client.Datasets.PostRows(workspaceCollectionName, workspaceId, datasetid, tableName, datajson);


        }

        static string CreateDatasets(string workspaceCollectionName, string workspaceId, PowerBIClient client)
        {

            Dataset ds = new Dataset();
            ds.Name = "testdataset";
            Table table1 = new Table();
            table1.Name = "testTable";


            Column column1 = new Column("id", "Int64");
            Column column2 = new Column("name", "string");

            table1.Columns = new List<Column>() { column1, column2 };

            ds.Tables = new List<Table>() { table1 };

            var response = client.Datasets.PostDataset(workspaceCollectionName, workspaceId, ds);

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Object>(response.ToString());
            return obj["id"].ToString();
        }



    }
}
