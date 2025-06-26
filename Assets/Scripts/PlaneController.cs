using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneController : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)]
    float rotationSpeed = 0.5f;
    
    private Vector3 m_rotationInput;
    private bool m_isRotating;

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>() + "WASD");
        m_rotationInput = new Vector3(context.ReadValue<Vector2>().y, 0f, context.ReadValue<Vector2>().x);
        if (context.started) m_isRotating = true;
        if (context.canceled) m_isRotating = false;
    }

    void Update()
    {
        if (m_isRotating) Rotate(m_rotationInput);
    }

    void Rotate(Vector3 rotation)
    {
        transform.Rotate(m_rotationInput * rotationSpeed);
    }
    
    
}
