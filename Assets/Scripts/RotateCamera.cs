using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float[] zAngles;         // List of Z angles to rotate to (e.g., [0, 45, 90])
    [SerializeField] private float rotationSpeed = 10עןאf;
    private int currentIndex = 0;
    private Quaternion targetRotation;

    private void Start()
    {
        if (zAngles.Length == 0) return;
        SetNextTarget();
    }

    private void Update()
    {
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            SetNextTarget();
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void SetNextTarget()
    {
        currentIndex = (currentIndex + 1) % zAngles.Length;
        float z = zAngles[currentIndex];
        targetRotation = Quaternion.Euler(0f, 0f, z);
    }
}