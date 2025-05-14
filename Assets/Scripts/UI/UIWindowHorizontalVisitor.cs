using UnityEngine;

public class UIWindowHorizontalVisitor : UIWindowVisitor
{
    // 1 : right, -1 : left
    private int _direction;

    public void SetDirection(int direction)
    {
        _direction = direction;
    }
    
    public override void Visit(UIWindow window)
    {
        Debug.Log(_direction);
        
            window.onHorizontalDown?.Invoke(_direction);
    }
}
