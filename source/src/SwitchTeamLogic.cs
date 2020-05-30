﻿using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace RTSCamera
{
    class SwitchTeamLogic : MissionLogic
    {
        public delegate void SwitchTeamDelegate();

        public event SwitchTeamDelegate PreSwitchTeam;
        public event SwitchTeamDelegate PostSwitchTeam;

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            if (Mission.InputManager.IsKeyPressed(InputKey.F9))
                SwapTeam();
        }

        public void SwapTeam()
        {
            bool firstTime = Mission.PlayerEnemyTeam.PlayerOrderController.Owner == null;
            var targetAgent = !Utility.IsAgentDead(Mission.PlayerEnemyTeam.PlayerOrderController.Owner)
                ? Mission.PlayerEnemyTeam.PlayerOrderController.Owner
                : !Utility.IsAgentDead(Mission.PlayerEnemyTeam.GeneralAgent) ? Mission.PlayerEnemyTeam.GeneralAgent : Mission.PlayerEnemyTeam.Leader;
            if (targetAgent == null)
            {
                Utility.DisplayLocalizedText("str_em_enemy_wiped_out");
                return;
            }
            if (!Utility.IsPlayerDead()) // MainAgent may be null because of free camera mode.
            {
                Mission.MainAgent.Controller = Agent.ControllerType.AI;
                Mission.MainAgent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
            }
            Utility.DisplayLocalizedText("str_em_switch_to_enemy_team");

            PreSwitchTeam?.Invoke();
            Mission.PlayerTeam = Mission.PlayerEnemyTeam;
            targetAgent.Controller = Agent.ControllerType.Player;
            PostSwitchTeam?.Invoke();

            if (firstTime)
            {
                foreach (var formation in Mission.PlayerTeam.FormationsIncludingEmpty)
                {
                    bool isAIControlled = formation.IsAIControlled;
                    formation.PlayerOwner = Mission.MainAgent;
                    formation.IsAIControlled = isAIControlled;
                }
            }
        }
    }
}
