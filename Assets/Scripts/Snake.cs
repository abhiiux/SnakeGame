using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Snake : MonoBehaviour
{
    [SerializeField] InputAction inputAction;
    private float cellSize;
    
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private enum State
    {
        Alive,
        Dead
    }
    
    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private Vector2 directionalValue;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
        this.cellSize = levelGrid.GetCellSize();
        
        // FIXED: Start snake in the center of the actual grid
        int centerX = levelGrid.GetWidth() / 2;
        int centerY = levelGrid.GetHeight() / 2;
        gridPosition = new Vector2Int(centerX, centerY);
        
        Debug.Log($"Snake starting at grid position: {gridPosition} (Grid size: {levelGrid.GetWidth()}x{levelGrid.GetHeight()})");
        
        // Set initial world position
        Vector2 worldPos = levelGrid.GridToWorldPosition(gridPosition);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        
        Debug.Log($"Snake world position: {transform.position}");
    }
    
    private void OnEnable()
    {
        inputAction.Enable();
        inputAction.started += GetInput;
    }
    
    private void OnDisable() 
    {
        inputAction.started -= GetInput;    
    }
    
    private void Awake()
    {
        // Remove hardcoded position - will be set in Setup()
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;

        snakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
    }
    
    public void GetInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Vector2 input = context.ReadValue<Vector2>();
        Debug.Log($" input is receiving {input}");

        if (input.x > 0.5f && gridMoveDirection != Direction.Left)
        {
            gridMoveDirection = Direction.Right;
        }
        else if (input.x < -0.5f && gridMoveDirection != Direction.Right)
        {
            gridMoveDirection = Direction.Left;
        }
        else if (input.y > 0.5f && gridMoveDirection != Direction.Down)
        {
            gridMoveDirection = Direction.Up;
        }
        else if (input.y < -0.5f && gridMoveDirection != Direction.Up)
        {
            gridMoveDirection = Direction.Down;
        }
    }

    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);
            
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, +1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }

            gridPosition += gridMoveDirectionVector;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBodyPart();
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            // FIXED: Use GridToWorldPosition for proper positioning
            Vector2 worldPos = levelGrid.GridToWorldPosition(gridPosition);
            transform.position = new Vector3(worldPos.x, worldPos.y, 0);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

            UpdateSnakeBodyParts();
            
            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                Vector2Int snakeBodyPartPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBodyPartPosition)
                {
                    state = State.Dead;
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                    GameHandler.SnakeDied();
                }
            }
        }
    }

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count, levelGrid));
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        private LevelGrid levelGrid;

        public SnakeBodyPart(int bodyIndex, LevelGrid levelGrid)
        {
            this.levelGrid = levelGrid;
            
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1 - bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;

            // FIXED: Use GridToWorldPosition for proper positioning
            Vector2 worldPos = levelGrid.GridToWorldPosition(snakeMovePosition.GetGridPosition());
            transform.position = new Vector3(worldPos.x, worldPos.y, 0);

            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left:
                            angle = 0 + 45;
                            break;
                        case Direction.Right:
                            angle = 0 - 45;
                            break;
                    }
                    break;
                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left:
                            angle = 180 - 45;
                            break;
                        case Direction.Right:
                            angle = 180 + 45;
                            break;
                    }
                    break;
                case Direction.Left:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = +90;
                            break;
                        case Direction.Down:
                            angle = 180 - 45;
                            break;
                        case Direction.Up:
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Right:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down:
                            angle = 180 + 45;
                            break;
                        case Direction.Up:
                            angle = -45;
                            break;
                    }
                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }

    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }
    }
}