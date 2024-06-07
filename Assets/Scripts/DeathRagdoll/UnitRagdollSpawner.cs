using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;

    [SerializeField] private Transform originalRootBone;

    private HealthSystem healthSystem;

    public void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        //healthSystem.onDead += HealthSystem_OnDead;
    }

    public void Start()
    {
        
    }



    /*
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
       Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
        unitRagdoll.Setup(originalRootBone);
    }
    */
}
