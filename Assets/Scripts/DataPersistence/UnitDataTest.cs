using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataTest
{

    public int health;
    public int grenades;
    public int ammo;

    public UnitDataTest(int health, int grenades, int ammo, int actionPoint)
    {
        this.health = health;
        this.grenades = grenades;
        this.ammo = ammo;
        this.actionPoint = actionPoint;
    }

    private int actionPoint;
   

}
