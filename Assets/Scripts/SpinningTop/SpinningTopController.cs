using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpinningTopController : MonoBehaviour
{
	private Vector2 m_moveInput = Vector2.zero;
	private Vector2 m_currentDirection = Vector2.zero;
	private float speed = 0f;

	[Header("Axis")] 
	[SerializeField] private bool reverseVertical = false;
	[SerializeField] private bool reverseHorizontal = false;
	[SerializeField] private bool switchVerticalHorizontal = false;
	[Header("Max speed")]
	[SerializeField] private float maxSpeed = 5f;
	[Header("Seconds to reach maxSpeed")]
	[SerializeField] private float timeToMaxSpeed = 0.5f;
	[Header("Seconds to stop due to inertia")]
	[SerializeField] private float inertiaTime = 0.7f;
	[Header("Tilt angle in degrees")]
	[SerializeField] private float tiltAngle = 10f;
	[Header("Speed to return to upright")]
	[SerializeField] private float rotationReturnSpeed = 5f;
	[Header("How quickly direction changes (higher = less inertia)")]
	[SerializeField] private float directionLerpSpeed = 5f;
	[Header("Jump force")]
	[SerializeField] private float jumpForce = 5f;
	[Header("Ground check distance")]
	[SerializeField] private float groundCheckDistance = 0.1f;
	[Header("Y coordinate point where the spinning top respawns")]
	[SerializeField] private float respawnThreshold = -30f;

	[Header("JumpSoundEmitter")] 
	[SerializeField] private StudioEventEmitter moveSoundEmitter;
	[SerializeField] private StudioEventEmitter jumpSoundEmitter;
	[SerializeField] private StudioEventEmitter destroySoundEmitter;
	
	private float acceleration => maxSpeed / timeToMaxSpeed;
	private float deceleration => maxSpeed / inertiaTime;
	private Quaternion defaultRotation;
	private Rigidbody rb;
	private bool isGrounded = false;

	private Vector3 respawnPoint;

	private void Start()
	{
		defaultRotation = transform.rotation;
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}

		respawnPoint = transform.position;
		
		GameManager.instance.OnLevelStart += OnLevelStarted;
		GameManager.instance.OnLevelEnd += OnLevelEnded;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if(!moveSoundEmitter.IsPlaying()) moveSoundEmitter.Play();
		Debug.Log("Move");
		m_moveInput = context.ReadValue<Vector2>();
		if (reverseVertical) m_moveInput.y *= -1;
		if (reverseHorizontal) m_moveInput.x *= -1;
		if (switchVerticalHorizontal) (m_moveInput.x, m_moveInput.y) = (m_moveInput.y, m_moveInput.x);
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if(!jumpSoundEmitter.IsPlaying()) jumpSoundEmitter.Play();
		
		if (context.started && isGrounded)
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	private bool CheckOutOfBounds()
	{
		return transform.position.y < respawnThreshold;
	}

	private void Respawn()
	{
		destroySoundEmitter.Play();
    
		transform.position = respawnPoint;
		transform.rotation = defaultRotation;
		rb.angularVelocity = Vector3.zero;
		rb.linearVelocity = Vector3.zero;
	}

	private void FixedUpdate()
	{
		if (CheckOutOfBounds())
		{
			GameManager.instance.DecreaseLives();
			Respawn();
		}
		// Ground check
		isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.01f);

		// Inertia-based movement
		Vector2 velocity = m_currentDirection * speed;

		if (m_moveInput != Vector2.zero)
		{
			Vector2 desiredDir = m_moveInput.normalized;
			Vector2 desiredVelocity = desiredDir * maxSpeed;
			velocity = Vector2.Lerp(velocity, desiredVelocity, directionLerpSpeed * Time.deltaTime);
			speed = velocity.magnitude;
			m_currentDirection = speed > 0.01f ? velocity.normalized : Vector2.zero;
		}
		else
		{
			speed -= deceleration * Time.deltaTime;
			speed = Mathf.Max(speed, 0f);
			velocity = m_currentDirection * speed;
		}

		if (speed > 0.01f && m_currentDirection != Vector2.zero)
		{
			Move(velocity);
		}
		else
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, defaultRotation, rotationReturnSpeed * Time.deltaTime);
		}
	}

	private void Move(Vector2 velocity)
	{
		// ���������̂ݎ蓮�ړ��i�W�����v��d�͂�Rigidbody�ɔC����j
		Vector3 move = new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;
		rb.MovePosition(rb.position + move);

		// Tilt in the direction of movement
		//if (velocity != Vector2.zero)
		//{
		//	Vector3 forward = new Vector3(velocity.x, 0, velocity.y).normalized;
		//	Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
		//	Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
		//	targetRotation *= Quaternion.AngleAxis(tiltAngle, right);
		//	transform.rotation = targetRotation;
		//}
	}
	
	private void OnLevelStarted()
	{
		Respawn();
		gameObject.SetActive(true);
	}

	private void OnLevelEnded()
	{
		gameObject.SetActive(false);
	}
}
