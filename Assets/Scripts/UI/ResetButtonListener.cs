using UnityEngine;

public class ResetButtonListener : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Reset button clicked");
        GameManager.instance.ResetLevel();
    }
}
