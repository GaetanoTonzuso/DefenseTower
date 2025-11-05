using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameDevHQ.FileBase.Dual_Gatling_Gun
{
    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Dual_Gatling_Gun : MonoBehaviour, IAttack, IDamageable
    {
        [SerializeField] private GameObject _firePoint; //Reference to hold Shooting Point
        [SerializeField] private float _fireDistance = 30f;

        [SerializeField]
        private Transform[] _gunBarrel; //Reference to hold the gun barrel
        [SerializeField]
        private GameObject[] _muzzleFlash; //reference to the muzzle flash effect to play when firing
        [SerializeField]
        private ParticleSystem[] _bulletCasings; //reference to the bullet casing effect to play when firing
        [SerializeField]
        private AudioClip _fireSound; //Reference to the audio clip

        private AudioSource _audioSource; //reference to the audio source component
        private bool _startWeaponNoise = true;

        //Targets and Rotation settings
        [SerializeField] private GameObject _turret;
        [SerializeField] private float _rotationSpeed = 3f;
        [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
        [SerializeField] private Transform _currentTarget;

        Vector3 directionTarget;
        Quaternion targetDirection;
        private bool _hasTarget;
        private bool _isShooting;

        //Turret Health
        [SerializeField] private float _health = 75f;
        public float Health { get; set; }

        [SerializeField] private int _atkDamage = 4;
        public int AtkDamage { get; set; }

        private void OnEnable()
        {
            EventService.Instance.OnEnemyDie.AddListener(OnEnemyDie);
        }

        private void OnDisable()
        {
            EventService.Instance.OnEnemyDie.RemoveListener(OnEnemyDie);
        }

        // Use this for initialization
        void Start()
        {
            Health = _health;
            AtkDamage = _atkDamage;
            _muzzleFlash[0].SetActive(false); //setting the initial state of the muzzle flash effect to off
            _muzzleFlash[1].SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource = GetComponent<AudioSource>(); //ssign the Audio Source to the reference variable
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = _fireSound; //assign the clip to play
        }

        // Update is called once per frame
        void Update()
        {
            AimTarget();
        }

        private void FixedUpdate()
        {
            if (!_isShooting) return;

            Attack();
        }

        private void AimTarget()
        {
            if (_hasTarget)
            {
                Vector3 targetPos = _currentTarget.position + Vector3.up * -0.6f;
                directionTarget = targetPos - _turret.transform.position;

                if (directionTarget.sqrMagnitude < 0.001f) return;

                targetDirection = Quaternion.LookRotation(directionTarget);

                _turret.transform.rotation = Quaternion.Slerp(_turret.transform.rotation, targetDirection, Time.deltaTime * _rotationSpeed);

                //Check if we are aiming it and then fire
                float angleDifference = Quaternion.Angle(_turret.transform.rotation, targetDirection);
                if (angleDifference < 1.5f)
                {
                    _isShooting = true;
                    RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel
                    
                    //for loop to iterate through all muzzle flash objects
                    for (int i = 0; i < _muzzleFlash.Length; i++)
                    {
                        _muzzleFlash[i].SetActive(true); //enable muzzle effect particle effect
                        _bulletCasings[i].Emit(1); //Emit the bullet casing particle effect   
                    }

                    if (_startWeaponNoise == true) //checking if we need to start the gun sound
                    {
                        _audioSource.Play(); //play audio clip attached to audio source
                        _startWeaponNoise = false; //set the start weapon noise value to false to prevent calling it again
                    }
                }
                else
                {
                    _isShooting = false;
                }
            }

            else if (!_hasTarget) //Check if there is no target
            {
                //for loop to iterate through all muzzle flash objects
                for (int i = 0; i < _muzzleFlash.Length; i++)
                {
                    _muzzleFlash[i].SetActive(false); //enable muzzle effect particle effect
                }
                _audioSource.Stop(); //stop the sound effect from playing
                _startWeaponNoise = true; //set the start weapon noise value to true
            }
        }

        // Method to rotate gun barrel 
        void RotateBarrel() 
        {
            _gunBarrel[0].transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
            _gunBarrel[1].transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemies.Add(other.GetComponent<Enemy>());
                if (_enemies.Count > 0)
                {
                    _hasTarget = true;
                    AimTarget aimPoint = _enemies[0].GetComponentInChildren<AimTarget>();
                    _currentTarget = aimPoint.transform;
                }
            }

            if (other.CompareTag("EnemyWeapon"))
            {
                if (Health > 0)
                {
                    IAttack attack = other.GetComponentInParent<IAttack>();
                    Damage(attack.AtkDamage);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemies.Remove(other.GetComponent<Enemy>());
                if (_enemies.Count > 0)
                {
                    _hasTarget = true;
                    AimTarget aimPoint = _enemies[0].GetComponentInChildren<AimTarget>();
                    _currentTarget = aimPoint.transform;
                }
                else
                {
                    _hasTarget = false;
                    _currentTarget = null;
                }
            }
        }

        public void Damage(float damage)
        {
            Health -= damage;
            Debug.Log("Current Health: " + Health);
            if (Health < 1)
            {
                Debug.Log("Destroy this turret");
            }
        }

        public void Attack()
        {
            FireRay();
        }

        private void FireRay()
        {
            Debug.Log("Start Ray");
            RaycastHit hit;
            Debug.DrawRay(_firePoint.transform.position, _firePoint.transform.forward * _fireDistance, Color.red, 5f);
            if (Physics.Raycast(_firePoint.transform.position, _firePoint.transform.forward, out hit, _fireDistance))
            {
                if (hit.collider != null)
                {
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.Damage(AtkDamage);
                    }
                }
            }
        }

        private void OnEnemyDie(Enemy enemy)
        {
            _enemies.Remove(enemy);
            if (_enemies.Count > 0)
            {
                _hasTarget = true;
                AimTarget aimPoint = _enemies[0].GetComponentInChildren<AimTarget>();
                _currentTarget = aimPoint.transform;
            }
            else
            {
                _hasTarget = false;
                _currentTarget = null;
            }
        }
    }

}
