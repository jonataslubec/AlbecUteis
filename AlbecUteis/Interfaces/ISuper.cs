using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;


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
    public interface ITb<T> : IDisposable where T : new()
    {
         int scopeIdentity();
         void insert(T t);
         void insertQueue(T t);
         void insertAsync(T t);
         void insertDelegate(T t);
         void update(T t);
         void updateQueue(T t);
         void updateAsync(T t);
         void updateDelegate(T t);
         void delete(int id);
         void deleteQueue(int id);
         void deleteAsync(int id);
         void deleteDelegate(int id);
         void delete(string w = "", params object[] args);
         void deleteIn<U>(string w = "", params U[] args);
         List<T> select(string w = "", params object[] args);
         T selectObject(string w = "", params object[] args);
         List<U> selectNew<U>(string w, object arg0, params U[] anonymous);
         List<U> selectNew<U>(string w, object arg0, object arg1, params U[] anonymous);
         List<U> selectNew<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous);
         List<U> selectNew<U>(params U[] anonymous);
         List<U> selectNew<U>(string w = "", params U[] anonymous);
         U selectNewObject<U>(string w, object arg0, params U[] anonymous);
         U selectNewObject<U>(string w, object arg0, object arg1, params U[] anonymous);
         U selectNewObject<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous);
         U selectNewObject<U>(params U[] anonymous);
         U selectNewObject<U>(string w, params U[] anonymous);
         U selectMax<U>(string w, object arg0, params U[] anonymous);
         U selectMax<U>(string w, object arg0, object arg1, params U[] anonymous);
         U selectMax<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous);
         U selectMax<U>(string w, params U[] anonymous);
         int selectCount(string w = "", params object[] args);
         bool selectExists(string w = "", params object[] args);
         List<comboBox> selectCombo(string id, string descricao);
         List<U> innerJoin<U>(string w, string arg0, string C1, string T2, string C2, params U[] anonymous);
         T findById(int? id);
         U findById<U>(int? id, params U[] anonymous);
         T find(string w = "", params object[] args);
         //void Dispose();
    }



    public interface IDb
    {
         void run(Dictionary<string, Stream> EmbeddedScripts = null);
         void addQueue(string sql);
         void CreateDatabase();
         void CreateOrUpdateDatabase();
         void CreateDatabaseEmbeddedScripts(Dictionary<string, Stream> EmbeddedScripts);
         void UpdateDatabaseEmbeddedScripts(Dictionary<string, Stream> EmbeddedScripts);
         void preparedDB();
         DataTable procedure(string nameProcedure, Dictionary<string, object> list);
         List<U> queryNew<U>(string w, object arg0, params U[] anonymous);
         List<U> queryNew<U>(string w, object arg0, object arg1, params U[] anonymous);
         List<U> queryNew<U>(string w, object arg0, object arg1, object arg2, params U[] anonymous);
         List<U> queryNew<U>(params U[] anonymous);
         List<U> queryNew<U>(string query, params U[] anonymous);
         SelectList selectCombo(string table, string id, string descricao, object selected);
         SelectList selectCombo(string table, string id, string descricao);
         SelectList queryCombo(string sql, params object[] args);
         SelectList queryCombo(string sql, int selected = 0, params object[] args);
         SelectList selectComboActive(object selected = null);
         SelectList selectComboBool(object selected = null);
         SelectList selectComboSexo(object selected = null);
         DataTable query(string sql, params object[] args);
         DataTable query(string sql);
         int execute(string sql, params object[] args);
         int executeThrow(string sql);
         int execute(string sql);
         Task<int> executeAsync(string sql);
    }
}

