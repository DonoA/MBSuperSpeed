using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MBSuperSpeed
{


    public class SubModule : MBSubModuleBase
    {
        private bool isHeldX = false;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage("Loaded MBSuperSpeed.", Color.FromUint(4282569842U)));
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Mission.Current == null)
            {
                return;
            }

            ScreenBase topScreen = ScreenManager.TopScreen;

            bool isTriggeredZ = topScreen != null && topScreen.DebugInput.IsControlDown() && topScreen.DebugInput.IsKeyPressed(TaleWorlds.InputSystem.InputKey.Z);
            bool isTriggeredX = topScreen != null && topScreen.DebugInput.IsControlDown() && topScreen.DebugInput.IsKeyDown(TaleWorlds.InputSystem.InputKey.X);

            if (isTriggeredX != isHeldX)
            {
                Mission.Current.SetFastForwardingFromUI(isTriggeredX);
                InformationManager.DisplayMessage(new InformationMessage("Vroom = " + Mission.Current.IsFastForward, Color.FromUint(4282569842U)));
            }
            isHeldX = isTriggeredX;

            if (isTriggeredZ)
            {
                Mission.Current.SetFastForwardingFromUI(!Mission.Current.IsFastForward);
                InformationManager.DisplayMessage(new InformationMessage("Vroom = " + Mission.Current.IsFastForward, Color.FromUint(4282569842U)));
            }

        }
    }
}
