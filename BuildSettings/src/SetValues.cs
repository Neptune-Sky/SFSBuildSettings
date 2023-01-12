using System;
using System.Globalization;
using SFS.Audio;
using SFS.Input;
using SFS.Translations;
using SFS.UI;

namespace BuildSettings
{
    public class SetValues
    {
        public static void SetSmallMove()
        {
            Menu.textInput.Open(Loc.main.Cancel, Loc.main.Save, SetNewSmallMove, CloseMode.Current, TextInputMenu.Element("Set small move increment:" + Environment.NewLine, string.Empty));

            void SetNewSmallMove(string[] value)
            {
                float new_value;
                try
                {
                    SoundPlayer.main.clickSound.Play();
                    new_value = Math.Abs(float.Parse(value[0], CultureInfo.InvariantCulture));
                    Config.settings.smallMove = new_value;
                }
                catch
                {
                    SoundPlayer.main.denySound.Play();
                    MsgDrawer.main.Log("Could not parse value, the new move value will not be set");
                    return;
                }
                MsgDrawer.main.Log("Small move set to " + new_value.ToString());
            }
        }
        public static void SetSmallResize()
        {
            Menu.textInput.Open(Loc.main.Cancel, Loc.main.Save, SetNewSmallResize, CloseMode.Current, TextInputMenu.Element("Set small resize increment:" + Environment.NewLine, string.Empty));

            void SetNewSmallResize(string[] value)
            {
                float new_value;
                try
                {
                    SoundPlayer.main.clickSound.Play();
                    new_value = Math.Abs(float.Parse(value[0], CultureInfo.InvariantCulture));
                    Config.settings.smallResize = new_value;
                }
                catch
                {
                    SoundPlayer.main.denySound.Play();
                    MsgDrawer.main.Log("Could not parse value, the new resize value will not be set");
                    return;
                }
                MsgDrawer.main.Log("Small resize set to " + new_value.ToString());
            }
        }
    }
}

