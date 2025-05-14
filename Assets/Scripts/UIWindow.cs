using UnityEngine;
using UnityEngine.Events;

public class UIWindow : MonoBehaviour, IUIWindowObserver
{
    public bool isInputBlocked;
    // 이벤트들
    public UnityEvent onEnterDown;
    public UnityEvent<int> onHorizontalDown;
    public UnityEvent<int> onVerticalDown;

    public void SetInputBlockMode(bool isBlocked)
    {
        isInputBlocked = isBlocked;
    }
    
    public void ObserverUpdate(UIWindowVisitor visitor)
    {
        if (!isInputBlocked)
            visitor.Visit(this);
    }
}
