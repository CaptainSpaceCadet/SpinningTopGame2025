using UnityEngine;
using UnityEngine.InputSystem;

public class SpinningTopController : MonoBehaviour
{
	private Vector2 m_moveInput = Vector2.zero;
	private Vector2 m_currentDirection = Vector2.zero;
	private float speed = 0f;
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
	private float acceleration => maxSpeed / timeToMaxSpeed;
	private float deceleration => maxSpeed / inertiaTime;
	private Quaternion defaultRotation;
	private Rigidbody rb;
	private bool isGrounded = false;

	private void Start()
	{
		defaultRotation = transform.rotation;
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		m_moveInput = context.ReadValue<Vector2>();
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started && isGrounded)
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	private void Update()
	{
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
		// 水平方向のみ手動移動（ジャンプや重力はRigidbodyに任せる）
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
}
