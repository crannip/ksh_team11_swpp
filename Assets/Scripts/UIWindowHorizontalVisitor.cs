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
        if (_direction == 1 || _direction == -1) 
            window.onHorizontalDown.Invoke(_direction);
    }
}
