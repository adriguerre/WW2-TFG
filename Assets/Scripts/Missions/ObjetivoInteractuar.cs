using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetivoInteractuar : MonoBehaviour
{

    public static event EventHandler onObjetivoCompletado;

    [SerializeField] private string objetiveText;
    [SerializeField] private GameObject objeto;
    [SerializeField] private bool isCompleted;
    [SerializeField] private string objectiveName;




    private ObjetivoInteractuable objetivo_Objeto;
    void Start()
    {
         isCompleted = false;
         objetivo_Objeto = objeto.GetComponent<ObjetivoInteractuable>();
    }

    public ObjetivoInteractuable getObjeto()
    {
        return objetivo_Objeto;
    }


    public void SetIsCompleted(bool compl)
    {
        this.isCompleted = compl; 
    }

    public string GetObjetiveText()
    {
        return objetiveText;
    }
    public string GetObjectiveName()
    {
        return objectiveName; 
    }

    public string getObjectiveText()
    {
        return objetiveText;
    }
    public bool getIsCompleted()
    {
        return isCompleted;
    }


    public void objetivoCompletado()
    {
        this.getObjeto().GetStar().SetActive(false);
        this.isCompleted = true;
        onObjetivoCompletado?.Invoke(this, EventArgs.Empty);
    }
}
