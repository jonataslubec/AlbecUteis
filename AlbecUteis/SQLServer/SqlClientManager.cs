using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

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

public class SqlClientManager
{
    public static void createDB(string databaseName, string connectionString)
    {
        String str;
        connectionString = connectionString.Replace(databaseName, "master");
        SqlConnection myConn = new SqlConnection(connectionString);

        str = "CREATE DATABASE " + databaseName + " ON PRIMARY " +
            "(NAME = " + databaseName + "_Data, " +
            "FILENAME = 'D:\\" + databaseName + "_Data.mdf', " +
            "SIZE = 4MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
            "LOG ON (NAME = " + databaseName + "_Log, " +
            "FILENAME = 'D:\\" + databaseName + "_Log.ldf', " +
            "SIZE = 1MB, " +
            "MAXSIZE = 5MB, " +
            "FILEGROWTH = 10%)";

        SqlCommand myCommand = new SqlCommand(str, myConn);

        try
        {
            myConn.Open();
            myCommand.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Trace.Write(ex.Message);
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }
    }



    public static void createUser(string user, string password, string databaseName, string connectionString)
    {
        String str;
        String connectionStringMaster = connectionString.Replace(databaseName, "master");
        SqlConnection myConn = new SqlConnection(connectionStringMaster);

        str = "CREATE LOGIN " + user + " WITH PASSWORD = '" + password + "';";
        str += "CREATE USER " + user + " FROM LOGIN " + user + ";";

        SqlCommand myCommand = new SqlCommand(str, myConn);

        try
        {
            myConn.Open();
            myCommand.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Trace.Write(ex.Message);
        }

        myConn = new SqlConnection(connectionString);

        str = "CREATE USER " + user + " FROM LOGIN " + user + ";";

        str += "EXEC SP_ADDROLEMEMBER 'DB_DATAREADER', '" + user + "';";
        str += "EXEC SP_ADDROLEMEMBER 'DB_DATAWRITER', '" + user + "';";
        str += "EXEC SP_ADDROLEMEMBER 'DB_OWNER', '" + user + "';";

        myCommand = new SqlCommand(str, myConn);
        try
        {
            myConn.Open();
            myCommand.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Trace.Write(ex.Message);
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }
    }



    public static void createSchema(string schema, string user, string connectionString)
    {
        String str;
        SqlConnection myConn = new SqlConnection(connectionString);

        str = "CREATE SCHEMA " + schema + ";";

        SqlCommand myCommand = new SqlCommand(str, myConn);

        try
        {
            myConn.Open();
            myCommand.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Trace.Write(ex.Message);
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }
    }


    public static void setDefaultSchema(string schema, string user, string connectionString)
    {
        String str;
        SqlConnection myConn = new SqlConnection(connectionString);

        str = " ALTER USER " + user + " With DEFAULT_SCHEMA = " + schema + ";";

        SqlCommand myCommand = new SqlCommand(str, myConn);

        try
        {
            myConn.Open();
            myCommand.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Trace.Write(ex.Message);
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }
    }



    public static bool existsDB(string databaseName, string connectionString)
    {
        DataTable dt = new DataTable();
        connectionString = connectionString.Replace(databaseName, "master");
        SqlConnection myConn = new SqlConnection(connectionString);
        string str = @"if(Exists(select * from sys.databases where name = '" + databaseName + "')) begin select 1 end";
        SqlCommand myCommand = new SqlCommand(str, myConn);
        myConn.Open();

        try
        {
            dt.Load(myCommand.ExecuteReader());

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }

    }


    public static bool existsUser(string user, string connectionString)
    {
        DataTable dt = new DataTable();

        SqlConnection myConn = new SqlConnection(connectionString);
        string str = @"if(Exists(select u.name from sys.sysusers u where u.name = '" + user + "'))  begin select 1 end";
        SqlCommand myCommand = new SqlCommand(str, myConn);
        myConn.Open();

        try
        {
            dt.Load(myCommand.ExecuteReader());

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }

    }


    public static bool existsSchema(string schema, string connectionString)
    {
        if (String.IsNullOrEmpty(schema))
            return false;

        DataTable dt = new DataTable();
        SqlConnection myConn = new SqlConnection(connectionString);
        string str = @"if(Exists(select s.name from sys.schemas s where s.name = '" + schema + "'))  begin select 1 end";
        SqlCommand myCommand = new SqlCommand(str, myConn);
        myConn.Open();
        try
        {
            dt.Load(myCommand.ExecuteReader());

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }

    }
}

