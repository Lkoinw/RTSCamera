﻿using MissionLibrary.HotKey;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.Engine.Screens;

namespace MissionSharedLibrary.View.HotKey
{
    public class GameKeyConfigView : MissionView
    {
        private GauntletLayer _gauntletLayer;
        private GameKeyConfigVM _dataSource;
        private KeybindingPopup _keybindingPopup;
        private IHotKeySetter _currentGameKey;
        private bool _enableKeyBindingPopupNextTick;

        public const string KeyBindRequestEventId = "KeyBindRequest";
        public const string KeyBindRequestReceiverId = "GameKeyConfigView";

        public GameKeyConfigView()
        {
            ViewOrderPriority = 1000;
        }

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            _keybindingPopup = new KeybindingPopup(SetHotKey, MissionScreen);
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            _keybindingPopup.OnToggle(false);
            _keybindingPopup = null;
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (_gauntletLayer == null)
                return;
            if (!_keybindingPopup.IsActive && _gauntletLayer.Input.IsHotKeyReleased("Exit"))
            {
                _dataSource.ExecuteCancel();
            }
            _keybindingPopup.Tick();
            if (_enableKeyBindingPopupNextTick)
            {
                _enableKeyBindingPopupNextTick = false;
                _keybindingPopup.OnToggle(true);
            }
        }

        public void Activate()
        {
            _dataSource = new GameKeyConfigVM(AGameKeyCategoryManager.Get(), OnKeyBindRequest, Deactivate);
            _gauntletLayer = new GauntletLayer(ViewOrderPriority) {IsFocusLayer = true};
            _gauntletLayer.LoadMovie("MissionLibraryOptionsGameKeyScreen", _dataSource);
            _gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            _gauntletLayer.IsFocusLayer = true;
            MissionScreen.AddLayer(_gauntletLayer);
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        public void Deactivate()
        {
            _gauntletLayer.InputRestrictions.ResetInputRestrictions();
            MissionScreen.RemoveLayer(_gauntletLayer);
            _gauntletLayer = null;
            _dataSource.OnFinalize();
            _dataSource = null;
        }

        private void OnKeyBindRequest(IHotKeySetter requestedHotKeyToChange)
        {
            _currentGameKey = requestedHotKeyToChange;
            _enableKeyBindingPopupNextTick = true;
        }

        private void SetHotKey(Key key)
        {
            //if (_dataSource.Groups.First<GameKeyGroupVM>((g => g.GameKeys.Contains(this._currentGameKey))).GameKeys.Any<GameKeyOptionVM>(keyVM => keyVM.CurrentKey.InputKey == key.InputKey))
            //    InformationManager.AddQuickInformation(new TextObject("{=n4UUrd1p}Already in use"));
            /*else*/ if (_gauntletLayer.Input.IsHotKeyReleased("Exit"))
            {
                _currentGameKey = null;
                _keybindingPopup.OnToggle(false);
            }
            else
            {
                _currentGameKey?.Set(key.InputKey);
                _currentGameKey = null;
                _keybindingPopup.OnToggle(false);
            }
        }

    }
}
