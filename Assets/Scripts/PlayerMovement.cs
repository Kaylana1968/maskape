using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [Header("Réglages")]
    public float speed = 5f;
    private InputAction action;
    public float mouseSensitivity = 20f;
    public Transform cameraTransform; // Glisse la Main Camera ici
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private Color[] listColor = new Color[] {new Color(255,0,0), new Color(0, 0, 255), new Color(0, 255, 0) };
    private string currentColor;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Start()
    {

        GameObject[] gameObjectsRed = GameObject.FindGameObjectsWithTag("Red");
        GameObject[] gameObjectsBlue = GameObject.FindGameObjectsWithTag("Blue");
        GameObject[] gameObjectsGreen = GameObject.FindGameObjectsWithTag("Green");
        GameObject Mask = GameObject.FindGameObjectWithTag("Mask");
        currentColor = "Red";

        Mask.gameObject.GetComponent<Image>().color = listColor[0];

        foreach (GameObject gameObject in gameObjectsRed)
        {
            MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
            ms.enabled = true;
        }
        foreach (GameObject gameObject in gameObjectsBlue)
        {
           MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
            ms.enabled = false;
        }
        foreach (GameObject gameObject in gameObjectsGreen)
        {
            MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
            ms.enabled = false;
        }
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

    public void OnMask(InputValue value)
    {
        GameObject[] gameObjectsRed = GameObject.FindGameObjectsWithTag("Red");
        GameObject[] gameObjectsBlue = GameObject.FindGameObjectsWithTag("Blue");
        GameObject[] gameObjectsGreen = GameObject.FindGameObjectsWithTag("Green");
        GameObject Mask = GameObject.FindGameObjectWithTag("Mask");
        if (currentColor == "Red") 
        {
            Mask.gameObject.GetComponent<Image>().color = new Color (255,255,255);
            currentColor = "Blue";
            foreach (GameObject gameObject in gameObjectsRed)
            {
                Debug.Log("yo");
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = false;
            }
            foreach (GameObject gameObject in gameObjectsBlue)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = true; ;
            }

        }

        else if (currentColor == "Blue")
        {
            currentColor = "Green";
            foreach (GameObject gameObject in gameObjectsBlue)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = false;
            }
            foreach (GameObject gameObject in gameObjectsGreen)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = true;
            }
            Mask.gameObject.GetComponent<Image>().color = listColor[2];
        }

        else if (currentColor == "Green")
        {
            currentColor = "Red";
            foreach (GameObject gameObject in gameObjectsGreen)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = false;
            }
            foreach (GameObject gameObject in gameObjectsRed)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = true;
            }
            Mask.gameObject.GetComponent<Image>().color = listColor[0];
        }
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
        controller.Move(move * speed * Time.deltaTime);
    }
}