using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform medikitTransform;
    [SerializeField] private Transform missShootPrefab;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransfrom;


    private Unit unit;
    private HealthSystem healthSystem;
    private MoveAction moveAction;


    private void Awake()
    {
        unit = GetComponent<Unit>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onDead += HealthSystem_OnDead;
        healthSystem.onWounded += HealthSystem_OnWounded;
        healthSystem.onDamagedNotDead += HealthSystem_OnDamagedNotDead;
        healthSystem.onRevived += HealthSystem_OnRevived;
        unit.onCoverChanged += Unit_OnCoverChanged;
        Unit.onAnyUnitAlerted += Unit_OnAnyUnitAlerted;
        unit.onActiveUnit += Unit_OnActiveUnit;
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            this.moveAction = moveAction;
            moveAction.onStartMoving += moveAction_onStartMoving;
            moveAction.onStopMoving += moveAction_onStopMoving;
            //moveAction.onStartRoll += moveAction_OnStartRolling;
        }

       if(TryGetComponent<HealAction>(out HealAction healAction))
        {
            healAction.onStartHealing += healAction_onStartHealing;
            healAction.onStopHealing += healAction_onStopHealing;
        }

       if(TryGetComponent<GrenadeAction>(out GrenadeAction grenadeAction))
        {
            grenadeAction.onGrenadeLaunched += grenadeAction_onGrenadeLaunched;
        }
        if (TryGetComponent<ExplosiveAction>(out ExplosiveAction explosiveAction))
        {
            explosiveAction.onStartPlacing += explosiveAction_OnStartPlacing;
            explosiveAction.onStopPlacing += explosiveAction_OnStopPlacing;
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
            shootAction.onAnyMissShoot += ShootAction_OnAnyMissShoot;
        }
        if (TryGetComponent<ReloadAction>(out ReloadAction reloadAction))
        {
            reloadAction.onStartReloading += reloadAction_OnStartReloading;
            reloadAction.onStopReloading += reloadAction_OnStopReloading;
        }
        if (TryGetComponent<ReloadRocketAction>(out ReloadRocketAction reloadRocketAction))
        {
            reloadRocketAction.onStartReloading += reloadAction_OnStartReloading;
            reloadRocketAction.onStopReloading += reloadAction_OnStopReloading;
        }
        if (TryGetComponent<SniperShootAction>(out SniperShootAction sniperShoot))
        {
            sniperShoot.onSniperShoot += SniperShoot_OnSniperShoot;
            sniperShoot.onAnyMissShoot += SniperShoot_OnAnyMissShoot;
        }
        if(TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_onSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_onSwordActionCompleted;
        }
        if (TryGetComponent<ReviveAction>(out ReviveAction reviveAction))
        {
            reviveAction.onStartReviving += healAction_onStartReviving;
            reviveAction.onStopReviving += healAction_onStopReviving;
        }
        if (TryGetComponent<MoveBonusAction>(out MoveBonusAction moveBonusAction))
        {
            moveBonusAction.onStartBonus += MoveBonusAction_OnStartBonus;
            moveBonusAction.onStopBonus += MoveBonusAction_onStopBonus;
        }

    }

    private void reloadAction_OnStopReloading(object sender, EventArgs e)
    {
        animator.SetBool("Reloading", false);
    }

    private void reloadAction_OnStartReloading(object sender, EventArgs e)
    {
        animator.SetBool("Reloading", true);
    }

    private void Unit_OnActiveUnit(object sender, EventArgs e) //Con esto pasamos las unidades aliadas directamente al estado de alarma
    {
        Unit unit = sender as Unit;

        if (!unit.IsEnemy())
        {
            animator.SetTrigger("Alarmed");
        }
    }

    private void Unit_OnAnyUnitAlerted(object sender, EventArgs e)
    {
        if (unit.IsEnemy())
        {
            if(animator != null)
                animator.SetTrigger("Alarmed");
        }
    }

    private void explosiveAction_OnStartPlacing(object sender, EventArgs e)
    {
        animator.SetBool("PlacingBomb", true);
    }

    private void explosiveAction_OnStopPlacing(object sender, EventArgs e)
    {
        animator.SetBool("PlacingBomb", false);
    }

    private void Start()
    {
        EquipRifle();
    }

    private void MoveBonusAction_onStopBonus(object sender, EventArgs e)
    {
        animator.SetBool("Bonus", false);
    }

    private void MoveBonusAction_OnStartBonus(object sender, EventArgs e)
    {
        animator.SetBool("Bonus", true);
    }


    private void healAction_onStopReviving(object sender, EventArgs e)
    {
        animator.SetBool("Reviving", false);
    }

    private void healAction_onStartReviving(object sender, EventArgs e)
    {
        animator.SetBool("Reviving", true);
    }


    private void CreateProjectiles(Vector3 position, Unit targetUnit)
    {
        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
        Vector3 targerUnitShootAtPosition = targetUnit.GetWorldPosition();
        targerUnitShootAtPosition.y = shootPointTransform.position.y;
        bulletProjectile.Setup(targerUnitShootAtPosition);
    }
    private void InstantiateMissShoot(Unit unit)
    {
        //Instanciamos el prefab de missShot y lo destruimos pasado un segundo
        Transform missShootTransform = Instantiate(missShootPrefab, unit.GetWorldPosition() + Vector3.up * 2f + Vector3.left * 2f, Quaternion.identity); ;
        MissShoot missShoot = missShootTransform.GetComponent<MissShoot>();
        missShoot.Setup(unit.GetGridPosition());
    }
    private void EquipKnife()
    {
        swordTransfrom.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransfrom.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }

    private void EquipMedikit()
    {
        rifleTransform.gameObject.SetActive(false);
        medikitTransform.gameObject.SetActive(true);
    }

    private void HideMedikit()
    {
        rifleTransform.gameObject.SetActive(true);
        medikitTransform.gameObject.SetActive(false);
    }
    private void Unit_OnCoverChanged(object sender, EventArgs e)
    {
        switch (unit.GetCoverType())
        {
            case CoverType.None:
                animator.SetBool("Crouched", false);
                break;
            case CoverType.Half:
                animator.SetBool("Crouched", true);
                break;
            case CoverType.Full:
                animator.SetBool("Crouched", true);
                break;
        }
    }
    private void HealthSystem_OnWounded(object sender, EventArgs e)
    {
        DeathAnimation(true);    
    }

    private void HealthSystem_OnDamagedNotDead(object sender, EventArgs e)
    {
        animator.SetTrigger("DamageReceived");
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        DeathAnimation(false);
    }
    private void HealthSystem_OnRevived(object sender, EventArgs e)
    {
        animator.SetTrigger("Revived");
    }
    private void DeathAnimation(bool Wounded)
    {
        animator.SetBool("Crouched", false);
        if (Wounded)
            animator.SetInteger("DieIndex", UnityEngine.Random.Range(1, 3));
        else
            animator.SetInteger("DieIndex", UnityEngine.Random.Range(0, 5));

        animator.SetTrigger("Die");

      
    }

    private void SniperShoot_OnAnyMissShoot(object sender, Unit e)
    {
        InstantiateMissShoot(e);
    }

    private void SniperShoot_OnSniperShoot(object sender, SniperShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");
        CreateProjectiles(shootPointTransform.position, e.targetUnit);
    }

    private void grenadeAction_onGrenadeLaunched(object sender, EventArgs e)
    {
        animator.SetTrigger("ThrowGrenade");
    }
    private void suppresionShoot_onSupressionShot(object sender, SupressionAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");
        CreateProjectiles(shootPointTransform.position, e.targetUnit);
        CreateProjectiles(shootPointTransform.position + Vector3.left * 0.4f, e.targetUnit);
        //Y habría que disparar mas balas
    }

 
    private void RocketAction_OnRocketLaunched(object sender, EventArgs e)
    {
        animator.SetTrigger("Shoot");
    }

    private void ShootAction_OnAnyMissShoot(object sender, Unit e)
    {
        InstantiateMissShoot(e);
    }
    private void shootAction_onShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        

        animator.SetTrigger("Shoot");
        CreateProjectiles(shootPointTransform.position, e.targetUnit);
    }


    /*
        private void moveAction_OnStartRolling(object sender, EventArgs e)
        {

            if (UnityEngine.Random.Range(1, 100) >= 90)
            {
                    animator.SetTrigger("Rolling");
            }
        }

        */

    private void healAction_onStopHealing(object sender, EventArgs e)
    {
      
        animator.SetBool("Healing", false);
        HideMedikit();
    }
    private void healAction_onStartHealing(object sender, EventArgs e)
    {
        EquipMedikit();
        animator.SetBool("Healing", true);
        
    }

    private void SwordAction_onSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void SwordAction_onSwordActionStarted(object sender, EventArgs e)
    {
        EquipKnife();
        animator.SetTrigger("SwordSlash");
    }
    private void moveAction_onStartMoving(object sender, EventArgs e)
    {
        switch (unit.GetCoverType())
        {
            case CoverType.None:
                if(animator != null)
                    animator.SetBool("IsWalking", true);
                break;
            case CoverType.Half:
                if (animator != null)
                {
                    float crouchedMoveSpeed = 3f;
                    moveAction.SetMoveSpeed(crouchedMoveSpeed);
                    animator.SetBool("IsWalkingCrouched", true);
                }
                break;
            case CoverType.Full:
                if (animator != null)
                {
                    float crouchedMoveSpeedFull = 2f;
                    moveAction.SetMoveSpeed(crouchedMoveSpeedFull);
                    animator.SetBool("IsWalkingCrouched", true);
                }
                break;
        }
    }
    private void moveAction_onStopMoving(object sender, EventArgs e)
    {
        float normalMoveSpeed = moveAction.GetNormalMoveSpeed();
        moveAction.SetMoveSpeed(normalMoveSpeed);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsWalkingCrouched", false);   
        animator.SetBool("Rolling", false);
    }


 
}
