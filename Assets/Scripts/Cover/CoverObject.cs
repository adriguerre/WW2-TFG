using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObject : MonoBehaviour
{
    [SerializeField] private CoverType coverType;
    private GridPosition gridPosition;
    public void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetCoverObjectAtGridPosition(gridPosition, this);
        //Dado que solo hacemos set de donde lo ponemos, solo cuenta esa, si la cobertura es mas larga, no la cuenta. 
    }
    public CoverType GetCoverType()
    {
        return coverType;
    }

}
public enum CoverType
{
    None,
    Half,
    Full,
}
