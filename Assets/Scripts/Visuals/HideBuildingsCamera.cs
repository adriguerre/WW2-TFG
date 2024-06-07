using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBuildingsCamera : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            meshRenderer.enabled = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        
            meshRenderer.enabled = true;
        
    }
}
