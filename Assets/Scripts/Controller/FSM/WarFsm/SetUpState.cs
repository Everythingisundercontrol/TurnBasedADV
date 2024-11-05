public class SetUpState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.SetUpOnEnter();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        WarManager.Instance.SetUpOnExit();
    }
}