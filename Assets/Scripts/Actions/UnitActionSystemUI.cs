using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class UnitActionSystemUI : MonoBehaviour
{

    public static UnitActionSystemUI Instance { get; private set; }


    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private TextMeshProUGUI grenadesLeftText;
    [SerializeField] private Material bulletAvailable;
    [SerializeField] private Material bulletNotAvailable;
    [Header("Balas")]
    [SerializeField] private Image bullet1;
    [SerializeField] private Image bullet2;
    [SerializeField] private Image bullet3;
    [SerializeField] private Image bullet4;
    [Header("Puntos de accion")]
    [SerializeField] private Image AP1;
    [SerializeField] private Image AP2;  
    [SerializeField] private Image AP_Extra;
    [Header("Balas cohete")]
    [SerializeField] private Image rocketAmmo;
    [Header("Icono granadas")]
    [SerializeField] private Image grenade1;
    [SerializeField] private Image grenade2;

    private List<ActionButtonUI> actionButtonList;



    private void Awake()
    {
        actionButtonList = new List<ActionButtonUI>();

    }

    public void UpdateActionButtonsOnLoad()
    {
        UpdateActionPoints();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }
    private void DataPersistence_OnLoad(object sender, EventArgs e)
    {
       // UpdateActionButtonsOnLoad();
    }


    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //Recogemos la llamada del evento de cuando cambiamos de personaje
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; //Recogemos la llamada del evento de cuando cambiamos de personaje
        UnitActionSystem.Instance.onActionStarted += UnitActionSystem_onActionStarted; //Recogemos la llamada del evento de cuando cambiamos de personaje


        DataPersistenceManager.Instance.onLoad += DataPersistence_OnLoad;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        UpdateActionPoints();
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void CreateUnitActionButtons()
    {
        foreach(Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonList.Clear();


       Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            //Instanciamos cada prefab con una accion
            Transform actionButtonTransorm =  Instantiate(actionButtonPrefab, actionButtonContainerTransform); //Cogemos uno de los baseAction ( es decir una accion normal)
            ActionButtonUI actionButtonUI = actionButtonTransorm.GetComponent<ActionButtonUI>(); //Cogemos el componente hijo
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonList.Add(actionButtonUI);
        }
        
    }


    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) //Y aqui es lo que hariamos cuando cambia el personaje
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
        UpdateGrenadesVisual();
        UpdateRemainingBullets();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) //Creamos el evento, que se llama cada vez que cambiamos de action
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_onActionStarted(object sender, EventArgs e)
    {
        UpdateGrenadesVisual();
        UpdateActionPoints();
        UpdateRemainingBullets();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e) {
        UpdateActionPoints();
        UpdateRemainingBullets();
    }


    public void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }

    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        //Actualizar cuadrados
        int puntos = selectedUnit.GetActionPoints();
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
                    if (AP2 != null)
                        AP2.color = Color.black;
                    break;
                case 2:
                    AP1.color = Color.green;
                    AP2.color = Color.green;
                    break;
                case 3:
                    AP_Extra.enabled = true;
                    AP1.color = Color.green;
                    AP2.color = Color.green;
                    break;
            }
        }
      
    }

    private void UpdateGrenadesVisual()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if(selectedUnit.GetAction<GrenadeAction>() != null) //Si tiene granada, le ponemos el texto
        {
            int grenadesLeft = selectedUnit.GetAction<GrenadeAction>().GetGrenadesLeft();
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
            grenadesLeftText.enabled = false;
        }
    }

    private void UpdateRemainingBullets()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit.GetAction<ShootAction>() != null) //Si dispara, comprobamos los iconos
        {
            if(rocketAmmo != null && bullet1 != null && bullet2 != null && bullet3 != null && bullet4 != null)
            {
                rocketAmmo.enabled = false;
                bullet1.enabled = true;
                bullet2.enabled = true;
                bullet3.enabled = true;
                bullet4.enabled = true;
            }
            
            if (selectedUnit.GetAction<ShootAction>() != null)
            {
                if (bullet1 != null && bullet2 != null && bullet3 != null && bullet4 != null)
                {
                    switch (selectedUnit.GetAction<ShootAction>().GetRemainingBullets())
                    {
                        case 0:
                            bullet4.material = bulletNotAvailable;
                            break;
                        case 1:
                            bullet3.material = bulletNotAvailable;
                            break;
                        case 2:
                            bullet2.material = bulletNotAvailable;
                            break;
                        case 3:
                            bullet1.material = bulletNotAvailable;
                            break;
                        case 4:

                            bullet1.material = bulletAvailable;
                            bullet2.material = bulletAvailable;
                            bullet3.material = bulletAvailable;
                            bullet4.material = bulletAvailable;
                            break;

                    }
                }
            }
           
        }
        else //Ahora mismo lo quitamos, pero realmente si no tiene disparar, será el cohete, y por tanto será el icono del cohete
        {
            if (bullet1 != null && bullet2 != null && bullet3 != null && bullet4 != null)
            {

                bullet1.enabled = false;
                bullet2.enabled = false;
                bullet3.enabled = false;
                bullet4.enabled = false;
            }
            if(selectedUnit.GetAction<RocketAction>() != null && rocketAmmo != null)
            {
                switch (selectedUnit.GetAction<RocketAction>().GetIsLoaded())
                {
                    case true:
                        rocketAmmo.enabled = true;
                        break;
                    case false:
                        rocketAmmo.enabled = false;
                        break;
                }
            }
           

        }
    }
}
