using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;

    Camera _mainCamera;
    LayerMask _groundLayer;
    Rigidbody _rb;
    Vector3 _movementInput = new Vector3();
    Vector3 _lookAtPosition;

    bool _stopInput;

    void Awake()
    {
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        GameManager.GameWon += GameEnded;
        GameManager.GameLost += GameEnded;
    }

    void OnDisable()
    {
        GameManager.GameWon -= GameEnded;
        GameManager.GameLost -= GameEnded;
    }

    void Start()
    {
        GameManager.Singleton.ResetGame();
    }

    void Update()
    {
        if (!_stopInput)
        {
            MovementInput();
            _lookAtPosition = GetMousePosition();
        }
    }

    void FixedUpdate()
    {
        RotatePlayer();
        MovePlayer();
    }

    Vector3 GetMousePosition()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    void MovementInput()
    {
        _movementInput.x = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f ? Input.GetAxis("Horizontal") : 0f;
        _movementInput.z = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f ? Input.GetAxis("Vertical") : 0f;
    }

    void MovePlayer()
    {
        _rb.MovePosition(_rb.position + _movementInput.normalized * Time.fixedDeltaTime * movementSpeed);
    }

    void RotatePlayer()
    {
        if (_lookAtPosition != Vector3.zero)
        {
            _rb.MoveRotation(Quaternion.LookRotation(_lookAtPosition - transform.position, Vector3.up));
        }
    }

    void GameEnded()
    {
        _stopInput = true;
        _movementInput = Vector3.zero;
    }

}
