using UnityEngine;

[CreateAssetMenu(fileName = "Boid Settings", menuName = "ScriptableObjects/Boids", order = 1)]
public class BoidSettings : ScriptableObject {
    [Header("Debug")]
    public bool EnableMouseTargeting = false;

    [Header("Speed")]
    public float minSpeed = 2;
    public float maxSpeed = 3;
    public float rotationSpeed = .5f;
    public float maxSteerForce = 100f;

    [Header("Vision")]
    public float visionRange = 2f;
    public float visionAngle = 120;

    [Header("Forces")]
    //Boid Collision Avoidance Vector Konstant
    public float Ksep = 1.5f;
    //Obstacle Collision Avoidance Vector Konstant
    public float Kcav = 1f;
    //Forward Vector Konstant
    public float Kfow = 0.1f;
    // Alignment Konstant
    public float Kali = 2f;
    // Cohesion  Konstant
    public float Kcoh = 1f;

    public float mouseTargetingSpeed = 10f;

    [Header("Global constants")]
    public int ObstacleLayer = 3;
}
