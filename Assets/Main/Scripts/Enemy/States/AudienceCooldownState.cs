public class AudienceCooldownState : AudienceEnemyStateBase
{
    public AudienceCooldownState(bool needsExitTime, AudienceEnemy enemy, float cooldown)
        : base(needsExitTime, enemy, cooldown) { }
    public override void OnEnter()
    {
        base.OnEnter();
        RequestedExit = true;
    }
}