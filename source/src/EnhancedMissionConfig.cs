﻿using System;
using System.Xml.Serialization;

namespace EnhancedMission
{
    public class EnhancedMissionConfig : EnhancedMissionConfigBase<EnhancedMissionConfig>
    {
        protected static Version BinaryVersion => new Version(1, 0);

        protected override void UpgradeToCurrentVersion()
        {
            switch (ConfigVersion?.ToString())
            {
                default:
                    Utility.DisplayLocalizedText("str_config_incompatible");
                    ResetToDefault();
                    Serialize();
                    break;
                case "1.0":
                    break;
            }
        }

        private static EnhancedMissionConfig _instance;

        public string ConfigVersion { get; set; } = BinaryVersion.ToString();

        public bool UseFreeCameraByDefault = false;

        public int PlayerFormation = 4;

        public bool DisableDeath = false;

        public bool UseRealisticBlocking = false;

        public bool ChangeCombatAI = false;

        public int CombatAI = 100;

        public float SlowMotionFactor = 1;


        public static EnhancedMissionConfig Get()
        {
            if (_instance == null)
            {
                _instance = CreateDefault();
                _instance.SyncWithSave();
            }

            return _instance;
        }

        private static EnhancedMissionConfig CreateDefault()
        {
            return new EnhancedMissionConfig();
        }

        protected override XmlSerializer serializer => new XmlSerializer(typeof(EnhancedMissionConfig));

        protected override void CopyFrom(EnhancedMissionConfig other)
        {
            this.ConfigVersion = other.ConfigVersion;
            this.UseFreeCameraByDefault = other.UseFreeCameraByDefault;
            this.PlayerFormation = other.PlayerFormation;
            this.DisableDeath = other.DisableDeath;
            this.UseRealisticBlocking = other.UseRealisticBlocking;
            this.ChangeCombatAI = other.ChangeCombatAI;
            this.CombatAI = other.CombatAI;
        }

        public override void ResetToDefault()
        {
            CopyFrom(CreateDefault());
        }
        [XmlIgnore]
        protected override string SaveName => SavePath + nameof(EnhancedMissionConfig) + ".xml";
        [XmlIgnore]
        protected override string[] OldNames { get; } = { };
    }
}