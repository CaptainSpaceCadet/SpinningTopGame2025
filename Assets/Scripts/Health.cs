using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 400;

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Top")) return;
        collision.gameObject.GetComponent<MovementController>().GainHealth(health);
        Destroy(gameObject);
    }
}
