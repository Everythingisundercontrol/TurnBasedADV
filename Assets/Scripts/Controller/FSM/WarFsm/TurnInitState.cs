public class TurnInitState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.TurnInitOnEnter();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        WarManager.Instance.TurnInitOnExit();
    }
}