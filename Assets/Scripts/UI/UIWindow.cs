using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UIWindow : MonoBehaviour, IUIWindowObserver
{
    public bool isInputBlocked;
    // 이벤트들
    public UnityEvent onEnterDown;
    public UnityEvent<int> onHorizontalDown;
    public UnityEvent<int> onVerticalDown;

    protected virtual void Start()
    {
        GameManager.Instance.UIManager.RegisterObserver(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.UIManager.UnregisterObserver(this);
    }
    

    public void SetInputBlockMode(bool isBlocked)
    {
        isInputBlocked = isBlocked;
    }
    
    public void ObserverUpdate(UIWindowVisitor visitor)
    {
        visitor.Visit(this);
    }
}
