using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public UIManager UIManager { get; private set; }
    public DataManager DataManager { get; private set; }
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UIManager = GetComponentInChildren<UIManager>();
        DataManager = GetComponentInChildren<DataManager>();
    }
}
