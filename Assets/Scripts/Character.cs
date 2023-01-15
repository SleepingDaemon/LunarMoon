using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health = 100f;
    [SerializeField] private Collider _col;
    public float Health { get => _health; set { _health = value; } }
    private BlazeAI _blazeAI = null;

    private void Awake()
    {
        _blazeAI = GetComponent<BlazeAI>();
    }

    public void OnDamage(float amount)
    {
        _blazeAI.Hit(gameObject, true);
        _health -= amount;

        if (_health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        _blazeAI.Death(true, _blazeAI.enemyToAttack);
        _col.enabled = false;
    }
}