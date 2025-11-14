using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missile_Launcher.Missile;


namespace GameDevHQ.FileBase.Missile_Launcher
{
    public class Missile_Launcher : MonoBehaviour,IDamageable , IWeapon
    {
        public enum MissileType
        {
            Normal,
            Homing
        }


        [SerializeField]
        private GameObject _missilePrefab; //holds the missle gameobject to clone
        [SerializeField]
        private MissileType _missileType; //type of missle to be launched
        [SerializeField]
        private GameObject[] _misslePositions; //array to hold the rocket positions on the turret
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
        [SerializeField]
        private Transform _target; //Who should the rocket fire at?

        [SerializeField] private GameObject _explosionPrefab;
        public GameObject upgradePanel;

        //Targets and Rotation settings
        [SerializeField] private GameObject _turret;
        [SerializeField] private float _rotationSpeed = 3f;
        [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
        [SerializeField] private Transform _currentTarget;

        Vector3 directionTarget;
        Quaternion targetDirection;
        private bool _hasTarget;
        private bool _isPlaced;

        [SerializeField] private float _fireRate = 0.5f;
        private float _nextFire = 0;

        //Turret Health
        [SerializeField] private float _health = 30f;
        public float Health { get; set; }

        private void OnEnable()
        {
            EventService.Instance.OnEnemyDie.AddListener(OnEnemyDie);
            EventService.Instance.OnActionPerformed.AddListener(ActiveUpgradePanel);
        }

        private void OnDisable()
        {
            EventService.Instance.OnEnemyDie.RemoveListener(OnEnemyDie);
            EventService.Instance.OnActionPerformed.RemoveListener(ActiveUpgradePanel);
        }

        private void Start()
        {
            Health = _health;
            upgradePanel = GameObject.Find("Canvas-UI").transform.Find("Upgrade_Missile").gameObject;
            StartCoroutine(Place());
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

        private void Attack()
        {
            if (_launched == false) // if we launched the rockets
            {
                _launched = true; //set the launch bool to true
                StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets. 
            }
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositions.Length; i++) //for loop to iterate through each missle position
            {
                GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

                rocket.transform.parent = _misslePositions[i].transform; //set the rockets parent to the missle launch position 
                rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocket.transform.parent = null; //set the rocket parent to null

                rocket.GetComponent<GameDevHQ.FileBase.Missile_Launcher.Missile.Missile>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

                _misslePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired
                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
                if (_currentTarget == null) yield return null;

            }

            for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
        }

      
        public void Damage(float damage)
        {
            Health -= damage;
            if (Health < 1)
            {
                Instantiate(_explosionPrefab,transform.position,Quaternion.identity);
                EventService.Instance.OnWeaponDestroyed.InvokeEvent();
                Destroy(this.gameObject, 0.3f);
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

        private IEnumerator Place()
        {
            yield return new WaitForSeconds(2);
            _isPlaced = true;
        }

        private void ActiveUpgradePanel()
        {
            Vector3 mousePos = PlayerController.Instance.playerInput.Player.Mouse.ReadValue<Vector2>();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7))
            {
                IWeapon weapon = hit.collider.GetComponent<IWeapon>();
                if (weapon != null && _isPlaced)
                {
                    upgradePanel.SetActive(true);
                    EventService.Instance.OnUpdateWeapon.InvokeEvent(this.transform, this.gameObject);
                }
            }
        }
    }
}

