using UnityEngine;

public class Koinobori : MonoBehaviour
{
	[SerializeField] private bool fallForever = false;
	
	[Range(1f, 180f), Header("Degree between world Y-Axis and the normal of the contact point")]
	[SerializeField] private float theta = 30.0f;
	[SerializeField] private float initialFallSpeed = 0.5f;
	[SerializeField] private float acceleration = 0.5f;
	
	// Private members
	private float currentFallSpeed = 0f;
	private float currentRiseSpeed = 0f;
	private bool isFalling = false;
	
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Vector3 initialLocalScale;

	private void Start()
	{
		currentFallSpeed = initialFallSpeed;
		
		RecordInitialState();
		GameManager.instance.OnLevelStart += OnLevelStarted;
	}

	private void Update()
	{
		// start falling when the player is on top of koinobori, it keeps falling once it's touched
		// - I think this looks really weird in practice
		if (isFalling) Fall();
		else if (transform.position.y < initialPosition.y) Rise();
	}

	// Movement functions
	private void Fall()
	{
		currentFallSpeed += acceleration * Time.deltaTime;
		transform.position += Vector3.down * (currentFallSpeed * Time.deltaTime);
	}

	private void Rise()
	{
		currentRiseSpeed += acceleration * Time.deltaTime;
		transform.position += Vector3.up * (currentRiseSpeed * Time.deltaTime);
	}
	
	private void RecordInitialState()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		initialLocalScale = transform.localScale;
	}

	// Reset functions
	private void ResetToInitialState()
	{
		transform.position = initialPosition;
		transform.rotation = initialRotation;
		transform.localScale = initialLocalScale;
		
		currentFallSpeed = 0f;
		currentRiseSpeed = 0f;
		isFalling = false;
	}

	// Event functions
	private void OnLevelStarted()
	{
		ResetToInitialState();
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
			// so instead we could use normal == up, but I did this for the sake of generalization
			float angle = Vector3.Angle(normal, up);

			Debug.Log(angle);

			if (angle <= theta)
			{
				Debug.Log("hit from above");
				isFalling=true;
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (fallForever) return;
		if (!isFalling) return;
		
		if (collision.gameObject.CompareTag("player"))
		{
			isFalling = false;
		}
	}
}
