using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    //No puedo guardar la unidad en si, porque guarda la direccion de memoria de esa unidad, tengo que guardar los valores que quiera de la unidad y 
    //con ello recreamos el prefab, debo guardar vida, granadas, municion, puntos de accion y posicion


    public int health;
    public int grenades;
    public int ammo;
    public TypeOfSoldier typeOfSoldier;
    public int actionPoint;
    public Vector3 position;
    public bool isWounded;

    public UnitData(int health, int grenades, int ammo, string typeOfSoldier, int actionPoint, Vector3 position, bool isWounded)
    {
        this.health = health;
        this.grenades = grenades;
        this.ammo = ammo;
        SetSoldier(typeOfSoldier);
        this.actionPoint = actionPoint;
        this.position = position;
        this.isWounded = isWounded;
    }

    public bool GetIsWounded()
    {
        return isWounded;
    }

    public void SetIsWounded(bool wou)
    {
        this.isWounded = wou;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetGrenades()
    {
        return grenades; 
    }

    public Unit Convert(UnitData unit)
    {
        return null;
    }

    public int GetAmmo()
    {
        return ammo; 
    }

    public TypeOfSoldier GetTypeOfSoldier()
    {
        return typeOfSoldier; 
    }
    public void SetSoldier(string aux)
    {
        switch (aux)
        {

            case "INF":
                this.typeOfSoldier = TypeOfSoldier.INF;
                break;
            case "MED":
                this.typeOfSoldier = TypeOfSoldier.MED;
                break;
            case "SUPP":
                this.typeOfSoldier = TypeOfSoldier.SUPP;
                break;
            case "AT":
                this.typeOfSoldier = TypeOfSoldier.AT;
                break;
            case "OF":
                this.typeOfSoldier = TypeOfSoldier.OF;
                break;
            case "REC":
                this.typeOfSoldier = TypeOfSoldier.REC;
                break;
        }
    }


    public int GetActionPoints()
    {
        return actionPoint;
    }

    public Vector3 GetPosition()
    {
        return position;
    }


    public void setHealth(int aux)
    {
        this.health = aux;
    }

    public void setGrenades(int aux)
    {
        grenades = aux; 
    }

    public void setAmmo(int aux)
    {
        this.ammo = aux; 
    }


    public void setActionPoints(int aux)
    {
        this.actionPoint = aux; 
    }

    public void setPosition(Vector3 aux)
    {
        this.position = aux; 
    }
}
