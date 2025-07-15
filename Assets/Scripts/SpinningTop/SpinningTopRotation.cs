using UnityEngine;

public class SpinningTopRotation : MonoBehaviour
{
	[Tooltip("Degrees per second")]
	[SerializeField] private float rotationSpeed = 360f;

	void Update()
	{
		// Continuously rotate the object around the Y axis
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
	}
}