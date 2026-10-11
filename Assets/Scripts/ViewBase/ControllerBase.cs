using Zenject;

public abstract class ControllerBase 
{
    [Inject] protected DataManager dataManager;
}
