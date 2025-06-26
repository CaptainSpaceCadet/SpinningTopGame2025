using UnityEngine;

public class SpinningTopRotation : MonoBehaviour
{
	[SerializeField] private float rotationSpeed = 360f; // Degrees per second

	void Update()
	{
		// Continuously rotate the object around the Y axis
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
	}
}