using System;
using System.IO;

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

public class FileAsync
{
    public delegate void FileCopyDelegate(string sourceFile, string destFile);
    public delegate void FileDeleteDelegate(string sourceFile);
    public static string deleteFile = "";

    //Criação do Delegate
    public static void Copy(string sourceFile, string destFile)
    {
        FileCopyDelegate del = new FileCopyDelegate(FileCopy);
        IAsyncResult result = del.BeginInvoke(sourceFile, destFile, CallBackAfterFileCopied, null);
    }

    //Inicio do delegate
    public static void FileCopy(string sourceFile, string destFile)
    {
        File.Copy(sourceFile, destFile);
        deleteFile = destFile;
    }

    //Fim do delegate
    public static void CallBackAfterFileCopied(IAsyncResult result)
    {
        FileAsync.Delete(deleteFile);
    }


    //////////////////////////////////////////////////////////////////////

    //Criação do Delegeate Delete
    public static void Delete(string deleteFile)
    {
        FileDeleteDelegate del = new FileDeleteDelegate(FileDelete);
        IAsyncResult result = del.BeginInvoke(deleteFile, CallBackAfterFileDelete, null);
    }


    //Inicio do delegate Delete
    public static void FileDelete(string deleteFile)
    {
        string[] fileBackupForDelete = Directory.GetFiles(deleteFile.Substring(0, deleteFile.LastIndexOf("\\")));

        foreach (string file in fileBackupForDelete)
        {
            if (file != deleteFile)
            {
                File.Delete(file);
            }
        }
    }


    //Fim do delegate Delete
    public static void CallBackAfterFileDelete(IAsyncResult result)
    {
        //Copy e delete finalizado
    }
}
