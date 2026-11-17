using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PoliceChaseAI : MonoBehaviour
{
    public Transform target;
    public WantedSystem wantedSystem;
    public float acceleration = 6500f;
    public float maxSpeed = 24f;
    public float turnSpeed = 75f;
    public float stopDistance = 5f;
    public AudioSource sirenAudio;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.mass = 1900f;
        body.centerOfMass = new Vector3(0, -0.4f, 0);
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        bool shouldSound = target != null &&
            wantedSystem != null && wantedSystem.wantedLevel > 0;
        if (sirenAudio != null && sirenAudio.isPlaying != shouldSound)
        {
            if (shouldSound) sirenAudio.Play();
            else sirenAudio.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (target == null || wantedSystem == null || wantedSystem.wantedLevel <= 0)
            return;

        Vector3 toTarget = target.position - transform.position;
        Vector3 flat = Vector3.ProjectOnPlane(toTarget, Vector3.up);
        if (flat.magnitude <= stopDistance)
        {
            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 2f * Time.fixedDeltaTime);
            return;
        }

        Quaternion desired = Quaternion.LookRotation(flat.normalized, Vector3.up);
        body.MoveRotation(Quaternion.RotateTowards(
            body.rotation, desired, turnSpeed * Time.fixedDeltaTime));

        if (body.velocity.magnitude < maxSpeed)
            body.AddForce(transform.forward * acceleration * Time.fixedDeltaTime,
                ForceMode.Force);
    }

    private void OnDestroy()
    {
        if (sirenAudio != null) sirenAudio.Stop();
    }
}
