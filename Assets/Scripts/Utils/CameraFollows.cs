using UnityEngine;

public class CameraFollows : MonoBehaviour
{
    public Transform player;
    public Transform bottomLeft;
    public Transform topRight;
    public float smoothSpeed = 0.125f;

    private Vector3 targetPosition;
    private Vector2 cameraDimensions;

    [SerializeField] private Vector2 offset;

    void Start()
    {
        cameraDimensions = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize * 2,
            Camera.main.orthographicSize * 2
        );
    }

    private void LateUpdate()
    {
        if (player == null) return;
        targetPosition = player.position + new Vector3(offset.x, offset.y, transform.position.z);

        float minX = bottomLeft.position.x + cameraDimensions.x / 2;
        float maxX = topRight.position.x - cameraDimensions.x / 2;
        float minY = bottomLeft.position.y + cameraDimensions.y / 2;
        float maxY = topRight.position.y - cameraDimensions.y / 2;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
