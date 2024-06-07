using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PhotoIndividual : MonoBehaviour
{
    [Header("Avatar")]
    [SerializeField] private Image avatar;
    [Header("Balas")]
    [SerializeField] private TextMeshProUGUI bullets;
    [Header("Rol")]
    [SerializeField] private TextMeshProUGUI rol;
    [Header("Puntos de accion")]
    [SerializeField] private Image AP1;
    [SerializeField] private Image AP2;
    [SerializeField] private Image AP_Extra;
    [Header("Selected Unit")]
    [SerializeField] private Image selected;
    [Header("Icono granadas")]
    [SerializeField] private Image grenade1;
    [SerializeField] private Image grenade2;
    [Header("Vida")]
    [SerializeField] private Image vida;
    [Header("Herido")]
    [SerializeField] private GameObject wounded;

    public Unit unit;


    private void Start()
    {

        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //Recogemos la llamada del evento de cuando cambiamos de personaje
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        UnitActionSystem.Instance.onActionStarted += UnitActionSystem_onActionStarted;
        HealthSystem.onAnyUnitDamaged += HealthSystem_OnAnyUnitDamaged;
        HealthSystem.onAnyRevived += HealthSystem_OnAnyUnitRevived;
        UpdateAll();
    }

    private void HealthSystem_OnAnyUnitRevived(object sender, Unit e)
    {
       if(unit.GetHealthNormalized() > 0)
        {
            if(wounded != null)
            {
                wounded.SetActive(false);
            }
           
            UpdateAll();
        }
    }

    private void HealthSystem_OnAnyUnitDamaged(object sender, EventArgs e)
    {
        UpdateHealthBarPoints();
        if(unit.GetHealthNormalized() == 0)
        {
            ActivateWoundedAvatar();
        }
    }

    private void UnitActionSystem_onActionStarted(object sender, EventArgs e)
    {
        UpdateSelectedUnit();
        UpdateActionPoints();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }

    private void ActivateWoundedAvatar()
    {
        wounded.SetActive(true);
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
        UpdateHealthBarPoints();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }

    public void SetAvatar(Sprite avatar)
    {
        
        this.avatar.sprite = avatar;
    }
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateAll();
    }

    public void UpdateAll()
    {
        UpdateSelectedUnit();
        UpdateActionPoints();
        UpdateHealthBarPoints();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }

    public void UpdateOnLoad()
    {
        UpdateActionPoints();
        UpdateHealthBarPoints();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }



    public void avatarSelected()
    {
        UnitActionSystem.Instance.SetSelectedUnit(unit);
    }

    private void UpdateHealthBarPoints()
    {
        vida.fillAmount = unit.GetHealthNormalized();
    }

    private void UpdateActionPoints()
    {
      
        //Actualizar cuadrados
        int puntos = unit.GetActionPoints();
        if(AP1 != null && AP2 != null)
        {
            switch (puntos)
            {
                case 0:
                    AP1.color = Color.black;
                    AP2.color = Color.black;
                    break;
                case 1:
                    AP1.color = Color.green;
                    AP2.color = Color.black;
                    break;
                case 2:
                    AP1.color = Color.green;
                    AP2.color = Color.green;
                    break;
                case 3:
                    if(AP_Extra != null)
                        AP_Extra.enabled = true;
                    AP1.color = Color.green;
                    AP2.color = Color.green;
                    break;
            }
        }
     
    }

    private void UpdateRemainingBullets()
    {
        

        if (unit.GetAction<ShootAction>() != null) //Si dispara, comprobamos los iconos
        {
 
            switch (unit.GetAction<ShootAction>().GetRemainingBullets())
            {
                case 0:
                    bullets.text = "[0/4]";
                    break;
                case 1:
                    bullets.text = "[1/4]";
                    break;
                case 2:
                    bullets.text = "[2/4]";
                    break;
                case 3:
                    bullets.text = "[3/4]";
                    break;
                case 4:
                    bullets.text = "[4/4]";
                    break;

            }
        }
        else //Ahora mismo lo quitamos, pero realmente si no tiene disparar, será el cohete, y por tanto será el icono del cohete
        {
            if(unit.GetAction<RocketAction>() != null)
            {
                switch (unit.GetAction<RocketAction>().GetIsLoaded())
                {
                    case true:
                        bullets.text = "[1/1]";
                        break;
                    case false:
                        bullets.text = "[0/1]";
                        break;
                }
            }
           

        }
    }


    private void UpdateGrenadesVisual()
    {
        
    
        if (unit.GetAction<GrenadeAction>() != null) //Si tiene granada, le ponemos el texto
        {
            int grenadesLeft = unit.GetAction<GrenadeAction>().GetGrenadesLeft();
            switch (grenadesLeft)
            {
                case 0:
                    grenade1.color = Color.black;
                    grenade2.color = Color.black;
                    break;
                case 1:
                    grenade1.color = Color.white;
                    grenade2.color = Color.black;
                    break;
                case 2:
                    grenade1.color = Color.white;
                    grenade2.color = Color.white;
                    break;
            }
        }
        else
        {
            if(grenade1 != null && grenade2 != null)
            {
                grenade1.enabled = false;
                grenade2.enabled = false;
            }
        }
    }

    private void UpdateSelectedUnit()
    {
        if(unit != null)
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            selected.enabled = unit == selectedUnit;
        }
    
    }

    public void SetRol(string role)
    {
        rol.text = role;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

}
