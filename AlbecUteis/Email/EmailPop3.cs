using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenPop.Common.Logging;
using OpenPop.Mime;
using OpenPop.Mime.Decode;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using System.Web;

public class EmailPop3
{

    /// <summary>
    /// Example showing:
    ///  - how to fetch all messages from a POP3 server
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <returns>All Messages on the POP3 server</returns>
    public static List<msg_message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password, int count)
    {
        // The client disconnects from the server when being disposed
        using (Pop3Client client = new Pop3Client())
        {
            // Connect to the server
            client.Connect(hostname, port, useSsl);

            // Authenticate ourselves towards the server
            client.Authenticate(username, password);

            // Get the number of messages in the inbox
            int messageCount = client.GetMessageCount();

            // We want to download all messages
            //List<Message> allMessages = new List<Message>(messageCount);

            // Messages are numbered in the interval: [1, messageCount]
            // Ergo: message numbers are 1-based.
            // Most servers give the latest message the highest number
            messageCount = count != 0 ? count : messageCount;

            List<msg_message> list = new List<msg_message>(messageCount);

            for (int i = messageCount; i > 0; i--)
            {

                msg_message item = new msg_message();

                Message msg = client.GetMessage(i);

                item.msg_content_description = msg.Headers.ContentDescription;
                item.msg_content_id = msg.Headers.ContentId;
                item.msg_date = msg.Headers.Date;
                item.msg_date_sent = DateTimeUTC.Convert(msg.Headers.DateSent);
                item.msg_disposition_notification_to = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.DispositionNotificationTo);
                item.msg_from_address = msg.Headers.From.Address.ToString();
                item.msg_from_display = msg.Headers.From.DisplayName.ToString();
                item.msg_importance = msg.Headers.Importance.ToString();
                item.msg_keyword = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Keywords);
                item.msg_message_id = msg.Headers.MessageId;
                item.msg_mime_version = msg.Headers.MimeVersion;
                item.msg_received = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Received);
                item.msg_reply_to = msg.Headers.ReplyTo.ToString();
                item.msg_return_path = msg.Headers.ReturnPath.ToString();
                item.msg_subject = msg.Headers.Subject;
                item.msg_to = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.To);
                item.msg_bcc = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Bcc);
                item.msg_bcc = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Cc);
                list.Add(item);

            }

            // Now return the fetched messages
            return list;
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to use UID's (unique ID's) of messages from the POP3 server
    ///  - how to download messages not seen before
    ///    (notice that the POP3 protocol cannot see if a message has been read on the server
    ///     before. Therefore the client need to maintain this state for itself)
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <param name="seenUids">
    /// List of UID's of all messages seen before.
    /// New message UID's will be added to the list.
    /// Consider using a HashSet if you are using >= 3.5 .NET
    /// </param>
    /// <returns>A List of new Messages on the server</returns>
    public static List<msg_message> FetchUnDownloadMessages(string hostname, int port, bool useSsl, string username, string password, List<string> seenUids, int countRetreive, string nomePessoa, string nomeProjeto)
    {
        int c = 0;
        // The client disconnects from the server when being disposed
        using (Pop3Client client = new Pop3Client())
        {
            // Connect to the server
            client.Connect(hostname, port, useSsl);

            // Authenticate ourselves towards the server
            client.Authenticate(username, password);

            // Fetch all the current uids seen
            List<string> uids = client.GetMessageUids();

            // Create a list we can return with all new messages
            //List<Message> newMessages = new List<Message>();

            List<msg_message> list = new List<msg_message>();


            // All the new messages not seen by the POP3 client
            // for (int i = 0; i < uids.Count; i++)
            for (int i = uids.Count - 1; i >= 0 && c < countRetreive; i--, c++)
            {

                string currentUidOnServer = uids[i];
                if (!seenUids.Contains(currentUidOnServer))
                {
                    // We have not seen this message before.
                    // Download it and add this new uid to seen uids

                    // the uids list is in messageNumber order - meaning that the first
                    // uid in the list has messageNumber of 1, and the second has 
                    // messageNumber 2. Therefore we can fetch the message using
                    // i + 1 since messageNumber should be in range [1, messageCount]
                    //Message unseenMessage = client.GetMessage(i + 1);

                    // Add the message to the new messages
                    //newMessages.Add(unseenMessage);

                    msg_message item = new msg_message();

                    Message msg = client.GetMessage(i + 1);

                    item.msg_unique_id = uids[i].ToString();
                    item.msg_content_description = msg.Headers.ContentDescription;
                    item.msg_content_id = msg.Headers.ContentId;
                    item.msg_date = msg.Headers.Date;
                    item.msg_date_sent = DateTimeUTC.Convert(msg.Headers.DateSent);
                    item.msg_disposition_notification_to = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.DispositionNotificationTo);
                    item.msg_from_address = msg.Headers.From.Address.ToString();
                    item.msg_from_display = msg.Headers.From.DisplayName.ToString();
                    item.msg_importance = msg.Headers.Importance.ToString();
                    item.msg_keyword = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Keywords);
                    item.msg_message_id = msg.Headers.MessageId;
                    item.msg_mime_version = msg.Headers.MimeVersion;
                    item.msg_received = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Received);
                    item.msg_reply_to = "";
                    item.msg_return_path = "";
                    item.msg_subject = msg.Headers.Subject;
                    item.msg_to = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.To);
                    item.msg_bcc = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Bcc);
                    item.msg_bcc = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.Cc);
                    item.msg_was_read = false;
                    item.msg_status = "Entrada";
                    item.msg_account_email = username;


                    MessagePart body = msg.FindFirstHtmlVersion(); //html
                    if (body != null)
                    {
                        item.msg_bodyHtml = body.GetBodyAsText();
                    }
                    else
                    {
                        body = msg.FindFirstPlainTextVersion();
                        if (body != null)
                        {
                            // The message had a text/plain version - show that one
                            item.msg_bodyHtml = body.GetBodyAsText();// text
                        }
                        else
                        {
                            // Try to find a body to show in some of the other text versions
                            List<MessagePart> bodyVersions = msg.FindAllTextVersions();
                            if (bodyVersions.Count >= 1)
                                item.msg_bodyHtml = bodyVersions[0].GetBodyAsText();
                            else
                                item.msg_bodyHtml = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
                        }
                    }

                    List<MessagePart> attachments = msg.FindAllAttachments();

                    foreach (MessagePart attachment in attachments)
                    {

                        item.msg_attachments_file_name += attachment.FileName + "|";
                        item.msg_attachments_media_type += attachment.ContentType.MediaType + "|";

                        double size = 1200000 / (double)1024;

                        if (size > 1050)
                        {
                            size = size / (double)1024;
                            item.msg_attachments_type_data += "MB" + "|";
                        }
                        else
                        {
                            item.msg_attachments_type_data += "KB" + "|";
                        }


                        item.msg_attachments_size += size.ToString("N3") + "|";


                        //item.msg_attachments_path = Path.Combine(@"\Email\Anexos\", attachment.FileName);


                        //item.msg_attachments_body = attachment.Body;

                        //string filePath = Path.Combine(HttpContext.Current.Server.MapPath(@"\Email\Anexos\"), attachment.FileName);


                        //Cria diretorio para Usuario logado
                        DirectoryInfo di = Directory.CreateDirectory(
                                                     Path.Combine(
                                                                    HttpContext.Current.Server.MapPath(@"\Email\Anexos\"),
                                                                    nomeProjeto + "\\" + nomePessoa
                                                                  )
                                                                     );

                        string filePath = Path.Combine(di.FullName + "\\", attachment.FileName);


                        //Caminho salva no banco
                        item.msg_attachments_path += Path.Combine(Path.Combine(@"\Email\Anexos\", nomeProjeto + "\\" + nomePessoa), attachment.FileName) + "|";

                        //Salva anexo na pasta
                        FileStream Stream = new FileStream(filePath, FileMode.Create);
                        BinaryWriter BinaryStream = new BinaryWriter(Stream);
                        BinaryStream.Write(attachment.Body);
                        BinaryStream.Close();
                    }

                    //int t1 = item.msg_attachments_file_name.Length;
                    //int t2 = item.msg_attachments_media_type.Length;
                    //int t3 = item.msg_attachments_path.Length;


                    //item.msg_attachments_file_name.Substring(0, t1 - 1);
                    //item.msg_attachments_media_type.Substring(0, t2 - 1);
                    //item.msg_attachments_path.Substring(0, t3 - 1);


                    //item.msg_bodyHtml = msg.MessagePart.GetBodyAsText();
                    list.Add(item);

                    // Add the uid to the seen uids, as it has now been seen
                    seenUids.Add(currentUidOnServer);

                }
            }

            // Return our new found messages
            return list;
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to use UID's (unique ID's) of messages from the POP3 server
    ///  - how to download messages not seen before
    ///    (notice that the POP3 protocol cannot see if a message has been read on the server
    ///     before. Therefore the client need to maintain this state for itself)
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <param name="seenUids">
    /// List of UID's of all messages seen before.
    /// New message UID's will be added to the list.
    /// Consider using a HashSet if you are using >= 3.5 .NET
    /// </param>
    /// <returns>A List of new Messages on the server</returns>

    public static List<msg_message> FetchUnDownloadMessagesCheckingHeaderBlob(string hostname, int port, bool useSsl, string username, string password, List<string> seenUids, string nomePessoa, string nomeProjeto, DateTime dataCadastro, int countRetreive)
    {
        try
        {
            if (hostname != "")
            {
                // string a = Anexo.nomePessoa();
                string container = "anexosemails";

                int c = 0;
                // The client disconnects from the server when being disposed
                using (Pop3Client client = new Pop3Client())
                {
                    // Connect to the server
                    client.Connect(hostname, port, useSsl);

                    // Authenticate ourselves towards the server
                    client.Authenticate(username, password);

                    // Fetch all the current uids seen
                    List<string> uids = client.GetMessageUids();

                    // Create a list we can return with all new messages
                    //List<Message> newMessages = new List<Message>();

                    List<msg_message> list = new List<msg_message>();


                    // All the new messages not seen by the POP3 client
                    // for (int i = 0; i < uids.Count; i++)
                    for (int i = uids.Count - 1; i >= 0 && c < countRetreive; i--, c++)
                    {

                        string currentUidOnServer = uids[i];
                        if (!seenUids.Contains(currentUidOnServer))
                        {

                            MessageHeader headers = client.GetMessageHeaders(i + 1);

                            RfcMailAddress from = headers.From;
                            string contaEmail = username;
                            string subject = headers.Subject;
                            DateTime dataEmail = DateTimeUTC.Convert(headers.DateSent);

                            // Only want to download message if:
                            //  - is from test@xample.com
                            //  - has subject "Some subject"

                            Console.WriteLine("Data email > " + dataEmail.ToString());
                            Console.WriteLine("Data cadastro > " + dataCadastro);

                            if (from.HasValidMailAddress && dataEmail > dataCadastro)
                            {
                                string timestamp = DateTimeUTC.Now().ToString("yyyyMMddHHmmssfff");

                                // Download the full message

                                // We have not seen this message before.
                                // Download it and add this new uid to seen uids

                                // the uids list is in messageNumber order - meaning that the first
                                // uid in the list has messageNumber of 1, and the second has 
                                // messageNumber 2. Therefore we can fetch the message using
                                // i + 1 since messageNumber should be in range [1, messageCount]
                                //Message unseenMessage = client.GetMessage(i + 1);

                                // Add the message to the new messages
                                //newMessages.Add(unseenMessage);

                                msg_message item = new msg_message();

                                Message msg = client.GetMessage(i + 1);

                                item.msg_unique_id = uids[i].ToString();
                                item.msg_content_description = msg.Headers.ContentDescription;
                                item.msg_content_id = msg.Headers.ContentId;
                                item.msg_date = msg.Headers.Date;
                                item.msg_date_sent = DateTimeUTC.Convert(msg.Headers.DateSent);
                                item.msg_from_address = msg.Headers.From.Address.ToString();
                                item.msg_from_display = msg.Headers.From.DisplayName.ToString();
                                item.msg_importance = msg.Headers.Importance.ToString();
                                item.msg_message_id = msg.Headers.MessageId;
                                item.msg_mime_version = msg.Headers.MimeVersion;
                                item.msg_return_path = "";
                                item.msg_subject = msg.Headers.Subject;

                                if (msg.Headers.ReplyTo != null)
                                    item.msg_reply_to = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.ReplyTo);

                                foreach (var a in msg.Headers.DispositionNotificationTo) { item.msg_disposition_notification_to += a + "|"; }
                                foreach (var a in msg.Headers.Keywords) { item.msg_keyword += a + "|"; }
                                foreach (var a in msg.Headers.Received) { item.msg_received += a + "|"; }
                                foreach (var a in msg.Headers.To) { item.msg_to += a + "|"; }
                                foreach (var a in msg.Headers.Bcc) { item.msg_bcc += a + "|"; }
                                foreach (var a in msg.Headers.Cc) { item.msg_cc += a + "|"; }

                                item.msg_was_read = false;
                                item.msg_status = "Entrada";
                                item.msg_account_email = username;

                                MessagePart body = msg.FindFirstHtmlVersion(); //html
                                if (body != null)
                                {
                                    item.msg_bodyHtml = body.GetBodyAsText();
                                }
                                else
                                {
                                    body = msg.FindFirstPlainTextVersion();
                                    if (body != null)
                                    {
                                        // The message had a text/plain version - show that one
                                        item.msg_bodyHtml = body.GetBodyAsText();// text
                                    }
                                    else
                                    {
                                        // Try to find a body to show in some of the other text versions
                                        List<MessagePart> bodyVersions = msg.FindAllTextVersions();
                                        if (bodyVersions.Count >= 1)
                                            item.msg_bodyHtml = bodyVersions[0].GetBodyAsText();
                                        else
                                            item.msg_bodyHtml = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
                                    }
                                }

                                //Tenta recuperar o Text
                                body = msg.FindFirstPlainTextVersion();
                                if (body != null)
                                {
                                    // The message had a text/plain version - show that one
                                    item.msg_bodyText = body.GetBodyAsText();// text
                                }
                                else
                                {
                                    // Try to find a body to show in some of the other text versions
                                    List<MessagePart> bodyVersions = msg.FindAllTextVersions();
                                    if (bodyVersions.Count >= 1)
                                        item.msg_bodyText = bodyVersions[0].GetBodyAsText();
                                    else
                                        item.msg_bodyText = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
                                }


                                List<MessagePart> attachments = msg.FindAllAttachments();

                                //*******************************
                                //Inline
                                //******************************

                                foreach (MessagePart attachment in attachments)
                                {

                                    //*******************************
                                    //Inline - gmail ou Bol
                                    //******************************
                                    if (attachment.ContentDisposition.Inline)
                                    {

                                        item.msg_attachments_file_name += "inline@j34kdla@09838Dhakded" + "|";
                                        item.msg_attachments_media_type += Arquivo.GetMIMEType(attachment.FileName) + "|";//attachment.ContentType.MediaType
                                        item.msg_attachments_type_data += Arquivo.Tipo(attachment.Body.Length) + "|";
                                        item.msg_attachments_size += Arquivo.Tamanho(attachment.Body.Length) + "|";

                                        AzureStorageBlob b = new AzureStorageBlob();

                                        string caminho = nomeProjeto + "/" + nomePessoa + "/" + contaEmail + "/" + timestamp + "/" + Path.GetFileName(attachment.FileName);
                                        string caminhoAnexo = b.returnPathForBlob(container, caminho + "|");
                                        string caminhoInline = b.returnPathForBlob(container, caminho);

                                        item.msg_attachments_path += caminhoAnexo;

                                        //Salva anexo Storage Azure
                                        Stream stream = new MemoryStream(attachment.Body);
                                        b.saveStreamToBlob(container, caminho, stream, attachment.ContentDisposition.DispositionType, Arquivo.GetMIMEType(attachment.FileName));

                                        //Coloca as imagens no corpo html
                                        item.msg_bodyHtml = item.msg_bodyHtml.Replace("cid:", "").Replace(attachment.ContentId, caminhoInline);
                                    }

                                    //*******************************
                                    //Inline - Hotmail
                                    //******************************

                                    else if (attachment.ContentId != null)
                                    {
                                        if (attachment.ContentId.IndexOf("inline") != -1)
                                        {
                                            item.msg_attachments_file_name += "inline@j34kdla@09838Dhakded" + "|";
                                            item.msg_attachments_media_type += Arquivo.GetMIMEType(attachment.FileName) + "|";//attachment.ContentType.MediaType
                                            item.msg_attachments_type_data += Arquivo.Tipo(attachment.Body.Length) + "|";
                                            item.msg_attachments_size += Arquivo.Tamanho(attachment.Body.Length) + "|";

                                            AzureStorageBlob b = new AzureStorageBlob();

                                            string caminho = nomeProjeto + "/" + nomePessoa + "/" + contaEmail + "/" + timestamp + "/" + Path.GetFileName(attachment.FileName);
                                            string caminhoAnexo = b.returnPathForBlob(container, caminho + "|");
                                            string caminhoInline = b.returnPathForBlob(container, caminho);

                                            item.msg_attachments_path += caminhoAnexo;

                                            //Salva anexo Storage Azure
                                            Stream stream = new MemoryStream(attachment.Body);
                                            b.saveStreamToBlob(container, caminho, stream, attachment.ContentDisposition.DispositionType, Arquivo.GetMIMEType(attachment.FileName));

                                            //Coloca as imagens no corpo html
                                            item.msg_bodyHtml = item.msg_bodyHtml.Replace("cid:", "").Replace(attachment.ContentId, caminhoInline);
                                        }

                                        //*******************************
                                        //Inline - Yahoo
                                        //******************************
                                        else
                                        {
                                            item.msg_attachments_file_name += "inline@j34kdla@09838Dhakded" + "|";
                                            item.msg_attachments_media_type += Arquivo.GetMIMEType(attachment.FileName) + "|";//attachment.ContentType.MediaType
                                            item.msg_attachments_type_data += Arquivo.Tipo(attachment.Body.Length) + "|";
                                            item.msg_attachments_size += Arquivo.Tamanho(attachment.Body.Length) + "|";

                                            AzureStorageBlob b = new AzureStorageBlob();

                                            string caminho = nomeProjeto + "/" + nomePessoa + "/" + contaEmail + "/" + timestamp + "/" + Path.GetFileName(attachment.FileName);
                                            string caminhoAnexo = b.returnPathForBlob(container, caminho + "|");
                                            string caminhoInline = b.returnPathForBlob(container, caminho);

                                            item.msg_attachments_path += caminhoAnexo;

                                            //Salva anexo Storage Azure
                                            Stream stream = new MemoryStream(attachment.Body);
                                            b.saveStreamToBlob(container, caminho, stream, attachment.ContentDisposition.DispositionType, Arquivo.GetMIMEType(attachment.FileName));

                                            //Coloca as imagens no corpo html
                                            item.msg_bodyHtml = item.msg_bodyHtml.Replace("cid:", "").Replace(attachment.ContentId, caminhoInline);

                                        }
                                    }
                                    //Anexo comum
                                    else
                                    {
                                        item.msg_attachments_file_name += Path.GetFileName(attachment.FileName) + "|";
                                        item.msg_attachments_media_type += Arquivo.GetMIMEType(attachment.FileName) + "|";//attachment.ContentType.MediaType
                                        item.msg_attachments_type_data += Arquivo.Tipo(attachment.Body.Length) + "|";
                                        item.msg_attachments_size += Arquivo.Tamanho(attachment.Body.Length) + "|";

                                        AzureStorageBlob b = new AzureStorageBlob();

                                        string caminho = nomeProjeto + "/" + nomePessoa + "/" + contaEmail + "/" + timestamp + "/" + Path.GetFileName(attachment.FileName);
                                        string caminhoAnexo = b.returnPathForBlob(container, caminho + "|");

                                        item.msg_attachments_path += caminhoAnexo;

                                        //Salva anexo Storage Azure
                                        Stream stream = new MemoryStream(attachment.Body);
                                        b.saveStreamToBlob(container, caminho, stream, attachment.ContentDisposition.DispositionType, Arquivo.GetMIMEType(attachment.FileName));
                                    }

                                }

                                //item.msg_bodyHtml = msg.MessagePart.GetBodyAsText();
                                list.Add(item);

                                // Add the uid to the seen uids, as it has now been seen
                                seenUids.Add(currentUidOnServer);
                            }
                        }
                    }

                    // Return our new found messages
                    return list;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return new List<msg_message>();
    }

    /// <summary>
    /// Example showing:
    ///  - how to use UID's (unique ID's) of messages from the POP3 server
    ///  - how to download messages not seen before
    ///    (notice that the POP3 protocol cannot see if a message has been read on the server
    ///     before. Therefore the client need to maintain this state for itself)
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <param name="seenUids">
    /// List of UID's of all messages seen before.
    /// New message UID's will be added to the list.
    /// Consider using a HashSet if you are using >= 3.5 .NET
    /// </param>
    /// <returns>A List of new Messages on the server</returns>

    public static List<msg_message> FetchUnDownloadMessagesCheckingHeader(string hostname, int port, bool useSsl, string username, string password, List<string> seenUids, DateTime dataCadastro, int countRetreive, string nomePessoa, string nomeProjeto)
    {
        try
        {

            if (hostname != "")
            {

                int c = 0;
                // The client disconnects from the server when being disposed
                using (Pop3Client client = new Pop3Client())
                {
                    // Connect to the server
                    client.Connect(hostname, port, useSsl);

                    // Authenticate ourselves towards the server
                    client.Authenticate(username, password);

                    // Fetch all the current uids seen
                    List<string> uids = client.GetMessageUids();

                    // Create a list we can return with all new messages
                    //List<Message> newMessages = new List<Message>();

                    List<msg_message> list = new List<msg_message>();


                    // All the new messages not seen by the POP3 client
                    // for (int i = 0; i < uids.Count; i++)
                    for (int i = uids.Count - 1; i >= 0 && c < countRetreive; i--, c++)
                    {

                        string currentUidOnServer = uids[i];
                        if (!seenUids.Contains(currentUidOnServer))
                        {

                            MessageHeader headers = client.GetMessageHeaders(i + 1);

                            RfcMailAddress from = headers.From;
                            string contaEmail = username;
                            string subject = headers.Subject;
                            DateTime dataEmail = headers.DateSent;

                            // Only want to download message if:
                            //  - is from test@xample.com
                            //  - has subject "Some subject"
                            if (from.HasValidMailAddress && dataEmail.Date > dataCadastro)
                            {
                                string timestamp = DateTimeUTC.Now().ToString("yyyyMMddHHmmssfff");

                                // Download the full message

                                // We have not seen this message before.
                                // Download it and add this new uid to seen uids

                                // the uids list is in messageNumber order - meaning that the first
                                // uid in the list has messageNumber of 1, and the second has 
                                // messageNumber 2. Therefore we can fetch the message using
                                // i + 1 since messageNumber should be in range [1, messageCount]
                                //Message unseenMessage = client.GetMessage(i + 1);

                                // Add the message to the new messages
                                //newMessages.Add(unseenMessage);

                                msg_message item = new msg_message();

                                Message msg = client.GetMessage(i + 1);

                                item.msg_unique_id = uids[i].ToString();
                                item.msg_content_description = msg.Headers.ContentDescription;
                                item.msg_content_id = msg.Headers.ContentId;
                                item.msg_date = msg.Headers.Date;
                                item.msg_date_sent = DateTimeUTC.Convert(msg.Headers.DateSent);
                                item.msg_from_address = msg.Headers.From.Address.ToString();
                                item.msg_from_display = msg.Headers.From.DisplayName.ToString();
                                item.msg_importance = msg.Headers.Importance.ToString();
                                item.msg_message_id = msg.Headers.MessageId;
                                item.msg_mime_version = msg.Headers.MimeVersion;
                                item.msg_return_path = "";
                                item.msg_subject = msg.Headers.Subject;

                                if (msg.Headers.ReplyTo != null)
                                    item.msg_reply_to = Newtonsoft.Json.JsonConvert.SerializeObject(msg.Headers.ReplyTo);

                                foreach (var a in msg.Headers.DispositionNotificationTo) { item.msg_disposition_notification_to += a + "|"; }
                                foreach (var a in msg.Headers.Keywords) { item.msg_keyword += a + "|"; }
                                foreach (var a in msg.Headers.Received) { item.msg_received += a + "|"; }
                                foreach (var a in msg.Headers.To) { item.msg_to += a + "|"; }
                                foreach (var a in msg.Headers.Bcc) { item.msg_bcc += a + "|"; }
                                foreach (var a in msg.Headers.Cc) { item.msg_cc += a + "|"; }

                                item.msg_was_read = false;
                                item.msg_status = "Entrada";
                                item.msg_account_email = username;

                                MessagePart body = msg.FindFirstHtmlVersion(); //html
                                if (body != null)
                                {
                                    item.msg_bodyHtml = body.GetBodyAsText();
                                }
                                else
                                {
                                    body = msg.FindFirstPlainTextVersion();
                                    if (body != null)
                                    {
                                        // The message had a text/plain version - show that one
                                        item.msg_bodyHtml = body.GetBodyAsText();// text
                                    }
                                    else
                                    {
                                        // Try to find a body to show in some of the other text versions
                                        List<MessagePart> bodyVersions = msg.FindAllTextVersions();
                                        if (bodyVersions.Count >= 1)
                                            item.msg_bodyHtml = bodyVersions[0].GetBodyAsText();
                                        else
                                            item.msg_bodyHtml = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
                                    }
                                }



                                //Tenta recuperar o Text
                                body = msg.FindFirstPlainTextVersion();
                                if (body != null)
                                {
                                    // The message had a text/plain version - show that one
                                    item.msg_bodyText = body.GetBodyAsText();// text
                                }
                                else
                                {
                                    // Try to find a body to show in some of the other text versions
                                    List<MessagePart> bodyVersions = msg.FindAllTextVersions();
                                    if (bodyVersions.Count >= 1)
                                        item.msg_bodyText = bodyVersions[0].GetBodyAsText();
                                    else
                                        item.msg_bodyText = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
                                }




                                List<MessagePart> attachments = msg.FindAllAttachments();



                                foreach (MessagePart attachment in attachments)
                                {
                                    //Inline - gmail ou Bol
                                    if (attachment.ContentDisposition.Inline)
                                    {

                                        item.msg_attachments_file_name += "inline@j34kdla@09838Dhakded" + "|";
                                        item.msg_attachments_media_type += attachment.ContentType.MediaType + "|";


                                        //Tipo de dado
                                        double size = attachment.Body.Length / (double)1024;

                                        //Recupero em MB
                                        if (size > 1050)
                                        {
                                            size = size / (double)1024;
                                            item.msg_attachments_type_data += "MB" + "|";

                                            //Tamnho
                                            item.msg_attachments_size += size.ToString("N2") + "|";

                                        }
                                        //Recupero em KB
                                        else
                                        {
                                            if (size > 1)
                                            {
                                                item.msg_attachments_type_data += "KB" + "|";

                                                //Tamnho
                                                item.msg_attachments_size += size.ToString("N3") + "|";
                                            }
                                            else
                                            {

                                                item.msg_attachments_type_data += "Bytes" + "|";

                                                //Tamnho
                                                item.msg_attachments_size += attachment.Body.Length.ToString() + "|";

                                            }
                                        }





                                        //item.msg_attachments_path = Path.Combine(@"\Email\Anexos\", attachment.FileName);


                                        //item.msg_attachments_body = attachment.Body;

                                        //string filePath = Path.Combine(HttpContext.Current.Server.MapPath(@"\Email\Anexos\"), attachment.FileName);



                                        //Cria diretorio para Usuario logado
                                        DirectoryInfo di = Directory.CreateDirectory(
                                                                     Path.Combine(
                                                                                    HttpContext.Current.Server.MapPath(@"\Email\Anexos\"),
                                                                                    Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp
                                                                                  )
                                                                                     );


                                        //string nomeArquivo = "";

                                        //if(attachment.FileName == "(no name)")
                                        //    nomeArquivo = "sem nome"



                                        string filePath = Path.Combine(di.FullName + "\\", attachment.FileName);


                                        //Caminho salva no banco
                                        item.msg_attachments_path += Path.Combine(Path.Combine(@"\Email\Anexos\", Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp), attachment.FileName) + "|";

                                        string caminhoAttachmentInline = Path.Combine(Path.Combine(@"\Email\Anexos\", Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp), attachment.FileName);


                                        //Salva anexo na pasta
                                        FileStream Stream = new FileStream(filePath, FileMode.Create);
                                        BinaryWriter BinaryStream = new BinaryWriter(Stream);
                                        BinaryStream.Write(attachment.Body);
                                        BinaryStream.Close();


                                        //Coloca as imagens no corpo html
                                        item.msg_bodyHtml = item.msg_bodyHtml.Replace("cid:", "").Replace(attachment.ContentId, caminhoAttachmentInline);

                                    }
                                    else if (attachment.ContentId != null) //Hotmail ou Yahoo
                                    {
                                        //Hotmail
                                        if (attachment.ContentId.IndexOf("inline") != -1)
                                        {
                                            item.msg_attachments_file_name += "inline@j34kdla@09838Dhakded" + "|";
                                            item.msg_attachments_media_type += attachment.ContentType.MediaType + "|";


                                            //Tipo de dado
                                            double size = attachment.Body.Length / (double)1024;

                                            //Recupero em MB
                                            if (size > 1050)
                                            {
                                                size = size / (double)1024;
                                                item.msg_attachments_type_data += "MB" + "|";

                                                //Tamnho
                                                item.msg_attachments_size += size.ToString("N2") + "|";

                                            }
                                            //Recupero em KB
                                            else
                                            {
                                                if (size > 1)
                                                {
                                                    item.msg_attachments_type_data += "KB" + "|";

                                                    //Tamnho
                                                    item.msg_attachments_size += size.ToString("N3") + "|";
                                                }
                                                else
                                                {

                                                    item.msg_attachments_type_data += "Bytes" + "|";

                                                    //Tamnho
                                                    item.msg_attachments_size += attachment.Body.Length.ToString() + "|";

                                                }
                                            }





                                            //item.msg_attachments_path = Path.Combine(@"\Email\Anexos\", attachment.FileName);


                                            //item.msg_attachments_body = attachment.Body;

                                            //string filePath = Path.Combine(HttpContext.Current.Server.MapPath(@"\Email\Anexos\"), attachment.FileName);



                                            //Cria diretorio para Usuario logado
                                            DirectoryInfo di = Directory.CreateDirectory(
                                                                         Path.Combine(
                                                                                        HttpContext.Current.Server.MapPath(@"\Email\Anexos\"),
                                                                                        Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp
                                                                                      )
                                                                                         );


                                            //string nomeArquivo = "";

                                            //if(attachment.FileName == "(no name)")
                                            //    nomeArquivo = "sem nome"



                                            string filePath = Path.Combine(di.FullName + "\\", attachment.FileName);


                                            //Caminho salva no banco
                                            item.msg_attachments_path += Path.Combine(Path.Combine(@"\Email\Anexos\", Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp), attachment.FileName) + "|";

                                            string caminhoAttachmentInline = Path.Combine(Path.Combine(@"\Email\Anexos\", Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp), attachment.FileName);


                                            //Salva anexo na pasta
                                            FileStream Stream = new FileStream(filePath, FileMode.Create);
                                            BinaryWriter BinaryStream = new BinaryWriter(Stream);
                                            BinaryStream.Write(attachment.Body);
                                            BinaryStream.Close();


                                            //Coloca as imagens no corpo html
                                            item.msg_bodyHtml = item.msg_bodyHtml.Replace("cid:", "").Replace(attachment.ContentId, caminhoAttachmentInline);
                                        }
                                        //Yahoo
                                        else
                                        {
                                            item.msg_attachments_file_name += attachment.FileName + "|";
                                            item.msg_attachments_media_type += attachment.ContentType.MediaType + "|";


                                            //Tipo de dado
                                            double size = attachment.Body.Length / (double)1024;

                                            //Recupero em MB
                                            if (size > 1050)
                                            {
                                                size = size / (double)1024;
                                                item.msg_attachments_type_data += "MB" + "|";

                                                //Tamnho
                                                item.msg_attachments_size += size.ToString("N2") + "|";

                                            }
                                            //Recupero em KB
                                            else
                                            {
                                                if (size > 1)
                                                {
                                                    item.msg_attachments_type_data += "KB" + "|";

                                                    //Tamnho
                                                    item.msg_attachments_size += size.ToString("N3") + "|";
                                                }
                                                else
                                                {

                                                    item.msg_attachments_type_data += "Bytes" + "|";

                                                    //Tamnho
                                                    item.msg_attachments_size += attachment.Body.Length.ToString() + "|";

                                                }
                                            }





                                            //item.msg_attachments_path = Path.Combine(@"\Email\Anexos\", attachment.FileName);


                                            //item.msg_attachments_body = attachment.Body;

                                            //string filePath = Path.Combine(HttpContext.Current.Server.MapPath(@"\Email\Anexos\"), attachment.FileName);



                                            //Cria diretorio para Usuario logado
                                            DirectoryInfo di = Directory.CreateDirectory(
                                                                         Path.Combine(
                                                                                        HttpContext.Current.Server.MapPath(@"\Email\Anexos\"),
                                                                                        Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp
                                                                                      )
                                                                                         );


                                            //string nomeArquivo = "";

                                            //if(attachment.FileName == "(no name)")
                                            //    nomeArquivo = "sem nome"



                                            string filePath = Path.Combine(di.FullName + "\\", attachment.FileName);


                                            //Caminho salva no banco
                                            item.msg_attachments_path += Path.Combine(Path.Combine(@"\Email\Anexos\", Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp), attachment.FileName) + "|";

                                            //Salva anexo na pasta
                                            FileStream Stream = new FileStream(filePath, FileMode.Create);
                                            BinaryWriter BinaryStream = new BinaryWriter(Stream);
                                            BinaryStream.Write(attachment.Body);
                                            BinaryStream.Close();
                                        }
                                    }
                                    //Anexo 
                                    else
                                    {
                                        item.msg_attachments_file_name += attachment.FileName + "|";
                                        item.msg_attachments_media_type += attachment.ContentType.MediaType + "|";


                                        //Tipo de dado
                                        double size = attachment.Body.Length / (double)1024;

                                        //Recupero em MB
                                        if (size > 1050)
                                        {
                                            size = size / (double)1024;
                                            item.msg_attachments_type_data += "MB" + "|";

                                            //Tamnho
                                            item.msg_attachments_size += size.ToString("N2") + "|";

                                        }
                                        //Recupero em KB
                                        else
                                        {
                                            if (size > 1)
                                            {
                                                item.msg_attachments_type_data += "KB" + "|";

                                                //Tamnho
                                                item.msg_attachments_size += size.ToString("N3") + "|";
                                            }
                                            else
                                            {

                                                item.msg_attachments_type_data += "Bytes" + "|";

                                                //Tamnho
                                                item.msg_attachments_size += attachment.Body.Length.ToString() + "|";

                                            }
                                        }





                                        //item.msg_attachments_path = Path.Combine(@"\Email\Anexos\", attachment.FileName);


                                        //item.msg_attachments_body = attachment.Body;

                                        //string filePath = Path.Combine(HttpContext.Current.Server.MapPath(@"\Email\Anexos\"), attachment.FileName);



                                        //Cria diretorio para Usuario logado
                                        DirectoryInfo di = Directory.CreateDirectory(
                                                                     Path.Combine(
                                                                                    HttpContext.Current.Server.MapPath(@"\Email\Anexos\"),
                                                                                    Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp
                                                                                  )
                                                                                     );


                                        //string nomeArquivo = "";

                                        //if(attachment.FileName == "(no name)")
                                        //    nomeArquivo = "sem nome"



                                        string filePath = Path.Combine(di.FullName + "\\", attachment.FileName);


                                        //Caminho salva no banco
                                        item.msg_attachments_path += Path.Combine(Path.Combine(@"\Email\Anexos\", Remover.Acentos(nomeProjeto) + "\\" + Remover.Acentos(nomePessoa) + "\\" + contaEmail + "\\" + timestamp), attachment.FileName) + "|";

                                        //Salva anexo na pasta
                                        FileStream Stream = new FileStream(filePath, FileMode.Create);
                                        BinaryWriter BinaryStream = new BinaryWriter(Stream);
                                        BinaryStream.Write(attachment.Body);
                                        BinaryStream.Close();
                                    }

                                }





                                //int t1 = item.msg_attachments_file_name.Length;
                                //int t2 = item.msg_attachments_media_type.Length;
                                //int t3 = item.msg_attachments_path.Length;


                                //item.msg_attachments_file_name.Substring(0, t1 - 1);
                                //item.msg_attachments_media_type.Substring(0, t2 - 1);
                                //item.msg_attachments_path.Substring(0, t3 - 1);


                                //item.msg_bodyHtml = msg.MessagePart.GetBodyAsText();
                                list.Add(item);

                                // Add the uid to the seen uids, as it has now been seen
                                seenUids.Add(currentUidOnServer);

                            }
                        }
                    }

                    // Return our new found messages
                    return list;
                }
            }
        }
        catch (Exception)
        {
            //
        }

        return new List<msg_message>();
    }

    //public static string Remover.Acentos(string texto)
    //{
    //    if (string.IsNullOrEmpty(texto))
    //        return String.Empty;
    //    else
    //    {
    //        byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(texto);
    //        return System.Text.Encoding.UTF8.GetString(bytes);
    //    }
    //}

    /// <summary>
    /// Example showing:
    ///  - how to fetch only headers from a POP3 server
    ///  - how to examine some of the headers
    ///  - how to fetch a full message
    ///  - how to find a specific attachment and save it to a file
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <param name="messageNumber">
    /// The number of the message to examine.
    /// Must be in range [1, messageCount] where messageCount is the number of messages on the server.
    /// </param>
    public void HeadersFromAndSubject(string hostname, int port, bool useSsl, string username, string password, int messageNumber)
    {
        // The client disconnects from the server when being disposed
        using (Pop3Client client = new Pop3Client())
        {
            // Connect to the server
            client.Connect(hostname, port, useSsl);

            // Authenticate ourselves towards the server
            client.Authenticate(username, password);

            // We want to check the headers of the message before we download
            // the full message
            MessageHeader headers = client.GetMessageHeaders(messageNumber);

            RfcMailAddress from = headers.From;
            string subject = headers.Subject;

            // Only want to download message if:
            //  - is from test@xample.com
            //  - has subject "Some subject"
            if (from.HasValidMailAddress && from.Address.Equals("test@example.com") && "Some subject".Equals(subject))
            {
                // Download the full message
                Message message = client.GetMessage(messageNumber);

                // We know the message contains an attachment with the name "useful.pdf".
                // We want to save this to a file with the same name
                foreach (MessagePart attachment in message.FindAllAttachments())
                {
                    if (attachment.FileName.Equals("useful.pdf"))
                    {
                        // Save the raw bytes to a file
                        File.WriteAllBytes(attachment.FileName, attachment.Body);
                    }
                }
            }
        }
    }

    //public List<Message> getMessageByMessgeId(string messageId)
    //{
    //    int messageCount = client.GetMessageCount();

    //    List<Message> allMessages = new List<Message>(messageCount);

    //    for (int i = messageCount; i > 0; i--)
    //    {
    //        if (client.GetMessageHeaders(i).MessageId == messageId)
    //        {
    //                allMessages.Add(client.GetMessage(i));
    //                break;
    //        }
    //    }

    //    return allMessages;
    //}


    //public List<Message> findMessageByCountId(int id)
    //{
    //    int messageCount = client.GetMessageCount();

    //    List<Message> allMessages = new List<Message>(messageCount);

    //    allMessages.Add(client.GetMessage(id));

    //    allMessages.Add(client.GetMessage(id));
    //    return allMessages;
    //}



    //public int getMessageCountClient()
    //{
    //   return client.GetMessageCount();
    //}

    /// <summary>
    /// Example showing:
    ///  - how to delete fetch an emails headers only
    ///  - how to delete a message from the server
    /// </summary>
    /// <param name="client">A connected and authenticated Pop3Client from which to delete a message</param>
    /// <param name="messageId">A message ID of a message on the POP3 server. Is located in <see cref="MessageHeader.MessageId"/></param>
    /// <returns><see langword="true"/> if message was deleted, <see langword="false"/> otherwise</returns>
    public bool DeleteMessageByMessageId(Pop3Client client, string messageId)
    {
        // Get the number of messages on the POP3 server
        int messageCount = client.GetMessageCount();

        // Run trough each of these messages and download the headers
        for (int messageItem = messageCount; messageItem > 0; messageItem--)
        {
            // If the Message ID of the current message is the same as the parameter given, delete that message
            if (client.GetMessageHeaders(messageItem).MessageId == messageId)
            {
                // Delete
                client.DeleteMessage(messageItem);
                return true;
            }
        }

        // We did not find any message with the given messageId, report this back
        return false;
    }


    /// <summary>
    /// Example showing:
    ///  - how to delete fetch an emails headers only
    ///  - how to delete a message from the server
    /// </summary>
    /// <param name="client">A connected and authenticated Pop3Client from which to delete a message</param>
    /// <param name="messageId">A message ID of a message on the POP3 server. Is located in <see cref="MessageHeader.MessageId"/></param>
    /// <returns><see langword="true"/> if message was deleted, <see langword="false"/> otherwise</returns>
    //public Message FindMessageByMessageId(Pop3Client client, string messageId)
    //{
    //    Message msg;
    //    // Get the number of messages on the POP3 server
    //    int messageCount = client.GetMessageCount();

    //    // Run trough each of these messages and download the headers
    //    for (int messageItem = messageCount; messageItem > 0; messageItem--)
    //    {
    //        // If the Message ID of the current message is the same as the parameter given, delete that message
    //        if (client.GetMessageHeaders(messageItem).MessageId == messageId)
    //        {
    //            // Delete

    //           msg = client.GetMessage(1);
    //            //msg =  messageItem;
    //        }
    //    }
    //   // return msg;

    //    // We did not find any message with the given messageId, report this back
    //}

    /// <summary>
    /// Example showing:
    ///  - how to a find plain text version in a Message
    ///  - how to save MessageParts to file
    /// </summary>
    /// <param name="message">The message to examine for plain text</param>
    public void FindPlainTextInMessage(Message message)
    {
        MessagePart plainText = message.FindFirstPlainTextVersion();
        if (plainText != null)
        {
            // Save the plain text to a file, database or anything you like
            plainText.Save(new FileInfo("plainText.txt"));
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to find a html version in a Message
    ///  - how to save MessageParts to file
    /// </summary>
    /// <param name="message">The message to examine for html</param>
    public void FindHtmlInMessage(Message message)
    {
        MessagePart html = message.FindFirstHtmlVersion();
        if (html != null)
        {
            // Save the plain text to a file, database or anything you like
            html.Save(new FileInfo("html.txt"));
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to find a MessagePart with a specified MediaType
    ///  - how to get the body of a MessagePart as a string
    /// </summary>
    /// <param name="message">The message to examine for xml</param>
    public void FindXmlInMessage(Message message)
    {
        MessagePart xml = message.FindFirstMessagePartWithMediaType("text/xml");
        if (xml != null)
        {
            // Get out the XML string from the email
            string xmlString = xml.GetBodyAsText();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            // Load in the XML read from the email
            doc.LoadXml(xmlString);

            // Save the xml to the filesystem
            doc.Save("test.xml");
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to delete a specific message from a server
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <param name="messageNumber">
    /// The number of the message to delete.
    /// Must be in range [1, messageCount] where messageCount is the number of messages on the server.
    /// </param>
    public void DeleteMessageOnServer(string hostname, int port, bool useSsl, string username, string password, int messageNumber)
    {
        // The client disconnects from the server when being disposed
        using (Pop3Client client = new Pop3Client())
        {
            // Connect to the server
            client.Connect(hostname, port, useSsl);

            // Authenticate ourselves towards the server
            client.Authenticate(username, password);

            // Mark the message as deleted
            // Notice that it is only MARKED as deleted
            // POP3 requires you to "commit" the changes
            // which is done by sending a QUIT command to the server
            // You can also reset all marked messages, by sending a RSET command.
            client.DeleteMessage(messageNumber);

            // When a QUIT command is sent to the server, the connection between them are closed.
            // When the client is disposed, the QUIT command will be sent to the server
            // just as if you had called the Disconnect method yourself.
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to use UID's (unique ID's) of messages from the POP3 server
    ///  - how to download messages not seen before
    ///    (notice that the POP3 protocol cannot see if a message has been read on the server
    ///     before. Therefore the client need to maintain this state for itself)
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
    /// <param name="username">Username of the user on the server</param>
    /// <param name="password">Password of the user on the server</param>
    /// <param name="seenUids">
    /// List of UID's of all messages seen before.
    /// New message UID's will be added to the list.
    /// Consider using a HashSet if you are using >= 3.5 .NET
    /// </param>
    /// <returns>A List of new Messages on the server</returns>
    public List<Message> FetchUnseenMessagesOld(string hostname, int port, bool useSsl, string username, string password, List<string> seenUids)
    {
        // The client disconnects from the server when being disposed
        using (Pop3Client client = new Pop3Client())
        {
            // Connect to the server
            client.Connect(hostname, port, useSsl);

            // Authenticate ourselves towards the server
            client.Authenticate(username, password);

            // Fetch all the current uids seen
            List<string> uids = client.GetMessageUids();

            // Create a list we can return with all new messages
            List<Message> newMessages = new List<Message>();

            // All the new messages not seen by the POP3 client
            for (int i = 0; i < uids.Count; i++)
            {
                string currentUidOnServer = uids[i];
                if (!seenUids.Contains(currentUidOnServer))
                {
                    // We have not seen this message before.
                    // Download it and add this new uid to seen uids

                    // the uids list is in messageNumber order - meaning that the first
                    // uid in the list has messageNumber of 1, and the second has 
                    // messageNumber 2. Therefore we can fetch the message using
                    // i + 1 since messageNumber should be in range [1, messageCount]
                    Message unseenMessage = client.GetMessage(i + 1);

                    // Add the message to the new messages
                    newMessages.Add(unseenMessage);

                    // Add the uid to the seen uids, as it has now been seen
                    seenUids.Add(currentUidOnServer);
                }
            }

            // Return our new found messages
            return newMessages;
        }
    }

    /// <summary>
    /// Example showing:
    ///  - how to set timeouts
    ///  - how to override the SSL certificate checks with your own implementation
    /// </summary>
    /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
    /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
    /// <param name="timeouts">Read and write timeouts used by the Pop3Client</param>
    public void BypassSslCertificateCheck(string hostname, int port, int timeouts)
    {
        // The client disconnects from the server when being disposed
        using (Pop3Client client = new Pop3Client())
        {
            // Connect to the server using SSL with specified settings
            // true here denotes that we connect using SSL
            // The certificateValidator can validate the SSL certificate of the server.
            // This might be needed if the server is using a custom normally untrusted certificate
            client.Connect(hostname, port, true, timeouts, timeouts, certificateValidator);

            // Do something extra now that we are connected to the server
        }
    }

    private bool certificateValidator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
    {
        // We should check if there are some SSLPolicyErrors, but here we simply say that
        // the certificate is okay - we trust it.
        return true;
    }

    /// <summary>
    /// Example showing:
    ///  - how to save a message to a file
    ///  - how to load a message from a file at a later point
    /// </summary>
    /// <param name="message">The message to save and load at a later point</param>
    /// <returns>The Message, but loaded from the file system</returns>
    public Message SaveAndLoadFullMessage(Message message)
    {
        // FileInfo about the location to save/load message
        FileInfo file = new FileInfo("someFile.eml");

        // Save the full message to some file
        message.Save(file);

        // Now load the message again. This could be done at a later point
        Message loadedMessage = Message.Load(file);

        // use the message again
        return loadedMessage;
    }

    /// <summary>
    /// Example showing:
    ///  - How to change logging
    ///  - How to implement your own logger
    /// </summary>
    public void ChangeLogging()
    {
        // All logging is sent trough logger defined at DefaultLogger.Log
        // The logger can be changed by calling DefaultLogger.SetLog(someLogger)

        // By default all logging is sent to the System.Diagnostics.Trace facilities.
        // These are not very useful if you are not debugging
        // Instead, lets send logging to a file:
        DefaultLogger.SetLog(new FileLogger());
        FileLogger.LogFile = new FileInfo("MyLoggingFile.log");

        // It is also possible to implement your own logging:
        DefaultLogger.SetLog(new MyOwnLogger());
    }

    class MyOwnLogger : ILog
    {
        public void LogError(string message)
        {
            Console.WriteLine("ERROR!!!: " + message);
        }

        public void LogDebug(string message)
        {
            // Dont want to log debug messages
        }
    }

    /// <summary>
    /// Example showing:
    ///  - How to provide custom Encoding class
    ///  - How to use UTF8 as default Encoding
    /// </summary>
    /// <param name="customEncoding">Own Encoding implementation</param>
    public void InsertCustomEncodings(Encoding customEncoding)
    {
        // Lets say some email contains a characterSet of "iso-9999-9" which
        // is fictional, but is really just UTF-8.
        // Lets add that mapping to the class responsible for finding
        // the Encoding from the name of it
        EncodingFinder.AddMapping("iso-9999-9", Encoding.UTF8);

        // It is also possible to implement your own Encoding if
        // the framework does not provide what you need
        EncodingFinder.AddMapping("specialEncoding", customEncoding);

        // Now, if the EncodingFinder is not able to find an encoding, lets
        // see if we can find one ourselves
        EncodingFinder.FallbackDecoder = CustomFallbackDecoder;
    }

    Encoding CustomFallbackDecoder(string characterSet)
    {
        // Is it a "foo" encoding?
        if (characterSet.StartsWith("foo"))
            return Encoding.ASCII; // then use ASCII

        // If no special encoding could be found, provide UTF8 as default.
        // You can also return null here, which would tell OpenPop that
        // no encoding could be found. This will then throw an exception.
        return Encoding.UTF8;
    }

    // Other examples to show, that is in the library
    // Show how to build a TreeNode representation of the Message hierarchy using the
    // TreeNodeBuilder class in OpenPopTest

}
