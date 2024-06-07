using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private int healthMax = 100;
    private int woundedChance = 100;

    public event EventHandler onDead;
    public event EventHandler onDamaged;
    public event EventHandler onDamagedNotDead;
    public event EventHandler onHealed;
    public event EventHandler onWounded;
    public event EventHandler onRevived;
    public static event EventHandler<Unit> onAnyRevived;
    public static event EventHandler onAnyUnitDamaged;

    private void Awake()
    {
        health = healthMax;
    }

    public void SetHealth(int aux)
    {
        this.health = aux;
    }

    public void Damage(int damageAmount, Unit thisUnit)
    {
        health -= damageAmount;

        if(health < 0)
        {
            health = 0;
        }
        else
        {
            onDamagedNotDead?.Invoke(this, EventArgs.Empty);
        }

        onDamaged?.Invoke(this, EventArgs.Empty);
        onAnyUnitDamaged?.Invoke(this, EventArgs.Empty);
        if (health == 0)
        {
            
            if (!thisUnit.IsEnemy() && calculateChance(woundedChance))
                onWounded?.Invoke(this, EventArgs.Empty);
            else
                Die();
        }
    }

    public void UnitWoundedOnLoad()
    {
        
        onWounded?.Invoke(this, EventArgs.Empty);
       
    }


    private bool calculateChance(int number)
    {
        int chance = UnityEngine.Random.Range(0, 100);

        return chance < number;
    }


    public void Heal(int healAmount)
    {
        health += healAmount;
        //Habria que esperar un poco, o esperar que termine la animacion
        onHealed?.Invoke(this, EventArgs.Empty);
        if (health > healthMax)
        {

            health = healthMax;
        }
    }
    public void Revive(int healAmount, Unit unitRevived)
    {
        health += healAmount;
        onHealed?.Invoke(this, EventArgs.Empty);
        onRevived?.Invoke(this, EventArgs.Empty);
        onAnyRevived?.Invoke(this, unitRevived);
    }

    private void Die()
    {
        onDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float) health / healthMax;
    }

    public int GetUnitHealth()
    {
        return health;
    }

    public int GetMaxUnitHealth()
    {
        return healthMax;
    }
}
