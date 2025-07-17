using System;
using Unity.VisualScripting;
using UnityEngine;

public class Ohajiki : MonoBehaviour
{
    // state
    private enum OhajikiState
    {
        Grounded, Falling, Teleportable
    }
    private OhajikiState state = OhajikiState.Grounded;
    
    // Fields shown in the inspector
    [Header("Ohajiki options")]
    [SerializeField] public float speed = 2.0f;
    [SerializeField] public float yKillBound = -30;

    [SerializeField] private Collider movingCollider;
    [SerializeField] private Collider physicsCollider;
    
    // Assigned during creation
    [Header("Assigned by instantiator")]
    public bool original = false;
    public Collider groundedBounds;
    
    // Private members
    private Rigidbody rb;
    
    // Initial transform
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialLocalScale;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        
        RecordInitialState();
        GameManager.instance.OnLevelStart += OnLevelStarted;
        GameManager.instance.OnLevelEnd += OnLevelEnded;
    }
    
    void Update()
    {
        if (state == OhajikiState.Falling && transform.position.y < yKillBound)
        {
            SetTeleportable();
        }
    }
    
    void FixedUpdate()
    {
        if (state == OhajikiState.Grounded)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }

    // State functions
    public void SetGrounded()
    {
        state = OhajikiState.Grounded;
        
        // layer
        gameObject.layer = 0;
        
        // physics
        movingCollider.enabled = true;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void SetFalling()
    {
        state = OhajikiState.Falling;
        
        // layer
        gameObject.layer = 0;
        
        // physics
        movingCollider.enabled = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void SetTeleportable()
    {
        state = OhajikiState.Teleportable;
        
        // layer
        gameObject.layer = 6;
        
        // physics
        movingCollider.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
	
    // Reset functions
    private void RecordInitialState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialLocalScale = transform.localScale;
    }

    private void ResetToInitialState()
    {
        gameObject.SetActive(true);
        
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialLocalScale;
        
        SetGrounded();
    }

    // Event functions
    private void OnLevelStarted()
    {
        if (original) ResetToInitialState();
        else
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(0, yKillBound, 0);
        }
    }

    private void OnLevelEnded()
    {
        SetTeleportable();
        gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Ohajiki TriggerExit");
        //Debug.Log(other.gameObject.name);
        if (other == groundedBounds)
        {
            //Debug.Log("Ohajiki TriggerExit Grounded");
            SetFalling();
        }
    }
}
