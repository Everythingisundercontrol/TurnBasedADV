public class EndTurnState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.EndTurnOnEnter();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        WarManager.Instance.EndTurnOnExit();
    }
}