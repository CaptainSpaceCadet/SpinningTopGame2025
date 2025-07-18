using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void OnNumKeys(InputAction.CallbackContext context)
    {
        int keyVal = (int) context.action.ReadValue<float>();
        
        if (keyVal-1 < 0) return;
        Debug.Log("Load scene of index " + (keyVal-1) + " of " + SceneManager.sceneCountInBuildSettings + " scenes");
        if (keyVal-1 >= SceneManager.sceneCountInBuildSettings) return;
        
        SceneManager.LoadScene(keyVal-1);
    }

    public void OnM_Menu(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Title");
    }
}
