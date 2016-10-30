using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

public class GlobalStorage : MonoBehaviour
{
    public bool DebugMode;

    [SerializeField] private string filePath;

    public string FilePathPath
    {
        get { return filePath; }
        private set { filePath = value; }
    }

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

    [Serializable]
    private class GlobalStorageObject
    {
        [HideInInspector] public string Key;
        [TextArea(3, 10)] [SerializeField] private string value;
        [SerializeField] private string filePath;

        private string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public GlobalStorageObject(string key, string value)
        {
            Key = key;
            Value = value;
            FilePath = GetSavePath(key);
        }
    }

    [SerializeField] private List<GlobalStorageNumber> numbers = new List<GlobalStorageNumber>();

    [SerializeField] private List<GlobalStorageString> strings = new List<GlobalStorageString>();

    [SerializeField] private List<GlobalStorageBoolean> booleans = new List<GlobalStorageBoolean>();

    [SerializeField] private List<GlobalStorageObject> objects = new List<GlobalStorageObject>();
    private string _file1;

    #region Singleton definition

    private static GlobalStorage instance;

    private static void SelfInstantiateInCurrentScene()
    {
        if (GameObject.Find(GetSelfName()) != null) return;
        var newGlobalStorage = new GameObject().AddComponent<GlobalStorage>();
        newGlobalStorage.name = GetSelfName();
    }

    private void GrantSingleInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    private void Awake()
    {
        FilePathPath = GetSavePath(GetSelfName());
        GrantSingleInstance();
        DontDestroyOnLoad(gameObject);
        LoadSelf();
    }

    public static void Save<T>(string key, T data)
    {
        SelfInstantiateInCurrentScene();

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
                var serializedData = JsonUtility.ToJson(data, true);
                if (key.Equals(GetSelfName()))
                {
                    var globalStorage = (GlobalStorage) (object) data;
                    var objectsBeforeSaving = new List<GlobalStorageObject>(globalStorage.objects);
                    globalStorage.objects = null;
                    serializedData = JsonUtility.ToJson(data, true);
                    globalStorage.objects = objectsBeforeSaving;
                }
                else
                {
                    instance.objects.Add(new GlobalStorageObject(key, serializedData));
                }
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
        SelfInstantiateInCurrentScene();

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
        SelfInstantiateInCurrentScene();

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
                instance.objects.RemoveAll(x => x.Key == key);
                if (!File.Exists(savePath)) return;
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
            DebugMessage("Data loaded from " + savePath);
            var fileArray = new DirectoryInfo(UnityEngine.Application.persistentDataPath).GetFiles("*.json");
            foreach (var fileInfo in fileArray)
            {
                var fileName = fileInfo.Name.Replace(".json", "");
                if (fileName.Equals(selfName)) continue;
                var fileSavePath = GetSavePath(fileName);
                var fileSerializedData = File.ReadAllText(fileSavePath);
                instance.objects.Add(new GlobalStorageObject(fileName, fileSerializedData));
                DebugMessage("Data loaded from " + fileSavePath);
            }
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