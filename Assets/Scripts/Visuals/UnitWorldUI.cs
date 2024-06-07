using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class UnitWorldUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Image coverType;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Material fullCover; 
    [SerializeField] private Material halfCover;
    [SerializeField] private TextMeshProUGUI hitPercentChance;
    [SerializeField] private Image unitDead;
    [SerializeField] private Image suppresed;
    [SerializeField] private Image wounded;
    private MeshRenderer meshRenderer;
    private Image[] imageList;
    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        imageList = unit.GetComponentsInChildren<Image>();
        unitDead.enabled = false;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        unit.onUnitDestroyed += Unit_OnUnitDestroyed;
        SupressionAction.onAnySupressionShoot += SupressionAction_onAnySupressionShoot;
        unit.onSuppresed += Unit_OnSuppresed;
        unit.onCoverChanged += Unit_OnCoverChanged;
        unit.onEndedSuppression += Unit_OnEndedSuppression;
        unit.onGetHitPercent += Unit_OnGetHitPercent;
        healthSystem.onDamaged += HealthSystem_OnDamaged;
        healthSystem.onHealed += HealthSystem_OnHealed;
        healthSystem.onWounded += HealthSystem_OnWounded;
    
        healthSystem.onRevived += HealthSystem_OnRevived;
        UnitActionSystem.onAnySelectedChangedSoHidePercent += UnitSelectedVisual_onAnySelectedChangedSoHidePercent;

        UpdateActionPointsText();
        UpdateHealthBar();
        if (unit.IsEnemy())
            HideHitPercent();
    }

    private void Unit_OnUnitWoundedLoad(object sender, EventArgs e)
    {
        Debug.Log("ESTAMOS DENTRO");
    }

    private void Unit_OnUnitDestroyed(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        TextMeshProUGUI actionPointsTextUnit = unit.GetComponentInChildren<TextMeshProUGUI>();

        foreach (Image image in imageList)
        {
            image.enabled = false;
        }
        actionPointsTextUnit.enabled = false;
        if (!unit.IsEnemy())
            imageList[3].enabled = true;
    }

    private void Unit_OnGetHitPercent(object sender, Unit.OnFlankShoot e)
    {
        ShowHitPercent();
        if (e.flanked)
        {
            imageList[5].enabled = true;
            hitPercentChance.text = e.chance.ToString("0.##") + "%";
            hitPercentChance.color = Color.yellow;
        }
        else
            hitPercentChance.text = e.chance.ToString("0.##") + "%";
       


    }

    private void UnitSelectedVisual_onAnySelectedChangedSoHidePercent(object sender, EventArgs e)
    {
        if (unit.IsEnemy())
            HideHitPercent();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void UpdateCoverImage()
    {
        switch (unit.GetCoverType())
        {
            case CoverType.None:

                coverType.enabled = false;
                break;
            case CoverType.Half:

                coverType.enabled = true;
                coverType.material = halfCover;
                break;
            case CoverType.Full:
                coverType.enabled = true;
                coverType.material = fullCover;
                break;
        }
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void HideHitPercent()
    {
        if (imageList[5] != null)
            imageList[5].enabled = false;
        if (hitPercentChance != null)
        { 
            hitPercentChance.enabled = false;
            hitPercentChance.color = Color.red;
        }
            
    }

    private void ShowHitPercent()
    {
        hitPercentChance.enabled = true; 
    }

    private void Unit_OnEndedSuppression(object sender, EventArgs e)
    {
        imageList[4].enabled = false;
    }
    private void Unit_OnSuppresed(object sender, EventArgs e)
    {
        imageList[4].enabled = true;
    }
    private void Unit_OnCoverChanged(object sender, EventArgs e)
    {
        UpdateCoverImage();
    }
  

  


    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
    private void SupressionAction_onAnySupressionShoot(object sender, SupressionAction.OnShootEventArgs e)
    {
        //Recibimos el evento, y lo derivamos a la unit, para ya cambiar el estado, y desde la unidad, volvemos al UI, donde cambiamos el icono
        Unit targetUnit = e.targetUnit;
        targetUnit.SetSupressedUnit();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
    private void HealthSystem_OnRevived(object sender, EventArgs e)
    {
        TextMeshProUGUI actionPointsTextUnit = GetComponentInChildren<TextMeshProUGUI>();
        actionPointsTextUnit.enabled = true;
        imageList[2].enabled = true;
        imageList[5].enabled = false;
        UpdateCoverImage();
    }
    private void HealthSystem_OnHealed(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
    public void HealthSystem_OnWounded(object sender, EventArgs e)
    {
        TextMeshProUGUI actionPointsTextUnit = GetComponentInChildren<TextMeshProUGUI>();
        actionPointsTextUnit.enabled = false;
        if (imageList[2] != null)
            imageList[2].enabled = false;
        if (imageList[5] != null)
            imageList[5].enabled = true;
    }

    public void OnWoundedLoad()
    {
        TextMeshProUGUI actionPointsTextUnit = GetComponentInChildren<TextMeshProUGUI>();
        actionPointsTextUnit.enabled = false;
        wounded.enabled = true;
        if (imageList != null)
        {
            Debug.Log(imageList[2]);
            // imageList[2].enabled = false;
           
        }
    }
}
