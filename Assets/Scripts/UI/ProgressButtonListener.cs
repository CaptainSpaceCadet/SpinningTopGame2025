using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressButtonListener : MonoBehaviour
{
    public void OnButtonClick()
    {
        GameManager.instance.ProgressLevel();
    }
}
