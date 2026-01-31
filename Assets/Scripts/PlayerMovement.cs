using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Réglages")]
    public float speed = 5f;
    public float mouseSensitivity = 20f;
    public Transform cameraTransform; // Glisse la Main Camera ici
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Reçoit l'input de déplacement (Z,Q,S,D)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Reçoit l'input de la souris (Delta)
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Update()
    {
        // --- ROTATION (SOURIS) ---
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotation verticale (on fait pivoter la caméra)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotation horizontale (on fait pivoter tout le player)
        transform.Rotate(Vector3.up * mouseX);

        // --- DÉPLACEMENT (ZQSD) ---
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(speed * Time.deltaTime * move);
    }
}