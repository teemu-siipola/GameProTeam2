using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVacuum : MonoBehaviour
{
    public ParticleSystem _vfx;
    public float vacuumRadius;
    public float vacuumingAngle;
    public bool debug;
    public AudioSource sfxSource, sfxBackup, loopSource;
    public SoundEffects sfx;

    public int Inventory { get { return _inventory; } }

    bool _isVacuuming;
    bool _stopInput;
    int _inventory;
    LayerMask _pigLayer;
    List<PigAI> _storedPigList = new List<PigAI>();

    void Awake()
    {
        _pigLayer = 1 << LayerMask.NameToLayer("Pig");
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
        vacuumRadius = GameManager.Singleton.variables.playerVacuumRadius;
        vacuumingAngle = GameManager.Singleton.variables.playerVacuumAngle;
    }

    void Update()
    {
        PlayerInput();
        if (_isVacuuming)
        {
            Vacuum();
        }
    }

    void PlayerInput()
    {
        if (_stopInput) return;

        if (Input.GetMouseButtonDown(1))
        {
            StartVacuum();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            EndVacuum();
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (_inventory > 0)
                StartCoroutine(ShootPig());
            else sfxSource.PlayOneShot(sfx.vacuumEmpty);
        }
        

        if (debug && Input.GetKeyDown(KeyCode.Mouse2))
            _inventory++;
            
    }

    void StartVacuum()
    {
        _isVacuuming = true;
        if (_vfx != null)
        {
            _vfx.Play();
            sfxSource.PlayOneShot(sfx.vacuumStart);
            //sfxLoop.Play();
            loopSource.clip = sfx.vacuumLoop;
            loopSource.PlayDelayed(sfx.vacuumStart.length);
        }
    }

    void EndVacuum()
    {
        _isVacuuming = false;
        if (_vfx != null)
        {
            _vfx.Stop();
        }
        loopSource.Stop();
        sfxSource.PlayOneShot(sfx.vacuumEnd);
        //sfxEnd.PlayDelayed(0.1f);
    }

    void Vacuum()
    {
        ScanForPigs();
    }

    void ScanForPigs()
    {
        Collider[] pigs = Physics.OverlapSphere(transform.position, vacuumRadius, _pigLayer);

        if (debug)
        {
            Debug.DrawRay(transform.position, transform.forward * vacuumRadius, Color.red);
        }

        foreach (Collider col in pigs)
        {
            if (PigInSuctionSector(col.transform))
            {
                PigAI pig = col.GetComponent<PigAI>();
                if (pig != null && pig._pigState != PigAI.PigState.Vacuuming)
                    StartCoroutine(VacuumPigRoutine2(pig));
            }
        }
    }

    bool PigInSuctionSector(Transform pig)
    {
        Vector3 playerPos = _vfx.transform.position;
        playerPos.y = 0;
        Vector3 pigPos = pig.position;
        pigPos.y = 0;

        Vector3 playerToPig = pigPos - playerPos;
        float angle = Vector3.Angle(transform.forward, playerToPig);
        return angle < vacuumingAngle; 
    }

    IEnumerator ShootPig()
    {
        sfxSource.PlayOneShot(sfx.vacuumSpit);
        _inventory--;
        yield return new WaitForSeconds(0.4f);
        PigAI pig = _storedPigList[ _storedPigList.Count - 1];
        _storedPigList.Remove(pig);

        pig.transform.position = _vfx.transform.position;
        pig.gameObject.SetActive(true);
        PigPhysicsFunctions physics = pig.GetComponent<PigPhysicsFunctions>();
        physics.TurnPhysicsOn();
        physics.AddForce((transform.forward + transform.up * GameManager.Singleton.variables.pigShootUpwardBias).normalized * GameManager.Singleton.variables.pigShootingForce);
        pig.StartCoroutine(pig.ChangeToRollingAfterSeconds(4f));
        PigAnimator ator = pig.GetComponent<PigAnimator>();
        ator.Spit();
    }

    void GameEnded()
    {
        _stopInput = true;
    }

    void StorePig(PigAI pig)
    {
        sfxSource.PlayOneShot(sfx.vacuumSuck);
        sfxBackup.PlayOneShot(sfx.RandomPigSucked());
        pig.gameObject.SetActive(false);
        _inventory++;
        _storedPigList.Add(pig);
    }

    IEnumerator VacuumPigRoutine(PigAI pig)
    {
        pig._pigState = PigAI.PigState.Vacuuming;
        PigPhysicsFunctions physics = pig.GetComponent<PigPhysicsFunctions>();
        physics.TurnPhysicsOff();
        PigAnimator ator = pig.GetComponent<PigAnimator>();
        ator.Vacuum();
        Vector3 originalPosition = pig.transform.position;
        float lerpTime = 3f;
        float timer = 0f;
        Quaternion startRotation = pig.transform.rotation;
        while (_isVacuuming && timer < lerpTime)
        {
            pig.transform.position = Vector3.Lerp(originalPosition, _vfx.transform.position + _vfx.transform.forward, timer / lerpTime);
            Quaternion target = Quaternion.LookRotation(transform.position - pig.transform.position);
            Quaternion rot;
            rot = Quaternion.Slerp(startRotation, target, timer / (lerpTime * 0.5f));
            pig.transform.rotation = rot;
            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;
        ator.ResetAnimations();
        if (_isVacuuming)
        {
            StorePig(pig);
        }
        else
        {
            pig.StartCoroutine(pig.ChangeToRollingAfterSeconds(0.2f));
            physics.TurnPhysicsOn();
            yield return new WaitForSeconds(0.2f);
            ator.ResetAnimations();
        }
    }

    IEnumerator VacuumPigRoutine2(PigAI pig)
    {
        //move towards target position with constant speed
        pig._pigState = PigAI.PigState.Vacuuming;
        PigPhysicsFunctions physics = pig.GetComponent<PigPhysicsFunctions>();
        physics.TurnPhysicsOff();

        Quaternion startRotation = pig.transform.rotation;
        float pigVacuumSpeed = GameManager.Singleton.variables.pigVacuumSpeedTowardsPlayer;
        float distance = Vector3.Distance(pig.transform.position, _vfx.transform.position);
        float animationTime = 1f;
        float rotationTime = 1f;
        float acceleration = GameManager.Singleton.variables.pigVacuumAccelerationTowardsPlayer;
        bool once = false;
        while (_isVacuuming && animationTime > 0f && PigInSuctionSector(pig.transform))
        {
            yield return null;
            Quaternion target = Quaternion.LookRotation(_vfx.transform.position - pig.transform.position);
            Quaternion rot;
            rot = Quaternion.Slerp(startRotation, target, (1f - rotationTime) / 1f);
            rotationTime = Mathf.Clamp(rotationTime -= Time.deltaTime, 0f, 1f);
            pig.transform.rotation = rot;

            if (distance > 1f)
            {
                Vector3 movementDelta = Time.deltaTime * pigVacuumSpeed * (_vfx.transform.position - pig.transform.position).normalized;
                pig.transform.position += movementDelta;
                distance = Vector3.Distance(pig.transform.position, _vfx.transform.position);
                pigVacuumSpeed += acceleration * Time.deltaTime;
            }
            //player reached, enter 'seconds mode
            else
            {
                if (!once)
                {
                    pig.GetComponent<PigAnimator>().Vacuum();
                    once = true;
                }
                pig.transform.position = _vfx.transform.position + (_vfx.transform.forward * 2f);
                pig.transform.LookAt(_vfx.transform.position - Vector3.up*0.5f);
                animationTime -= Time.deltaTime;
            }
        }

        pig.GetComponent<PigAnimator>().ResetAnimations();
        if (_isVacuuming && PigInSuctionSector(pig.transform))
        {
            StorePig(pig);
        }
        else
        {
            pig.StartCoroutine(pig.ChangeToRollingAfterSeconds(0.2f));
            physics.TurnPhysicsOn();
            yield return new WaitForSeconds(0.2f);
            pig.GetComponent<PigAnimator>().ResetAnimations();
        }

    }
}
