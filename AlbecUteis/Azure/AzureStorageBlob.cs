using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


public class AzureStorageBlob
    {
        CloudStorageAccount storageAccount;

        public AzureStorageBlob()
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
            return "http://teste.blob.core.windows.net/" + container + "/" + path;
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

        public void createContainerToBlob(string name)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(name);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public void deleteContainer(string name)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(name);
            container.DeleteIfExists();
        }


        public void deleteAllBlobsContainer(string nameContainer, string nomeProjeto, string nomePessoa)
        {
            try
            {
                createContainerToBlob(nameContainer);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);

                IEnumerable<IListBlobItem> list = container.ListBlobs(null, true);

                foreach (IListBlobItem item in list.Where(x => x.Uri.ToString().IndexOf(nomeProjeto) > 0 && x.Uri.ToString().IndexOf(nomePessoa) > 0))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        blob.DeleteIfExists();
                    }
                }
            }
            catch (Exception) { }

        }


        public void copyBlobToAnotherContainer(string nameContainer, string blob, Uri blobSource)
        {
            createContainerToBlob(nameContainer);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob);

            blockBlob.StartCopy(blobSource);
        }


        public CloudBlockBlob getBlobReferenceFromContainer(string nameContainer, string blob)
        {
            createContainerToBlob(nameContainer);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob objBlob = (CloudBlockBlob)item;

                    if (objBlob.Name.IndexOf(blob) != -1)
                    {
                        return objBlob;
                    }
                }
            }

            return container.GetBlockBlobReference(blob);
        }



        public void saveStreamToBlob(string nameContainer,
                                     string blob,
                                     Stream stream,
                                     string ContentDisposition,
                                     string ContentType
                                    )
        {
            //Cria container se não existir
            createContainerToBlob(nameContainer);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob);
            blockBlob.Properties.ContentType = ContentType;
            blockBlob.Properties.ContentDisposition = ContentDisposition;

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromStream(stream);
        }


        /// <summary>
        /// Salva arquivo no container e caminho passados, recuperando o arquivo do caminho temporário.
        /// </summary>
        /// <param name="nameContainer">Nome do container Azure Storage</param>
        /// <param name="blob">Nome do arquivo (pode contar barras /)</param>
        /// <param name="pathFile">Caminho (temporário) onde o arquivo está salvo</param>
        public void saveFileToBlob(string nameContainer,
                                   string blob,
                                   string pathFile,
                                   string ContentDisposition,
                                   string ContentType)
        {
            try
            {
                //Cria container se não existir
                createContainerToBlob(nameContainer);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob);
                blockBlob.Properties.ContentType = ContentType;
                blockBlob.Properties.ContentDisposition = ContentDisposition;

                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fileStream = System.IO.File.OpenRead(pathFile))
                {
                    blockBlob.UploadFromStream(fileStream);
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }
        }

        public IEnumerable<IListBlobItem> getBlobFromContainer(string nameContainer)
        {
            createContainerToBlob(nameContainer);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);

            return container.ListBlobs(null, true);

            // Loop over items within the container and output the length and URI.
            //foreach (IListBlobItem item in container.ListBlobs(null, false))
            //{
            //    if (item.GetType() == typeof(CloudBlockBlob))
            //    {
            //        CloudBlockBlob blob = (CloudBlockBlob)item;
            //        Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

            //    }
            //    else if (item.GetType() == typeof(CloudPageBlob))
            //    {
            //        CloudPageBlob pageBlob = (CloudPageBlob)item;

            //        Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

            //    }
            //    else if (item.GetType() == typeof(CloudBlobDirectory))
            //    {
            //        CloudBlobDirectory directory = (CloudBlobDirectory)item;

            //        Console.WriteLine("Directory: {0}", directory.Uri);
            //    }
            //}
        }



        public void downloadToBlob()
        {
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Retrieve reference to a blob named "photo1.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("photo1.jpg");

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(@"path\myfile"))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }


        public void deleteBlob(string nameContainer, string blobWithExtension)
        {
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(nameContainer);

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobWithExtension);

            // Delete the blob.
            blockBlob.Delete();
        }
    }
