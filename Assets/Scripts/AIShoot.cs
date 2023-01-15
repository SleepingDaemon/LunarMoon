using BlazeAISpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
    [HideInInspector] public LineRenderer lr;
    [HideInInspector] public BlazeAI blaze;
    [HideInInspector] public BlazeAISpace.CoverShooterBehaviour coverShooter;

    public Transform gun;
    public Material shootMaterial;
    public AudioSource gunShot;
    public AudioClip _gunShotClip;
    public int rayDistance;
    public LayerMask enemyLayers;
    public float damageAmount;
    public int _maxRandomNumber;

    [System.Obsolete]
    void Start()
    {
        blaze = GetComponent<BlazeAI>();
        coverShooter = GetComponent<CoverShooterBehaviour>();
        lr = GetComponent<LineRenderer>();

        lr.enabled = false;
        lr.SetWidth(0.05f, 0.03f);
    }

    public void ShootTarget()
    {
        lr.material = shootMaterial;
        gunShot.PlayOneShot(_gunShotClip);

        var enemy = blaze.enemyToAttack.gameObject;

        var enemyAI = enemy.GetComponent<BlazeAI>();
        if(enemyAI.enemyToAttack == null)
            enemyAI.SetTarget(this.gameObject, false, true);
        //if (enemyAI.enemyToAttack != null)
        //{
        //    enemyAI.Attack();
        //}

        var randomNumber = Random.Range(0, _maxRandomNumber);
        var randomNumberTWO = Random.Range(0, _maxRandomNumber);
        if(randomNumber == randomNumberTWO)
        {
            Debug.Log("Shot Hit: " + enemy.gameObject.name + " -  From: " + gameObject.name);

            var enemyHealth = enemy.GetComponent<Character>();
            enemyHealth.OnDamage(damageAmount);
        }
        else
        {
            Debug.Log(gameObject.name + ": Missed");
        }

        lr.enabled = true;
        lr.SetPosition(0, gun.position + new Vector3(0f, 0.2f, 0f));
        lr.SetPosition(1, coverShooter.hitPoint + new Vector3(0f, 1.2f, 0f));

        StartCoroutine(TurnRendererOff());
    }

    IEnumerator TurnRendererOff()
    {
        yield return new WaitForSeconds(0.2f);

        lr.enabled = false;
    }
}
