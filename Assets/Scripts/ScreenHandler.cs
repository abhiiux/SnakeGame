using UnityEngine;

public class ScreenHandler : MonoBehaviour
{
    [SerializeField] float cellSize;
    private float width, height;
    private Vector2 gridOffset;

    void Awake()
    {
        Camera mainCamera = Camera.main;
        float zDistance = Mathf.Abs(mainCamera.transform.position.z); // use proper depth

        // Convert screen corners to world positions
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));

        // Calculate grid offset and size
        gridOffset = new Vector2(bottomLeft.x, bottomLeft.y);

        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;

        this.width = Mathf.Max(1, Mathf.FloorToInt(worldWidth / cellSize));
        this.height = Mathf.Max(1, Mathf.FloorToInt(worldHeight / cellSize));
    }
    
    void Start()
    {
        // GameHandler.instance.SetScreenSize(width, height);
        GameAssets.instance.SetScreenSize(width, height);
    }
    
}
