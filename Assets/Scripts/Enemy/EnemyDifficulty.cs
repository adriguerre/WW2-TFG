using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDifficulty : MonoBehaviour
{

    public static EnemyDifficulty Instance { get; private set; }


    [SerializeField] private Difficult difficulty;
    public void Awake()
    {
        if (Instance != null)
        {
            
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Difficult GetDifficult()
    {
        return difficulty;
    }

    public void SetDifficult(Difficult diff)
    {
        this.difficulty = diff;
    }


}
