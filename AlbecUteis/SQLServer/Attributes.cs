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


namespace Attributes
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TooltipAttribute : Attribute
    {
        public string Message { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class PrimaryKeyAttribute : System.Attribute
    {
        public PrimaryKeyAttribute primaryKey;
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ForeignKeyAttribute : System.Attribute
    {
        public ForeignKeyAttribute foreignKey;
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class NotMappedAttribute : System.Attribute
    {
        public NotMappedAttribute notMapped;
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TableSchema : System.Attribute
    {
        public string schema;

        public TableSchema(string schema)
        {
            this.schema = schema;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class TableColumn : System.Attribute
    {
        public string columnName;
        public bool primaryKey;
        public string description;

        public TableColumn()
        {
            columnName = "";
            description = "";
            primaryKey = false;
        }
    }
}

