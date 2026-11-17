using UnityEngine;

/// <summary>Small bridge for Unity UI buttons. Hook Button PointerDown/Up events to these methods.</summary>
public class AndroidInputBridge : MonoBehaviour
{
    public VehicleController activeVehicle;
    public ThirdPersonPlayer player;

    public void AccelerateDown() { if (activeVehicle != null) activeVehicle.SetThrottle(1f); }
    public void ReverseDown() { if (activeVehicle != null) activeVehicle.SetThrottle(-1f); }
    public void ThrottleUp() { if (activeVehicle != null) activeVehicle.SetThrottle(0f); }
    public void BrakeDown() { if (activeVehicle != null) activeVehicle.SetBrake(true); }
    public void BrakeUp() { if (activeVehicle != null) activeVehicle.SetBrake(false); }
    public void SteerLeft() { if (activeVehicle != null) activeVehicle.SetSteering(-1f); }
    public void SteerRight() { if (activeVehicle != null) activeVehicle.SetSteering(1f); }
    public void SteeringRelease() { if (activeVehicle != null) activeVehicle.SetSteering(0f); }
}
