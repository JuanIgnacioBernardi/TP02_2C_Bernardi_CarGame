using UnityEngine;

public class AudienceAimingState : AudienceEnemyStateBase
{
    public AudienceAimingState(bool needsExitTime, AudienceEnemy enemy, float windUpTime)
        : base(needsExitTime, enemy, windUpTime) { }

    public override void OnEnter()
    {
        base.OnEnter();
        RequestedExit = true; 
    }
    public override void OnLogic()
    {
        // Look at the target while aiming
        if (Enemy.CarTarget != null)
        {
            Vector3 dir = Enemy.CarTarget.position - Enemy.transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                Enemy.transform.rotation = Quaternion.LookRotation(dir);
        }
        base.OnLogic();
    }
}