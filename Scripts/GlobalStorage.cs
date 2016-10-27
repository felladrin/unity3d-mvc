using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

public class GlobalStorage : MonoBehaviour
{
    public bool DebugMode;

    [System.Serializable]
    class GlobalStorageNumber
    {
        [HideInInspector]
        public string Key;
        public double Val;

        public GlobalStorageNumber(string key, double val)
        {
            this.Key = key;
            this.Val = val;
        }
    }

    [System.Serializable]
    class GlobalStorageString
    {
        [HideInInspector]
        public string Key;
        public string Val;

        public GlobalStorageString(string key, string val)
        {
            this.Key = key;
            this.Val = val;
        }
    }

    [System.Serializable]
    class GlobalStorageBoolean
    {
        [HideInInspector]
        public string Key;
        public bool Val;

        public GlobalStorageBoolean(string key, bool val)
        {
            this.Key = key;
            this.Val = val;
        }
    }

    [SerializeField]
    List<GlobalStorageNumber> numbers;

    [SerializeField]
    List<GlobalStorageString> strings;

    [SerializeField]
    List<GlobalStorageBoolean> booleans;

    [SerializeField]
    List<string> objects;

    #region Singleton definition
    static GlobalStorage instance;

    void GrantSingleInstance()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    void Awake()
    {
        GrantSingleInstance();
        DontDestroyOnLoad(gameObject);
        LoadSelf();
    }

    public static void Save<T>(string key, T data)
    {
        if (String.IsNullOrEmpty(key))
            return;

        string savePath = GetSavePath(key);

        Delete(key);

        if (TypeCodeIsNumeric(typeof(T)))
        {
            instance.numbers.Add(new GlobalStorageNumber(key, Convert.ToDouble(data)));
            SaveSelf();
        }
        else if (TypeCodeIsString(typeof(T)))
        {
            instance.strings.Add(new GlobalStorageString(key, Convert.ToString(data)));
            SaveSelf();
        }
        else if (TypeCodeIsBoolean(typeof(T)))
        {
            instance.booleans.Add(new GlobalStorageBoolean(key, Convert.ToBoolean(data)));
            SaveSelf();
        }
        else
        {
            if (data == null)
                return;

            try
            {
                if (!key.Equals(GetSelfName()))
                    instance.objects.Add(key);

                string serializedData = JsonUtility.ToJson(data, true);
                File.WriteAllText(savePath, serializedData);
                DebugMessage("Data stored to " + savePath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    public static T Load<T>(string key)
    {
        T data = default(T);

        if (String.IsNullOrEmpty(key))
            return data;

        string savePath = GetSavePath(key);

        try
        {
            if (TypeCodeIsNumeric(typeof(T)))
            {
                data = (T)Convert.ChangeType(instance.numbers.Find(x => x.Key == key).Val, typeof(T));
                DebugMessage("Data loaded from Numbers Array");
            }
            else if (TypeCodeIsString(typeof(T)))
            {
                data = (T)Convert.ChangeType(instance.strings.Find(x => x.Key == key).Val, typeof(T));
                DebugMessage("Data loaded from Strings Array");
            }
            else if (TypeCodeIsBoolean(typeof(T)))
            {
                data = (T)Convert.ChangeType(instance.booleans.Find(x => x.Key == key).Val, typeof(T));
                DebugMessage("Data loaded from Booleans Array");
            }
            else
            {
                if (File.Exists(savePath))
                {
                    string serializedData = File.ReadAllText(savePath);
                    data = JsonUtility.FromJson<T>(serializedData);
                    DebugMessage("Data loaded from " + savePath);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

        return data;
    }


    public static void Delete(string key)
    {
        if (String.IsNullOrEmpty(key))
            return;

        try
        {
            if (instance.numbers.RemoveAll(x => x.Key == key) > 0 || instance.strings.RemoveAll(x => x.Key == key) > 0 || instance.booleans.RemoveAll(x => x.Key == key) > 0)
            {
                SaveSelf();
            }
            else
            {
                string savePath = GetSavePath(key);
                if (File.Exists(savePath))
                {
                    instance.objects.Remove(key);
                    File.Delete(savePath);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }

    static string GetSavePath(string fileName)
    {
        if (String.IsNullOrEmpty(fileName))
            return null;

        return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, fileName + ".json");
    }

    static bool TypeCodeIsNumeric(Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            default:
                return false;
        }
    }

    static bool TypeCodeIsString(Type type)
    {
        if (Type.GetTypeCode(type) == TypeCode.String || Type.GetTypeCode(type) == TypeCode.Char)
            return true;

        return false;
    }

    static bool TypeCodeIsBoolean(Type type)
    {
        if (Type.GetTypeCode(type) == TypeCode.Boolean)
            return true;

        return false;
    }

    static void SaveSelf()
    {
        Save(GetSelfName(), instance);
    }

    static void LoadSelf()
    {
        try
        {
            string selfName = GetSelfName();
            string savePath = GetSavePath(selfName);
            if (File.Exists(savePath))
            {
                string serializedData = File.ReadAllText(savePath);
                JsonUtility.FromJsonOverwrite(serializedData, instance);
                FileInfo[] fileArray = new DirectoryInfo(UnityEngine.Application.persistentDataPath).GetFiles("*.json");
                foreach (FileInfo fileInfo in fileArray)
                {
                    string fileName = fileInfo.Name.Replace(".json", "");
                    if (!fileName.Equals(selfName))
                        instance.objects.Add(fileName);
                }
                DebugMessage("Data loaded from " + savePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }

    static string GetSelfName()
    {
        return MethodBase.GetCurrentMethod().DeclaringType.ToString();
    }

    static void DebugMessage(string message)
    {
        if (instance.DebugMode)
            Debug.Log(message);
    }
}
