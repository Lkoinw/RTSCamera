﻿using RTSCamera.Config;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace RTSCamera
{
    class HideHUDView : MissionView
    {
        private GameKeyConfig _gameKeyConfig;
        private SwitchFreeCameraLogic _switchFreeCameraLogic;
        private bool _oldDisplayTargetingReticule = true;
        private bool _hideUI = false;
        private bool _isTemporarilyOpenUI = false;

        public override void OnBehaviourInitialize()
        {
            base.OnBehaviourInitialize();

            _switchFreeCameraLogic = Mission.GetMissionBehaviour<SwitchFreeCameraLogic>();
            if (_switchFreeCameraLogic != null)
            {
                _switchFreeCameraLogic.ToggleFreeCamera += OnToggleFreeCamera;
            }
            _gameKeyConfig = GameKeyConfig.Get();
            _oldDisplayTargetingReticule = BannerlordConfig.DisplayTargetingReticule;
        }

        public override void OnRemoveBehaviour()
        {
            base.OnRemoveBehaviour();

            if (_switchFreeCameraLogic != null)
                _switchFreeCameraLogic.ToggleFreeCamera -= OnToggleFreeCamera;
            RecoverTargetingReticule();

            MBDebug.DisableAllUI = false;
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if (TaleWorlds.InputSystem.Input.IsKeyPressed(_gameKeyConfig.GetKey(GameKeyEnum.ToggleHUD)) || (MBDebug.DisableAllUI && TaleWorlds.InputSystem.Input.IsKeyPressed(InputKey.Home)))
                ToggleUI();

            if (!_isTemporarilyOpenUI)
            {
                if (ScreenManager.FocusedLayer != MissionScreen.SceneLayer)
                {
                    _isTemporarilyOpenUI = true;
                    BeginTemporarilyOpenUI();
                }
            }
            else
            {
                if (ScreenManager.FocusedLayer == MissionScreen.SceneLayer)
                {
                    _isTemporarilyOpenUI = false;
                    EndTemporarilyOpenUI();
                }
            }
        }

        public void ToggleUI()
        {
            MBDebug.DisableAllUI = !_hideUI && !MBDebug.DisableAllUI;
            _hideUI = MBDebug.DisableAllUI;
        }

        public void BeginTemporarilyOpenUI()
        {
            _hideUI = MBDebug.DisableAllUI;
            MBDebug.DisableAllUI = false;
        }

        public void EndTemporarilyOpenUI()
        {
            MBDebug.DisableAllUI = _hideUI;
        }

        private void OnToggleFreeCamera(bool freeCamera)
        {
            if (freeCamera)
            {
                _oldDisplayTargetingReticule = BannerlordConfig.DisplayTargetingReticule;
                BannerlordConfig.DisplayTargetingReticule = false;
            }
            else
            {
                BannerlordConfig.DisplayTargetingReticule = _oldDisplayTargetingReticule;
            }
        }

        private void RecoverTargetingReticule()
        {
            if (_switchFreeCameraLogic != null && _switchFreeCameraLogic.isSpectatorCamera && !BannerlordConfig.DisplayTargetingReticule)
            {
                BannerlordConfig.DisplayTargetingReticule = _oldDisplayTargetingReticule;
            }
        }
    }
}
