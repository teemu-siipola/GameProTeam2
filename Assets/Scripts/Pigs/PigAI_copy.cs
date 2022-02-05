using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    public PigState _pigState;
    [SerializeField]
    private float _pigSpeedMultiplier;
    [SerializeField]
    private float _maximumPigVelocity;
    private GameObject _player;
    private PlayerSensor _sensor;
    private GameObject _sensorObject;
    private Rigidbody _rigidbody;
    private SphereCollider _collider;
    private SphereCollider _sensorCollider;
    private Quaternion tmpRot = Quaternion.identity;
    private Vector3 _newDirection;
    private Vector3 _playerDirection;
    private Vector3 _splineDirection;
    private Vector3 interpRot;
    private Vector3 referenceVelocity;
    private float _playerTriggerWait = 0.0f;
    private float _playerForce;
    private float _playerDistance;
    private bool _playerTriggered = false;
    private bool _isPlayerDetected;

    public enum PigState
    {
        Rolling,
        Walking,
        Noncontrolled,
    }

    void Awake()
    {
        _sensorObject = gameObject.transform.Find("PlayerSensor").gameObject;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<SphereCollider>();
        _sensor = GetComponentInChildren<PlayerSensor>();
        _sensorCollider = _sensorObject.GetComponent<SphereCollider>();
    }

    void Start()
    {
        StartCoroutine(LerpSpline());
        _player = GameObject.FindGameObjectWithTag("Player");

    }

    void FixedUpdate()
    {
        UpdateDirection();
        MoveEntity();
        RotateEntity();
    }

    void Update()
    {
        DetectPlayerDistance();
        _playerForce = Mathf.Clamp(100 - (((_playerDistance) / _sensorCollider.radius - 2f) * 100.0f), 0f, 100f);
        //if(_playerForce < 220 && _isPlayerDetected)
        //{
        //    _isPlayerDetected = false;
        //    _playerDistance = _sensorCollider.radius +1f;
        //}
        //_playerTriggerWait += Time.deltaTime;
        //if(_playerTriggered)
        //{
        //    if(_playerTriggerWait > 0.2f)
        //    {
        //        FlipDirection();
        //        _playerTriggerWait = 0;
        //        _playerTriggered = false;
        //    }
        //}

        // debug functionality, delete later
        if (Input.GetKeyDown(KeyCode.N))
        {
            _pigState = PigState.Rolling;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _pigState = PigState.Walking;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
            FlipDirection();
    }

    void DetectPlayerDistance()
    {
        if (_player == null)
            return;

        _playerDistance = Vector3.Distance(_player.transform.position, transform.position);
        _playerDirection = (_player.transform.position - transform.position).normalized;
        if (!_isPlayerDetected)
        {
            if (_playerDistance < _sensorCollider.radius)
            {
                _isPlayerDetected = true;
            }
        }
        else
        {
            if (_playerDistance > _sensorCollider.radius * 2f)
            {
                _isPlayerDetected = false;
            }
        }
    }

    // extremely suspicious code
    public void OnPlayerSensorTriggerStay(Collider collider)
    {
        return;
        _isPlayerDetected = true;
        Vector3 colliderPosition = collider.transform.position;
        _playerDistance = Vector3.Distance(transform.position, colliderPosition);

        int layerMask = 1 << 7;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, colliderPosition - transform.position, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, colliderPosition - transform.position, Color.yellow);
            _playerDirection = Vector3.Project(colliderPosition * 10 - transform.position * 10, hit.normal).normalized;
        }
    }

    public void OnPlayerSensorTriggerExit(Collider collider)
    {
        return;
        _isPlayerDetected = false;
        _playerDistance = _sensorCollider.radius;
    }

    private void MoveEntity()
    {

        switch (_pigState)
        {
            case PigState.Rolling:
                if (!_isPlayerDetected)
                    if (_rigidbody.velocity.magnitude < _maximumPigVelocity)
                    {
                        _rigidbody.AddForce(_newDirection * _pigSpeedMultiplier * 25 * Time.fixedDeltaTime);
                    }

                if (_isPlayerDetected)
                    if (_rigidbody.velocity.magnitude < (_maximumPigVelocity * 30f))
                    {
                        _rigidbody.AddForce((_playerDirection * -1) * (_playerForce * 0.5f) * _pigSpeedMultiplier * Time.fixedDeltaTime);
                    }
                break;

            case PigState.Walking:
                if (!_isPlayerDetected && _playerTriggered == false)
                    if (_rigidbody.velocity.magnitude < _maximumPigVelocity)
                    {
                        //_rigidbody.position = Vector3.Lerp(_rigidbody.position, _rigidbody.position - _splineDirection, Time.fixedDeltaTime * _pigSpeedMultiplier * 0.1f);
                        //_rigidbody.position = Vector3.SmoothDamp(_rigidbody.position, _rigidbody.position - _splineDirection, ref referenceVelocity, 0.3f);
                        _rigidbody.MovePosition(_rigidbody.position - _splineDirection * Time.fixedDeltaTime * _pigSpeedMultiplier * 0.01f);
                    }
                if (_isPlayerDetected)
                    if (true)
                    {
                        _playerTriggered = true;
                        // todo: implement bounciness
                        /*
                        _playerTriggered = true;
                        float yjump = 0f;
                        if(_playerForce > 45f)
                            yjump = (transform.position.y + 1f) * 0.01f;
                        else
                            yjump = transform.position.y * 0.001f;*/

                        //_rigidbody.position = Vector3.Lerp(_rigidbody.position, _rigidbody.position + new Vector3(_playerDirection.x * -1, 0, _playerDirection.z * -1).normalized * _pigSpeedMultiplier * (_playerForce*5f) * 0.0001f, Time.fixedDeltaTime);
                        Vector3 delta = new Vector3(-_playerDirection.x, 0f, -_playerDirection.z).normalized * _pigSpeedMultiplier * 0.0005f * _playerForce * Time.fixedDeltaTime; //playerForce
                        Debug.DrawLine(_rigidbody.position, _rigidbody.position + delta, Color.red, 0.5f);
                        Debug.LogError("this movement behavuiot");
                        _rigidbody.MovePosition(_rigidbody.position + delta);
                    }
                break;
        }
    }

    private void RotateEntity()
    {
        switch (_pigState)
        {
            case PigState.Rolling:
                _collider.material.dynamicFriction = 1.0f;
                _collider.material.staticFriction = 1.0f;
                break;

            case PigState.Walking:
                interpRot = _splineDirection.normalized;
                _collider.material.dynamicFriction = 0.0f;
                _collider.material.staticFriction = 0.0f;

                if (!_isPlayerDetected)
                    _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, Quaternion.LookRotation(interpRot * -1f, Vector3.up), 360f * Time.fixedDeltaTime);
                if (_isPlayerDetected)
                {
                    if (_playerDirection.magnitude > 0.1)
                        tmpRot = Quaternion.LookRotation(new Vector3(_playerDirection.x, 0, _playerDirection.z).normalized * -1f, Vector3.up);

                    _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, tmpRot, 360f * Time.fixedDeltaTime);
                }
                break;
        }
    }

    private void UpdateDirection()
    {
        _newDirection = GetRandomDirection();
    }

    private Vector3 GetRandomDirection()
    {
        Vector3 vec3 = new Vector3(
            Random.Range(-1.0f, 1.0f),
                          0,
            Random.Range(-1.0f, 1.0f)).normalized;
        return vec3;
    }

    private Vector3 FlipDirection()
    {
        _splineDirection *= -1;
        return _newDirection * -1;
    }

    private IEnumerator LerpSpline()
    {
        for (;;)
        {
            _splineDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.001f, Random.Range(-1.0f, 1.0f)).normalized;
            yield return new WaitForSeconds(5f);
        }
    }

    public IEnumerator ChangeToRollingAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _pigState = PigState.Rolling;
    }

}
