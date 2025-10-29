using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour , IDamageable
{
    public float Health { get; set; }
    private float _speed = 1.5f;
    private int _warfundsCredits = 150;

    private Animator _anim;
    private NavMeshAgent _agent;
    private int _currentTarget;
    [SerializeField] private Transform[] _targets;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
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

            _agent.SetDestination(_targets[_currentTarget].position);
        }
    }


    protected void Update()
    {
        MoveAI();
    }

    private void MoveAI()
    {
        if (_agent.remainingDistance < 0.5f && _agent != null)
        {
            _currentTarget++;
            if (_currentTarget < _targets.Length)
            {
                _agent.SetDestination(_targets[_currentTarget].position);
            }
            else
            {
                _agent.isStopped = true;
            }
        }
    }

    public void Damage(float damage)
    {

    }
}
