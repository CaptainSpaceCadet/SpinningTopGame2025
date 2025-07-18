using UnityEngine;
using UnityEngine.InputSystem;

public class MainTitle : MonoBehaviour
{
    [SerializeField] private string loadSceneName = "Level1";
     public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) LoadLevel();
    }

	private void LoadLevel()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(loadSceneName);
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
