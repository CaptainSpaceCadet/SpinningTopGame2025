using UnityEngine;

public class Ohajiki : MonoBehaviour
{
    public enum OhajikiState
    {
        Grounded, Falling, Teleportable
    }
    
    [SerializeField] private bool original = false;
    
    [SerializeField] private float speed = 2.0f;

    // TODO: Fix this!!!
    [SerializeField] private Vector2 xBound;
    [SerializeField] private Vector2 zBound;

    // TODO: Fix this!!!
    [SerializeField] private float yKillBound = -30;

    [SerializeField] private float dx = 0.0f;
    
    public bool isTeleportable = false;
    
    private Rigidbody rb;
    
    [SerializeField] private OhajikiState state = OhajikiState.Grounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        
        xBound = new Vector2(-21, 274);
        zBound = new Vector2(-5, 5);
        
        RecordInitialState();
        GameManager.instance.OnLevelStart += OnLevelStarted;
        GameManager.instance.OnLevelEnd += OnLevelEnded;
    }

    bool IsOutOfBounds()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        
        float xMin = Mathf.Min(xBound.x, xBound.y);
        float xMax = Mathf.Max(xBound.x, xBound.y);
        float zMin = Mathf.Min(zBound.x, zBound.y);
        float zMax = Mathf.Max(zBound.x, zBound.y);

        return (x < xMin || x > xMax ||
                z < zMin || z > zMax);
    }

    
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.position.x); // for some reason without this line it doesn't work!!!
        if (state == OhajikiState.Grounded && transform.position.x > 23)
        {
            Debug.Log("Ohajiki is out of bounds");
            SetFalling();
        } else if (transform.position.y < yKillBound) 
        {
            // TODO: FIX THIS!!!
            if (original)
            {
                gameObject.SetActive(false);
                return;
            }
            
            SetTeleportable();
        }
    }
    
    void FixedUpdate()
    {
        if (state == OhajikiState.Grounded)
        {
            dx = (this.transform.forward * (speed * Time.deltaTime)).magnitude;
            transform.position += this.transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            dx = 0.0f;
        }
    }

    // void OnLevelEnded()
    // {
    //     Destroy(gameObject);
    // }

    public void SetGrounded()
    {
        state = OhajikiState.Grounded;
        
        // layer
        gameObject.layer = 0;
        
        // physics
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void SetFalling()
    {
        state = OhajikiState.Falling;
        
        // layer
        gameObject.layer = 0;
        
        // physics
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void SetTeleportable()
    {
        state = OhajikiState.Teleportable;
        
        // layer
        gameObject.layer = 6;
        
        // physics
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    // too lazy to get this to work properly
    
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.layer == 3)
    //     {
    //         isGrounded = true;
    //         rb.linearVelocity = Vector3.zero;
    //         rb.useGravity = false;
    //     }
    // }
    //
    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.layer == 3)
    //     {
    //         isGrounded = false;
    //         rb.linearVelocity = Vector3.zero;
    //         rb.useGravity = true;
    //     }
    // }
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialLocalScale;
	
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

    private void OnLevelStarted()
    {
        ResetToInitialState();
    }

    private void OnLevelEnded()
    {
        gameObject.SetActive(false);
    }
    
}
