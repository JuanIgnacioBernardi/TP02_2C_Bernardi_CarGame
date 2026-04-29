using UnityHFSM;

public abstract class AudienceEnemyStateBase : State<AudienceState, AudienceEvent>
{
    protected readonly AudienceEnemy Enemy;
    protected bool RequestedExit;
    protected float ExitTime;
    protected AudienceEnemyStateBase(bool needsExitTime, AudienceEnemy enemy, float exitTime = 0.1f)
    {
        Enemy = enemy;
        ExitTime = exitTime;
        this.needsExitTime = needsExitTime;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        RequestedExit = false;
    }
    public override void OnLogic()
    {
        base.OnLogic();
        if (RequestedExit && timer.Elapsed >= ExitTime)
            fsm.StateCanExit();
    }
    public override void OnExitRequest()
    {
        if (!needsExitTime) fsm.StateCanExit();
        else RequestedExit = true;
    }
}