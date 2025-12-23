using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class AllData
{
    public SoundData soundData = new SoundData();
}

public class SaveLoadManager : SingletonManager<SaveLoadManager>
{
    public AllData datas;
    string path;

    protected override void Awake()
    {
        base.Awake();
        path = Path.Combine(Application.persistentDataPath, "GameData.json");
        LoadDatas();
    }

    public void LoadDatas()
    {
        if (!File.Exists(path))
        {
            datas = new AllData();
            SaveDatas();
            return;
        }
        string json = File.ReadAllText(path);
        datas = JsonConvert.DeserializeObject<AllData>(json);

        if (datas == null) { datas = new AllData(); }
    }

    public void SaveDatas()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
        };
        string json = JsonConvert.SerializeObject(datas, settings);
        File.WriteAllText(path, json);
    }

    private void OnApplicationQuit()
    {
        SaveDatas();
    }
}
