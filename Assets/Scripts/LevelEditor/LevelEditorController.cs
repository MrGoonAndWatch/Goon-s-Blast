using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Constants;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelEditorController : MonoBehaviour
{
    [SerializeField]
    private Transform _cursor;
    [SerializeField]
    private int _tileSize = 1;
    [SerializeField]
    private TMP_Text _currentBlockDisplayText;
    [SerializeField]
    private TilePrefabLookup _lookup;
    [SerializeField]
    private TMP_InputField _levelNameInput;
    [SerializeField]
    private GameObject _mainEditorCanvas;
    [SerializeField]
    private GameObject _minimizedEditorCanvas;
    [SerializeField]
    private PlayerControls _editorControls;
    [SerializeField]
    private GameObject _cameraContainer;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private float _cameraMaxDistanceFromCursor = 8.0f;
    [SerializeField]
    private float _cameraMoveSpeed = 0.5f;

    private const float HorizontalMouseSensitivity = 1.0f;
    private const float VerticalMouseSensitivity = 1.0f;
    private float _verticalLookRotation;

    private InputAction _rotateCamera;
    private InputAction _moveXZ;
    private InputAction _moveUp;
    private InputAction _moveDown;
    private InputAction _tileNextForwards;
    private InputAction _tileBack;
    private InputAction _placeTile;
    private InputAction _saveLevel;
    private InputAction _minimizeUi;

    private bool _disableInputs;
    private bool _movingHorizontally;
    private bool _movingVertically;

    private LevelData _levelData;
    private List<LevelEditorTile> _generatedTiles;

    private int _currentX;
    private int _currentY;
    private int _currentZ;
    private TileType _currentBlockType;
    private bool _minimized = true;

    private Vector2 _rotateCameraInput;
    private bool _rotatingCamera;

    void Awake()
    {
        _editorControls = new PlayerControls();
    }

    void OnEnable()
    {
        _moveXZ = _editorControls.Editor.MoveXZ;
        _moveUp = _editorControls.Editor.MoveUp;
        _moveDown = _editorControls.Editor.MoveDown;
        _tileNextForwards = _editorControls.Editor.NextTile;
        _tileBack = _editorControls.Editor.PreviousTile;
        _placeTile = _editorControls.Editor.PlaceTile;
        _saveLevel = _editorControls.Editor.SaveMenu;
        _minimizeUi = _editorControls.Editor.HelpMenu;
        _rotateCamera = _editorControls.Editor.RotateCamera;

        _moveXZ.Enable();
        _moveUp.Enable();
        _moveDown.Enable();
        _tileNextForwards.Enable();
        _tileBack.Enable();
        _placeTile.Enable();
        _saveLevel.Enable();
        _minimizeUi.Enable();
        _rotateCamera.Enable();

        _moveXZ.performed += OnMoveCursor;
        _moveXZ.canceled += OnStopMovingCursor;
        _moveUp.performed += OnMoveCursorUp;
        _moveDown.performed += OnMoveCursorDown;
        _tileNextForwards.performed += OnTileNext;
        _tileBack.performed += OnTileBack;
        _placeTile.performed += OnPlaceTile;
        _saveLevel.performed += OnSaveLevel;
        _minimizeUi.performed += OnMinimize;
        _rotateCamera.performed += OnRotateCamera;
        _rotateCamera.canceled += OnRotateCameraEnd;
    }

    void OnDisable()
    {
        _moveXZ.Disable();
        _moveUp.Disable();
        _moveDown.Disable();
        _tileNextForwards.Disable();
        _tileBack.Disable();
        _placeTile.Disable();
        _saveLevel.Disable();
        _minimizeUi.Disable();
        _rotateCamera.Disable();
    }

    private void Start()
    {
        // TODO: Allow loading an existing level in for init state.
        _generatedTiles = new List<LevelEditorTile>();
        _levelData = new LevelData();
        UpdateBlockTypeDisplay();
        UpdateCursor();
    }

    private void Update()
    {
        if(_rotatingCamera)
            HandleRotateCamera();
        FollowCursorWithCamera();
    }

    private void HandleRotateCamera()
    {
        _cameraContainer.transform.Rotate(Vector3.up * _rotateCameraInput.x * HorizontalMouseSensitivity);
        _verticalLookRotation += _rotateCameraInput.y * VerticalMouseSensitivity;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);
        _camera.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
    }

    private void FollowCursorWithCamera()
    {
        var distance = (_camera.transform.position - _cursor.position).magnitude;
        if (distance <= _cameraMaxDistanceFromCursor)
            return;

        _cameraContainer.transform.position = Vector3.MoveTowards(_cameraContainer.transform.position, _cursor.position, _cameraMoveSpeed);
    }

    private void OnRotateCamera(InputAction.CallbackContext context)
    {
        _rotateCameraInput = context.ReadValue<Vector2>();
        _rotatingCamera = true;
    }

    private void OnRotateCameraEnd(InputAction.CallbackContext context)
    {
        _rotatingCamera = false;
    }

    private void MoveCursorForwards()
    {
        _movingVertically = true;
        _currentZ += _tileSize;
        UpdateCursor();
    }

    private void MoveCursorBackwards()
    {
        _movingVertically = true;
        _currentZ -= _tileSize;
        UpdateCursor();
    }

    private void MoveCursorLeft()
    {
        _movingHorizontally = true;
        _currentX -= _tileSize;
        UpdateCursor();
    }
    private void MoveCursorRight()
    {
        _movingHorizontally = true;
        _currentX += _tileSize;
        UpdateCursor();
    }

    private void MoveCursorUp()
    {
        _currentY += _tileSize;
        UpdateCursor();
    }

    private void MoveCursorDown()
    {
        _currentY -= _tileSize;
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        _cursor.position = new Vector3(_currentX, _currentY, _currentZ);
    }

    private void ToggleMinimize()
    {
        if (_minimized)
        {
            _mainEditorCanvas.SetActive(true);
            _minimizedEditorCanvas.SetActive(false);
        }
        else
        {
            _mainEditorCanvas.SetActive(false);
            _minimizedEditorCanvas.SetActive(true);
        }

        _minimized = !_minimized;
    }

    private void SetTile()
    {
        if (_currentBlockType == TileType.None)
            EraseCurrentTile();
        else
            AddCurrentTile();
    }

    private void EraseCurrentTile()
    {
        var tilesRemoved = _levelData.Tiles.RemoveAll(d => d.X == _currentX && d.Y == _currentY && d.Z == _currentZ);
        if (tilesRemoved == 0)
            return;

        RemoveCurrentTile();
    }

    private void AddCurrentTile()
    {
        var currentTile = _levelData.Tiles.FirstOrDefault(d => d.X == _currentX && d.Y == _currentY && d.Z == _currentZ);
        if (currentTile == null)
            _levelData.Tiles.Add(new TileData { Type = _currentBlockType, X = _currentX, Y = _currentY, Z = _currentZ });
        else
        {
            currentTile.Type = _currentBlockType;
            RemoveCurrentTile();
        }

        var currentPos = new Vector3(_currentX, _currentY, _currentZ);
        var newObj = Instantiate(_lookup.GetPrefab(_currentBlockType), currentPos, Quaternion.identity).GameObject();
        _generatedTiles.Add(new LevelEditorTile
        {
            GeneratedObject = newObj,
            X = _currentX,
            Y = _currentY,
            Z = _currentZ
        });
    }

    private void RemoveCurrentTile()
    {
        var tile = _generatedTiles.FirstOrDefault(t => t.X == _currentX && t.Y == _currentY && t.Z == _currentZ);
        if (tile == null)
            return;
        Destroy(tile.GeneratedObject);
        _generatedTiles.Remove(tile);
    }
    
    private void CycleBlockBackwards()
    {
        var nextBlockIndex = (int)_currentBlockType - 1;
        if (nextBlockIndex < 0)
        {
            var blockTypes = Enum.GetValues(typeof(TileType));
            nextBlockIndex = (int)blockTypes.GetValue(blockTypes.Length - 1);
        }
        _currentBlockType = (TileType)nextBlockIndex;
        UpdateBlockTypeDisplay();
    }

    private void CycleBlockForward()
    {
        var nextBlockIndex = (int)_currentBlockType + 1;
        var blockTypes = Enum.GetValues(typeof(TileType));
        if (nextBlockIndex >= blockTypes.Length)
            nextBlockIndex = 0;
        _currentBlockType = (TileType)nextBlockIndex;
        UpdateBlockTypeDisplay();
    }

    // TODO: Update visual indicator to player when this happens!
    private void UpdateBlockTypeDisplay()
    {
        _currentBlockDisplayText.text = $"Current Tile: {_currentBlockType}";
    }

    private void SaveLevel()
    {
        // TODO: Do something better than debug.log!!!
        if (string.IsNullOrEmpty(_levelNameInput.text))
        {
            Debug.LogError("CANNOT SAVE FILE, NO LEVEL NAME GIVEN!");
            return;
        }

        var dir = Path.Combine(Application.persistentDataPath, "CustomLevels");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, $"{_levelNameInput.text}.level");

        // TODO: Do something better than debug.log!!!
        if (File.Exists(filePath))
            Debug.LogError($"CANNOT SAVE LEVEL '{_levelNameInput.text}', LEVEL ALREADY EXISTS!");
        else
        {
            var levelDataJson = JsonConvert.SerializeObject(_levelData, Formatting.None);
            File.WriteAllText(filePath, levelDataJson);
        }
    }

    // TODO: Make this work for joystick better (handle in update loop)!
    private void OnMoveCursor(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;

        var moveDir = context.ReadValue<Vector2>();
        if (moveDir.x > 0 && !_movingHorizontally)
            MoveCursorRight();
        else if (moveDir.x < 0 && !_movingHorizontally)
            MoveCursorLeft();
        if (moveDir.y > 0 && !_movingVertically)
            MoveCursorForwards();
        else if (moveDir.y < 0 && !_movingVertically)
            MoveCursorBackwards();
    }

    private void OnStopMovingCursor(InputAction.CallbackContext context)
    {
        _movingHorizontally = false;
        _movingVertically = false;
    }

    private void OnMoveCursorUp(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        MoveCursorUp();
    }

    private void OnMoveCursorDown(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        MoveCursorDown();
    }

    private void OnTileNext(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        CycleBlockForward();
    }

    private void OnTileBack(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        CycleBlockBackwards();
    }

    private void OnPlaceTile(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        SetTile();
    }

    private void OnSaveLevel(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        SaveLevel();
    }

    private void OnMinimize(InputAction.CallbackContext context)
    {
        if (_disableInputs)
            return;
        ToggleMinimize();
    }

    public void OnSaveFilenameInputFocus(string contents)
    {
        _disableInputs = true;
    }

    public void OnSaveFilenameInputUnfocus(string contents)
    {
        _disableInputs = false;
    }

    public void OnExitLevelEditor()
    {
        PhotonNetwork.LoadLevel((int)GameConstants.LevelIndexes.MainMenu);
    }
}
