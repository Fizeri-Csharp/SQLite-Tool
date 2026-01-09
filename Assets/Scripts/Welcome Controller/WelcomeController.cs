using Michsky.MUIP;
using SFB;
using SQLite;
using System;
using System.IO;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WelcomeController : MonoBehaviour
{
    public TMP_InputField NameDatabase;
    public CustomDropdown TypeDB;
    public string[] typesDatabase;
    private string typeDB;

    private void Awake()
    {
        typeDB = ".sql";
        TypeDB.onValueChanged.AddListener(OnDropDownChanged);
    }

    private void OnDropDownChanged(Int32 value)
    {
        typeDB = typesDatabase[value];
        Debug.Log(typeDB);
    }

    public void CreateDB()
    {
        string dbName = NameDatabase.text + typeDB;
        Database.Instance.NameDB = dbName;
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"SQL Tool", dbName);
        Database.Instance._db = new SQLiteConnection(path);

        Database.Instance._db.Execute(
    "CREATE TABLE IF NOT EXISTS users (" +
    "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
    "username TEXT NOT NULL DEFAULT 'User', " +
    "level INT DEFAULT 1" +
    ")"
);

        Debug.Log("SQLite DB path: " + path);
        
        

        SceneManager.LoadScene(1);
    }


    public void SelectFile()
    {
        
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Database File", "", new ExtensionFilter[] { new ExtensionFilter("db", "sql") },false);
        if(paths.Length > 0)
        {
            string path = paths[0];
            Database.Instance._db = new SQLiteConnection(path);
            SceneManager.LoadScene(1);
            Database.Instance.NameDB = Path.GetFileNameWithoutExtension(path);
        }

    }
}

