using UnityEngine;

/// <summary>Basic third-person placeholder movement. Connect UI buttons/joystick for Android.</summary>
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float gravity = -20f;
    public Transform cameraTransform;
    public bool runHeld;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector2 moveInput;

    private void Awake() => controller = GetComponent<CharacterController>();

    // Call this from a mobile joystick or UI input script.
    public void SetMoveInput(Vector2 input) => moveInput = Vector2.ClampMagnitude(input, 1f);
    public void SetRun(bool value) => runHeld = value;

    private void Update()
    {
        Transform cam = cameraTransform != null ? cameraTransform : Camera.main.transform;
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        Vector3 move = forward * moveInput.y + right * moveInput.x;

        if (move.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(move), 12f * Time.deltaTime);
        }

        controller.Move(move.normalized * (runHeld ? runSpeed : walkSpeed) * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
}
