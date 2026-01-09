using Michsky.MUIP;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEditor;
using UnityEngine;

public class TablesController : MonoBehaviour
{
    public List<string> NameOfAllTables;

    public CustomDropdown TableSelect;

    public Transform Content;

    public GameObject RowPrefab;



    private void Start()
    {
        NameOfAllTables = GetTablesName();
        TableSelect.onValueChanged.AddListener(ChangeTable);
        
    }


    public void RefreshTables()
    {
        NameOfAllTables = GetTablesName();
    }

    private void ChangeTable(Int32 value)
    {
        RefreshRows();
    }

    public List<string> GetTablesName()
    {
        var tables = new List<string>();

        var result = Database.Instance._db.Query<TableName>("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'");
        foreach (var table in result)
        {
            tables.Add(table.name);
            TableSelect.items.Add(new CustomDropdown.Item
            {
                itemName = table.name,
                itemIcon = null,
                
            });
            TableSelect.SetupDropdown();
        }
        RefreshRows();
        return tables;
        
    }

    

    public void RefreshRows()
    {
       
        foreach (Transform child in Content)
            Destroy(child.gameObject);

        if (TableSelect.items.Count == 0) return;

        string tableName = TableSelect.items[TableSelect.selectedItemIndex].itemName;

        
        var columns = Database.Instance._db.Query<ColumnInfo>($"PRAGMA table_info('{tableName}')");

        
        string query = $"SELECT * FROM {tableName}";

       
        var stmt = SQLite3.Prepare2(Database.Instance._db.Handle, query);

        try
        {

            while (SQLite3.Step(stmt) == SQLite3.Result.Row)
            {
                GameObject go = Instantiate(RowPrefab, Content, false);
                RowUI rowUI = go.GetComponent<RowUI>();

                var uiTexts = rowUI.InitUI(columns.Count);
                object primaryKeyVal = null;

                for (int i = 0; i < columns.Count; i++)
                {
                    string val = SQLite3.ColumnString(stmt, i);
                    uiTexts[i].text = $"{columns[i].name}: {val}";

                    // Проверяем, является ли эта колонка первичным ключом (pk == 1)
                    if (columns[i].pk == 1)
                    {
                        primaryKeyVal = val;
                    }
                }

                // Настраиваем удаление для этой конкретной строки
                string currentTable = tableName; // Локальная переменная для замыкания
                rowUI.SetupRow(primaryKeyVal);
                rowUI.OnDeleteRequest += (id) => DeleteRecord(currentTable, columns.Find(c => c.pk == 1).name, id);
            }
        }
        finally
        {
            
            SQLite3.Finalize(stmt);
        }
    }

    public void DeleteRecord(string table, string pkColumnName, object idValue)
    {
        if (idValue == null)
        {
            Debug.LogError("Не удалось найти Primary Key для удаления");
            return;
        }

        // Формируем SQL запрос: DELETE FROM Table WHERE id = '123'
        string query = $"DELETE FROM {table} WHERE {pkColumnName} = '{idValue}'";

        try
        {
            Database.Instance._db.Execute(query);
            Debug.Log($"Запись {idValue} удалена из {table}");

            // Обновляем интерфейс, чтобы строка исчезла
            RefreshRows();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка удаления: " + e.Message);
        }
    }

}



public class TableName
{
    public string name { get; set; }
}


class ColumnInfo
{
    public int cid { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public int notnull { get; set; }
    public string dflt_value { get; set; }
    public int pk { get; set; }
}
