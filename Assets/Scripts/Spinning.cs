using UnityEngine;

public class Spinning : MonoBehaviour
{
    public bool IsDead;
    
    [SerializeField] private float initialSpinSpeed = 1000;
    [SerializeField] private bool doSpin = false;
    [SerializeField] private float currentSpinSpeed;

    private Rigidbody rb;
    public GameObject playerGraphics;
    public float CurrentSpinSpeed { get => currentSpinSpeed; }
    
    void Start()
    {
        currentSpinSpeed = initialSpinSpeed;
    }

    public void ReduceSpinSpeed(float amount)
    {
        Debug.Log("Spin reduced to " + currentSpinSpeed);
        currentSpinSpeed -= amount;
        if (currentSpinSpeed < 100)
        {
            currentSpinSpeed = 0;
            IsDead = true;
            Debug.Log("Dead");
            playerGraphics.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
    }

    private void FixedUpdate()
    {
        if (!doSpin) return;
        
        playerGraphics.transform.Rotate(new Vector3(0, currentSpinSpeed*Time.deltaTime, 0));
    }
}
