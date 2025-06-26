using UnityEngine;

public class SpinningTop : MonoBehaviour
{
    [SerializeField] private float initialTorque = 100f;
    
    [SerializeField] private Transform plane;
    
    [Range(0f, 360f)] [SerializeField]
    private float xRotation;
    [Range(0f, 360f)] [SerializeField]
    private float zRotation;
    
    Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(new Vector3(0, initialTorque, 0));
    }

    public void OnPlayerMove()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, zRotation);
        //eulerAngles.y = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
