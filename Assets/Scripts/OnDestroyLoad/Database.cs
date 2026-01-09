using SQLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Database : MonoBehaviour
{
    public static Database Instance;

    public SQLiteConnection _db;

    public SQLiteConnection DB;

    public string NameDB;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SetNameDB;

    }


    private void SetNameDB(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            TextMeshProUGUI text = GameObject.Find("NameDatabaseText").GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"Database: {NameDB}";
            }
        }
       
    }

}
