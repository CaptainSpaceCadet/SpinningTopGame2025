using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxVelocityChange = 4f;
    [SerializeField] private float tiltAmount = 10f;
    [SerializeField] private float maxSlopeAngle = 80;
    
    [SerializeField] private Spinning spinning;
    
    [SerializeField] private float baseDamagePerCollision = 10f;
    [SerializeField] private float attackCoefficient = 1.0f;
    [SerializeField] private float defenseCoefficient = 1.0f;
    
    private bool isRagdoll = false;
    private bool isGrounded = false;
    private Vector3 m_lastStadiumNorm = Vector3.up;
    private Vector3 m_velocityVector = Vector3.zero;
    
    public Rigidbody rb;
    
    private Vector2 m_MoveInput;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(this.name + " started");
        rb = GetComponent<Rigidbody>();
    }

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
    }

    public void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (contact.otherCollider.CompareTag("Stadium"))
            {
                // Assumes all other collisions are unimportant
                if (Vector3.Angle(contact.normal, m_lastStadiumNorm) > maxSlopeAngle) return;
                isGrounded = true;
                m_lastStadiumNorm = contact.normal;
                return;
            }
        }
    }

    // handle collisions with other spinning tops
    public void OnCollisionEnter(Collision other)
    {
        //Debug.Log("collision with " + other.gameObject.name);
        if (!other.collider.CompareTag("Top")) return;
        MovementController otherControl = other.gameObject.GetComponent<MovementController>();
        Debug.Log("collision with top my speed = " + rb.linearVelocity.magnitude + " their speed = " + otherControl.rb.linearVelocity.magnitude);
        if (rb.linearVelocity.magnitude > otherControl.rb.linearVelocity.magnitude)
        {
            otherControl.TakeDamage(baseDamagePerCollision * attackCoefficient);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log(this.name+ " took damage");
        spinning.ReduceSpinSpeed(damage * defenseCoefficient);
    }

    // Update is called once per frame
    void Update()
    {
        if (spinning.IsDead)
        {
            Die();
        }
        
        //Apply Movement and Tilt
        Move(CalculateNewVelocity(m_MoveInput));
        transform.rotation = CalculateNewRotation(m_MoveInput);
    }

    private void Die()
    {
        spinning.enabled = false;
        this.enabled = false;
    }

    private Vector3 CalculateNewVelocity(Vector2 moveInput)
    {
        Vector3 movementX = transform.right * moveInput.x;
        Vector3 movementZ = transform.forward * moveInput.y;
        return (movementX + movementZ).normalized * speed;
    }

    private void Move(Vector3 movementVelocityVector)
    {
        m_velocityVector = movementVelocityVector;
    }

    private Quaternion CalculateNewRotation(Vector3 moveInput)
    {
        var playerTilt = Quaternion.Euler(moveInput.y * speed * tiltAmount, 0, -1 * moveInput.x * speed * tiltAmount);
        return (isGrounded ? Quaternion.FromToRotation(Vector3.up, m_lastStadiumNorm) * playerTilt : playerTilt);
    }
    
    private void FixedUpdate()
    {
        //if (isRagdoll) return;
        if (m_velocityVector == Vector3.zero) return;

        //Get rigidbody's current velocity
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = (m_velocityVector - velocity);

        //Apply a force by the amount of velecity change to reach the target velocity
        velocityChange.x = Mathf.Clamp(velocityChange.x,-maxVelocityChange,maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0f;


        rb.AddForce(velocityChange, ForceMode.Acceleration);
    }
}
