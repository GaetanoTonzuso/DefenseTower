using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle;

namespace GameDevHQ.FileBase.Missle_Launcher_Dual_Turret
{
    public class Missle_Launcher : MonoBehaviour, IDamageable , IWeapon
    {
        [SerializeField]
        private GameObject _missilePrefab; //holds the missle gameobject to clone
        [SerializeField]
        private GameObject[] _misslePositionsLeft; //array to hold the rocket positions on the turret
        [SerializeField]
        private GameObject[] _misslePositionsRight; //array to hold the rocket positions on the turret
        [SerializeField]
        private float _fireDelay; //fire delay between rockets
        [SerializeField]
        private float _launchSpeed; //initial launch speed of the rocket
        [SerializeField]
        private float _power; //power to apply to the force of the rocket
        [SerializeField]
        private float _fuseDelay; //fuse delay before the rocket launches
        [SerializeField]
        private float _reloadTime; //time in between reloading the rockets
        [SerializeField]
        private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        private bool _launched; //bool to check if we launched the rockets

        [SerializeField] private float _fireRate = 0.5f;
        private float _nextFire = 0;

        [SerializeField] private GameObject _explosionPrefab;

        //Targets and Rotation settings
        [SerializeField] private GameObject _turret;
        [SerializeField] private float _rotationSpeed = 3f;
        [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
        [SerializeField] private Transform _currentTarget;

        Vector3 directionTarget;
        Quaternion targetDirection;
        private bool _hasTarget;

        [SerializeField] private float _health = 80f;
        public float Health { get; set; }

        private void OnEnable()
        {
            EventService.Instance.OnEnemyDie.AddListener(OnEnemyDie);
        }

        private void OnDisable()
        {
            EventService.Instance.OnEnemyDie.RemoveListener(OnEnemyDie);
        }

        private void Start()
        {
            Health = _health;
        }

        private void Update()
        {
            AimTarget();
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
                    if (Time.time > _nextFire)
                    {
                        _nextFire = _fireRate + Time.time;
                        Attack();
                    }
                }
            }
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositionsLeft.Length; i++) //for loop to iterate through each missle position
            {
                GameObject rocketLeft = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket
                GameObject rocketRight = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

                rocketLeft.transform.parent = _misslePositionsLeft[i].transform; //set the rockets parent to the missle launch position 
                rocketRight.transform.parent = _misslePositionsRight[i].transform; //set the rockets parent to the missle launch position 

                rocketLeft.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocketRight.transform.localPosition = Vector3.zero; //set the rocket position values to zero

                rocketLeft.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocketRight.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction

                rocketLeft.transform.parent = null; //set the rocket parent to null
                rocketRight.transform.parent = null; //set the rocket parent to null

                rocketLeft.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missle>().AssignMissleRules(_launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 
                rocketRight.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missle>().AssignMissleRules(_launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

                _misslePositionsLeft[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired
                _misslePositionsRight[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            for (int i = 0; i < _misslePositionsLeft.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositionsLeft[i].SetActive(true); //enable fake rocket to show ready to fire
                _misslePositionsRight[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
        }

        public void Damage(float damage)
        {
            Health -= damage;
            Debug.Log("Current Health: " + Health);
            if (Health < 1)
            {
                Instantiate(_explosionPrefab,transform.position,Quaternion.identity);
                EventService.Instance.OnWeaponDestroyed.InvokeEvent();
                Destroy(this.gameObject);
            }
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

        private void Attack()
        {
            if (_launched == false) // if we launched the rockets
            {
                _launched = true; //set the launch bool to true
                StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets. 
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
                StopCoroutine(FireRocketsRoutine());
            }
            else
            {
                _hasTarget = false;
                _currentTarget = null;
            }
        }
    }
}

