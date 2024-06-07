using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetiveData
{

    public string objectiveText;
    public bool isCompleted;
    public string objectiveName; 


    public ObjetiveData(string objectiveText, bool isCompleted, string objectiveName)
    {
        this.objectiveText = objectiveText;
        this.isCompleted = isCompleted;
        this.objectiveName = objectiveName;
    }

    public string GetObjectiveText()
    {
        return objectiveText;
    }
    public string GetObjectiveName()
    {
        return objectiveName;
    }
}
