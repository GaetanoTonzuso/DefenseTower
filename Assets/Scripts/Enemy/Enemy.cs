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

    private SkinnedMeshRenderer _skinnedMesh;
    private Material _mat;
        
    private void OnEnable()
    {
        EnableObject();
    }

    private void EnableObject()
    {
        Health = _health;
        _isDead = false;
        _hasDetectedTower = false;
        _attackRoutine = null;
        _skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _mat = _skinnedMesh.material;
        _mat.SetFloat("_DissolveAmount", 0f);
        GetComponent<CapsuleCollider>().enabled = true;

        if (_anim != null)
        {
            _anim.ResetTrigger("Death");
        }

        _targets = GameObject.FindGameObjectsWithTag("Target");
        _targets = _targets.OrderBy(go => go.transform.GetComponent<Target>().TargetId).ToArray();

        if (_agent != null)
        {
            _agent.enabled = true;

            if (NavMesh.SamplePosition((_targets[0].transform.position), out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
                _agent.speed = _speed;
                _agent.isStopped = false;
                _agent.SetDestination(_targets[_currentTargetId].transform.position);
            }
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
        if (_hasDetectedTower && _target == null)
        {
            StopAttack();
            _hasDetectedTower = false;

            // Riprende il movimento verso il percorso
            if (_agent != null && _currentTargetId < _targets.Length)
            {
                _agent.isStopped = false;
                _agent.stoppingDistance = 0f;
                _agent.SetDestination(_targets[_currentTargetId].transform.position);
            }

            return;
        }

        if (_hasDetectedTower && _target != null)
        {
            transform.LookAt(_target);
        }

        MoveAI();
    }

    private void MoveAI()
    {
        if (_hasDetectedTower || _agent == null || _target != null && _hasDetectedTower) return;

        if (_agent.remainingDistance < 0.5f && _agent != null)
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

    private void StopAttack()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
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
        _agent.isStopped = true;
        _agent.enabled = false;
        EventService.Instance.OnEnemyDie.InvokeEvent(this);
        StopAttack();
        _dissolve.StartDissolveRoutine();
        //Dissolve will disable at end of routine
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turret") && !_hasDetectedTower)
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
