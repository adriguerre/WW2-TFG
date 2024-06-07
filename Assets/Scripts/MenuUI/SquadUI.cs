using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadUI : MonoBehaviour
{
    public static SquadUI Instance { get; private set; }

    List<PhotoIndividual> individualUI; 
    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }



    [SerializeField] private Transform photoPrefab;
    [SerializeField] private Transform photoPrefabContainerTransform;



    private void Start()
    {
        individualUI = new List<PhotoIndividual>();
        CreateSquadPhotos();
    }

    public void UpdateUiOnLoad(Unit unit)
    { 

        CreateSquadPhotos(unit);
    }

   public void CreateSquadPhotos(Unit unit)
    {

        Transform photoTransform = Instantiate(photoPrefab, photoPrefabContainerTransform); //Cogemos uno de los baseAction ( es decir una accion normal)
        PhotoIndividual photoIndividual = photoTransform.GetComponent<PhotoIndividual>(); //Cogemos el componente hijo

        photoIndividual.SetRol(unit.GetRole());
        photoIndividual.SetUnit(unit);
        photoIndividual.SetAvatar(unit.GetAvatar());
        photoIndividual.UpdateOnLoad();

        
    }


    public void DestroyAllSquadUI()
    {
       PhotoIndividual[] gameObjects = GetComponentsInChildren<PhotoIndividual>();

        foreach(PhotoIndividual gameObject in gameObjects)
        {
            Destroy(gameObject.gameObject);
        }
    }

 
    public void CreateSquadPhotos()
    {
        foreach (Unit friendlyUnit in UnitManager.Instance.GetFriendlyUnitList())
        {
            //Instanciamos cada prefab con una accion
            Transform photoTransform = Instantiate(photoPrefab, photoPrefabContainerTransform); //Cogemos uno de los baseAction ( es decir una accion normal)
            PhotoIndividual photoIndividual = photoTransform.GetComponent<PhotoIndividual>(); //Cogemos el componente hijo

            photoIndividual.SetRol(friendlyUnit.GetRole());
            photoIndividual.SetUnit(friendlyUnit);
            photoIndividual.SetAvatar(friendlyUnit.GetAvatar());
            
        }
    }


  
}
