using UnityEngine;

/// <summary>Arcade-style Rigidbody vehicle foundation; tune per vehicle and add wheel colliders later.</summary>
[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    public float acceleration = 8500f;
    public float reverseAcceleration = 4500f;
    public float maxSpeed = 32f;
    public float steeringPower = 85f;
    public float brakeStrength = 7f;

    [HideInInspector] public float throttleInput;
    [HideInInspector] public float steeringInput;
    [HideInInspector] public bool brakeInput;
    public bool IsOccupied { get; set; }

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.mass = 1700f;
        body.centerOfMass = new Vector3(0, -0.45f, 0);
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    // Connect Android UI buttons/steering controls to these methods.
    public void SetThrottle(float value) => throttleInput = Mathf.Clamp(value, -1f, 1f);
    public void SetSteering(float value) => steeringInput = Mathf.Clamp(value, -1f, 1f);
    public void SetBrake(bool value) => brakeInput = value;

    private void FixedUpdate()
    {
        if (!IsOccupied) return;

        float speed = Vector3.Dot(body.velocity, transform.forward);
        if (throttleInput > 0 && speed < maxSpeed)
            body.AddForce(transform.forward * throttleInput * acceleration * Time.fixedDeltaTime, ForceMode.Force);
        else if (throttleInput < 0)
            body.AddForce(transform.forward * throttleInput * reverseAcceleration * Time.fixedDeltaTime, ForceMode.Force);

        float steeringFactor = Mathf.Clamp01(body.velocity.magnitude / 4f);
        float turn = steeringInput * steeringPower * steeringFactor * Time.fixedDeltaTime;
        body.MoveRotation(body.rotation * Quaternion.Euler(0, turn, 0));

        if (brakeInput)
            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, brakeStrength * Time.fixedDeltaTime);
    }
}
