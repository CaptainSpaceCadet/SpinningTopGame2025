using UnityEngine;

public class Ohajiki : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.forward * speed * Time.deltaTime;
    }
}
