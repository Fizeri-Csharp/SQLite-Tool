using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SwichToolMenu : MonoBehaviour
{
    public Color DisableButton;
    public Color EnableButton;

    public List<GameObject> Buttons;
    public List<GameObject> Panels;



    public void SwitchPanel(int index)
    {
        for (int i = 0; i < Panels.Count; i++)
        {
            Panels[i].SetActive(false);
        }
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].GetComponent<Image>().color = DisableButton;
        }
        Panels[index].SetActive(true);
        Buttons[index].GetComponent<Image>().color = EnableButton;
    }
}
