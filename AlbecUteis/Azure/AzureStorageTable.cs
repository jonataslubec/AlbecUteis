using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//      ,--._______,-. 
//         ,','  ,    .  ,_`-. 
//        / /  ,' , _` ``. |  )       `-.. 
//       (,';'""`/ '"`-._ ` \/ ______    \\ 
//         : ,o.-`- ,o.  )\` -'      `---.)) 
//         : , __  ^-.   '|   `.      `    `. 
//         |/ __:_     `. |  ,  `       `    \ 
//         | ( ,-.`-.    ;'  ;   `       :    ; 
//         | |  ,   `.      /     ;      :    \ 
//         ;-'`:::._,`.__),'             :     ; 
//        / ,  `-   `--                  ;     | 
//       /  \                   `       ,      | 
//      (    `     :              :    ,\      | 
//       \   `.    :     :        :  ,'  \    : 
//        \    `|-- `     \ ,'    ,-'     :-.-'; 
//        :     |`--.______;     |        :    : 
//         :    /           |    |         |   \ 
//         |    ;           ;    ;        /     ; 
//       _/--' | Black Dog :`-- /         \_:_:_| 
//     ,',','  |           |___ \ 
//     `^._,--'           / , , .) 
//                        `-._,-' 


public class AzureStorageTable
{
    CloudStorageAccount storageAccount;

    public AzureStorageTable()
    {
        storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
    }

    /// <summary>
    /// Retorna a URL do sistema mais o container e caminho passados.
    /// </summary>
    /// <param name="container"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public string returnPathForBlob(string container, string path)
    {
        return "https://teste.blob.core.windows.net/" + container + "/" + path;
    }

    /// <summary>
    /// Retorna a Base do diretório com o TimeStamp para salvar arquivos temporários
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string returnPathTempForBlob(string fileName)
    {
        string timestamp = DateTimeUTC.Now().ToString("yyyyMMddHHmmssfff");
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\", timestamp + "_" + fileName);
    }

    public void createTable(string name)
    {
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference(name);
        table.CreateIfNotExists();
    }

    public void deleteTable(string name)
    {
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference(name);
        table.DeleteIfExists();
    }

    public void insertTableEntityToTable(string tableName, ITableEntity tableEntity)
    {
        // Create the table client.
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        // Create the CloudTable object that represents the "people" table.
        CloudTable table = tableClient.GetTableReference(tableName);

        // Create the TableOperation object that inserts the customer entity.
        TableOperation insertOperation = TableOperation.Insert(tableEntity);

        // Execute the insert operation.
        table.Execute(insertOperation);
    }

    public void insertTableEntityToTableBatchOperation(string tableName, List<ITableEntity> ListTableEntity)
    {
        // Create the table client.
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        // Create the CloudTable object that represents the "people" table.
        CloudTable table = tableClient.GetTableReference(tableName);

        // Create the batch operation.
        TableBatchOperation batchOperation = new TableBatchOperation();

        // Add both customer entities to the batch insert operation.
        foreach (ITableEntity tableEntity in ListTableEntity)
        {
            batchOperation.Insert(tableEntity);
        }
        // Execute the batch operation.
        table.ExecuteBatch(batchOperation);
    }

    public IEnumerable<DynamicTableEntity> getTableEntityFromTable(string tableName)
    {
        // Create the table client.
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        // Create the CloudTable object that represents the "people" table.
        CloudTable table = tableClient.GetTableReference(tableName);

        // Construct the query operation for all customer entities where PartitionKey="Smith".
        //TableQuery query = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));
        TableQuery query = new TableQuery();

        //TableQuery rangeQuery = new TableQuery().Where(TableQuery.CombineFilters(
        //TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"),
        //TableOperators.And,
        //TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "E")));

        // Print the fields for each customer.
        //foreach (CustomerEntity entity in table.ExecuteQuery(query))
        //{
        //    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
        //        entity.Email, entity.PhoneNumber);
        //}


        return table.ExecuteQuery(query).ToList();
    }


    public TableResult retrieveFromTable(string tableName, ITableEntity tableEntity)
    {
        // Create the table client.
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        // Create the CloudTable object that represents the "people" table.
        CloudTable table = tableClient.GetTableReference(tableName);

        // Create a retrieve operation that takes a customer entity.
        TableOperation retrieveOperation = TableOperation.Retrieve("Smith", "Ben");

        // Execute the retrieve operation.
        TableResult retrievedResult = table.Execute(retrieveOperation);


        return (TableResult)retrievedResult.Result;
        //// Print the phone number of the result.
        //if (retrievedResult.Result != null)
        //    Console.WriteLine((retrievedResult.Result));
        //else
        //    Console.WriteLine("The phone number could not be retrieved.");
    }


    public void updateTableEntityToTable(string tableName, ITableEntity tableEntity)
    {
        // Create the table client.
        //CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        //// Create the CloudTable object that represents the "people" table.
        //CloudTable table = tableClient.GetTableReference(tableName);

        //// Create a retrieve operation that takes a customer entity.
        //TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

        //// Execute the operation.
        //TableResult retrievedResult = table.Execute(retrieveOperation);

        //// Assign the result to a CustomerEntity object.
        //CustomerEntity updateEntity = (CustomerEntity)retrievedResult.Result;

        //if (updateEntity != null)
        //{
        //    // Change the phone number.
        //    updateEntity.PhoneNumber = "425-555-0105";

        //    // Create the InsertOrReplace TableOperation.
        //    TableOperation updateOperation = TableOperation.Replace(updateEntity);

        //    // Execute the operation.
        //    table.Execute(updateOperation);

        //    Console.WriteLine("Entity updated.");
        //}

    }


    public void deleteTableEntityToTable(string tableName, ITableEntity tableEntity)
    {
        // Create a retrieve operation that expects a customer entity.
        //TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

        //// Execute the operation.
        //TableResult retrievedResult = table.Execute(retrieveOperation);

        //// Assign the result to a CustomerEntity.
        //CustomerEntity deleteEntity = (CustomerEntity)retrievedResult.Result;

        //// Create the Delete TableOperation.
        //if (deleteEntity != null)
        //{
        //    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

        //    // Execute the operation.
        //    table.Execute(deleteOperation);

        //    Console.WriteLine("Entity deleted.");
        //}

    }
}

