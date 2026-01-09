using SQLite;
using UnityEngine;
using TMPro;
using System.Text;
using System;

public class ConsoleManager : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text output;
    private TablesController tableController;

    private SQLiteConnection db;

    void Start()
    {
        db = Database.Instance._db;
        tableController = FindAnyObjectByType<TablesController>();
        if (tableController == null)
        {
            Print("No find Table Controller");
        }
        Print("SQLite-net console ready");
    }

    public void Execute()
    {
        string sql = input.text;

        if (string.IsNullOrWhiteSpace(sql))
            return;

        try
        {
            // SELECT
            if (sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                var result = db.Query<object>(sql);
                Print(FormatResult(result));
            }
            // ¬—≈ Œ—“¿À‹Õ€≈ SQL
            else
            {
                int rows = db.Execute(sql);
                Print($"OK | Rows affected: {rows}");
            }
        }
        catch (Exception e)
        {
            Print("ERROR:\n" + e.Message);
        }

        input.text = "";
        tableController.RefreshRows();
        tableController.RefreshTables();
    }

    private string FormatResult(System.Collections.Generic.List<object> rows)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var row in rows)
        {
            sb.AppendLine(row.ToString());
        }

        if (rows.Count == 0)
            sb.AppendLine("(no rows)");

        return sb.ToString();
    }

    private void Print(string msg)
    {
        output.text += msg + "\n\n";
    }

    private void OnDestroy()
    {
        db?.Close();
    }
}
