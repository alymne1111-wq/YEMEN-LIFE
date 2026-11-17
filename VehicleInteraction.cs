using UnityEngine;

/// <summary>Basic enter/exit interaction. Call EnterOrExitVehicle from an Android UI button.</summary>
public class VehicleInteraction : MonoBehaviour
{
    public float interactionRange = 4f;
    public Transform exitPoint;
    public GameObject playerVisual;
    public MonoBehaviour playerMovementScript;

    private VehicleController currentVehicle;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody != null) playerRigidbody.isKinematic = true;
    }

    public void EnterOrExitVehicle()
    {
        if (currentVehicle != null) ExitVehicle();
        else TryEnterVehicle();
    }

    private void TryEnterVehicle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);
        VehicleController nearest = null;
        float best = float.MaxValue;

        foreach (Collider hit in hits)
        {
            VehicleController vehicle = hit.GetComponentInParent<VehicleController>();
            if (vehicle == null || vehicle.IsOccupied) continue;
            float distance = Vector3.Distance(transform.position, vehicle.transform.position);
            if (distance < best) { best = distance; nearest = vehicle; }
        }

        if (nearest == null) return;

        currentVehicle = nearest;
        currentVehicle.IsOccupied = true;
        transform.SetParent(currentVehicle.transform);
        transform.localPosition = new Vector3(0, 1.2f, 0);
        if (playerVisual != null) playerVisual.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = false;
    }

    private void ExitVehicle()
    {
        currentVehicle.IsOccupied = false;
        currentVehicle.SetThrottle(0);
        currentVehicle.SetSteering(0);
        transform.SetParent(null);
        transform.position = exitPoint != null
            ? exitPoint.position
            : currentVehicle.transform.position + currentVehicle.transform.right * 2f;
        if (playerVisual != null) playerVisual.SetActive(true);
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        currentVehicle = null;
    }
}
