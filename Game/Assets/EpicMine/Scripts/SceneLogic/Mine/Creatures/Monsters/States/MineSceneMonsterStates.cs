using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BlackTemple.EpicMine.Assets.EpicMine.Scripts.SceneLogic.Mine.Creatures.Monsters.States
{
    public class MineSceneMonsterStates
    {

        public class BaseState
        {
            protected Action LockAction;
            protected Action UnLockAction;
            protected int Delay;
            protected BaseState State;

            public BaseState(Action lockAction = null, Action unLockAction = null, int delay = 0)
            {
                LockAction = lockAction;
                UnLockAction = unLockAction;
                Delay = delay;
            }

            public virtual void Enter()
            {
               
            }

            protected virtual Task Execute()
            {
                return null;
            }

            public virtual void Exit()
            {

            }
        }

        public class IdleState : BaseState
        {
            public AttackState Attack;
            public Task Execute;

            public IdleState(BaseState state, Action lockAction, Action unLockAction, int delay) : base(lockAction, unLockAction, delay)
            {
            }

          //  protected override async Task Execute()
          //  {
             //   Attack = new 
               // await Task.Delay(TimeSpan.FromSeconds(Delay));

          //  }

            public override void Exit()
            {
                base.Exit();

                Execute.Dispose();
          //      Ability.Dispose();
            }

            public void GetScared(bool state)
            {

            }
        }

        public class AttackState : BaseState
        {
            public AttackState(Action lockAction = null, Action unLockAction= null, int delay = 0) : base(lockAction, unLockAction, delay)
            {
            }
        }
    }
}
