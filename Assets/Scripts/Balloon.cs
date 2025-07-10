using UnityEngine;

public class Balloon : MonoBehaviour
{
	[SerializeField] private GameManager gameManager;
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("player"))
		{
			gameManager.IncreaseBalloons();
			Destroy(gameObject);
		}
	}
}
