using UnityEngine.SceneManagement;
using UnityEngine;

public class quitToMenu : MonoBehaviour
{

    public void Quit()
    {
        Database.Instance._db = null;
        Database.Instance.NameDB = string.Empty;
        SceneManager.LoadScene(0);
    }
}
