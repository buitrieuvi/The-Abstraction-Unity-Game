using Zenject;

public abstract class ControllerBase 
{
    [Inject] protected DataManager dataManager;
}

public class ElevatorController : ControllerBase 
{
    
}