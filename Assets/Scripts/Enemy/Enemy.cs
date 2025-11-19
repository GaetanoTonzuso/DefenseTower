using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Dissolve))]
public class Enemy : MonoBehaviour, IDamageable, IAttack
{
    public float Health { get; set; }
    [SerializeField] private float _health = 100;
    public int AtkDamage { get; set; }
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
    [SerializeField] private float _attackDistance = 1.25f;
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
        _target = null;
        _attackRoutine = null;
        _currentTargetId = 0;

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

            if (NavMesh.SamplePosition(_targets[0].transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _agent.stoppingDistance = 0.5f;
                _agent.Warp(hit.position);
                _agent.speed = _speed;
                _agent.isStopped = false;

                if (CanMove())
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
            Debug.LogError("Anim is NULL on: " + gameObject.name);

        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
            Debug.Log("Agent is NULL on " + gameObject.name);

        _agent.stoppingDistance = 1f;

        if (_agent != null)
        {
            _agent.speed = _speed;

            if (CanMove())
                _agent.SetDestination(_targets[_currentTargetId].transform.position);
        }

        _dissolve = GetComponent<Dissolve>();
        if (_dissolve == null)
            Debug.LogError("Dissolve is NULL");
    }

    private bool CanMove()
    {
        return _agent != null && _agent.enabled && _agent.isOnNavMesh;
    }

    protected void Update()
    {
        if (!CanMove()) return;

        if (_hasDetectedTower && (_target == null || _target.gameObject == null))
        {
            StopAttack();
            _hasDetectedTower = false;

            if (_currentTargetId < _targets.Length && CanMove())
            {
                _target = null;
                _agent.isStopped = false;
                _agent.stoppingDistance = 1f;
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
        if (_hasDetectedTower || _agent == null || (_target != null && _hasDetectedTower)) return;
        if (!CanMove()) return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_agent.velocity.sqrMagnitude < 0.1f)
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
        else if (Health < 1 && !_isDead)
        {
            Death();
        }
    }

    private void Death()
    {
        _isDead = true;
        _anim.SetTrigger("Death");

        GetComponent<CapsuleCollider>().enabled = false;

        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }

        EventService.Instance.OnEnemyDie.InvokeEvent(this);
        StopAttack();
        _dissolve.StartDissolveRoutine();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turret") && !_hasDetectedTower && CanMove())
        {
            _agent.stoppingDistance = _attackDistance;
            _hasDetectedTower = true;
            _target = other.transform;
            _agent.SetDestination(_target.position);
            Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            _agent.stoppingDistance = 1f;
            _hasDetectedTower = false;

            if (CanMove() && _currentTargetId < _targets.Length)
            {
                _agent.SetDestination(_targets[_currentTargetId].transform.position);
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        while (_hasDetectedTower && _target != null)
        {
            float dist = Vector3.Distance(transform.position, _target.position);

            if (dist > _agent.stoppingDistance)
            {
                if (!_agent.isStopped)
                    _agent.SetDestination(_target.position);

                yield return null;
                continue;
            }

            _agent.isStopped = true;

            _anim.SetTrigger("Attack");
            yield return _attackRoutineSeconds;
        }
        _agent.isStopped = false;
        _attackRoutine = null;
    }
}
