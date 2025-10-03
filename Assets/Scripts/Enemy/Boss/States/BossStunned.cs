using UnityEngine;

public class BossStunned : IBossState
{
    public void UpdateState(BossStateMachine controller)
    {

    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.isStunned = true;
    }

    public void OnExit(BossStateMachine controller)
    {
        controller.isStunned = false;
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
