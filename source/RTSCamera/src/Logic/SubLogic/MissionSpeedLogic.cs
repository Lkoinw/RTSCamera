﻿using MissionSharedLibrary.Utilities;
using RTSCamera.Config;
using RTSCamera.Config.HotKey;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.Logic.SubLogic
{
    public class MissionSpeedLogic
    {
        private readonly RTSCameraLogic _logic;
        private readonly RTSCameraConfig _config = RTSCameraConfig.Get();
        private const int TimeSpeedConstant = 23454;

        public Mission Mission => _logic.Mission;

        public MissionSpeedLogic(RTSCameraLogic logic)
        {
            _logic = logic;
        }

        public void AfterStart()
        {
            if (_config.SlowMotionMode)
            {
                Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(_config.SlowMotionFactor, TimeSpeedConstant));
            }
        }

        public void OnMissionTick(float dt)
        {
            if (RTSCameraGameKeyCategory.GetKey(GameKeyEnum.Pause).IsKeyPressed(Mission.InputManager))
            {
                TogglePause();
            }

            if (RTSCameraGameKeyCategory.GetKey(GameKeyEnum.SlowMotion).IsKeyPressed(Mission.InputManager))
            {
                SetSlowMotionMode(!_config.SlowMotionMode);
            }
        }

        public void TogglePause()
        {
            var paused = !MissionState.Current.Paused;
            MissionState.Current.Paused = paused;
            Utility.DisplayLocalizedText(paused ? "str_rts_camera_mission_paused" : "str_rts_camera_mission_continued");
        }

        public void SetSlowMotionMode(bool slowMotionMode)
        {
            if (_config.SlowMotionMode == slowMotionMode)
                return;
            if (!_config.SlowMotionMode && slowMotionMode)
            {
                Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(_config.SlowMotionFactor, TimeSpeedConstant));
            }
            else
            {
                Mission.RemoveTimeSpeedRequest(TimeSpeedConstant);
            }
            _config.SlowMotionMode = slowMotionMode;
            Utility.DisplayLocalizedText(slowMotionMode ? "str_rts_camera_slow_motion_enabled" : "str_rts_camera_normal_mode_enabled");
            _config.Serialize();
        }

        public void SetSlowMotionFactor(float factor)
        {
            _config.SlowMotionFactor = factor;
            if (_config.SlowMotionMode)
            {
                Mission.RemoveTimeSpeedRequest(TimeSpeedConstant);
                Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(factor, TimeSpeedConstant));
            }
        }

        //public void ApplySlowMotionFactor()
        //{
        //    if (Math.Abs(_config.SlowMotionFactor - 1.0f) < 0.01f)
        //        SetNormalMode();
        //    else
        //    {
        //        SetFastForwardModeImpl(false);
        //        SetSlowMotionModeImpl(_config.SlowMotionFactor);
        //        SetFastForwardModeImpl(false);
        //        Utility.DisplayLocalizedText("str_rts_camera_slow_motion_enabled");
        //    }
        //}

        //public void SetFastForwardMode()
        //{
        //    Mission.SetFastForwardingFromUI(true);
        //    Utility.DisplayLocalizedText("str_rts_camera_fast_forward_mode_enabled");
        //}
    }
}
