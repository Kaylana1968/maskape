using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float rayDistance = 2f;
    private Camera cam;
    private void Awake() => cam = Camera.main;

    public void OnClick(InputValue value)
    {
        if (!value.isPressed) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit, rayDistance))
        {
            if (hit.collider.TryGetComponent(out Animator animator))
            {
                animator.enabled = true;
            }
        }
    }
}