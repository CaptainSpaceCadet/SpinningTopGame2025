using UnityEngine;

public class Ohajiki : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    [SerializeField] private Vector2 xBound;
    [SerializeField] private Vector2 zBound;

    [SerializeField] private float yKillBound = -30;
    
    public bool cullable = false;
    
    private Rigidbody rb;
    private bool isGrounded = true;
    
    void Start()
    {
        GameManager.instance.OnLevelEnd += OnLevelEnded;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        
        xBound = new Vector2(-21, 274);
        zBound = new Vector2(-5, 5);
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
        if (isGrounded && transform.position.x > 23)
        {
            Debug.Log("Ohajiki is out of bounds");
            isGrounded = false;
            
            rb.isKinematic = false;
            rb.useGravity = true;
        } else if (transform.position.y < yKillBound)
        {
            cullable = true;
            gameObject.layer = 6;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
    
    void FixedUpdate()
    {
        if (isGrounded) transform.position += this.transform.forward * (speed * Time.deltaTime);
    }

    void OnLevelEnded()
    {
        Destroy(gameObject);
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
}
