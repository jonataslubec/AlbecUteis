using Attributes;
using System;

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

public class comboBox
{
    public string value { get; set; }
    public string text { get; set; }
}


public class bac_backup
{
    [PrimaryKey]
    [TableColumn(columnName = "bac_id")]
    public int id { get; set; }

    [TableColumn(columnName = "bac_nome")]
    public string nome { get; set; }

    [TableColumn(columnName = "bac_data")]
    public DateTime dataHora { get; set; }

    public string createTableSQLite()
    {
        return @"CREATE TABLE  IF NOT EXISTS  [bac_backup] (
                    [bac_id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [bac_nome] teXT  NULL,
                    [bac_data] teXT  NULL
                 );";
    }

    public string createTableSQLServer()
    {
        return @"CREATE TABLE [bac_backup] ( 
                    [bac_id] INTEGER IDENTITY PRIMARY KEY,
                    [bac_nome] varchar(300)  NULL,
                    [bac_data] varchar(50)  NULL
                  );";
    }
}


public class upd_updater
{
    [PrimaryKey]
    [TableColumn(columnName = "upd_id")]
    public int id { get; set; }

    [TableColumn(columnName = "upd_descricao")]
    public string descricao { get; set; }

    [TableColumn(columnName = "upd_data")]
    public DateTime dataHora { get; set; }

    [TableColumn(columnName = "upd_versao")]
    public string versao { get; set; }

    [TableColumn(columnName = "upd_ultimo_arquivo")]
    public int ultimo_arquivo { get; set; }

    [TableColumn(columnName = "upd_qtde_arquivos")]
    public int qtde_arquivos { get; set; }

    [TableColumn(columnName = "upd_arquivos")]
    public string arquivos { get; set; }

    [TableColumn(columnName = "upd_schema_atual")]
    public string schemaAtual { get; set; }

    public string createTableSQLite()
    {
        return @"CREATE TABLE  IF NOT EXISTS  [upd_updater] (
                    [upd_id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [upd_descricao] TEXT  NULL,
                    [upd_data] TEXT  NULL,
                    [upd_versao] TEXT  NULL,
                    [upd_arquivos] TEXT  NULL,
                    [upd_qtde_arquivos] INTEGER  NULL,
                    [upd_ultimo_arquivo] INTEGER  NULL,
                    [upd_schema_atual] INTEGER  NULL
                 );";
    }

    public string createTableSQLServer()
    {
        return @"CREATE TABLE [upd_updater] (
                    [upd_id] INTEGER  IDENTITY PRIMARY KEY,
                    [upd_descricao] varchar(1000)  NULL,
                    [upd_data] varchar(50)  NULL,
                    [upd_versao] varchar(100)  NULL,
                    [upd_arquivos] varchar(max)  NULL,
                    [upd_qtde_arquivos] INTEGER  NULL,
                    [upd_ultimo_arquivo] INTEGER  NULL,
                    [upd_schema_atual] varchar(10)  NULL
                 );";
    }
}

public partial class msg_message
{
    [PrimaryKey]
    public int msg_id { get; set; }

    public string msg_attachments_hash { get; set; }

    public string msg_content_description { get; set; }

    public string msg_content_id { get; set; }

    public string msg_keyword { get; set; }

    public string msg_disposition_notification_to { get; set; }

    public string msg_received { get; set; }

    public string msg_importance { get; set; }

    public string msg_cc { get; set; }

    public string msg_bcc { get; set; }

    //[Required(ErrorMessage = "Obrigatório informar um e-mail")]
    public string msg_to { get; set; }

    public string msg_from_display { get; set; }

    public string msg_reply_to { get; set; }

    //Esta data e recuperada no momento que o email é enviado.
    //Tratra-se da data local onde a aplicação está
    public string msg_date { get; set; }

    //Esta data e recuperada no momento que o email é enviado, porém ela tem o utc local na hora do recebimento de
    //onde a aplicação está.
    public DateTime? msg_date_sent { get; set; }

    public string msg_message_id { get; set; }

    public string msg_mime_version { get; set; }

    public string msg_return_path { get; set; }

    public string msg_subject { get; set; }

    public string msg_bodyHtml { get; set; }

    public string msg_bodyText { get; set; }

    public string msg_unique_id { get; set; }

    public bool? msg_was_read { get; set; }

    public string msg_status { get; set; }

    public string msg_attachments { get; set; }

    public int Pessoa { get; set; }

    public string msg_from_address { get; set; }

    public string msg_attachments_media_type { get; set; }

    public string msg_attachments_file_name { get; set; }

    public string msg_attachments_body { get; set; }

    public string msg_attachments_path { get; set; }

    public string msg_attachments_size { get; set; }

    public string msg_attachments_size_thumbnail { get; set; }

    public string msg_attachments_type_data { get; set; }

    public string msg_account_email { get; set; }
}
