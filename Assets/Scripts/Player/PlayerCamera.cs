using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCamera : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private Camera _cam;
    private Vector2 _move;
    private Vector3 _clampedPos;

    private Vector2 _scrollMouse;

    private int _minFieldOfView = 25;
    private int _maxFieldOfView = 39;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 5f;
    [Header("Boundaries")]
    [SerializeField] private float _xMin = -39f;
    [SerializeField] private float _xMax = -34f;
    [SerializeField] private float _yMin = 15f;
    [SerializeField] private float _yMax = 22f;
    [SerializeField] private float _zMin = 15f;
    [SerializeField] private float _zMax = 22f;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        if (_cam == null)
        {
            Debug.LogError("Camera is null on Camera object");
        }

        _playerInput = new PlayerInput();
        _playerInput.Enable();
    }

    private void Update()
    {
        MoveCamera();
        Zoom();
    }

    private void MoveCamera()
    {
        //Get Value for XY
        _move = _playerInput.Player.Movement.ReadValue<Vector2>();

        //Check if we are moving to left,right up or down
        if (_move.x < 0)
        {
            _cam.transform.Translate((Vector3.left * _movementSpeed) * Time.deltaTime);
        }

        else if (_move.x > 0)
        {
            _cam.transform.Translate((Vector3.right * _movementSpeed) * Time.deltaTime);
        }

        if (_move.y < 0)
        {
            _cam.transform.Translate((Vector3.down * _movementSpeed) * Time.deltaTime);
        }

        else if (_move.y > 0)
        {
            _cam.transform.Translate((Vector3.up * _movementSpeed) * Time.deltaTime);
        }

        _clampedPos = new Vector3(Mathf.Clamp(_cam.transform.position.x, _xMin, _xMax), Mathf.Clamp(_cam.transform.position.y, _yMin, _yMax), Mathf.Clamp(_cam.transform.position.z, _zMin, _zMax));
        _cam.transform.position = _clampedPos;
    }

    private void Zoom()
    {
        _scrollMouse = _playerInput.Player.Zoom.ReadValue<Vector2>();

        if (_scrollMouse.y > 0 && _cam.fieldOfView > _minFieldOfView)
        {
            _cam.fieldOfView--;
        }

        if (_scrollMouse.y < 0 && _cam.fieldOfView < _maxFieldOfView)
        {
            _cam.fieldOfView++;
        }
    }
}
