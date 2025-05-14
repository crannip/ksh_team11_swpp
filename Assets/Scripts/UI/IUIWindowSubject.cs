public interface IUIWindowSubject
{
    public void RegisterObserver(IUIWindowObserver observer);
    public void UnregisterObserver(IUIWindowObserver observer);
    public void NotifyObservers();
}

