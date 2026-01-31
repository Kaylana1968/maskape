using UnityEngine;
using System.Collections;
using TMPro;

public class GrabbableCube : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 0.1f;
    [SerializeField] private bool debugLogs = true;

    public bool IsGrabbed { get; set; }

    private Vector3 startPos;
    private Quaternion startRot;

    private MeshRenderer mr;
    private Collider col;
    private Rigidbody rb;

    private bool respawning;

    private void Log(string msg)
    {
        if (debugLogs) Debug.Log("[GrabbableCube] " + name + " | " + msg);
    }

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        mr = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        Log("Awake tag=" + tag +
            " rb=" + (rb != null) +
            " col=" + (col != null) +
            " isTrigger=" + (col != null && col.isTrigger));
    }

    public Collider GetCollider() => col;

   public void RespawnNow(string reason)
        {
            if (debugLogs) Debug.Log("[GrabbableCube] " + name + " | RespawnNow called reason=" + reason + " respawning=" + respawning);
            if (respawning) return;
            StartCoroutine(RespawnRoutine(reason));
        }


    private IEnumerator RespawnRoutine(string reason)
    {
        respawning = true;

        Log("RESPAWN start reason=" + reason);

        IsGrabbed = false;

        if (mr != null) mr.enabled = false;
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        yield return new WaitForSeconds(respawnDelay);

        transform.SetPositionAndRotation(startPos, startRot);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        if (col != null) col.enabled = true;

        bool visible = (MaskChange.CurrentColorTag == tag);
        if (mr != null) mr.enabled = visible;

        Log("RESPAWN end visible=" + visible + " CurrentColorTag=" + MaskChange.CurrentColorTag);

        respawning = false;
    }
}
