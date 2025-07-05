using UnityEngine;

public class Koinobori : MonoBehaviour
{
	[Range(1f, 180f), Header("Degree between world Y-Axis and the normal of the contact point")]
	[SerializeField] private float theta = 30.0f;
	[SerializeField] private float initialFallSpeed = 0.5f;
	[SerializeField] private float acceleration  = 0.5f;

	private float currentFallSpeed = 0f;
	private bool isFalling = false;

	private void Start()
	{
		currentFallSpeed = initialFallSpeed;
	}

	private void Update()
	{
		// start falling when the player is on top of koinobori, it keeps falling once it's touched
		if (isFalling) Fall();
	}

	private void Fall()
	{
		currentFallSpeed += acceleration * Time.deltaTime;
		transform.position += Vector3.down * currentFallSpeed * Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("player"))
		{
			ContactPoint contact = collision.contacts[0];
			// normal pointing outward
			Vector3 normal = -contact.normal.normalized;


			Vector3 up = Vector3.up;

			// find a degree between world up vector and normal
			// however, because I used box collider, this is either 0, 90, or 180
			// so instead we could use normal == up, but i did this for the sake of generalization
			float angle = Vector3.Angle(normal, up);

			Debug.Log(angle);

			if (angle <= theta)
			{
				Debug.Log("hit from above");
				isFalling=true;
			}
		}
	}
}
