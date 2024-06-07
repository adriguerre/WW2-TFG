using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSound : MonoBehaviour
{

    private AudioSource audioSource;
    private Unit unit;
    private HealthSystem healthSystem;


    [Header("Explosiones granada")]
    [SerializeField] private List<AudioClip> grenadeSounds;
    [Header("Explosiones cohete")]
    [SerializeField] private List<AudioClip> rocketSounds;
    [Header("Ametralladora")]
    [SerializeField] private List<AudioClip> lmgSounds;
    [Header("Rifle")]
    [SerializeField] private List<AudioClip> rifleSounds;

    [Header("SMG")]
    [SerializeField] private List<AudioClip> smgSounds;

    [Header("Reload")]
    [SerializeField] private AudioClip reloadGun;
    [Header("Cuchillo")]
    [SerializeField] private AudioClip knifeAction;
    [Header("Interactuar")]
    [SerializeField] private AudioClip interactAction;
    [Header("MedKit")]
    [SerializeField] private AudioClip medkit;
    [Header("MedKit")]
    [SerializeField] private AudioClip rocketLaunching;
    [Header("Movimiento")]
    [SerializeField] private AudioClip running;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        unit = GetComponent<Unit>();
        
    }

     private void Awake()
    {
        unit = GetComponent<Unit>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onDead += HealthSystem_OnDead;
        healthSystem.onWounded += HealthSystem_OnWounded;
        healthSystem.onRevived += HealthSystem_OnRevived;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnGrenadeExploded;
        RocketProjectile.onAnyRocketExploded += RocketProjectile_OnRocketExploded;

        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.onStartMoving += moveAction_onStartMoving;
            moveAction.onStopMoving += moveAction_onStopMoving; 
            //moveAction.onStartRoll += moveAction_OnStartRolling;
        }

       if(TryGetComponent<HealAction>(out HealAction healAction))
        {
            healAction.onStartHealing += healAction_onStartHealing;
        }

       if(TryGetComponent<GrenadeAction>(out GrenadeAction grenadeAction))
        {
            grenadeAction.onGrenadeLaunched += grenadeAction_onGrenadeLaunched;
        }

        if (TryGetComponent<RocketAction>(out RocketAction rocketAction))
        {
            rocketAction.onRocketLaunched += RocketAction_OnRocketLaunched;
        }
        if (TryGetComponent<SupressionAction>(out SupressionAction supressionAction))
        {
            supressionAction.onSupressionShoot += suppresionShoot_onSupressionShot;
        }

       if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.onShoot += shootAction_onShoot;
        }
        if (TryGetComponent<ReloadAction>(out ReloadAction reloadAction))
        {
            reloadAction.onStartReloading += reloadAction_OnStartReloading;
        }
        if (TryGetComponent<ReloadRocketAction>(out ReloadRocketAction reloadRocketAction))
        {
            reloadRocketAction.onStartReloading += reloadAction_OnStartReloading;
        }
        if (TryGetComponent<SniperShootAction>(out SniperShootAction sniperShoot))
        {
            sniperShoot.onSniperShoot += SniperShoot_OnSniperShoot;

        }
        if(TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_onSwordActionStarted;
        }
        if (TryGetComponent<ReviveAction>(out ReviveAction reviveAction))
        {
            reviveAction.onStartReviving += healAction_onStartReviving;
        }

    }





    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////HEALTH SYSTEM//////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void HealthSystem_OnRevived(object sender, EventArgs e)
    {
     
    }

    private void HealthSystem_OnWounded(object sender, EventArgs e)
    {
        //TODO: Sonido de herido
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //TODO: Sonido de muerto
  
    }

    private void healAction_onStartReviving(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = medkit;
            audioSource.Play();
        }
    }
    private void healAction_onStartHealing(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = medkit;
            audioSource.Play();
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////ACTION/////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    private int GetRandomNumber(float max)
    {
        return (int)UnityEngine.Random.Range(0, max);
    }


    private void SwordAction_onSwordActionStarted(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = knifeAction;
            audioSource.Play();
        }
    }

    private void SniperShoot_OnSniperShoot(object sender, SniperShootAction.OnShootEventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(rifleSounds[GetRandomNumber(1)]);
        }
    }


    private void reloadAction_OnStartReloading(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = reloadGun;
            audioSource.Play();
        }
    }



    private void shootAction_onShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        if (audioSource != null)
        {
            switch (unit.GetTypeOfSoldier())
            {
                case TypeOfSoldier.OF:
                    audioSource.PlayOneShot(smgSounds[GetRandomNumber(1)]);
                    break;
                case TypeOfSoldier.SUPP:
                    audioSource.PlayOneShot(lmgSounds[GetRandomNumber(1)]);
                    break;
                default:
                    audioSource.PlayOneShot(rifleSounds[GetRandomNumber(1)]);
                    break;
            }
        }
    }
 


    private void moveAction_onStartMoving(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = running;
            audioSource.Play();
        }
    }

    private void moveAction_onStopMoving(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }



    private void suppresionShoot_onSupressionShot(object sender, SupressionAction.OnShootEventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(lmgSounds[GetRandomNumber(1)]);
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////EXPLOSIONES////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   private void grenadeAction_onGrenadeLaunched(object sender, EventArgs e)
    {
         //TODO: Sonido de lanzamiento
    }
    private void GrenadeProjectile_OnGrenadeExploded(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = grenadeSounds[0];
            audioSource.Play();
        }
    }
 
    private void RocketAction_OnRocketLaunched(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.clip = rocketLaunching;
            audioSource.Play();
        }
    }
    private void RocketProjectile_OnRocketExploded(object sender, EventArgs e)
    {
        if(audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(rocketSounds[GetRandomNumber(1)]);

        }
       
    }



}
