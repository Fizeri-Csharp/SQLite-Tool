using SQLite;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.MUIP;
using System.Collections;

public class Add_Row : MonoBehaviour
{
    public GameObject InputVirable_GG;
    public Transform content;

    public TablesController controller;

    public ButtonManager AddRowButton;

    public TextMeshProUGUI Status;

    private List<TMP_InputField> inputs;

    private void Start()
    {
        
    }


    private void OnEnable()
    {
        SpawnInputs();
        AddRowButton.onClick.AddListener(AddRowData);
        AddRowButton.UpdateUI();
    }

    private void OnDisable()
    {
        inputs.Clear();
        Delete_ALL_Children();
        AddRowButton.onClick.RemoveAllListeners();
    }

    private void SpawnInputs()
    {
        inputs = new List<TMP_InputField>();

        string tableName = controller.TableSelect.items[controller.TableSelect.selectedItemIndex].itemName;
        var columns = GetPragmaColumns(tableName);

        if (columns.Count == 0)
        {
            Debug.LogWarning($"Таблица {tableName} пуста или не существует!");
            return;
        }

        foreach (var col in columns)
        {
            // ❌ не создаём input для PRIMARY KEY
            if (col.pk == 1)
                continue;

            GameObject input = Instantiate(InputVirable_GG, content, false);

            input.transform.Find("Placeholder")
                .GetComponent<TextMeshProUGUI>().text = col.name;

            TMP_InputField field = input.GetComponentInChildren<TMP_InputField>();
            inputs.Add(field);
        }
    }


    private void AddRowData()
    {
        if (controller.TableSelect.items == null || controller.TableSelect.items.Count == 0)
        {
            Debug.LogWarning("Dropdown пустой, таблицу выбрать невозможно!");
            StartCoroutine(StatusView("Не найдено таблиц!", Color.red));
            return;
        }

        string tableName = controller.TableSelect.items[controller.TableSelect.selectedItemIndex].itemName;
        var columns = GetPragmaColumns(tableName);

        List<string> columnNames = new List<string>();
        List<string> values = new List<string>();

        int inputIndex = 0;

        foreach (var col in columns)
        {
            // ❌ PRIMARY KEY не вставляем
            if (col.pk == 1)
                continue;

            string inputValue = inputs[inputIndex].text.Trim();
            inputIndex++;

            // ❌ пустые значения не вставляем
            if (string.IsNullOrEmpty(inputValue))
                continue;

            inputValue = inputValue.Replace("'", "''");

            columnNames.Add(col.name);
            values.Add($"'{inputValue}'");
        }

        if (columnNames.Count == 0)
        {
            Debug.LogWarning("Нет данных для вставки");
            StartCoroutine(StatusView("Нет данных для вставки", Color.yellow));
            return;
        }

        string sql =
            $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) " +
            $"VALUES ({string.Join(", ", values)})";

        Database.Instance._db.Execute(sql);

        Debug.Log($"Данные добавлены в таблицу {tableName}");
        StartCoroutine(StatusView($"Данные добавлены в таблицу {tableName}", Color.green));
        controller.RefreshRows();
    }


    private void Delete_ALL_Children()
    {
        foreach(Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private List<PragmaColumn> GetPragmaColumns(string tableName)
    {
        string sql = $"PRAGMA table_info({tableName})";
        return Database.Instance._db.Query<PragmaColumn>(sql);
    }


    IEnumerator StatusView(string Text, Color colorText)
    {
        Status.text = Text;
        Status.color = colorText;
        yield return new WaitForSeconds(2.3f);
        Status.text = string.Empty;
    }

}
public class PragmaColumn
{
    public int cid { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public int notnull { get; set; }
    public string dflt_value { get; set; }
    public int pk { get; set; }
}
