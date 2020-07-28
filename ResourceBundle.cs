using UnhollowerRuntimeLib;
using UnityEngine;

namespace MControl
{
    class ResourceBundle
    {
        internal readonly Sprite MediaSettingsButton;
        internal readonly Sprite bigPlayPauseButton;
        internal readonly Sprite bigNextButton;
        internal readonly Sprite bigStopButton;

        internal ResourceBundle(AssetBundle bundle) {
            Sprite LoadSprite(string str) {
                Sprite sprite = bundle.LoadAsset(str, Il2CppType.Of<Sprite>()).Cast<Sprite>();
                sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                return sprite;
            }

            MediaSettingsButton = LoadSprite("MediaSettings.png");
            bigPlayPauseButton = LoadSprite("bigbutton_playpause.png");
            bigNextButton = LoadSprite("bigbutton_prevnext.png");
            bigStopButton = LoadSprite("bigbutton_stop.png");
        }
    }
}
