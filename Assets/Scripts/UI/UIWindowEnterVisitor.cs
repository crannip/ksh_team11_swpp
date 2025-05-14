using UnityEngine;

public class UIWindowEnterVisitor : UIWindowVisitor
{
    public override void Visit(UIWindow window)
    {
        Debug.Log("AQQQQQ");
        window.onEnterDown?.Invoke();
    }
}
