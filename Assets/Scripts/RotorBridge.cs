using UnityEngine;
public class RotorBridge : BridgeTile
{
    [SerializeField] float angularSpeed;
    [SerializeField] new BoxCollider collider;
    Vector3 Center => transform.position + transform.ToWorld(collider.center);
    bool clockwise;
    protected override void OnTakeFromPoolExtent()
    {
        clockwise = Random.Range(0, 2) > 0;
    }
    protected override void UpdateExtent()
    {
        transform.RotateAround(Center, clockwise ? Vector3.forward : Vector3.back, Time.deltaTime * angularSpeed);
    }
}
