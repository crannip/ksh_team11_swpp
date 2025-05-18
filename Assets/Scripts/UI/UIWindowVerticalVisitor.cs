using UnityEngine;

public class UIWindowVerticalVisitor : UIWindowVisitor
{
    // 1 : up, -1 : down
    private int _direction;

    public void SetDirection(int direction)
    {
        _direction = direction;
    }
    
    public override void Visit(UIWindow window)
    {
        window.onVerticalDown?.Invoke(_direction);
    }
}
