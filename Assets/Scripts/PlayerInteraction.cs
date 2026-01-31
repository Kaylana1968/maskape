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

    [Header("Grab Contact Check")]
    [SerializeField] private float overlapExpand = 1.08f;
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private float debugEverySeconds = 0.25f;

    private Rigidbody grabbedRb;
    private GrabbableCube grabbedCube;

    private bool grabbedWasKinematic;
    private bool grabbedUsedGravity;

    private float promptTimer;
    private float holdDistance;

    private bool grabHeld;
    private bool grabHeldPrev;

    private float nextDebugTime;

    private void Log(string msg)
    {
        if (debugLogs) Debug.Log("[PlayerInteraction] " + msg);
    }

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main != null ? Camera.main.transform : null;

        if (cameraTransform == null || promptText == null)
        {
            Debug.LogError("[PlayerInteraction] cameraTransform ou promptText manquant dans l'Inspector.");
            enabled = false;
            return;
        }

        promptText.gameObject.SetActive(false);
        Log("Start OK. Mouse.current=" + (Mouse.current != null));
    }

    void Update()
    {
        if (Mouse.current != null)
            grabHeld = Mouse.current.leftButton.isPressed;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactDistance);

        bool aimingCube = false;
        Rigidbody aimedRb = null;
        GrabbableCube aimedCube = null;
        float aimedDistance = minHoldDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (IsColorTag(hit.collider.tag))
            {
                var mr = hit.collider.GetComponent<MeshRenderer>();
                if (mr != null && mr.enabled)
                {
                    aimedRb = hit.collider.attachedRigidbody;
                    aimedCube = hit.collider.GetComponent<GrabbableCube>();

                    if (aimedRb != null && aimedCube != null)
                    {
                        aimingCube = true;
                        aimedDistance = hit.distance;
                    }
                    else
                    {
                        if (debugLogs) Log("Visé couleur mais pas grabbable (rb ou GrabbableCube manquant).");
                    }
                }
            }
        }

        bool grabPressedThisFrame = grabHeld && !grabHeldPrev;
        bool grabReleasedThisFrame = !grabHeld && grabHeldPrev;
        grabHeldPrev = grabHeld;

        if (grabPressedThisFrame)
        {
            Log("CLICK PRESS");

            if (aimingCube)
                Grab(aimedRb, aimedCube, aimedDistance);
            else
                Log("Pas de grab: pas de cube valide visé.");
        }

        if (grabReleasedThisFrame)
        {
            Log("CLICK RELEASE");
            ReleaseGrab("mouse release");
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
        if (grabbedRb == null || grabbedCube == null) return;

        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * holdDistance;
        Vector3 newPos = Vector3.Lerp(grabbedRb.position, targetPos, grabFollowSpeed * Time.fixedDeltaTime);
        grabbedRb.MovePosition(newPos);

        CheckGrabbedTouchesOtherColor();
    }

    private void Grab(Rigidbody rb, GrabbableCube cube, float distance)
    {
        grabbedRb = rb;
        grabbedCube = cube;

        grabbedWasKinematic = grabbedRb.isKinematic;
        grabbedUsedGravity = grabbedRb.useGravity;

        grabbedRb.isKinematic = true;
        grabbedRb.useGravity = false;

        grabbedCube.IsGrabbed = true;
        holdDistance = Mathf.Clamp(distance, minHoldDistance, maxHoldDistance);

        Log("GRAB OK -> " + grabbedRb.name + " holdDistance=" + holdDistance.ToString("0.00"));
    }

    private void ReleaseGrab(string reason)
    {
        if (grabbedCube != null) grabbedCube.IsGrabbed = false;

        if (grabbedRb != null)
        {
            grabbedRb.isKinematic = grabbedWasKinematic;
            grabbedRb.useGravity = grabbedUsedGravity;
        }

        grabbedRb = null;
        grabbedCube = null;

        Log("RELEASE -> " + reason);
    }

    private void CheckGrabbedTouchesOtherColor()
    {
        Collider c = grabbedCube.GetCollider();
        if (c == null)
        {
            Log("ERROR: grabbedCube collider null");
            return;
        }

        Bounds b = c.bounds;
        Vector3 halfExtents = b.extents * overlapExpand;

        Collider[] hits = Physics.OverlapBox(
            b.center,
            halfExtents,
            c.transform.rotation,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        if (debugLogs && Time.time >= nextDebugTime)
        {
            nextDebugTime = Time.time + debugEverySeconds;
            Log($"OverlapBox hits={hits.Length} grabbed={grabbedCube.name} tag={grabbedCube.tag} expand={overlapExpand:0.00}");
            for (int i = 0; i < hits.Length && i < 6; i++)
                Log($"  hit[{i}]={hits[i].name} tag={hits[i].tag}");
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Collider other = hits[i];
            if (other == null) continue;
            if (other == c) continue;
            if (other.transform == grabbedCube.transform) continue;

            string otherTag = other.tag;
            if (!IsColorTag(otherTag)) continue;
            if (otherTag == grabbedCube.tag) continue;

            Log("TOUCH DETECTED -> grabbed=" + grabbedCube.name + " (" + grabbedCube.tag + ") with " +
                other.name + " (" + otherTag + ")");

            GrabbableCube cubeRef = grabbedCube;

            ReleaseGrab("touch other color");
            cubeRef.RespawnNow("touch " + otherTag);

            return;
        }
    }

    private bool IsColorTag(string t)
    {
        return t == "Red" || t == "Blue" || t == "Green";
    }
}
