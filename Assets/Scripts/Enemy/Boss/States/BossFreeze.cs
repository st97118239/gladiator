using System;
using UnityEngine;

public class BossFreeze : IBossState
{
    public void UpdateState(BossStateMachine controller)
    {

    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.isFrozen = true;
    }

    public void OnExit(BossStateMachine controller)
    {
        controller.isFrozen = false;
    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}