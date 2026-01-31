using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TMP_Text promptText;

    [Header("Raycast")]
    [SerializeField] private float interactDistance = 3f;

    [Header("Prompt")]
    [SerializeField] private float promptHoldTime = 0.1f;

    [Header("Grab Follow")]
    [SerializeField] private float minHoldDistance = 1f;
    [SerializeField] private float maxHoldDistance = 4f;
    [SerializeField] private float grabFollowSpeed = 20f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Rigidbody grabbedRb;
    private bool grabbedWasKinematic;
    private bool grabbedUsedGravity;

    private float promptTimer;
    private float holdDistance;

    private bool grabHeld;
    private bool grabHeldPrev;

    private void Log(string msg)
    {
        if (debugLogs) Debug.Log("PlayerInteraction " + msg);
    }

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main != null ? Camera.main.transform : null;

        if (cameraTransform == null)
        {
            Debug.LogError("pas de cameraTransform (pas de MainCamera tag ou non assignée).");
            enabled = false;
            return;
        }

        if (promptText == null)
        {
            Debug.LogError("pas de text");
            enabled = false;
            return;
        }

        promptText.gameObject.SetActive(false);

        if (Mouse.current == null)
            Log("pas de souris détectée");
        else
            Log("souris detecter ");
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            grabHeld = Mouse.current.leftButton.isPressed;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactDistance);

        bool aimingCube = false;
        Rigidbody aimedRb = null;
        float aimedDistance = minHoldDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (debugLogs) Log($" j'ai viser : {hit.collider.name} tag={hit.collider.tag} dist={hit.distance:0.00}");

            if (hit.collider.CompareTag("Red") || hit.collider.CompareTag("Blue") || hit.collider.CompareTag("Green"))
            {
                var mr = hit.collider.GetComponent<MeshRenderer>();
                if (mr != null && mr.enabled)
                {
                    aimingCube = true;
                    aimedRb = hit.collider.attachedRigidbody;
                    aimedDistance = hit.distance;

                    if (aimedRb == null && debugLogs)
                        Log("je vise un cube sans Rigidbody");
                }
            }
        }

        bool grabPressedThisFrame = grabHeld && !grabHeldPrev;
        bool grabReleasedThisFrame = !grabHeld && grabHeldPrev;
        grabHeldPrev = grabHeld;

        if (grabPressedThisFrame)
        {
            Log("j'ai clicquer");

            if (aimingCube && aimedRb != null)
            {
                grabbedRb = aimedRb;

                grabbedWasKinematic = grabbedRb.isKinematic;
                grabbedUsedGravity = grabbedRb.useGravity;

                grabbedRb.isKinematic = true;
                grabbedRb.useGravity = false;

                holdDistance = Mathf.Clamp(aimedDistance, minHoldDistance, maxHoldDistance);

                Log($"j'ai grab -> {grabbedRb.name} distance={holdDistance:0.00}");
            }
            else
            {
                Log("je sais pas viser");
            }
        }

        if (grabReleasedThisFrame)
        {
            Log("j'ai relâché");

            if (grabbedRb != null)
            {
                Log("RELÂCHE -> " + grabbedRb.name);
                grabbedRb.isKinematic = grabbedWasKinematic;
                grabbedRb.useGravity = grabbedUsedGravity;
                grabbedRb = null;
            }
        }

        bool wantPrompt = aimingCube && grabbedRb == null;

        if (wantPrompt)
        {
            promptTimer = promptHoldTime;
            promptText.text = "Attraper avec clique gauche";
        }
        else
        {
            promptTimer -= Time.deltaTime;
        }

        promptText.gameObject.SetActive(promptTimer > 0f);
    }

    void FixedUpdate()
    {
        if (grabbedRb == null) return;

        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * holdDistance;
        Vector3 newPos = Vector3.Lerp(grabbedRb.position, targetPos, grabFollowSpeed * Time.fixedDeltaTime);

        if (debugLogs)
            Log($"j'ai bouger -> {grabbedRb.name} pos={grabbedRb.position} target={targetPos}");

        grabbedRb.MovePosition(newPos);
    }
}
