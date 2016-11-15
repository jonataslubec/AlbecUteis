
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
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



namespace super
{
    public class tb<T> : ITb<T> where T : new()
    {
        private DataTable dt = new DataTable();

        public tb()
        {
            super.db.run();

        }

        public tb(Dictionary<string, Stream> EmbeddedScripts)
        {
            super.db.run(EmbeddedScripts);
        }


        /// <summary>
        /// Recupera nome da classe atual
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string className()
        {
            return typeof(T).Name;
        }


        /// <summary>
        /// Recupera campos identity da tabela atual
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string identity()
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.isPrimaryKey())
                {
                    return prop.getColumnName();
                }
            }

            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.Name.Contains("id"))
                {
                    return prop.getColumnName();
                }
            }
            return "";
        }

        /// <summary>
        /// Verifica se o banco é SQLite
        /// </summary>
        /// <returns></returns>
        private bool isSQLite()
        {
            return (System.Configuration.ConfigurationManager.AppSettings.Get("typeBanco") == "SQLite");
        }



        /// <summary>
        /// Retorno ultimo id inserido
        /// </summary>
        /// <returns></returns>
        public int scopeIdentity()
        {
            string sql = " SELECT MAX(" + identity() + ") FROM " + className();

            dt = db.query(sql);

            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0][0].ToString());

            return 0;
        }



        /// <summary>
        /// Monta o sql para Insert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string sqlInsert(T t)
        {
            string sql = " INSERT INTO " + className();
            string campos = " ( ";
            string values = " VALUES ( ";

            foreach (PropertyInfo prop in t.GetType().GetProperties())
            {
                if (!prop.isPrimaryKey() && prop.isMapped() && !prop.isForeignKey())
                {
                    campos += prop.getColumnName() + " ,";
                    values += "'" + prop.GetValue(t).IsValueValidType().ValidParameter() + "' ,";
                }
            }

            campos = campos.Remove(campos.Length - 1) + ")";
            values = values.Remove(values.Length - 1) + ")";

            return sql + campos + values;
        }



        /// <summary>
        /// Executa INSERT síncrono
        /// </summary>
        /// <param name="t"></param>
        public void insert(T t)
        {
            db.execute(sqlInsert(t));
        }


        /// <summary>
        /// Executa INSERT Coloca na fila
        /// </summary>
        /// <param name="t"></param>
        public void insertQueue(T t)
        {
            db.addQueue(sqlInsert(t));
        }

        /// <summary>
        /// Executa INSERT Assíncrono
        /// </summary>
        /// <param name="t"></param>
        public async void insertAsync(T t)
        {
            Task<int> task = db.executeAsync(sqlInsert(t));
            int x = await task;
        }


        private delegate void insertAsyncDelegate(T t);
        public void insertDelegate(T t)
        {
            insertAsyncDelegate del = new insertAsyncDelegate(insertDelegateAsync);
            IAsyncResult result = del.BeginInvoke(t, callBack, null);
        }

        private void insertDelegateAsync(T t)
        {
            db.execute(sqlInsert(t));
        }

        private void callBack(IAsyncResult result)
        {
        }




        private string sqlUpdate(T t)
        {
            string sql = " UPDATE " + className();
            string camposValues = " SET ";
            string where = " WHERE ";

            foreach (PropertyInfo prop in t.GetType().GetProperties())
            {
                if (!prop.isPrimaryKey() && prop.isMapped() && !prop.isForeignKey())
                {
                    camposValues += prop.getColumnName() + " = '" + prop.GetValue(t).IsValueValidType().ValidParameter() + "' ,";
                }
                else if (prop.isPrimaryKey())
                {
                    where += prop.getColumnName() + " = '" + prop.GetValue(t) + "'";
                }
            }

            camposValues = camposValues.Remove(camposValues.Length - 1);
            return sql + camposValues + where;
        }



        /// <summary>
        /// Executa UPDATE síncrono
        /// </summary>
        /// <param name="t"></param>
        public void update(T t)
        {
            db.execute(sqlUpdate(t));
        }



        /// <summary>
        /// Executa UPDATE Coloca na fila
        /// </summary>
        /// <param name="t"></param>
        public void updateQueue(T t)
        {
            db.addQueue(sqlUpdate(t));
        }


        /// <summary>
        /// Execute UPDATE assíncrono
        /// </summary>
        /// <param name="t"></param>
        public async void updateAsync(T t)
        {
            Task<int> task = db.executeAsync(sqlUpdate(t));
            int x = await task;
        }


        private delegate void updateAsyncDelegate(T t);
        public void updateDelegate(T t)
        {
            updateAsyncDelegate del = new updateAsyncDelegate(updateDelegateAsync);
            IAsyncResult result = del.BeginInvoke(t, callBack, null);
        }
        private void updateDelegateAsync(T t)
        {
            db.execute(sqlUpdate(t));
        }





        private string sqlDelete(int id)
        {
            string sql = " DELETE FROM " + className();
            string where = " WHERE ";
            where += identity() + " = '" + id.IsNullObject() + "'";

            return sql + where + (isSQLite() ? "; VACUUM;" : "");
        }



        /// <summary>
        /// Executa DELETE síncrono
        /// </summary>
        /// <param name="id"></param>
        public void delete(int id)
        {
            db.execute(sqlDelete(id));
        }

        /// <summary>
        /// Executa DELETE Coloca na fila
        /// </summary>
        /// <param name="id"></param>
        public void deleteQueue(int id)
        {
            db.addQueue(sqlDelete(id));
        }

        /// <summary>
        /// Executa DELETE assíncrono
        /// </summary>
        /// <param name="id"></param>
        public async void deleteAsync(int id)
        {
            Task<int> task = db.executeAsync(sqlDelete(id));
            int x = await task;
        }



        private delegate void deleteAsyncDelegate(int id);
        public void deleteDelegate(int id)
        {
            deleteAsyncDelegate del = new deleteAsyncDelegate(deleteDelegateAsync);
            IAsyncResult result = del.BeginInvoke(id, callBack, null);
        }
        private void deleteDelegateAsync(int id)
        {
            db.execute(sqlDelete(id));
        }


        /// <summary>
        /// Executa DELETE com WHERE síncrono
        /// </summary>
        /// <param name="w">where</param>
        /// <param name="args">argumentos</param>
        public void delete(string w = "", params object[] args)
        {
            w = w.formatString(args);

            string sql = " DELETE FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            db.execute(sql + where + (isSQLite() ? "; VACUUM;" : ""));
        }

        /// <summary>
        ///  Executa DELETE com IN síncrono
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="w"></param>
        /// <param name="args"></param>
        public void deleteIn<U>(string w = "", params U[] args)
        {
            //w = w.formatString(arg0);

            string sql = " DELETE FROM " + className();

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " " + col.GetValue(typeof(U)) + ",";
            }

            cols = cols.Remove(cols.Length - 1);

            string where = w.Length > 0 ? " where " + w + " ( " + cols + " ) " : "";

            db.execute(sql + where + (isSQLite() ? "; VACUUM;" : ""));
        }







        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public List<T> select(string w = "", params object[] args)
        {
            w = w.formatString(args);

            string sql = " SELECT * FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + where);

            return dt.ToList<T>();
        }



        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public T selectObject(string w = "", params object[] args)
        {
            w = w.formatString(args);

            string sql = " SELECT * FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + where);

            return dt.ToObject<T>();
        }




        /// <summary>
        /// Select na tabela mapeada 
        /// </summary>
        /// <typeparam name="U">Anonymous Type</typeparam>
        /// <param name="w">Where</param>
        /// <param name="arg0">Object Where</param>
        /// <param name="anonymous">Anonymous Type Exemplo: new { usu_id = default(int), usu_nome = default(string)}</param>
        /// <returns></returns>
        public List<U> selectNew<U>(string w, object arg0, params U[] anonymous)
        {
            return selectNew(w.formatString(arg0), anonymous);
        }

        public List<U> selectNew<U>(string w, object arg0, object arg1, params U[] anonymous)
        {
            return selectNew(w.formatString(arg0, arg1), anonymous);
        }

        public List<U> selectNew<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous)
        {
            return selectNew(w.formatString(arg0, arg1, arg2), anonymous);
        }

        public List<U> selectNew<U>(params U[] anonymous)
        {
            return selectNew("", anonymous);
        }

        public List<U> selectNew<U>(string w = "", params U[] anonymous)
        {
            string sql = " SELECT ";

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " " + col.Name + ",";
            }

            cols = cols.Remove(cols.Length - 1);

            string sql2 = " FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + cols + sql2 + where);

            return dt.ToAnonymousList<U>(anonymous);
        }





        /// <summary>
        /// Select na tabela mapeada 
        /// </summary>
        /// <typeparam name="U">Anonymous Type</typeparam>
        /// <param name="w">Where</param>
        /// <param name="arg0">Object Where</param>
        /// <param name="anonymous">Anonymous Type Exemplo: new { usu_id = default(int), usu_nome = default(string)}</param>
        /// <returns></returns>
        public U selectNewObject<U>(string w, object arg0, params U[] anonymous)
        {
            w = w.formatString(arg0);
            return selectNewObject(w, anonymous);
        }

        public U selectNewObject<U>(string w, object arg0, object arg1, params U[] anonymous)
        {
            return selectNewObject(w.formatString(arg0, arg1), anonymous);
        }

        public U selectNewObject<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous)
        {
            w = w.formatString(arg0, arg1, arg2);

            return selectNewObject(w, anonymous);
        }

        public U selectNewObject<U>(params U[] anonymous)
        {
            return selectNewObject("", anonymous);
        }

        public U selectNewObject<U>(string w, params U[] anonymous)
        {
            string sql = " SELECT ";

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " " + col.Name + ",";
            }

            cols = cols.Remove(cols.Length - 1);

            string sql2 = " FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + cols + sql2 + where);
            return dt.ToAnonymousObject<U>(anonymous);
        }




        /// <summary>
        /// Select na tabela mapeada 
        /// </summary>
        /// <typeparam name="U">Anonymous Type</typeparam>
        /// <param name="w">Where</param>
        /// <param name="arg0">Object Where</param>
        /// <param name="anonymous">Anonymous Type Exemplo: new { usu_id = default(int), usu_nome = default(string)}</param>
        /// <returns></returns>
        public U selectMax<U>(string w, object arg0, params U[] anonymous)
        {
            w = w.formatString(arg0);
            return selectMax(w, anonymous);
        }

        public U selectMax<U>(string w, object arg0, object arg1, params U[] anonymous)
        {
            w = w.formatString(arg0, arg1);
            return selectMax(w, anonymous);
        }

        public U selectMax<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous)
        {
            w = w.formatString(arg0, arg1, arg2);
            return selectMax(w, anonymous);
        }
        public U selectMax<U>(string w, params U[] anonymous)
        {
            string sql = " SELECT ";

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " MAX(" + col.Name + ") as " + col.Name;
                break;
            }

            string sql2 = " FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + cols + sql2 + where);

            return dt.ToAnonymousObject<U>(anonymous);
        }



        /// <summary>
        /// Select na tabela mapeada 
        /// </summary>
        /// <typeparam name="U">Anonymous Type</typeparam>
        /// <param name="w">Where</param>
        /// <param name="arg0">Object Where</param>
        /// <param name="anonymous">Anonymous Type Exemplo: new { usu_id = default(int), usu_nome = default(string)}</param>
        /// <returns></returns>
        public int selectCount(string w = "", params object[] args)
        {
            w = w.formatString(args);

            string sql = " SELECT ";

            string cols = identity();

            string sql2 = " FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + cols + sql2 + where);

            return dt.Rows.Count;
        }




        /// <summary>
        /// Select na tabela mapeada 
        /// </summary>
        /// <typeparam name="U">Anonymous Type</typeparam>
        /// <param name="w">Where</param>
        /// <param name="arg0">Object Where</param>
        /// <param name="anonymous">Anonymous Type Exemplo: new { usu_id = default(int), usu_nome = default(string)}</param>
        /// <returns></returns>
        public bool selectExists(string w = "", params object[] args)
        {
            w = w.formatString(args);

            string sql = " SELECT ";

            string cols = identity();

            string sql2 = " FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + cols + sql2 + where);

            return (dt.Rows.Count > 0);
        }


        public List<comboBox> selectCombo(string id, string descricao)
        {
            string sql = " SELECT ";

            string cols = " " + id + " as value,";
            cols += " " + descricao + " as text ";

            string t = " FROM " + className() + " ";

            DataTable dt = db.query(sql + cols + t);

            return dt.ToList<comboBox>();
        }



        public List<U> innerJoin<U>(string w, string arg0, string C1, string T2, string C2, params U[] anonymous)
        {

            w = w.formatString(arg0);

            string sql = " SELECT ";

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " " + col.Name + ",";
            }

            cols = cols.Remove(cols.Length - 1);

            string sql2 = " FROM " + className();

            string inner = " INNER JOIN " + T2 + " ON ";

            string inner2 = className() + "." + C1 + " = " + T2 + "." + C2;

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + cols + sql2 + inner + inner2 + where);

            return dt.ToAnonymousList<U>(anonymous);
        }




        /// <summary>
        /// Encontra o objeto pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T findById(int? id)
        {
            string sql = " SELECT * FROM " + className();
            string camposValues = " ";

            camposValues += " WHERE " + identity() + " = '" + id.IsNullObject() + "'";

            dt = db.query(sql + camposValues);

            return dt.ToObject<T>();
        }

        public U findById<U>(int? id, params U[] anonymous)
        {
            string sql = " SELECT ";

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " " + col.Name + ",";
            }

            cols = cols.Remove(cols.Length - 1);

            string sql2 = " FROM " + className();

            string camposValues = " ";

            camposValues += " WHERE " + identity() + " = '" + id.IsNullObject() + "'";

            dt = db.query(sql + cols + sql2 + camposValues);

            return dt.ToAnonymousObject<U>(anonymous);
        }

        public T find(string w = "", params object[] args)
        {
            w = w.formatString(args);

            string sql = " SELECT * FROM " + className();

            string where = w.Length > 0 ? " where " + w : "";

            dt = db.query(sql + where);

            return dt.ToObject<T>();
        }

        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            handle.Dispose();
        }



    }



    public static class db 
    {
        static BlockingCollection<string> fila = new BlockingCollection<string>();
        private static Task task;

        private static string getConnectionStringSQLServer()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString + "MultipleActiveResultSets=True;";
        }

        private static string getConnectionStringSQLServerUser()
        {
            return getConnectionStringSQLServer().Substring(0, getConnectionStringSQLServer().IndexOf("User ID=")) + "User ID=" + getUserLoginBanco() + ";Password=" + getSenhaBanco() + ";MultipleActiveResultSets=True;";
        }


        private static string GetNameBancoSQLServer()
        {
            //data source=(localdb)\ProjectsV12;initial catalog=sisgpcv5_0;;
            int posPipe = getConnectionStringSQLServer().IndexOf("initial catalog=") + 16;
            string b = getConnectionStringSQLServer().Substring(posPipe, getConnectionStringSQLServer().Length - posPipe);
            string c = b.Substring(0, b.IndexOf(";"));
            return c;
        }


        private static string getSchema()
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("schemaBanco");
        }

        private static string getUserLoginBanco()
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("userLoginBanco");
        }

        private static string getSenhaBanco()
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("senhaBanco");
        }

        private static string getVersaoApp()
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("VersaoSistema");
        }




        /// <summary>
        /// Inicia Processamento da Fila
        /// </summary>
        /// <param name="EmbeddedScripts"></param>
        public static void run(Dictionary<string, Stream> EmbeddedScripts = null)
        {
            if ((task != null) && (task.IsCompleted == false ||
                                   task.Status == TaskStatus.Running ||
                                   task.Status == TaskStatus.WaitingToRun ||
                                   task.Status == TaskStatus.WaitingForActivation))
            {
                Trace.Write("já iniciada.");
            }
            else
            {
                //Inicia Task
                task = new Task(() => processar());
                task.Start();
                //Cria e atualiza DB, só roda a primeira vez que a aplicação é aberta
                if (EmbeddedScripts != null)
                    CreateDatabaseEmbeddedScripts(EmbeddedScripts);
                else
                    CreateDatabase();
            }
        }

        /// <summary>
        /// Adiciona na fila (Comando SQL)
        /// </summary>
        /// <param name="sql">string SQL</param>
        public static void addQueue(string sql)
        {
            fila.Add(sql);
        }

        /// <summary>
        /// Executa item toda vez que for adicionado a fila (Igual um Listener)
        /// </summary>
        private static void processar()
        {
            //Executa comando para cada item
            foreach (var item in fila.GetConsumingEnumerable())
            {
                execute(item);//Chama a super
            }
        }

        #region Verifica se banco SQLite Existe
        /// <summary>
        /// Verifica se banco não existe e cria-o e executa scripts
        /// </summary>
        public static void CreateDatabase()
        {
            string NameBancoSQLite = GetNameBancoSQLServer();
            string schema = getSchema();
            string user = getUserLoginBanco();
            string pass = getSenhaBanco();
            //bool execScript = false;

            bool existeBanco = SqlClientManager.existsDB(NameBancoSQLite, getConnectionStringSQLServer());

            if (!existeBanco)
            {
                SqlClientManager.createDB(NameBancoSQLite, getConnectionStringSQLServer());
                SqlClientManager.createUser(user, pass, NameBancoSQLite, getConnectionStringSQLServer());
                //execScript = true;
            }
            else
            {
                bool existeUser = SqlClientManager.existsUser(user, getConnectionStringSQLServer());

                if (!existeUser)
                    SqlClientManager.createUser(user, pass, NameBancoSQLite, getConnectionStringSQLServer());
            }

            if (schema.Length > 0)
            {
                bool existeSchema = SqlClientManager.existsSchema(schema, getConnectionStringSQLServer());

                if (!existeSchema)
                {
                    SqlClientManager.createSchema(schema, user, getConnectionStringSQLServer());
                    SqlClientManager.setDefaultSchema(schema, user, getConnectionStringSQLServer());
                    //execScript = true;
                }
                else
                {
                    SqlClientManager.setDefaultSchema(schema, user, getConnectionStringSQLServer());
                }
            }

            CreateOrUpdateDatabase();
        }

        #endregion


        /// <summary>
        /// Verifica se há necessidade de atualizar o banco de dados (Verifica a quantidade de scripts)
        /// </summary>
        public static void CreateOrUpdateDatabase()
        {

            upd_updater updater = super.db.query("SELECT TOP 1 * FROM upd_updater ORDER BY upd_id DESC").ToObject<upd_updater>();

            string[] arquivos_sql = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SQL\\SQLServer"), "*.sql");
            string arquivosExecutados = " ";

            if (arquivos_sql.Count() > updater.ultimo_arquivo)
            {
                for (int i = updater.ultimo_arquivo; i < arquivos_sql.Count(); i++)
                {
                    try
                    {
                        executeThrow(File.ReadAllText(arquivos_sql[i], Encoding.Default));
                        arquivosExecutados += "[" + Path.GetFileName(arquivos_sql[i]) + " =====> Sucesso ]\n";
                    }
                    catch (Exception e)
                    {
                        arquivosExecutados += "[" + Path.GetFileName(arquivos_sql[i]) + " =====> Erro: " + Regex.Replace(e.Message, @"\r\n?|\n", " ") + "]\n";
                    }
                }

                arquivosExecutados = arquivosExecutados.Remove(arquivosExecutados.Length - 1);

                string msg = "Atualização do banco de dados - Executar apenas scripts necessários";
                if (updater.ultimo_arquivo == 0)
                {
                    msg = "Criação do banco de dados - Executar todos os scripts";
                    execute(new upd_updater().createTableSQLServer());
                }

                updater.dataHora = DateTimeUTC.Now();
                updater.descricao = msg;
                updater.versao = "V" + getVersaoApp();
                updater.qtde_arquivos = (arquivos_sql.Count() - updater.ultimo_arquivo);
                updater.ultimo_arquivo = arquivos_sql.Count();
                updater.arquivos = arquivosExecutados;
                updater.schemaAtual = getSchema();

                new super.tb<upd_updater>().insert(updater);
            }
        }





        /// <summary>
        /// Verifica se banco não existe e cria-o e executa scripts
        /// </summary>
        public static void CreateDatabaseEmbeddedScripts(Dictionary<string, Stream> EmbeddedScripts)
        {
            string NameBancoSQLite = GetNameBancoSQLServer();
            string schema = getSchema();
            string user = getUserLoginBanco();
            string pass = getSenhaBanco();

            bool existeBanco = SqlClientManager.existsDB(NameBancoSQLite, getConnectionStringSQLServer());

            if (!existeBanco)
            {
                SqlClientManager.createDB(NameBancoSQLite, getConnectionStringSQLServer());
                SqlClientManager.createUser(user, pass, NameBancoSQLite, getConnectionStringSQLServer());
            }
            else
            {
                bool existeUser = SqlClientManager.existsUser(user, getConnectionStringSQLServer());

                if (!existeUser)
                    SqlClientManager.createUser(user, pass, NameBancoSQLite, getConnectionStringSQLServer());
            }

            if (schema.Length > 0)
            {
                bool existeSchema = SqlClientManager.existsSchema(schema, getConnectionStringSQLServer());

                if (!existeSchema)
                {
                    SqlClientManager.createSchema(schema, user, getConnectionStringSQLServer());
                    SqlClientManager.setDefaultSchema(schema, user, getConnectionStringSQLServer());
                }
                else
                {
                    SqlClientManager.setDefaultSchema(schema, user, getConnectionStringSQLServer());
                }
            }

            CreateOrUpdateDatabase(EmbeddedScripts);
        }


        public static void CreateOrUpdateDatabase(Dictionary<string, Stream> EmbeddedScripts)
        {
            upd_updater updater = super.db.query("SELECT TOP 1 * FROM upd_updater ORDER BY upd_id DESC").ToObject<upd_updater>();

            Dictionary<string, Stream> arquivos_sql = EmbeddedScripts;
            string arquivosExecutados = " ";


            if (arquivos_sql.Count() > updater.ultimo_arquivo)
            {
                for (int i = updater.ultimo_arquivo; i < arquivos_sql.Count(); i++)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(arquivos_sql.Values.ElementAt(i), System.Text.Encoding.Default, true))
                        {
                            executeThrow(sr.ReadToEnd());
                        }

                        arquivosExecutados += "[" + Path.GetFileName(arquivos_sql.Keys.ElementAt(i)) + " =====> Sucesso ]\n";
                    }
                    catch (Exception e)
                    {
                        arquivosExecutados += "[" + Path.GetFileName(arquivos_sql.Keys.ElementAt(i)) + " =====> Erro: " + Regex.Replace(e.Message, @"\r\n?|\n", " ") + "]\n";
                    }
                }

                arquivosExecutados = arquivosExecutados.Remove(arquivosExecutados.Length - 1);

                string msg = "Atualização do banco de dados - Executar apenas scripts necessários";
                if (updater.ultimo_arquivo == 0)
                {
                    msg = "Criação do banco de dados - Executar todos os scripts";
                    execute(new upd_updater().createTableSQLServer());
                }

                updater.dataHora = DateTimeUTC.Now();
                updater.descricao = msg;
                updater.versao = "V" + getVersaoApp();
                updater.qtde_arquivos = (arquivos_sql.Count() - updater.ultimo_arquivo);
                updater.ultimo_arquivo = arquivos_sql.Count();
                updater.arquivos = arquivosExecutados;
                updater.schemaAtual = getSchema();

                new super.tb<upd_updater>().insert(updater);
            }
        }




        /// <summary>
        /// Verifica se há necessidade de atualizar o banco de dados (Verifica a quantidade de scripts)
        /// </summary>
        //[Obsolete]
        //public static void UpdateDatabaseEmbeddedScripts(Dictionary<string, Stream> EmbeddedScripts)
        //{

        //    if (!new super.tb<bac_backup>().selectExists())
        //    {
        //        upd_updater updater = db.query("SELECT TOP 1 * FROM upd_updater ORDER BY upd_id DESC").ToObject<upd_updater>();

        //        //string[] arquivos_sql = Directory.GetFiles(EmbeddedScripts, "*.sql");
        //        Dictionary<string, Stream> arquivos_sql = EmbeddedScripts;

        //        string arquivosExecutados = " ";

        //        if (arquivos_sql.Count() > updater.ultimo_arquivo)
        //        {
        //            for (int i = updater.ultimo_arquivo; i < arquivos_sql.Count(); i++)
        //            {
        //                try
        //                {
        //                    using (StreamReader sr = new StreamReader(arquivos_sql.Values.ElementAt(i)))
        //                    {
        //                        // executeThrow(File.ReadAllText(sr.ReadToEnd(), Encoding.Default));
        //                        executeThrow(sr.ReadToEnd());
        //                    }

        //                    arquivosExecutados += "[" + Path.GetFileName(arquivos_sql.Keys.ElementAt(i)) + " =====> Sucesso ]\n";
        //                }
        //                catch (Exception e)
        //                {
        //                    arquivosExecutados += "[" + Path.GetFileName(arquivos_sql.Keys.ElementAt(i)) + " =====> Erro: " + Regex.Replace(e.Message, @"\r\n?|\n", " ") + "]\n";
        //                }
        //            }

        //            arquivosExecutados = arquivosExecutados.Remove(arquivosExecutados.Length - 1);

        //            updater.dataHora = DateTime.Now;
        //            updater.descricao = "Atualização do banco de dados - Executado scripts necessários";
        //            updater.versao = "V4.0";
        //            updater.qtde_arquivos = (arquivos_sql.Count() - updater.ultimo_arquivo);
        //            updater.ultimo_arquivo = arquivos_sql.Count();
        //            updater.arquivos = arquivosExecutados;

        //            new super.tb<upd_updater>().insert(updater);
        //        }
        //    }
        //}


       


        /// <summary>
        /// Prepara DB
        /// </summary>
        public static void preparedDB()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomSuper\frameworkDB"), "*.sql"))
            {
                execute(File.ReadAllText(file, Encoding.Default));
            }
        }


        /// <summary>
        /// Executa procedure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable procedure(string nameProcedure, Dictionary<string, object> list)
        {
            string sql = " EXEC " + nameProcedure;
            string camposValues = " ";

            foreach (KeyValuePair<string, object> item in list)
            {
                camposValues += "@" + item.Key + " = '" + item.Value.IsValueValidType() + "' ,";
            }
            camposValues = camposValues.Remove(camposValues.Length - 1);

            return query(sql + camposValues);
        }







        public static List<U> queryNew<U>(string w, object arg0, params U[] anonymous)
        {
            w = w.formatString(arg0);
            return queryNew(w, anonymous);
        }

        public static List<U> queryNew<U>(string w, object arg0, object arg1, params U[] anonymous)
        {
            w = w.formatString(arg0, arg1);
            return queryNew(w, anonymous);
        }

        public static List<U> queryNew<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous)
        {
            w = w.formatString(arg0, arg1, arg2);
            return queryNew(w, anonymous);
        }

        public static List<U> queryNew<U>(params U[] anonymous)
        {
            return queryNew("", anonymous);
        }

        public static List<U> queryNew<U>(string query, params U[] anonymous)
        {
            query = query.Replace("SELECT", "").Replace("@anonymous", "");

            string sql = " SELECT ";

            string cols = "";
            foreach (var col in typeof(U).GetProperties())
            {
                cols += " " + col.Name + ",";
            }

            cols = cols.Remove(cols.Length - 1);

            DataTable dt = db.query(sql + cols + query);

            return dt.ToAnonymousList<U>(anonymous);
        }




        public static SelectList selectCombo(string table, string id, string descricao, object selected)
        {
            string sql = " SELECT ";

            string cols = " " + id + " as value,";
            cols += " " + descricao + " as text ";

            string t = " FROM " + table + " ";

            DataTable dt = db.query(sql + cols + t);

            return new SelectList(dt.ToList<comboBox>(), "value", "text", selected);
        }

        public static SelectList selectCombo(string table, string id, string descricao)
        {
            string sql = " SELECT ";

            string cols = " " + id + " as value,";
            cols += " " + descricao + " as text ";

            string t = " FROM " + table + " ";

            DataTable dt = db.query(sql + cols + t);

            return new SelectList(dt.ToList<comboBox>(), "value", "text");
        }





        public static SelectList queryCombo(string sql, params object[] args)
        {
            sql = sql.formatString(args);
            return queryCombo(sql, 0);
        }

        public static SelectList queryCombo(string sql, int selected = 0, params object[] args)
        {
            sql = sql.formatString(args);

            DataTable dt = db.query(sql);
            object selectedObj = null;
            if (selected != 0)
                selectedObj = selected;

            return new SelectList(dt.ToList<comboBox>(), "value", "text", selectedObj);
        }



        public static SelectList selectComboActive(object selected = null)
        {
            return new SelectList(new List<SelectListItem>
                                                {
                                                    new SelectListItem { Text = "Inativo", Value = Boolean.FalseString},
                                                    new SelectListItem { Text = "Ativo", Value = Boolean.TrueString},
                                                }, "Value", "Text", selected);
        }

        public static SelectList selectComboBool(object selected = null)
        {
            return new SelectList(new List<SelectListItem>
                                                {
                                                    new SelectListItem { Text = "Não", Value = Boolean.FalseString},
                                                    new SelectListItem { Text = "Sim", Value = Boolean.TrueString},
                                                }, "Value", "Text", selected);
        }

        public static SelectList selectComboSexo(object selected = null)
        {
            return new SelectList(new List<SelectListItem>
                                                {
                                                    new SelectListItem { Text = "Feminino", Value = Boolean.FalseString},
                                                    new SelectListItem { Text = "Masculino", Value = Boolean.TrueString},
                                                }, "Value", "Text", selected);
        }

        public static SelectList selectNaturezaOperacao(object selected = null)
        {
            return new SelectList(new List<SelectListItem>
                                                {
                                                    new SelectListItem { Text = "Débito", Value = Boolean.FalseString},
                                                    new SelectListItem { Text = "Crédito", Value = Boolean.TrueString},
                                                }, "Value", "Text", selected);
        }


        /// <summary>
        /// Executa query com base no T-SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable query(string sql, params object[] args)
        {
            sql = sql.formatString(args);
            return query(sql);
        }

        public static DataTable query(string sql)
        {
            DataTable dt = new DataTable();

            SqlConnection con = new SqlConnection(getConnectionStringSQLServerUser());
            SqlCommand comm = new SqlCommand(sql, con);
            con.Open();
            try
            {
                dt.Load(comm.ExecuteReader());
            }
            catch (Exception)
            {
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return dt;
        }


        public static int execute(string sql, params object[] args)
        {
            sql = sql.formatString(args);
            return execute(sql);
        }
        /// <summary>
        /// Executa comando com base no T-SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int executeThrow(string sql)
        {
            int ret = 0;

            SqlConnection con = new SqlConnection(getConnectionStringSQLServerUser());
            // SqlCommand comm = new SqlCommand("SET DATEFORMAT DMY; " + sql, con);
            SqlCommand comm = new SqlCommand(sql, con);
            con.Open();
            //SqlTransaction transaction = con.BeginTransaction("Transaction");
            //comm.Transaction = transaction;
            try
            {
                ret = Convert.ToInt32(comm.ExecuteScalar());
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                throw e;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return ret;
        }



        public static int execute(string sql)
        {
            int ret = 0;

            SqlConnection con = new SqlConnection(getConnectionStringSQLServerUser());
            SqlCommand comm = new SqlCommand(sql, con);
            con.Open();
            //SqlTransaction transaction = con.BeginTransaction("Transaction");
            //comm.Transaction = transaction;
            try
            {
                ret = Convert.ToInt32(comm.ExecuteScalar());
                //transaction.Commit();
            }
            catch (Exception)
            {
                //transaction.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return ret;
        }

        public static async Task<int> executeAsync(string sql)
        {
            int ret = 0;


            SqlConnection con = new SqlConnection(getConnectionStringSQLServerUser());
            SqlCommand comm = new SqlCommand("SET DATEFORMAT DMY; " + sql, con);
            con.Open();
            SqlTransaction transaction = con.BeginTransaction("Transaction");
            comm.Transaction = transaction;
            try
            {
                Task<object> task = comm.ExecuteScalarAsync();
                ret = Convert.ToInt32(await task);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return ret;
        }
    }
}

