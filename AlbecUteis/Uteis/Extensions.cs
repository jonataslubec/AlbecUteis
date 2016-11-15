using Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

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

public static class Extensions
{

    /// <summary>
    /// Gera JSON com base no objeto
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToJson(this object obj)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }

    public static string ToJson<T>(this List<T> t, bool @interface = false)
    {
        if (@interface)
            return JsonConvert.SerializeObject(t, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        else
            return JsonConvert.SerializeObject(t);
    }

    public static string ToJson<T>(this T t, bool @interface = false)
    {
        if (@interface)
            return JsonConvert.SerializeObject(t, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        else
            return JsonConvert.SerializeObject(t);
    }


    /// <summary>
    /// Gera Object com base no JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T FromJson<T>(this string json)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }


    public static List<T> FromJson<T>(this string jsonValue, bool @interface = false)
    {
        if (@interface)
            return JsonConvert.DeserializeObject<List<T>>(jsonValue, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        else
            return JsonConvert.DeserializeObject<List<T>>(jsonValue);
    }

    public static T FromJSON_Object<T>(this string jsonValue, bool @interface = false)
    {
        if (@interface)
            return JsonConvert.DeserializeObject<T>(jsonValue, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        else
            return JsonConvert.DeserializeObject<T>(jsonValue);
    }



    /// <summary>
    /// Gera XML com base no Object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToXml(this object obj)
    {
        var stringwriter = new System.IO.StringWriter();
        var serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(stringwriter, obj);
        return stringwriter.ToString();
    }


    /// <summary>
    /// Gera Object com base no XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static T FromXml<T>(this string xml)
    {
        var stringReader = new System.IO.StringReader(xml);
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(stringReader);
    }





    /// <summary>
    /// Gera Object Generico com base no Datatable (Recupera as properties com até 3 níveis)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public static T ToObject<T>(this DataTable dataTable) where T : new()
    {
        var classObj = new T();

        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
            Select(item => new
            {
                Name = item.ColumnName,
                Type = item.DataType
            }).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            int countSetValue = 0;
            //Nivel 1
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dtlFieldNames.Find(x => x.Name == prop.getColumnName()) != null)
                {
                    setValueProperty(prop, classObj, dataRow);

                    if (exitFor(dataTable, ref countSetValue))
                    {
                        break;
                    }
                }
                else
                {
                    //Nivel 2
                    var addCampos = false;
                    try
                    {
                        var classNested = InstanciaClass(prop.PropertyType.Name);

                        foreach (PropertyInfo propNested in prop.PropertyType.GetProperties())
                        {
                            if (dtlFieldNames.Find(x => x.Name == propNested.getColumnName()) != null)
                            {
                                addCampos = true;

                                setValueProperty(propNested, classNested, dataRow);

                                if (exitFor(dataTable, ref countSetValue))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                //Nivel 3
                                var addCampos3 = false;

                                try
                                {
                                    var classNestedNivel3 = InstanciaClass(propNested.PropertyType.Name);

                                    foreach (PropertyInfo propNestedNivel3 in propNested.PropertyType.GetProperties())
                                    {
                                        if (dtlFieldNames.Find(x => x.Name == propNestedNivel3.getColumnName()) != null)
                                        {
                                            addCampos3 = true;

                                            setValueProperty(propNestedNivel3, classNestedNivel3, dataRow);

                                            if (exitFor(dataTable, ref countSetValue))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (addCampos3)
                                        classNested.GetType().GetProperty(propNested.Name).SetValue(classNested, classNestedNivel3, null);
                                }
                                catch (Exception) {
                                    continue;
                                }
                            }
                        }
                        if (addCampos)
                            classObj.GetType().GetProperty(prop.Name).SetValue(classObj, classNested, null);
                    }
                    catch (Exception) {
                        continue;
                    }
                }
            }
            break;
        }
        return classObj;
    }








    /// <summary>
    /// Gera List<Objects> Generico com base no Datatable (Recupera as properties com até 3 níveis)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this DataTable dataTable) where T : new()
    {
        var dataList = new List<T>();

        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
            Select(item => new
            {
                Name = item.ColumnName,
                Type = item.DataType
            }).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            var classObj = new T();

            int countSetValue = 0;

            //Nivel 1
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dtlFieldNames.Find(x => x.Name == prop.getColumnName()) != null)
                {
                    setValueProperty(prop, classObj, dataRow);

                    if (exitFor(dataTable, ref countSetValue))
                    {
                        break;
                    }
                }
                else
                {
                    //Nivel 2
                    var addCampos = false;
                    try
                    {
                        var classNested = InstanciaClass(prop.PropertyType.Name);

                        foreach (PropertyInfo propNested in prop.PropertyType.GetProperties())
                        {
                            if (dtlFieldNames.Find(x => x.Name == propNested.getColumnName()) != null)
                            {
                                addCampos = true;

                                setValueProperty(propNested, classNested, dataRow);

                                if (exitFor(dataTable, ref countSetValue))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                //Nivel 3
                                var addCampos3 = false;

                                try
                                {
                                    var classNestedNivel3 = InstanciaClass(propNested.PropertyType.Name);

                                    foreach (PropertyInfo propNestedNivel3 in propNested.PropertyType.GetProperties())
                                    {
                                        if (dtlFieldNames.Find(x => x.Name == propNestedNivel3.getColumnName()) != null)
                                        {
                                            addCampos3 = true;

                                            setValueProperty(propNestedNivel3, classNestedNivel3, dataRow);

                                            if (exitFor(dataTable, ref countSetValue))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (addCampos3)
                                        classNested.GetType().GetProperty(propNested.Name).SetValue(classNested, classNestedNivel3, null);
                                }
                                catch (Exception) { }
                            }
                        }
                        if (addCampos)
                            classObj.GetType().GetProperty(prop.Name).SetValue(classObj, classNested, null);
                    }
                    catch (Exception) { }
                }
            }

            dataList.Add(classObj);
        }

        return dataList;
    }



    /// <summary>
    /// Gera List Object dinamico (Anonymous Types) com base no Datatable (Utiliza Class ExpandoObject() Sensacional)
    /// </summary>
    /// <typeparam name="T">Tipo para Retorno</typeparam>
    /// <param name="dataTable"></param>
    /// <param name="anonymous"></param>
    /// <returns></returns>
    public static List<T> ToAnonymousList<T>(this DataTable dataTable, params T[] anonymous) /*where T : class*/
    {
        var dataList = new List<T>();

        dynamic modelo = (dynamic)anonymous[0];

        List<dynamic> list = new System.Collections.Generic.List<dynamic>();

        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
            Select(item => new
            {
                Name = item.ColumnName,
                Type = item.DataType
            }).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            int countSetValue = 0;

            dynamic dynamicExpando = new ExpandoObject();
            dynamic result = "";

            //Nivel 1
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dtlFieldNames.Find(x => x.Name == prop.getColumnName()) != null)
                {
                    ((IDictionary<string, object>)dynamicExpando).Add(prop.Name, setValue(prop.PropertyType, prop.Name, dataRow));

                    if (exitFor(dataTable, ref countSetValue))
                    {
                        break;
                    }
                }
            }

            dataList.Add((T)ConvertToExpando(dynamicExpando, modelo));
        }

        return dataList;
    }


    /// <summary>
    /// Gera List Object dinamico (Anonymous Types) com base no Datatable (Utiliza Class ExpandoObject() Sensacional)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"></param>
    /// <param name="anonymous"></param>
    /// <returns></returns>
    public static T ToAnonymousObject<T>(this DataTable dataTable, params T[] anonymous) /*where T : class*/
    {
        var dataList = new List<T>();

        dynamic modelo = (dynamic)anonymous[0];

        List<dynamic> list = new System.Collections.Generic.List<dynamic>();

        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
            Select(item => new
            {
                Name = item.ColumnName,
                Type = item.DataType
            }).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            int countSetValue = 0;

            dynamic dynamicExpando = new ExpandoObject();
            dynamic result = "";

            //Nivel 1
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dtlFieldNames.Find(x => x.Name == prop.getColumnName()) != null)
                {
                    ((IDictionary<string, object>)dynamicExpando).Add(prop.Name, setValue(prop.PropertyType, prop.Name, dataRow));

                    if (exitFor(dataTable, ref countSetValue))
                    {
                        break;
                    }
                }
            }

            dataList.Add((T)ConvertToExpando(dynamicExpando, modelo));
            return dataList[0];
        }

        return (T)modelo;
    }

    /// <summary>
    /// Gera List<Object> dinamico (Anonymous Types) com base no Datatable (Utiliza Class ExpandoObject() Sensacional)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"></param>
    /// <param name="anonymous"></param>
    /// <returns></returns>
    public static dynamic ToDynamicList<T>(this DataTable dataTable, params T[] anonymous) /*where T : class*/
    {
        var dataList = new List<T>();

        dynamic modelo = (dynamic)anonymous[0];

        List<dynamic> list = new System.Collections.Generic.List<dynamic>();

        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
            Select(item => new
            {
                Name = item.ColumnName,
                Type = item.DataType
            }).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            int countSetValue = 0;

            dynamic dynamicExpando = new ExpandoObject();
            dynamic result = "";

            //Nivel 1
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dtlFieldNames.Find(x => x.Name == prop.getColumnName()) != null)
                {
                    ((IDictionary<string, object>)dynamicExpando).Add(prop.Name, setValue(prop.PropertyType, prop.Name, dataRow));

                    if (exitFor(dataTable, ref countSetValue))
                    {
                        break;
                    }
                }
            }

            dataList.Add((T)ConvertToExpando(dynamicExpando, modelo));
        }


        List<ExpandoObject> joinData = new List<ExpandoObject>();

        foreach (var item in dataList)
        {
            IDictionary<string, object> itemExpando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(item.GetType()))
            {
                itemExpando.Add(property.Name, property.GetValue(item));
            }
            joinData.Add(itemExpando as ExpandoObject);
        }


        dynamic model = new ExpandoObject();
        model.Dynamic = joinData;

        return model;
    }



    /// <summary>
    /// Gera List<Object> dinamico (Anonymous Types) com base no Datatable (Utiliza Class ExpandoObject() Sensacional)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"></param>
    /// <param name="anonymous"></param>
    /// <returns></returns>
    public static dynamic ToDynamicObject<T>(this DataTable dataTable, params T[] anonymous) /*where T : class*/
    {
        var dataList = new List<T>();

        dynamic modelo = (dynamic)anonymous[0];

        List<dynamic> list = new System.Collections.Generic.List<dynamic>();

        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
            Select(item => new
            {
                Name = item.ColumnName,
                Type = item.DataType
            }).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            int countSetValue = 0;

            dynamic dynamicExpando = new ExpandoObject();
            dynamic result = "";

            //Nivel 1
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dtlFieldNames.Find(x => x.Name == prop.getColumnName()) != null)
                {
                    ((IDictionary<string, object>)dynamicExpando).Add(prop.Name, setValue(prop.PropertyType, prop.Name, dataRow));

                    if (exitFor(dataTable, ref countSetValue))
                    {
                        break;
                    }
                }
            }

            dataList.Add((T)ConvertToExpando(dynamicExpando, modelo));
            break;
        }



        IDictionary<string, object> expandoObject = new ExpandoObject();

        foreach (var item in dataList)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(item.GetType()))
            {
                expandoObject.Add(property.Name, property.GetValue(item));
            }
        }

        dynamic model = new ExpandoObject();
        model.Dynamic = expandoObject;

        return model;
    }






    /// <summary>
    /// Convert Object Expando para AnonymousTypes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="example"></param>
    /// <returns></returns>
    private static T ConvertToExpando<T>(ExpandoObject source, T modelo) where T : class
    {
        IDictionary<string, object> dict = source;

        var ctor = modelo.GetType().GetConstructors().Single();

        var parameters = ctor.GetParameters();

        var parameterValues = parameters.Select(p => dict[p.Name]).ToArray();

        return (T)ctor.Invoke(parameterValues);
    }


    /// <summary>
    /// Verifica se é necessário sair do for
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="countSetValue"></param>
    /// <returns></returns>
    private static bool exitFor(DataTable dataTable, ref int countSetValue)
    {
        countSetValue += 1;

        if (countSetValue == dataTable.Columns.Count) //4 = 4
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Instancia Class Dinamicamente
    /// </summary>
    /// <param name="nome"></param>
    /// <returns></returns>
    public static object InstanciaClassOld(string nome)
    {
        var classe = Assembly.GetExecutingAssembly().GetTypes().First(x => x.Name == nome);
        var instancia = Activator.CreateInstance(classe);
        return instancia;
    }

    public static object InstanciaClass(string nome)
    {
        if (valideType(nome) == null)
            throw new Exception();

        string appName = "ProjetoDDD.Domain"; //System.Web.HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;

        string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"bin") + "\\" + appName + ".dll";

        Assembly assembly = Assembly.LoadFrom(assemblyPath);

        var classe = assembly.GetTypes().First(x => x.Name == nome);

        return Activator.CreateInstance(classe);
    }


    public static string valideType(string type)
    {
        switch (type.ToLower())
        {
            case "string":
                return null;
            case "int":
                return null;
            case "int16":
                return null;
            case "int32":
                return null;
            case "int64":
                return null;
            case "boolean":
                return null;
            case "double":
                return null;
            case "float":
                return null;
            case "decimal":
                return null;
            default:
                return type;
        }
    }


    /// <summary>
    /// Seta Valor para Property Anonymous Types dinamicamente
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nameElement"></param>
    /// <param name="anondata"></param>
    /// <param name="dataRow"></param>
    private static void setValuePropertyAnonymous<T>(string nameElement, T anondata, DataRow dataRow)
    {
        string fieldName = "<" + nameElement + ">i__Field";
        Type t = typeof(T);

        FieldInfo field = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        field.SetValue(anondata, setValue(field.FieldType, nameElement, dataRow));
    }


    /// <summary>
    /// Seta valor para Property dinamicamente
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prop"></param>
    /// <param name="classObj"></param>
    /// <param name="dataRow"></param>
    private static void setValueProperty<T>(PropertyInfo prop, T classObj, DataRow dataRow)
    {
        prop.SetValue(classObj, setValue(prop.PropertyType, prop.getColumnName(), dataRow), null);
    }




    /// <summary>
    /// Valida e convert tipos primitivos
    /// </summary>
    /// <param name="type"></param>
    /// <param name="columnName"></param>
    /// <param name="dataRow"></param>
    /// <returns></returns>
    public static dynamic setValue(Type type, string columnName, DataRow dataRow)
    {
        if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            try
            {
                return Convert.ToDateTime(dataRow[columnName].IsNullObjectDateTime());
            }
            catch (Exception)
            {
                return default(DateTime?);
            }
        }
        else if (type == typeof(int) || type == typeof(int?))
        {
            return Convert.ToInt32(dataRow[columnName].IsNullObjectInteger());
        }
        else if (type == typeof(bool) || type == typeof(bool?))
        {
            return Convert.ToBoolean(dataRow[columnName].IsNullObjectBool());
        }
        else if (type == typeof(long) || type == typeof(long?))
        {
            return Convert.ToInt64(dataRow[columnName].IsNullObjectInteger());
        }
        else if (type == typeof(decimal) || type == typeof(decimal?))
        {
            return Convert.ToDecimal(dataRow[columnName].IsNullObjectDecimal());
        }
        else if (type == typeof(double) || type == typeof(double?))
        {
            try
            {
                return Convert.ToDouble(dataRow[columnName].IsNullObjectDouble());
            }
            catch (Exception)
            {
                return default(double);
            }
        }
        else if (type == typeof(float) || type == typeof(float?))
        {
            try
            {
                return Convert.ToSingle(dataRow[columnName].IsNullObjectFloat());
            }
            catch (Exception)
            {
                return default(float);
            }
        }
        else if (type == typeof(byte[]))
        {
            return Convert.ToByte(dataRow[columnName].IsNullObject());
        }
        else if (type == typeof(String))
        {
            if (dataRow[columnName].GetType() == typeof(DateTime))
            {
                if (dataRow[columnName] == null)
                    return string.Empty;

                return Convert.ToDateTime(dataRow[columnName].IsNullObject());
            }
            else
            {
                return Convert.ToString(dataRow[columnName].IsNullObject());
            }
        }
        return Convert.ToString(dataRow[columnName].IsNullObject());
    }


    /// <summary>
    /// Verifica se Objeto é null ou DBnull caso positivo retorna String.empty
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object IsNullObject(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}"  ? "" : obj);
    }

    public static bool IsNull(this object obj)
    {
        return (obj == null);
    }

    public static object IsNullObjectInteger(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}" || obj.ToString() == "" ? "0" : obj);
    }

    public static object IsNullObjectBool(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}" || obj.ToString() == "" ? false : obj);
    }

    public static object IsNullObjectDecimal(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}" || obj.ToString() == "" ? "0.0" : obj);
    }

    public static object IsNullObjectDouble(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}" || obj.ToString() == "" ? "0.0" : obj);
    }

    public static object IsNullObjectFloat(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}" || obj.ToString() == "" ? "0.0" : obj);
    }

    public static object IsNullObjectDateTime(this object obj)
    {
        return (obj == null || obj == DBNull.Value || obj.ToString() == "{}" || obj.ToString() == "" ? new DateTime() : obj);
    }


    //public static int Int(this object obj)
    //{
    //    return (obj == null ? Convert.ToInt32(obj) : default(int));
    //}





    /// <summary>
    /// Verifica se objeto é null ou Dbnull, 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object IsValueValidType(this object obj)
    {
        if (obj.IsNullObject().GetType() == typeof(string))
        {
            return (obj == null || obj == DBNull.Value ? "" : obj);
        }

        if (obj.IsNullObject().GetType() == typeof(int) || obj.IsNullObject().GetType() == typeof(long))
        {
            return (obj == null || obj == DBNull.Value ? 0 : obj);
        }

        if (obj.IsNullObject().GetType() == typeof(bool))
        {
            return (obj == null || obj == DBNull.Value ? 0 : obj);
        }

        if (obj.IsNullObject().GetType() == typeof(byte[]))
        {
            return (obj == null || obj == DBNull.Value ? default(byte) : obj);
        }

        if (obj.IsNullObject().GetType() == typeof(DateTime) || obj.IsNullObject().GetType() == typeof(DateTime?))
        {
            return (obj == null || obj == DBNull.Value ? default(DateTime).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(obj, new CultureInfo("pt-BR")).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        if (obj.IsNullObject().GetType() == typeof(decimal) || obj.IsNullObject().GetType() == typeof(double) || obj.IsNullObject().GetType() == typeof(float))
        {
            return (obj == null || obj == DBNull.Value ? "0.0" : obj.ToString().Replace(".", "").Replace(",", "."));//1.000,00  > 1000.00
        }

        return obj;
    }


    /// <summary>
    /// Verifica se Entity mapeada tem Atributos nas propertys, 
    /// caso positivo retorna o atributo, negativo a propria property
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static string getColumnName(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute(typeof(TableColumn)) as TableColumn;
        return (attr != null && attr.columnName != null && attr.columnName.Length > 0 ? attr.columnName : prop.Name);
    }


    /// <summary>
    /// Retorna property mapeada como Primary Key
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static bool isPrimaryKey(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute(typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
        return (attr != null ? true : false);
    }


    /// <summary>
    /// Verifica se a property tem o atributo NotMapped, caso positivo retorna false 
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static bool isForeignKey(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;
        return (attr != null ? true : false);
    }

    /// <summary>
    /// Verifica se a property tem o atributo NotMapped, caso positivo retorna false 
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static bool isMapped(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute(typeof(NotMappedAttribute)) as NotMappedAttribute;
        return (attr != null ? false : true);
    }

    public static string formatString(this string w, params object[] args)
    {
        if (w.Length == 0)
            return "";

        object[] argsAux = args;

        for (int i = 0; i < args.Length; i++)
            argsAux[i] = args[i].IsValueValidType();

        if (args.Length > 0)
            return w = string.Format(w, argsAux);
        else
            return w;
    }





    /// <summary>
    /// Retorna property mapeada como Primary Key
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static object ValidParameter(this object obj)
    {
        return obj.ToString().Replace("'", "");
    }


    /// <summary>
    /// Retorna MD5 de qualquer String
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string MD5(this string input)
    {
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }


    /// <summary>
    /// compara Between
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool ToBetween(this object obj, object val1, object val2)
    {
        return (Convert.ToInt64(obj) >= Convert.ToInt64(val1) && Convert.ToInt64(obj) <= Convert.ToInt64(val1));
    }

    /// <summary>
    /// compara Between
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToYMD(this DateTime obj)
    {
        return obj.ToString("yyyy/MM/dd");
    }

    /// <summary>
    /// compara Between
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToDMY(this DateTime obj)
    {
        return obj.ToString("dd/MM/yyyy");
    }

}

