using Michsky.MUIP;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateTableManager : MonoBehaviour
{
    private string tableName;
    private List<VirableData> _virableData = new List<VirableData>();

    public SwitchManager Unique;
    public SwitchManager NotNull;
    public SwitchManager PrimaryKey;


    public TMP_InputField DefaultInput;
    public TMP_InputField NameTableInput;
    public TMP_InputField VirableName;


    public CustomDropdown TypeVirable;


    public GameObject VirablePrefab;

    public GameObject AddVirablePanel;


    public Transform Content;

    private void Start()
    {
        NameTableInput.onSubmit.AddListener(SetName);
    }

    public void SetName(string name)
    {
        tableName = name;
    }

    public void AddVirable()
    {
        string name = VirableName.text;

        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Имя колонки пустое");
            return;
        }

        // Проверка на дубликаты
        if (_virableData.Exists(v => v.Name == name))
        {
            Debug.LogWarning("Колонка с таким именем уже существует");
            return;
        }

        VirableData data = new VirableData
        {
            Name = name,
            TypeVirable = TypeVirable.items[TypeVirable.selectedItemIndex].itemName,
            DefaultValue = string.IsNullOrEmpty(DefaultInput.text)
                ? null
                : DefaultInput.text.Trim(),

            isUnique = Unique.isOn,
            NotNull = NotNull.isOn,
            PrimaryKey = PrimaryKey.isOn
        };

        // PRIMARY KEY автоматически NOT NULL
        if (data.PrimaryKey)
            data.NotNull = true;

        _virableData.Add(data);

        Debug.Log($"Добавлена колонка: {data.Name}");
        CreateVirableUI(data.Name, data.DefaultValue, data.isUnique, data.TypeVirable);
        AddVirablePanel.SetActive(false);

    }

    public void CreateTable()
    {
        if (string.IsNullOrEmpty(tableName))
        {
            Debug.LogWarning("Имя таблицы не задано");
            return;
        }

        if (_virableData.Count == 0)
        {
            Debug.LogWarning("Нет колонок");
            return;
        }

        List<string> defs = new List<string>();

        foreach (var v in _virableData)
        {
            string def = $"{v.Name} {v.TypeVirable}";

            if (v.PrimaryKey)
                def += " PRIMARY KEY";

            if (v.NotNull)
                def += " NOT NULL";

            if (v.isUnique)
                def += " UNIQUE";

            if (!string.IsNullOrEmpty(v.DefaultValue))
            {
                // TEXT → в кавычках
                if (v.TypeVirable == "TEXT")
                    def += $" DEFAULT '{v.DefaultValue}'";
                else
                    def += $" DEFAULT {v.DefaultValue}";
            }

            defs.Add(def);
        }

        string sql =
            $"CREATE TABLE IF NOT EXISTS {tableName} (" +
            string.Join(", ", defs) +
            ")";

        Debug.Log(sql);
        Database.Instance._db.Execute(sql);
    }

    private void CreateVirableUI(string name,string defaultValue,bool isUnique,string type)
    {
        GameObject go = Instantiate(VirablePrefab, Content, false);
        go.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
        go.transform.Find("DefaultValue").GetComponent<TextMeshProUGUI>().text = "Default: " + defaultValue;
        go.transform.Find("Unique").GetComponent<TextMeshProUGUI>().text = "Is Unique: " + isUnique;
        go.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = type;
    }

}


public class VirableData
{
    public string Name;
    public string TypeVirable;
    public string DefaultValue = null;
    public bool isUnique;
    public bool NotNull;
    public bool PrimaryKey;
}
