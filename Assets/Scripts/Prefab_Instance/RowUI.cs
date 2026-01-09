using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Windows.Forms;
using Michsky.MUIP;

public class RowUI : MonoBehaviour
{
    public Transform content;
    public GameObject Text_Prefab;

    public ButtonManager DeleteRowButton;

    public static ButtonManager DeleteRowStaticButton;

    public delegate void DeleteAction(object idValue);


    public event DeleteAction OnDeleteRequest;

    private object rowId;


    public void SetupRow(object id)
    {
        rowId = id;
        // Настройка кнопки удаления (для MUIP ButtonManager)
        if (DeleteRowButton != null)
        {
            DeleteRowButton.onClick.RemoveAllListeners();
            DeleteRowButton.onClick.AddListener(() => OnDeleteRequest?.Invoke(rowId));
        }
    }

    private void Start()
    {
        DeleteRowStaticButton = DeleteRowButton;
    }

    public List<TextMeshProUGUI> InitUI(int count)
    {
        List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(Text_Prefab, content, false);
            // Попробуем взять компонент напрямую, если он на корне префаба
            var text = go.GetComponent<TextMeshProUGUI>();

            // Если на корне нет, ищем внутри
            if (text == null) text = go.GetComponentInChildren<TextMeshProUGUI>();

            if (text != null)
            {
                textList.Add(text);
                go.SetActive(true);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI не найден!");
            }
        }
        return textList;
    }


}
