using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer animatedMeshRenderer;
    [SerializeField] private Animator animator;


    public void Show(Material material)
    {
        meshRenderer.enabled = true;
        animatedMeshRenderer.enabled = true;
        meshRenderer.material = material;
        animatedMeshRenderer.material = material;
    }

    public void StartAnimation()
    {
        animator.SetBool("MouseOnGrid", true);
    }

    public void FinishAnimation()
    {
        animator.SetBool("MouseOnGrid", false);
    }


    public void Hide()
    {
        meshRenderer.enabled = false;
        animatedMeshRenderer.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
