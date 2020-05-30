﻿using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace RTSCamera
{
    [DefaultView]
    class AddAdditionalMissionBehaviourView : MissionView
    {
        public override void OnCreated()
        {
            base.OnCreated();
            var config = RTSCameraConfig.Get();
            List<MissionBehaviour> list = new List<MissionBehaviour>
            {
                new DisableDeathLogic(config),
                new MissionSpeedLogic(),
                new SwitchFreeCameraLogic(config),
                new ControlTroopLogic(),
                new FixScoreBoardAfterPlayerDeadLogic(),
                new CommanderLogic(),
                new HideHUDLogic(),
                new SwitchTeamLogic(),

                new MissionMenuView(),
                new FlyCameraMissionView(),
                new GameKeyConfigView(),
                new RTSCameraOrderTroopPlacer()
            };


            foreach (var missionBehaviour in list)
            {
                if (missionBehaviour is AddAdditionalMissionBehaviourView)
                    continue; // avoid accidentally add itself infinitely.
                AddMissionBehaviour(missionBehaviour);
            }

            foreach (var extension in RTSCameraExtension.Extensions)
            {
                foreach (var missionBehaviour in extension.CreateMissionBehaviours(Mission))
                {
                    AddMissionBehaviour(missionBehaviour);
                }
            }
        }

        public override void OnPreMissionTick(float dt)
        {
            var orderTroopPlacer = Mission.GetMissionBehaviour<OrderTroopPlacer>();
            if (orderTroopPlacer != null)
                Mission.RemoveMissionBehaviour(orderTroopPlacer);
        }

        private void AddMissionBehaviour(MissionBehaviour behaviour)
        {
            behaviour.OnAfterMissionCreated();
            Mission.AddMissionBehaviour(behaviour);
        }
    }
}
