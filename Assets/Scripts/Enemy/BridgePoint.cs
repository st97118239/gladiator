using UnityEngine;

public class BridgePoint : MonoBehaviour
{
    public BridgePoint otherBridgePoint;

    public RangedPoint[] rangedPoints;
    public Platform platform;

    public Transform bridgeEntrance;

    public bool isBroken;
}
