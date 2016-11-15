using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

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

public class EmailSMTP
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="useSsl"></param>
    /// <param name="useDefaultCredentials"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="oMsg"></param>
    public static bool enviar(string hostname, int port, bool useSsl, bool useDefaultCredentials, string username, string password, msg_message oMsg, System.IO.DirectoryInfo diretorioAnexo)
    {
        try
        {
            MailMessage mMailMessage = new MailMessage();

            mMailMessage.From = new MailAddress(oMsg.msg_from_address, oMsg.msg_from_display);

            //Destinatário
            if (oMsg.msg_to != null)
            {
                foreach (var email in oMsg.msg_to.Split(','))
                {
                    mMailMessage.To.Add(new MailAddress(email));
                }
            }

            //Copia

            if (oMsg.msg_cc != null)
            {
                foreach (var cc in oMsg.msg_cc.Split(','))
                {
                    mMailMessage.CC.Add(new MailAddress(cc));
                }
            }

            //Copia oculta
            if (oMsg.msg_bcc != null)
            {
                foreach (var bcc in oMsg.msg_bcc.Split(','))
                {
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                }
            }


            mMailMessage.Subject = oMsg.msg_subject;
            mMailMessage.Body = oMsg.msg_bodyHtml;

            mMailMessage.IsBodyHtml = true;
            mMailMessage.Priority = MailPriority.High;
            mMailMessage.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            mMailMessage.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");



            foreach (System.IO.FileInfo file in diretorioAnexo.GetFiles())
            {
                Attachment anexoOptions = new Attachment(file.FullName);
                anexoOptions.ContentDisposition.Inline = false;
                anexoOptions.ContentType.MediaType = Arquivo.GetMIMEType(file.FullName);
                anexoOptions.ContentDisposition.Size = file.Length;

                mMailMessage.Attachments.Add(anexoOptions);
            }



            SmtpClient mSmtpClient = new SmtpClient();

            mSmtpClient.Host = hostname;
            mSmtpClient.Port = port;
            mSmtpClient.UseDefaultCredentials = useDefaultCredentials;
            mSmtpClient.EnableSsl = useSsl;
            mSmtpClient.Credentials = new System.Net.NetworkCredential(username, password);

            // mSmtpClient.EnableSsl = chkSSL.CheckState

            mSmtpClient.Send(mMailMessage);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="useSsl"></param>
    /// <param name="useDefaultCredentials"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="oMsg"></param>
    public static bool enviar(string hostname, int port, bool useSsl, bool useDefaultCredentials, string username, string password, msg_message oMsg, IEnumerable<IListBlobItem> list)
    {
        try
        {
            MailMessage mMailMessage = new MailMessage();

            mMailMessage.From = new MailAddress(oMsg.msg_from_address, oMsg.msg_from_display);

            //Destinatário
            if (oMsg.msg_to != null)
            {
                foreach (var email in oMsg.msg_to.Split(','))
                {
                    mMailMessage.To.Add(new MailAddress(email));
                }
            }

            //Copia
            if (oMsg.msg_cc != null)
            {
                foreach (var cc in oMsg.msg_cc.Split(','))
                {
                    mMailMessage.CC.Add(new MailAddress(cc));
                }
            }

            //Copia oculta
            if (oMsg.msg_bcc != null)
            {
                foreach (var bcc in oMsg.msg_bcc.Split(','))
                {
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                }
            }

            mMailMessage.Subject = oMsg.msg_subject;
            mMailMessage.Body = oMsg.msg_bodyHtml;

            mMailMessage.IsBodyHtml = true;
            mMailMessage.Priority = MailPriority.High;
            mMailMessage.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            mMailMessage.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");


            foreach (IListBlobItem item in list)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    Byte[] data = new WebClient().DownloadData(blob.Uri.AbsoluteUri);
                    Attachment anexoOptions = new Attachment(new MemoryStream(data), blob.Name);

                    anexoOptions.ContentDisposition.Inline = false;
                    anexoOptions.ContentType.MediaType = Arquivo.GetMIMEType(blob.Name);
                    anexoOptions.ContentDisposition.Size = blob.Properties.Length;

                    mMailMessage.Attachments.Add(anexoOptions);
                }
            }


            SmtpClient mSmtpClient = new SmtpClient();

            mSmtpClient.Host = hostname;
            mSmtpClient.Port = port;
            mSmtpClient.UseDefaultCredentials = useDefaultCredentials;
            mSmtpClient.EnableSsl = useSsl;
            mSmtpClient.Credentials = new System.Net.NetworkCredential(username, password);

            // mSmtpClient.EnableSsl = chkSSL.CheckState

            mSmtpClient.Send(mMailMessage);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="useSsl"></param>
    /// <param name="useDefaultCredentials"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="oMsg"></param>
    public static string checkConfigOK(string hostname, int port, bool useSsl, bool useDefaultCredentials, string username, string password, msg_message oMsg)
    {
        try
        {
            MailMessage mMailMessage = new MailMessage();

            mMailMessage.From = new MailAddress(oMsg.msg_from_address, oMsg.msg_from_display);

            //Destinatário
            if (oMsg.msg_to != null)
            {
                foreach (var email in oMsg.msg_to.Split(','))
                {
                    mMailMessage.To.Add(new MailAddress(email));
                }
            }

            //Copia

            if (oMsg.msg_cc != null)
            {
                foreach (var cc in oMsg.msg_cc.Split(','))
                {
                    mMailMessage.CC.Add(new MailAddress(cc));
                }
            }

            //Copia oculta
            if (oMsg.msg_bcc != null)
            {
                foreach (var bcc in oMsg.msg_bcc.Split(','))
                {
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                }
            }


            mMailMessage.Subject = oMsg.msg_subject;
            mMailMessage.Body = oMsg.msg_bodyHtml;

            mMailMessage.IsBodyHtml = true;
            mMailMessage.Priority = MailPriority.High;
            mMailMessage.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            mMailMessage.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");


            SmtpClient mSmtpClient = new SmtpClient();

            mSmtpClient.Host = hostname;
            mSmtpClient.Port = port;
            mSmtpClient.UseDefaultCredentials = useDefaultCredentials;
            mSmtpClient.EnableSsl = useSsl;
            mSmtpClient.Credentials = new System.Net.NetworkCredential(username, password);

            // mSmtpClient.EnableSsl = chkSSL.CheckState

            mSmtpClient.Send(mMailMessage);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "OK";
    }
}


