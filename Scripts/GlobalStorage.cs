using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

public class GlobalStorage : MonoBehaviour
{
    public bool DebugMode;

    [Serializable]
    private class GlobalStorageNumber
    {
        [HideInInspector] public string Key;
        public double Value;

        public GlobalStorageNumber(string key, double value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    private class GlobalStorageString
    {
        [HideInInspector] public string Key;
        public string Value;

        public GlobalStorageString(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    private class GlobalStorageBoolean
    {
        [HideInInspector] public string Key;
        public bool Value;

        public GlobalStorageBoolean(string key, bool value)
        {
            Key = key;
            Value = value;
        }
    }

    [SerializeField] private List<GlobalStorageNumber> numbers = new List<GlobalStorageNumber>();

    [SerializeField] private List<GlobalStorageString> strings = new List<GlobalStorageString>();

    [SerializeField] private List<GlobalStorageBoolean> booleans = new List<GlobalStorageBoolean>();

    [SerializeField] private List<string> objects = new List<string>();

    #region Singleton definition

    private static GlobalStorage instance;

    private void GrantSingleInstance()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    private void Awake()
    {
        GrantSingleInstance();
        DontDestroyOnLoad(gameObject);
        LoadSelf();
    }

    public static void Save<T>(string key, T data)
    {
        if (string.IsNullOrEmpty(key))
            return;

        var savePath = GetSavePath(key);

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
                if (!key.Equals(GetSelfName()) && !instance.objects.Contains(key))
                    instance.objects.Add(key);

                var serializedData = JsonUtility.ToJson(data, true);
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
        var data = default(T);

        if (string.IsNullOrEmpty(key))
            return data;

        var savePath = GetSavePath(key);

        try
        {
            if (TypeCodeIsNumeric(typeof(T)))
            {
                data = (T) Convert.ChangeType(instance.numbers.Find(x => x.Key == key).Value, typeof(T));
                DebugMessage("Data loaded from Numbers Array");
            }
            else if (TypeCodeIsString(typeof(T)))
            {
                data = (T) Convert.ChangeType(instance.strings.Find(x => x.Key == key).Value, typeof(T));
                DebugMessage("Data loaded from Strings Array");
            }
            else if (TypeCodeIsBoolean(typeof(T)))
            {
                data = (T) Convert.ChangeType(instance.booleans.Find(x => x.Key == key).Value, typeof(T));
                DebugMessage("Data loaded from Booleans Array");
            }
            else
            {
                if (File.Exists(savePath))
                {
                    var serializedData = File.ReadAllText(savePath);
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
        if (string.IsNullOrEmpty(key))
            return;

        try
        {
            if (instance.numbers.RemoveAll(x => x.Key == key) > 0 || instance.strings.RemoveAll(x => x.Key == key) > 0 ||
                instance.booleans.RemoveAll(x => x.Key == key) > 0)
            {
                SaveSelf();
            }
            else
            {
                var savePath = GetSavePath(key);
                if (!File.Exists(savePath)) return;
                instance.objects.Remove(key);
                File.Delete(savePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }

    private static string GetSavePath(string fileName)
    {
        return string.IsNullOrEmpty(fileName)
            ? null
            : Path.Combine(UnityEngine.Application.persistentDataPath, fileName + ".json");
    }

    private static bool TypeCodeIsNumeric(Type type)
    {
        TypeCode[] typeCodes =
        {
            TypeCode.Decimal, TypeCode.Double, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64,
            TypeCode.Single, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64
        };
        return Array.IndexOf(typeCodes, Type.GetTypeCode(type)) > -1;
    }

    private static bool TypeCodeIsString(Type type)
    {
        return Type.GetTypeCode(type) == TypeCode.String || Type.GetTypeCode(type) == TypeCode.Char;
    }

    private static bool TypeCodeIsBoolean(Type type)
    {
        return Type.GetTypeCode(type) == TypeCode.Boolean;
    }

    private static void SaveSelf()
    {
        Save(GetSelfName(), instance);
    }

    private static void LoadSelf()
    {
        try
        {
            var selfName = GetSelfName();
            var savePath = GetSavePath(selfName);
            if (!File.Exists(savePath)) return;
            var serializedData = File.ReadAllText(savePath);
            var debugModeBeforeLoading = instance.DebugMode;
            JsonUtility.FromJsonOverwrite(serializedData, instance);
            instance.DebugMode = debugModeBeforeLoading;
            var fileArray = new DirectoryInfo(UnityEngine.Application.persistentDataPath).GetFiles("*.json");
            foreach (var fileInfo in fileArray)
            {
                var fileName = fileInfo.Name.Replace(".json", "");
                if (!fileName.Equals(selfName) && !instance.objects.Contains(fileName))
                    instance.objects.Add(fileName);
            }
            DebugMessage("Data loaded from " + savePath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }

    private static string GetSelfName()
    {
        var declaringType = MethodBase.GetCurrentMethod().DeclaringType;
        return declaringType != null ? declaringType.ToString() : null;
    }

    private static void DebugMessage(string message)
    {
        if (instance.DebugMode)
            Debug.Log(message);
    }
}