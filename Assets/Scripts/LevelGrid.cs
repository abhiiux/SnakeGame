using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private float width, height;
    private int gridWidth, gridHeight;
    private GameObject foodGameObject;
    private Snake snake;
    private Transform foodContainer;
    
    private Vector2 gridOffset;
    private float cellSize;
    private Camera mainCamera;

    public LevelGrid(Camera mainCamera, float cellSize = 2f)
    {
        this.cellSize = cellSize;
        this.mainCamera = mainCamera;
        
        // Calculate the actual visible world space bounds
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        // Get camera position
        Vector3 camPos = mainCamera.transform.position;
        
        // Calculate world bounds
        float leftEdge = camPos.x - (cameraWidth / 2f);
        float rightEdge = camPos.x + (cameraWidth / 2f);
        float bottomEdge = camPos.y - (cameraHeight / 2f);
        float topEdge = camPos.y + (cameraHeight / 2f);
        
        gridOffset = new Vector2(leftEdge, bottomEdge);
        
        this.width = cameraWidth;
        this.height = cameraHeight;
        
        // Calculate grid dimensions - how many cells fit in the screen
        this.gridWidth = Mathf.FloorToInt(width / cellSize);
        this.gridHeight = Mathf.FloorToInt(height / cellSize);
        
        // Ensure minimum grid size
        this.gridWidth = Mathf.Max(2, gridWidth);
        this.gridHeight = Mathf.Max(2, gridHeight);
        
        Logger($"=== LevelGrid Initialization ===");
        Logger($"Camera Orthographic Size: {mainCamera.orthographicSize}");
        Logger($"Camera Aspect: {mainCamera.aspect}");
        Logger($"Camera Position: {camPos}");
        Logger($"World Bounds: Left={leftEdge}, Right={rightEdge}, Bottom={bottomEdge}, Top={topEdge}");
        Logger($"Screen World Size: {width}x{height} units");
        Logger($"Grid: {gridWidth}x{gridHeight} cells");
        Logger($"CellSize: {cellSize}");
        Logger($"Grid Offset: {gridOffset}");
        
        foodContainer = new GameObject("Food Container").transform;
    }
    
    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }
    
    private void SpawnFood()
    {
        int maxAttempts = gridWidth * gridHeight;
        int attempts = 0;
        
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight));
            attempts++;
            
            if (attempts > maxAttempts)
            {
                Debug.LogError($"Unable to spawn food! Grid: {gridWidth}x{gridHeight}, Snake Length: {snake.GetFullSnakeGridPositionList().Count}");
                return;
            }
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        
        if (foodGameObject != null)
        {
            Object.Destroy(foodGameObject);
        }
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.transform.SetParent(foodContainer);
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        
        Vector2 worldPosition = GridToWorldPosition(foodGridPosition);
        foodGameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        
        Logger($"Food spawned at Grid: {foodGridPosition}, World: {worldPosition}");
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            
            // CMDebug.TextPopupMouse($"Snake Ate Food");
            GameHandler.AddScore();
            
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        // Wrap horizontally
        if (gridPosition.x < 0)
        {
            gridPosition.x = gridWidth - 1;
        }
        else if (gridPosition.x >= gridWidth)
        {
            gridPosition.x = 0;
        }
        
        // Wrap vertically
        if (gridPosition.y < 0)
        {
            gridPosition.y = gridHeight - 1;
        }
        else if (gridPosition.y >= gridHeight)
        {
            gridPosition.y = 0;
        }
        
        return gridPosition;
    }
    
    public Vector2 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector2(
            (gridPosition.x * cellSize) + gridOffset.x + (cellSize / 2f),
            (gridPosition.y * cellSize) + gridOffset.y + (cellSize / 2f)
        );
    }
    
    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt((worldPosition.x - gridOffset.x) / cellSize),
            Mathf.FloorToInt((worldPosition.y - gridOffset.y) / cellSize)
        );
    }
    
    public int GetWidth() => gridWidth;
    public int GetHeight() => gridHeight;
    public float GetScreenWidth() => width;
    public float GetScreenHeight() => height;
    public Vector2 GetGridOffset() => gridOffset;
    public float GetCellSize() => cellSize;
    
    public void Cleanup()
    {
        if (foodGameObject != null)
        {
            Object.Destroy(foodGameObject);
        }
        if (foodContainer != null)
        {
            Object.Destroy(foodContainer.gameObject);
        }
    }

    private void Logger(string message)
    {
#if UNITY_EDITOR
        Debug.Log($"{message}");
#endif
    }
}