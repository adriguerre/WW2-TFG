using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ActionBusyUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        UpdateVisual(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        UpdateVisual(isBusy);
    }


    private void UpdateVisual(bool isBusy)
    {
        gameObject.SetActive(isBusy);
       
    }
    
}
