public class UIWindowEnterVisitor : UIWindowVisitor
{
    public override void Visit(UIWindow window)
    {
        window.onEnterDown?.Invoke();
    }
}
