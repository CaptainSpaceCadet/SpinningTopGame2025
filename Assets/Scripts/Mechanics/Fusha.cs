using System;
using UnityEngine;

public class Fusha : MonoBehaviour
{
    public enum FallOffFunction
    {
        Linear,
        Smoothstep
    }
    
    [SerializeField] private float liftHeight;
    [SerializeField] private float liftDecayHeight;
    
    [SerializeField] private float maxLiftForce;
    
    [SerializeField] private FallOffFunction fallOffFunction;

    private GameObject player;
    private Rigidbody playerRb;
    
    private void Start()
    {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.height = (1f / transform.lossyScale.y) * liftHeight;
        Debug.Log("(1f / transform.lossyScale.y) * liftHeight " + (1f / transform.lossyScale.y) * liftHeight);
        capsuleCollider.center = new Vector3(0f, capsuleCollider.height / 2f, 0f);
        Debug.Log("capsuleCollider.center " + capsuleCollider.center);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position + Vector3.up * liftHeight / 2f, new Vector3(transform.localScale.x, liftHeight, transform.localScale.z));;
        Gizmos.color = new Color(0f, 0.2f, 0.4f);
        Gizmos.DrawWireCube(transform.position + Vector3.up * liftDecayHeight / 2f, new Vector3(transform.localScale.x, liftDecayHeight, transform.localScale.z));
    }
    
    // Event functions
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fusha TriggerEnter");
        if (other.gameObject.CompareTag("player"))
        {
            Debug.Log("Fusha TriggerEnter Player");
            player = other.gameObject;
            playerRb = other.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            float heightAboveFan = player.transform.position.y - transform.position.y;

            float liftForce;

            if (heightAboveFan > liftHeight) liftForce = 0f;
            else if (heightAboveFan < liftDecayHeight) liftForce = maxLiftForce;
            else
            {
                float heightAboveFanNormalized = Mathf.Clamp01((heightAboveFan - liftDecayHeight) / (liftHeight - liftDecayHeight));
                switch (fallOffFunction)
                {
                    case FallOffFunction.Linear:
                        liftForce = maxLiftForce * (1f - heightAboveFanNormalized);
                        break;
                    case FallOffFunction.Smoothstep:
                        liftForce = maxLiftForce * (1f - Mathf.SmoothStep(0f, 1f, heightAboveFanNormalized));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            playerRb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            player = null;
            playerRb = null;
        }
    }
}
