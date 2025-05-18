using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public UIManager UIManager { get; private set; }
    public DataManager DataManager { get; private set; }
    public SceneLoadManager SceneLoadManager { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UIManager = GetComponentInChildren<UIManager>();
        DataManager = GetComponentInChildren<DataManager>();
        SceneLoadManager = GetComponentInChildren<SceneLoadManager>();
    }
}
