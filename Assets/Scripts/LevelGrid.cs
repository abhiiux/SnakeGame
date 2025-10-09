using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private int width, height;
    private GameObject foodGameObject;
    private Snake snake;
    private Transform foodContainer;
    
    private Vector2 gridOffset;

    public LevelGrid(Camera mainCamera)
    {
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        gridOffset = new Vector2(bottomLeft.x, bottomLeft.y);
        
        this.width = Mathf.Max(1, Mathf.FloorToInt(topRight.x - bottomLeft.x) + 1);
        this.height = Mathf.Max(1, Mathf.FloorToInt(topRight.y - bottomLeft.y) + 1);
        
        foodContainer = new GameObject("Food Container").transform;
    }
    
    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }
    
    private void SpawnFood()
    {
        int maxAttempts = width * height;
        int attempts = 0;
        
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width - 2), Random.Range(0, height - 2));
            attempts++;
            
            if (attempts > maxAttempts)
            {
                Debug.LogError("Unable to spawn food - grid may be full!");
                return;
            }
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        
        // Clean up old food if it exists
        if (foodGameObject != null)
        {
            Object.Destroy(foodGameObject);
        }
        
        // Create new food
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.transform.SetParent(foodContainer);
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        
        // FIXED: Convert grid position to world position using offset
        Vector2 worldPosition = GridToWorldPosition(foodGridPosition);
        foodGameObject.transform.position = worldPosition;
        
        Debug.Log($"Food spawned at Grid: {foodGridPosition}, World: {worldPosition}");
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            
            CMDebug.TextPopupMouse("Snake Ate Food");
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
        // Wrap around grid boundaries (Pac-Man style)
        if (gridPosition.x < 0)
        {
            gridPosition.x = width - 1;
        }
        if (gridPosition.x >= width)
        {
            gridPosition.x = 0;
        }
        if (gridPosition.y < 0)
        {
            gridPosition.y = height - 1;
        }
        if (gridPosition.y >= height)
        {
            gridPosition.y = 0;
        }
        return gridPosition;
    }
    
    // NEW: Convert grid coordinates to world coordinates
    public Vector2 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector2(gridPosition.x + gridOffset.x, gridPosition.y + gridOffset.y);
    }
    
    // NEW: Convert world coordinates to grid coordinates
    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x - gridOffset.x),
            Mathf.FloorToInt(worldPosition.y - gridOffset.y)
        );
    }
    
    // Utility methods
    public int GetWidth() => width;
    public int GetHeight() => height;
    public Vector2 GetGridOffset() => gridOffset;
    
    // Cleanup method
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
}