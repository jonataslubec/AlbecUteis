using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Web;


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

public struct Zip
{
    public static void compactar(string startPath, string zipPath)
    {
        if (File.Exists(startPath))
            File.Delete(startPath);

        ZipFile.CreateFromDirectory(startPath, zipPath);
    }

    public static void descompactar(string zipPath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }
}



public struct Texto
{
    /// <summary>
    /// Remove caracteres não numéricos
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveNaoNumericos(string text)
    {
        System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
        string ret = reg.Replace(text, string.Empty);
        return ret;
    }
}



public struct Numero
{
    public static string zeroFirst(string numero)
    {
        if ((Convert.ToInt32(numero) < 10))
        {
            return "0" + numero;
        }
        else
        {
            return numero;
        }
    }
}


public struct Validacao
{

    /// <summary>
    /// Valida se um cpf é válido
    /// </summary>
    /// <param name="cpf"></param>
    /// <returns></returns>
    public static bool ValidaCPF(string cpf)
    {
        //Remove formatação do número, ex: "123.456.789-01" vira: "12345678901"
        cpf = Texto.RemoveNaoNumericos(cpf);

        if (cpf.Length > 11)
            return false;

        while (cpf.Length != 11)
            cpf = '0' + cpf;

        bool igual = true;
        for (int i = 1; i < 11 && igual; i++)
            if (cpf[i] != cpf[0])
                igual = false;

        if (igual || cpf == "12345678909")
            return false;

        int[] numeros = new int[11];

        for (int i = 0; i < 11; i++)
            numeros[i] = int.Parse(cpf[i].ToString());

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (10 - i) * numeros[i];

        int resultado = soma % 11;

        if (resultado == 1 || resultado == 0)
        {
            if (numeros[9] != 0)
                return false;
        }
        else if (numeros[9] != 11 - resultado)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (11 - i) * numeros[i];

        resultado = soma % 11;

        if (resultado == 1 || resultado == 0)
        {
            if (numeros[10] != 0)
                return false;
        }
        else
            if (numeros[10] != 11 - resultado)
                return false;

        return true;
    }

    public static bool ValidaCNPJ(string vrCNPJ)
    {
        string CNPJ = vrCNPJ.Replace(".", "");
        CNPJ = CNPJ.Replace("/", "");
        CNPJ = CNPJ.Replace("-", "");
        int[] digitos, soma, resultado;
        int nrDig; string ftmt;
        bool[] CNPJOk; ftmt = "6543298765432";
        digitos = new int[14];
        soma = new int[2]; soma[0] = 0;
        soma[1] = 0; resultado = new int[2];
        resultado[0] = 0; resultado[1] = 0;
        CNPJOk = new bool[2]; CNPJOk[0] = false;
        CNPJOk[1] = false;
        try
        {
            for (nrDig = 0; nrDig < 14; nrDig++)
            {
                digitos[nrDig] = int.Parse(CNPJ.Substring(nrDig, 1));
                if (nrDig <= 11) soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));
                if (nrDig <= 12) soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
            }

            for (nrDig = 0; nrDig < 2; nrDig++)
            {
                resultado[nrDig] = (soma[nrDig] % 11);
                if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1)) CNPJOk[nrDig] = (digitos[12 + nrDig] == 0);
                else CNPJOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
            }
            return (CNPJOk[0] && CNPJOk[1]);
        }
        catch
        {
            return false;
        }
    }
}




/// <summary>
/// Retorna a Data com o fuso horário do brasil, independente de onde a aplicação esteja hospedada.
/// </summary>
public struct DateTimeUTC
{
    public static DateTime Now()
    {
        return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }

    public static DateTime Convert(DateTime datetime)
    {
        return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }


    public static DateTime NowNetwork()
    {
        //default Windows time server
        const string ntpServer = "time.windows.com";

        // NTP message size - 16 bytes of the digest (RFC 2030)
        var ntpData = new byte[48];

        //Setting the Leap Indicator, Version Number and Mode values
        ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;

        //The UDP port number assigned to NTP is 123
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        //NTP uses UDP
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(ipEndPoint);

        //Stops code hang if NTP is blocked
        socket.ReceiveTimeout = 3000;

        socket.Send(ntpData);
        socket.Receive(ntpData);
        socket.Close();

        //Offset to get to the "Transmit Timestamp" field (time at which the reply 
        //departed the server for the client, in 64-bit timestamp format."
        const byte serverReplyTime = 40;

        //Get the seconds part
        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

        //Get the seconds fraction
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        //Convert From big-endian to little-endian
        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

        //**UTC** time
        var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

        return networkDateTime.ToLocalTime();
    }

    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
}





public struct Remover
{
    public static string Acentos(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return String.Empty;
        else
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(texto);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
    public static string Espaco(string texto)
    {
        return texto.Replace(" ", "");
    }

    public static string Replace(string texto)
    {
        return texto.Replace("'", "");
    }
}

public struct Imagem
{
    public static System.Drawing.Image Base64ToImage(string base64string)
    {
        System.Drawing.Image img = default(System.Drawing.Image);
        System.IO.MemoryStream MS = new System.IO.MemoryStream();

        int pos = base64string.IndexOf(",");
        base64string = base64string.Remove(0, pos + 1);

        string b64 = base64string.Replace(" ", "+");

        if ((b64.Length % 2 != 0))
        {
            b64 = b64.Substring(0, b64.Length - 1);
        }


        byte[] b = null;

        b = Convert.FromBase64String(b64);
        MS = new System.IO.MemoryStream(b);

        img = System.Drawing.Image.FromStream(MS);

        return img;
    }

    public static string FixBase64ForImage(string Image)
    {
        System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
        sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
        return sbText.ToString();
    }

    public static string ImageToBase64(Image image, string format)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            // Convert Image to byte[]
            image.Save(ms, ParseImageFormat(format));
            byte[] imageBytes = ms.ToArray();

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;

            //return "data:image/" + format + ";base64," + base64String;
        }
    }


    /// <summary>
    /// Reduz o tamanho da imagem para x largura e y altura pixels
    /// </summary>
    /// <param name="Image">Imagem a ser minimízada</param>
    /// <param name="width">Largura da nova imagem</param>
    /// <param name="height">Altura da nova imagem</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static System.Drawing.Image MinimizeImageBy(System.Drawing.Image Image, int width, int height)
    {
        return Image.GetThumbnailImage(width, height, null, IntPtr.Zero);
    }


    public static Image resizeImage(Image imgToResize, Size size)
    {
        int sourceWidth = imgToResize.Width;
        int sourceHeight = imgToResize.Height;

        float nPercent = 0;
        float nPercentW = 0;
        float nPercentH = 0;

        nPercentW = ((float)size.Width / (float)sourceWidth);
        nPercentH = ((float)size.Height / (float)sourceHeight);

        if (nPercentH < nPercentW)
            nPercent = nPercentH;
        else
            nPercent = nPercentW;

        int destWidth = (int)(sourceWidth * nPercent);
        int destHeight = (int)(sourceHeight * nPercent);

        Bitmap b = new Bitmap(destWidth, destHeight);
        Graphics g = Graphics.FromImage((Image)b);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        g.Dispose();

        return (Image)b;
    }



    public static System.Drawing.Imaging.ImageFormat ParseImageFormat(string type)
    {

        switch (type.ToLower())
        {

            case "jpg":


                return System.Drawing.Imaging.ImageFormat.Jpeg;
            case "jpeg":


                return System.Drawing.Imaging.ImageFormat.Jpeg;
            case "bmp":


                return System.Drawing.Imaging.ImageFormat.Bmp;
            case "gif":


                return System.Drawing.Imaging.ImageFormat.Gif;
            case "png":


                return System.Drawing.Imaging.ImageFormat.Png;
            case "tiff":


                return System.Drawing.Imaging.ImageFormat.Tiff;
            case "wmf":


                return System.Drawing.Imaging.ImageFormat.Wmf;
            case "emf":


                return System.Drawing.Imaging.ImageFormat.Emf;
            case "icon":


                return System.Drawing.Imaging.ImageFormat.Icon;
            case "ico":


                return System.Drawing.Imaging.ImageFormat.Icon;
            case "exif":


                return System.Drawing.Imaging.ImageFormat.Exif;
            default:

                return System.Drawing.Imaging.ImageFormat.Jpeg;
        }

    }

    public static string saveImageToAzure(string container, System.Drawing.Image imagem, string formato, string filename, string nomeProjeto)
    {
        string timestamp = DateTimeUTC.Now().ToString("yyyyMMddHHmmssfff");

        AzureStorageBlob b = new AzureStorageBlob();

        string caminho = nomeProjeto + "/" + timestamp + "/" + Path.GetFileName(filename);
        string caminhoAnexo = b.returnPathForBlob(container, caminho);
        string caminhoTemp = b.returnPathTempForBlob(filename);

        imagem.Save(caminhoTemp, Imagem.ParseImageFormat(formato));

        b.saveFileToBlob(container, caminho, caminhoTemp, "Attchments", Arquivo.GetMIMEType(filename));

        FileInfo fi = new FileInfo(caminhoTemp);
        if (fi.Exists)
            fi.Delete();

        return caminhoAnexo;
    }

    public static string saveImageToLocal(System.Drawing.Image imagem, string formato, string filename, string pasta, string nomeProjeto)
    {
        string timestamp = DateTimeUTC.Now().ToString("yyyyMMddHHmmssfff");
        string caminhoPasta = @"\FilesRepositories\" + pasta + @"\";

        string filePath = "";

        DirectoryInfo di = Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath(caminhoPasta), nomeProjeto));

        string fileNameTimeStamp = Path.GetFileNameWithoutExtension(filename) + "_" + timestamp + "." + formato;

        imagem.Save(Path.Combine(di.FullName, fileNameTimeStamp), Imagem.ParseImageFormat(formato));

        filePath = Path.Combine(Path.Combine(caminhoPasta, nomeProjeto), fileNameTimeStamp);

        return filePath;
    }
}



public struct Arquivo
{
    public static string Tipo(long length)
    {
        //Tipo de dado
        double size = length / (double)1024;

        //Recupero em MB
        if (size > 1050)
        {
            size = size / (double)1024;
            return "MB";
        }
        //Recupero em KB
        else
        {
            if (size > 1)
            {
                return "KB";
            }
            else
            {
                return "Bytes";
            }
        }
    }

    public static string Tamanho(long length)
    {
        //Tipo de dado
        double size = length / (double)1024;

        //Recupero em MB
        if (size > 1050)
        {
            size = size / (double)1024;
            //Tamnho
            return size.ToString("N2");

        }
        //Recupero em KB
        else
        {
            if (size > 1)
            {
                //Tamnho
                return size.ToString("N3");
            }
            else
            {
                //Tamnho
                return length.ToString();
            }
        }
    }


    private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
        {
        {"ai", "application/postscript"},
        {"aif", "audio/x-aiff"},
        {"aifc", "audio/x-aiff"},
        {"aiff", "audio/x-aiff"},
        {"asc", "text/plain"},
        {"atom", "application/atom+xml"},
        {"au", "audio/basic"},
        {"avi", "video/x-msvideo"},
        {"bcpio", "application/x-bcpio"},
        {"bin", "application/octet-stream"},
        {"bmp", "image/bmp"},
        {"cdf", "application/x-netcdf"},
        {"cgm", "image/cgm"},
        {"class", "application/octet-stream"},
        {"cpio", "application/x-cpio"},
        {"cpt", "application/mac-compactpro"},
        {"csh", "application/x-csh"},
        {"css", "text/css"},
        {"dcr", "application/x-director"},
        {"dif", "video/x-dv"},
        {"dir", "application/x-director"},
        {"djv", "image/vnd.djvu"},
        {"djvu", "image/vnd.djvu"},
        {"dll", "application/octet-stream"},
        {"dmg", "application/octet-stream"},
        {"dms", "application/octet-stream"},
        {"doc", "application/msword"},
        {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {"docm","application/vnd.ms-word.document.macroEnabled.12"},
        {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
        {"dtd", "application/xml-dtd"},
        {"dv", "video/x-dv"},
        {"dvi", "application/x-dvi"},
        {"dxr", "application/x-director"},
        {"eps", "application/postscript"},
        {"etx", "text/x-setext"},
        {"exe", "application/octet-stream"},
        {"ez", "application/andrew-inset"},
        {"gif", "image/gif"},
        {"gram", "application/srgs"},
        {"grxml", "application/srgs+xml"},
        {"gtar", "application/x-gtar"},
        {"hdf", "application/x-hdf"},
        {"hqx", "application/mac-binhex40"},
        {"htm", "text/html"},
        {"html", "text/html"},
        {"ice", "x-conference/x-cooltalk"},
        {"ico", "image/x-icon"},
        {"ics", "text/calendar"},
        {"ief", "image/ief"},
        {"ifb", "text/calendar"},
        {"iges", "model/iges"},
        {"igs", "model/iges"},
        {"jnlp", "application/x-java-jnlp-file"},
        {"jp2", "image/jp2"},
        {"jpe", "image/jpeg"},
        {"jpeg", "image/jpeg"},
        {"jpg", "image/jpeg"},
        {"js", "application/x-javascript"},
        {"kar", "audio/midi"},
        {"latex", "application/x-latex"},
        {"lha", "application/octet-stream"},
        {"lzh", "application/octet-stream"},
        {"m3u", "audio/x-mpegurl"},
        {"m4a", "audio/mp4a-latm"},
        {"m4b", "audio/mp4a-latm"},
        {"m4p", "audio/mp4a-latm"},
        {"m4u", "video/vnd.mpegurl"},
        {"m4v", "video/x-m4v"},
        {"mac", "image/x-macpaint"},
        {"man", "application/x-troff-man"},
        {"mathml", "application/mathml+xml"},
        {"me", "application/x-troff-me"},
        {"mesh", "model/mesh"},
        {"mid", "audio/midi"},
        {"midi", "audio/midi"},
        {"mif", "application/vnd.mif"},
        {"mov", "video/quicktime"},
        {"movie", "video/x-sgi-movie"},
        {"mp2", "audio/mpeg"},
        {"mp3", "audio/mpeg"},
        {"mp4", "video/mp4"},
        {"mpe", "video/mpeg"},
        {"mpeg", "video/mpeg"},
        {"mpg", "video/mpeg"},
        {"mpga", "audio/mpeg"},
        {"ms", "application/x-troff-ms"},
        {"msh", "model/mesh"},
        {"mxu", "video/vnd.mpegurl"},
        {"nc", "application/x-netcdf"},
        {"oda", "application/oda"},
        {"ogg", "application/ogg"},
        {"pbm", "image/x-portable-bitmap"},
        {"pct", "image/pict"},
        {"pdb", "chemical/x-pdb"},
        {"pdf", "application/pdf"},
        {"pgm", "image/x-portable-graymap"},
        {"pgn", "application/x-chess-pgn"},
        {"pic", "image/pict"},
        {"pict", "image/pict"},
        {"png", "image/png"},
        {"pnm", "image/x-portable-anymap"},
        {"pnt", "image/x-macpaint"},
        {"pntg", "image/x-macpaint"},
        {"ppm", "image/x-portable-pixmap"},
        {"ppt", "application/vnd.ms-powerpoint"},
        {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
        {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {"ps", "application/postscript"},
        {"qt", "video/quicktime"},
        {"qti", "image/x-quicktime"},
        {"qtif", "image/x-quicktime"},
        {"ra", "audio/x-pn-realaudio"},
        {"ram", "audio/x-pn-realaudio"},
        {"ras", "image/x-cmu-raster"},
        {"rdf", "application/rdf+xml"},
        {"rgb", "image/x-rgb"},
        {"rm", "application/vnd.rn-realmedia"},
        {"roff", "application/x-troff"},
        {"rtf", "text/rtf"},
        {"rtx", "text/richtext"},
        {"sgm", "text/sgml"},
        {"sgml", "text/sgml"},
        {"sh", "application/x-sh"},
        {"shar", "application/x-shar"},
        {"silo", "model/mesh"},
        {"sit", "application/x-stuffit"},
        {"skd", "application/x-koan"},
        {"skm", "application/x-koan"},
        {"skp", "application/x-koan"},
        {"skt", "application/x-koan"},
        {"smi", "application/smil"},
        {"smil", "application/smil"},
        {"snd", "audio/basic"},
        {"so", "application/octet-stream"},
        {"spl", "application/x-futuresplash"},
        {"src", "application/x-wais-source"},
        {"sv4cpio", "application/x-sv4cpio"},
        {"sv4crc", "application/x-sv4crc"},
        {"svg", "image/svg+xml"},
        {"swf", "application/x-shockwave-flash"},
        {"t", "application/x-troff"},
        {"tar", "application/x-tar"},
        {"tcl", "application/x-tcl"},
        {"tex", "application/x-tex"},
        {"texi", "application/x-texinfo"},
        {"texinfo", "application/x-texinfo"},
        {"tif", "image/tiff"},
        {"tiff", "image/tiff"},
        {"tr", "application/x-troff"},
        {"tsv", "text/tab-separated-values"},
        {"txt", "text/plain"},
        {"ustar", "application/x-ustar"},
        {"vcd", "application/x-cdlink"},
        {"vrml", "model/vrml"},
        {"vxml", "application/voicexml+xml"},
        {"wav", "audio/x-wav"},
        {"wbmp", "image/vnd.wap.wbmp"},
        {"wbmxl", "application/vnd.wap.wbxml"},
        {"wml", "text/vnd.wap.wml"},
        {"wmlc", "application/vnd.wap.wmlc"},
        {"wmls", "text/vnd.wap.wmlscript"},
        {"wmlsc", "application/vnd.wap.wmlscriptc"},
        {"wrl", "model/vrml"},
        {"xbm", "image/x-xbitmap"},
        {"xht", "application/xhtml+xml"},
        {"xhtml", "application/xhtml+xml"},
        {"xls", "application/vnd.ms-excel"},
        {"xml", "application/xml"},
        {"xpm", "image/x-xpixmap"},
        {"xsl", "application/xml"},
        {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
        {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
        {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
        {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {"xslt", "application/xslt+xml"},
        {"xul", "application/vnd.mozilla.xul+xml"},
        {"xwd", "image/x-xwindowdump"},
        {"xyz", "chemical/x-xyz"},
        {"zip", "application/zip"}
        };

    public static string GetMIMEType(string fileName)
    {
        //get file extension
        string extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (extension.Length > 0 &&
            MIMETypesDictionary.ContainsKey(extension.Remove(0, 1)))
        {
            return MIMETypesDictionary[extension.Remove(0, 1)];
        }
        return "unknown/unknown";
    }


    public static string getFileNameFromMessage(string mensagem)
    {
        mensagem = mensagem.ToString().Substring(0, mensagem.LastIndexOf("|"));

        string nomeArquivo = mensagem.ToString().Substring(mensagem.LastIndexOf("/"), (mensagem.Length - mensagem.LastIndexOf("/")));

        nomeArquivo = nomeArquivo.ToString().Substring(19, nomeArquivo.Length - 19);

        var ext = nomeArquivo.ToString().Substring(nomeArquivo.Length - 3, 3); //png

        nomeArquivo = nomeArquivo.ToString().Substring(0, nomeArquivo.Length - 3); //arquivo

        nomeArquivo = (nomeArquivo.Length > 14 ? nomeArquivo.Substring(0, 14) + "... ." + ext : nomeArquivo + ext);

        return nomeArquivo;
    }

    public static string getFullFileName(string mensagem)
    {
        mensagem = mensagem.ToString().Substring(0, mensagem.LastIndexOf("|"));

        string nomeArquivo = mensagem.ToString().Substring(mensagem.LastIndexOf("/"), (mensagem.Length - mensagem.LastIndexOf("/")));

        nomeArquivo = nomeArquivo.ToString().Substring(19, nomeArquivo.Length - 19);

        return nomeArquivo;
    }

    public static string getFileNameWithoutLength(string mensagem)
    {
        return mensagem.ToString().Substring(0, mensagem.LastIndexOf("|"));
    }

    public static string getLengthFile(string mensagem)
    {
        return mensagem.ToString().Substring(mensagem.LastIndexOf("|") + 1, ((mensagem.Length - 1) - mensagem.LastIndexOf("|")));
    }
}


public struct Uteis
{
    public static string GetIP()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string ip = "";

        if ((!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"])))
        {
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
        }
        else if ((!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])))
        {
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }
        else if ((!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"])))
        {
            ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        else if ((!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.UserHostAddress)))
        {
            ip = System.Web.HttpContext.Current.Request.UserHostAddress;
        }
        return (ip == "::1" ? "localhost" : ip);
    }

    public static string timeStamp()
    {
       return DateTime.Now.ToString("yyyyMMddHHmmssfff");
    }

    public static string GetColorHexa()
    {
        var random = new Random();
        return String.Format("#{0:X6}", random.Next(0x1000000));
    }
}
