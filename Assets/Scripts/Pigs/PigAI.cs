using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* - - - - - - - - - - - - - - - - - - - - - - *
*
*                   ~ Pig AI ~            
*
*   freerolling mode uses velocity
*   walk mode sets transform.position
*
*   current debug functionality:
*   - enable freerolling mode with "N"
*   - enable walk mode with "M"
*
* - - - - - - - - - - - - - - - - - - - - - - */

public class PigAI : MonoBehaviour
{
    [SerializeField] public PigState _pigState;
    [SerializeField] private float _pigSpeedMultiplier;
    [SerializeField] private float _maximumPigVelocity;
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
    private float _playerTriggerWait = 0.0f;
    private float _playerForce;
    private float _playerDistance;
    private bool _playerTriggered = false;
    private bool _isPlayerDetected;

    public enum PigState
    {
        Rolling,
        Walking,
        Vacuuming // Does not do anything
    }

    void Awake()
    {
        _sensorObject = gameObject.transform.Find("PlayerSensor").gameObject;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _sensor = GetComponentInChildren<PlayerSensor>();
        _sensorCollider = _sensorObject.GetComponent<SphereCollider>();
    }

    void Start()
    {
        StartCoroutine(LerpSpline());
    }
    
    void FixedUpdate() 
    {
        UpdateDirection();
        MoveEntity();
        RotateEntity();
    }

    void Update()
    {
        _playerForce = 100 - (((_playerDistance) / _sensorCollider.radius -2f) * 100.0f);
        if(_playerForce < 220 && _isPlayerDetected)
        {
            _isPlayerDetected = false;
            _playerDistance = _sensorCollider.radius +1f;
        }

        _playerTriggerWait += Time.deltaTime;
        if(_playerTriggered)
        {
            if(_playerTriggerWait > 0.2f)
            {
                FlipDirection();
                _playerTriggerWait = 0;
                _playerTriggered = false;
            }
        }

        // debug functionality, delete later
        if(Input.GetKeyDown(KeyCode.N))
        {
            _pigState = PigState.Rolling;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            _pigState = PigState.Walking;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 7)
            FlipDirection();
    }

    // extremely suspicious code
    public void OnPlayerSensorTriggerStay(Collider collider)
    {
        _isPlayerDetected = true;
        Vector3 colliderPosition = collider.transform.position;
        _playerDistance = Vector3.Distance(transform.position, colliderPosition);

        int layerMask = 1 << 7;
        layerMask = ~layerMask;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, colliderPosition - transform.position, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, colliderPosition - transform.position, Color.yellow);
            _playerDirection = Vector3.Project(colliderPosition * 10 - transform.position * 10, hit.normal).normalized;
        }
    }

    public void OnPlayerSensorTriggerExit(Collider collider)
    {
        _isPlayerDetected = false;
        _playerDistance = _sensorCollider.radius;
    }

    private void MoveEntity()
    {

        switch(_pigState)
        {
            case PigState.Rolling:
                if(!_isPlayerDetected)
                    if(_rigidbody.velocity.magnitude < _maximumPigVelocity)
                    {
                        _rigidbody.AddForce(_newDirection * _pigSpeedMultiplier * 25 * Time.fixedDeltaTime);
                    }

                if(_isPlayerDetected)
                    if(_rigidbody.velocity.magnitude < (_maximumPigVelocity*30f))
                    {
                        _rigidbody.AddForce((_playerDirection * -1) * (_playerForce * 0.5f) * _pigSpeedMultiplier * Time.fixedDeltaTime);
                    }
                break;

            case PigState.Walking:
                if(!_isPlayerDetected && _playerTriggered == false)
                    if(_rigidbody.velocity.magnitude < _maximumPigVelocity)
                    {
                        transform.position = Vector3.Lerp(transform.position, transform.position - _splineDirection, Time.fixedDeltaTime * _pigSpeedMultiplier * 0.1f);
                    }
                if(_isPlayerDetected)
                    if(true)
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

                        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(_playerDirection.x * -1, 0, _playerDirection.z * -1).normalized * _pigSpeedMultiplier * (_playerForce*5f) * 0.0001f, Time.fixedDeltaTime);
                    }
                break;
        }
    }

    private void RotateEntity()
    {
        switch(_pigState)
        {
            case PigState.Rolling:
                _collider.material.dynamicFriction = 1.0f;
                _collider.material.staticFriction = 1.0f;
                break;

            case PigState.Walking:
                interpRot = _splineDirection.normalized;
                _collider.material.dynamicFriction = 0.0f;
                _collider.material.staticFriction = 0.0f;

                if(!_isPlayerDetected)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(interpRot * -1f, Vector3.up), 360f * Time.fixedDeltaTime);
                if(_isPlayerDetected)
                {
                    if(_playerDirection.magnitude > 0.1)
                        tmpRot = Quaternion.LookRotation(new Vector3(_playerDirection.x, 0, _playerDirection.z).normalized * -1f, Vector3.up);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, tmpRot, 360f * Time.fixedDeltaTime);
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
        for(;;)
        {
            _splineDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.001f, Random.Range(-1.0f, 1.0f)).normalized;
            yield return new WaitForSeconds(5f);
        }
    }

}

// now with 50% more artificial intelligence! (and 50% less human intelligence)