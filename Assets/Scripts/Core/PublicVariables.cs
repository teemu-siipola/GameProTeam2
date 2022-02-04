using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Public Variables")]
public class PublicVariables : ScriptableObject
{
    public float playerMovementSpeed;
    public float playerVacuumAngle;
    public float playerVacuumRadius;
    public float pigVacuumSpeedTowardsPlayer;
    public float pigVacuumAccelerationTowardsPlayer;
    public float pigShootingForce;
    public float pigShootUpwardBias;
    public float playTime;
    public float cameraSmooth;
    public Vector3 cameraOffset;

}
