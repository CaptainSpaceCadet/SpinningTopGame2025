using UnityEngine;

public class Balloon : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("player"))
		{
			GameManager.instance.IncreaseBalloons();
			Destroy(gameObject);
		}
	}
}