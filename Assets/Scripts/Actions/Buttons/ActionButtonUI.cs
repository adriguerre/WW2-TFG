using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ActionButtonUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private Image actionImage;
    [SerializeField] private GameObject UISelected;
    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();
        //actionImage.material = baseAction.GetActionImage();
        //Aqui podría poner la foto que quiera para la habilidad
        //button.onClick.AddListener(MoveActionBtn_Click);
        //Y ahora con el uso de funciones anonimas para evitar tener que hacer el metodo MoveActionBtn_Click
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
            
        });
    }


    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        if(selectedBaseAction == baseAction)
            button.image.color = Color.green;
        else
            button.image.color = Color.white;
       // UISelected.SetActive(selectedBaseAction == baseAction);
    }
}
