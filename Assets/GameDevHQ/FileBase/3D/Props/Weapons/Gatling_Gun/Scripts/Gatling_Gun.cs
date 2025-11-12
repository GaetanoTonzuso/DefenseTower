using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace GameDevHQ.FileBase.Gatling_Gun
{
    /// <summary>
    /// This script will allow you to view the presentation of the Turret and use it within your project.
    /// Please feel free to extend this script however you'd like. To access this script from another script
    /// (Script Communication using GetComponent) -- You must include the namespace (using statements) at the top. 
    /// "using GameDevHQ.FileBase.Gatling_Gun" without the quotes. 
    /// 
    /// For more, visit GameDevHQ.com
    /// 
    /// @authors
    /// Al Heck
    /// Jonathan Weinberger
    /// </summary>

    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Gatling_Gun : MonoBehaviour , IDamageable , IAttack , IWeapon
    {
        [SerializeField] private GameObject _firePoint; //Reference to hold Shooting Point
        [SerializeField] private float _fireDistance = 30f;
        [SerializeField] private float _fireRate = 0.5f;
        [SerializeField] private GameObject _explosionPrefab;
        private float _nextFire = 0;

        private Transform _gunBarrel; //Reference to hold the gun barrel
        public GameObject Muzzle_Flash; //reference to the muzzle flash effect to play when firing
        public ParticleSystem bulletCasings; //reference to the bullet casing effect to play when firing
        public AudioClip fireSound; //Reference to the audio clip

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
        RaycastHit hit;

        //Turret Health
        [SerializeField] private float _health = 30f;
        public float Health { get; set;}

        [SerializeField] private int _atkDamage = 2;
        public int AtkDamage { get; set;}

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
            _gunBarrel = GameObject.Find("Barrel_to_Spin").GetComponent<Transform>(); //assigning the transform of the gun barrel to the variable
            Muzzle_Flash.SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource = GetComponent<AudioSource>(); //ssign the Audio Source to the reference variable
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = fireSound; //assign the clip to play
        }

        // Update is called once per frame
        void Update()
        {         
            AimTarget();

            if (!_isShooting) return;

            if (Time.time > _nextFire)
            {
                _nextFire = _fireRate + Time.time;
                Attack();
            }
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
                if(angleDifference < 1.5f)
                {
                    _isShooting = true;
                    RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel
                    Muzzle_Flash.SetActive(true); //enable muzzle effect particle effect
                    bulletCasings.Emit(1); //Emit the bullet casing particle effect  

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
                Muzzle_Flash.SetActive(false); //turn off muzzle flash particle effect
                _audioSource.Stop(); //stop the sound effect from playing
                _startWeaponNoise = true; //set the start weapon noise value to true
                _isShooting = false;
            }
        }

        // Method to rotate gun barrel 
        void RotateBarrel() 
        {
            if(_gunBarrel != null)
            _gunBarrel.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy"))
            {
                _enemies.Add(other.GetComponent<Enemy>());
                if(_enemies.Count > 0)
                {
                    _hasTarget = true;
                    AimTarget aimPoint = _enemies[0].GetComponentInChildren<AimTarget>();
                    _currentTarget = aimPoint.transform;
                }
            }

            if( other.CompareTag("EnemyWeapon"))
            {
                if(Health > 0)
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
            if(Health < 1)
            {
                Debug.Log("Destroy this turret");
                Instantiate(_explosionPrefab,transform.position, Quaternion.identity);
                //Send Message to PlaceZone and make it placeable
                EventService.Instance.OnWeaponDestroyed.InvokeEvent();
                Destroy(this.gameObject, 0.3f);
            }
        }

        public void Attack()
        {
            FireRay();
        }

        private void FireRay()
        {        
            if(Physics.Raycast(_firePoint.transform.position,_firePoint.transform.forward,out hit,_fireDistance, 1<<6))
            {
                if(hit.collider != null)
                {
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if(damageable != null)
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
