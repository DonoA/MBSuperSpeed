using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screen;

namespace MBSuperSpeed
{
    public class Settings
    {
        [XmlElement]
        public string FFKey { get; set; } = "X";

        [XmlElement]
        public string FFLockKey { get; set; } = "Z";
    }

    public static class XmlUtil
    {
        public static T SettingsFor<T>(string moduleName)
        {
            string settingsPath = Path.Combine(BasePath.Name, "Modules", moduleName, "settings.xml");
            try
            {
                using (XmlReader reader = XmlReader.Create(settingsPath))
                {
                    XmlRootAttribute root = new XmlRootAttribute();
                    root.ElementName = moduleName + ".Settings";
                    root.IsNullable = true;

                    if (reader.MoveToContent() != XmlNodeType.Element)
                    {
                        return default;
                    }

                    if (reader.Name != root.ElementName)
                    {
                        return default;
                    }

                    XmlSerializer serialiser = new XmlSerializer(typeof(T), root);
                    var loaded = (T)serialiser.Deserialize(reader);
                    return loaded;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }
    }

    public class SubModule : MBSubModuleBase
    {
        private bool isHeldFF = false;

        private TaleWorlds.InputSystem.InputKey FFKey = TaleWorlds.InputSystem.InputKey.X;
        private TaleWorlds.InputSystem.InputKey FFLockKey = TaleWorlds.InputSystem.InputKey.Z;

        protected override void OnSubModuleLoad()
        {
            try
            {
                var settings = XmlUtil.SettingsFor<Settings>("MBSuperSpeed");
                Enum.TryParse(settings.FFKey, out FFKey);
                Enum.TryParse(settings.FFLockKey, out FFLockKey);
            }
            catch(Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage("Failed to load config.", Color.FromUint(4282569842U)));
            }
        }

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

            try
            {
                MissionScreen? topScreen = ScreenManager.TopScreen as MissionScreen;

                bool isTriggeredLockFF = topScreen != null && topScreen.InputManager.IsControlDown() && topScreen.InputManager.IsKeyPressed(FFLockKey);
                bool isTriggeredFF = topScreen != null && topScreen.InputManager.IsControlDown() && topScreen.InputManager.IsKeyDown(FFKey);

                if (isTriggeredFF != isHeldFF)
                {
                    Mission.Current.SetFastForwardingFromUI(isTriggeredFF);
                    InformationManager.DisplayMessage(new InformationMessage("Vroom = " + Mission.Current.IsFastForward, Color.FromUint(4282569842U)));
                }
                isHeldFF = isTriggeredFF;

                if (isTriggeredLockFF)
                {
                    Mission.Current.SetFastForwardingFromUI(!Mission.Current.IsFastForward);
                    InformationManager.DisplayMessage(new InformationMessage("Vroom = " + Mission.Current.IsFastForward, Color.FromUint(4282569842U)));
                }
            }
            catch (Exception)
            {
                // Pass
            }
        }
    }
}
