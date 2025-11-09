using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Dissolve))]
public class Enemy : MonoBehaviour , IDamageable , IAttack
{
    public float Health { get; set; }
    [SerializeField] private float _health = 100;
    public int AtkDamage {  get; set; }
    private float _speed = 1.5f;
    

    private int _currentTargetId;
    private int _warfundsCredits = 150;
    public int WarfundsCredits { get { return _warfundsCredits; } }
    private bool _hasDetectedTower;
    private bool _isDead;

    [SerializeField] private Transform _target;
    private Coroutine _attackRoutine;
    private WaitForSeconds _attackRoutineSeconds = new WaitForSeconds(1.5f);
    private Animator _anim;
    private NavMeshAgent _agent;
    private Dissolve _dissolve;

    [SerializeField] private int _atkDamage = 20;
    [SerializeField] private GameObject[] _targets;


    private void OnEnable()
    {
        //Get the targets and sort by TargetID
        _targets = GameObject.FindGameObjectsWithTag("Target");
        _targets = _targets.OrderBy(go => go.transform.GetComponent<Target>().TargetId).ToArray();

        if (_agent != null)
        {
            _agent.speed = _speed;

            _agent.SetDestination(_targets[_currentTargetId].transform.position);
        }
    }

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        AtkDamage = _atkDamage;
        Health = _health;

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Anim is NULL on: " + gameObject.name);
        }

        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.Log("Agent is NULL on " + gameObject.name);
        }
        
        if(_agent != null)
        {
            _agent.speed = _speed;

            _agent.SetDestination(_targets[_currentTargetId].transform.position);
        }

        _dissolve = GetComponent<Dissolve>();
        if(_dissolve == null)
        {
            Debug.LogError("Dissolve is NULL");
        }
    }


    protected void Update()
    {
        if (_hasDetectedTower && _target != null)
        {
            transform.LookAt(_target);
        }

        MoveAI();
    }

    private void MoveAI()
    {
        if (_hasDetectedTower || _agent == null || _target != null) return;

        if(_agent.remainingDistance < 0.8f)
            {
                _currentTargetId++;
                if (_currentTargetId < _targets.Length)
                {
                    _agent.SetDestination(_targets[_currentTargetId].transform.position);
                }
                else
                {
                    _agent.isStopped = true;
                }
            }
    }

    public void Attack()
    {
        if (_attackRoutine == null)
        {
            _attackRoutine = StartCoroutine(AttackRoutine());
        }
    }

    public void Damage(float damage)
    {
        if (Health >= 1 && !_isDead)
        {
            Health -= damage;

        }

        else if(Health < 1 && !_isDead)
        {
            Death();
        }
    }

    private void Death()
    {
        _isDead = true;
        _anim.SetTrigger("Death");
        GetComponent<CapsuleCollider>().enabled = false;
        EventService.Instance.OnEnemyDie.InvokeEvent(this);
        _dissolve.StartDissolveRoutine();
        //Dissolve will disable at end of routine
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            _agent.stoppingDistance = 1.5f;
            _hasDetectedTower = true;
            _target = other.transform.parent;
            _agent.SetDestination(_target.position);
            Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Turret"))
        {
            _agent.stoppingDistance = 0f;
            _hasDetectedTower = false;
            _agent.SetDestination(_targets[_currentTargetId].transform.position);
        }
    }

    IEnumerator AttackRoutine()
    {
        
        while(Vector3.Distance(transform.position,_target.position) > _agent.stoppingDistance)
        {         
            yield return null;
        }

       _agent.isStopped = true;

        while (Vector3.Distance(transform.position, _target.position) <= _agent.stoppingDistance)
        {
            _anim.SetTrigger("Attack");
            yield return _attackRoutineSeconds;
        }

        _attackRoutine = null;
        yield return null;
    }

}
