using MelonLoader;
using System;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnhollowerRuntimeLib;
namespace MControl
{
    public static class BuildInfo {
        public const string Name = "MControl";
        public const string Author = "Janni9009";
        public const string Company = "VRChat Modding Group";
        public const string Version = "1.2";
        public const string DownloadLink = null;
    }
    public class MControl : MelonMod {
        public override void OnApplicationStart() {
            MelonPrefs.RegisterCategory("MControl", "MControl");
            MelonPrefs.RegisterBool("MControl", "ShowAtStartup", false, "Show Media Keys on game launch");
            MelonPrefs.RegisterBool("MControl", "MoveMediaVRCPlus", false, "Shift Media Keys over to the left to not obstruct VRC+ Pet");
        }
        public override void VRChat_OnUiManagerInit() {
            MelonCoroutines.Start(Init());
        }
        private IEnumerator Init() {
            MelonLogger.Log("Loading Sprite Data Bundle");
            
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MControl.resources.mco");
            using MemoryStream memStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memStream);
            var assetBundle = AssetBundle.LoadFromMemory_Internal(memStream.ToArray(), 0);
            assetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            Buttons = new ResourceBundle(assetBundle);

            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "wsock32.dll"))) {
                QuickMenu.prop_QuickMenu_0.GetComponent<BoxCollider>().size += new Vector3(0f, 840f, 0f);
                MelonLogger.Log("RubyLoader not found, therefore expanding QM collision up");
            }

            MelonLogger.Log("Constructing Media Control Buttons: 1/5");
            
            BaseButton = QuickMenu.prop_QuickMenu_0.transform.Find("ShortcutMenu/WorldsButton").gameObject;
            parentMenu = QuickMenu.prop_QuickMenu_0.transform.Find("ShortcutMenu");
            ShowButton = GameObject.Instantiate(BaseButton, parentMenu, true);
            var shbrt = ShowButton.GetComponent<RectTransform>();
            shbrt.localPosition = new Vector2(770f, 2010f);
            shbrt.localScale = shbrt.localScale / 3f;
            ShowButton.GetComponent<Image>().sprite = Buttons.MediaSettingsButton;
            Component.DestroyImmediate(ShowButton.GetComponent<UiTooltip>());
            Component.DestroyImmediate(ShowButton.GetComponentInChildren<Text>());
            ShowButtonButton = ShowButton.GetComponent<Button>();
            ShowButtonButton.name = "MC_ShowKeys";
            ShowButtonButton.onClick = new Button.ButtonClickedEvent();
            ShowButtonButton.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(ToggleButtons)));

            MelonLogger.Log("Constructing Media Control Buttons: 2/5");

            PrevButton = GameObject.Instantiate(BaseButton, parentMenu, true);
            var prbrt = PrevButton.GetComponent<RectTransform>();
            prbrt.localScale = prbrt.localScale / 1.5f;
            PrevButton.GetComponent<Image>().sprite = Buttons.bigNextButton;
            PrevButton.GetComponent<UiTooltip>().text = "Go to the previous song in your Playlist (click twice)\n or restart the current song";
            PrevButton.GetComponent<UiTooltip>().alternateText = "Go to the previous song in your Playlist (click twice)\n or restart the current song";
            PrevButton.transform.rotation *= Quaternion.Euler(0f, 0f, 180f);
            Component.DestroyImmediate(PrevButton.GetComponentInChildren<Text>());
            PrevButtonButton = PrevButton.GetComponent<Button>();
            PrevButtonButton.name = "MC_PreviousSong";
            PrevButtonButton.onClick = new Button.ButtonClickedEvent();
            PrevButtonButton.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(MediaControl.PrevTrack)));
            PrevButton.SetActive(MelonPrefs.GetBool("MControl", "ShowAtStartup"));

            MelonLogger.Log("Constructing Media Control Buttons: 3/5");

            PlayButton = GameObject.Instantiate(BaseButton, parentMenu, true);
            var plbrt = PlayButton.GetComponent<RectTransform>();
            plbrt.localScale = plbrt.localScale / 1.5f;
            PlayButton.GetComponent<Image>().sprite = Buttons.bigPlayPauseButton;
            PlayButton.GetComponent<UiTooltip>().text = "Pause or continue listening to the current song";
            PlayButton.GetComponent<UiTooltip>().alternateText = "Pause or continue listening to the current song";
            Component.DestroyImmediate(PlayButton.GetComponentInChildren<Text>());
            PlayButtonButton = PlayButton.GetComponent<Button>();
            PlayButtonButton.name = "MC_PlayPause";
            PlayButtonButton.onClick = new Button.ButtonClickedEvent();
            PlayButtonButton.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(MediaControl.PlayPause)));
            PlayButton.SetActive(MelonPrefs.GetBool("MControl", "ShowAtStartup"));

            MelonLogger.Log("Constructing Media Control Buttons: 4/5");

            StopButton = GameObject.Instantiate(BaseButton, parentMenu, true);
            var stbrt = StopButton.GetComponent<RectTransform>();
            stbrt.localScale = stbrt.localScale / 1.5f;
            StopButton.GetComponent<Image>().sprite = Buttons.bigStopButton;
            StopButton.GetComponent<UiTooltip>().text = "Stop the current song completely";
            StopButton.GetComponent<UiTooltip>().alternateText = "Stop the current song completely";
            Component.DestroyImmediate(StopButton.GetComponentInChildren<Text>());
            StopButtonButton = StopButton.GetComponent<Button>();
            StopButtonButton.name = "MC_StopSong";
            StopButtonButton.onClick = new Button.ButtonClickedEvent();
            StopButtonButton.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(MediaControl.Stop)));
            StopButton.SetActive(MelonPrefs.GetBool("MControl", "ShowAtStartup"));

            MelonLogger.Log("Constructing Media Control Buttons: 5/5");

            NextButton = GameObject.Instantiate(BaseButton, parentMenu, true);
            var nxbrt = NextButton.GetComponent<RectTransform>();
            nxbrt.localScale = nxbrt.localScale / 1.5f;
            NextButton.GetComponent<Image>().sprite = Buttons.bigNextButton;
            NextButton.GetComponent<UiTooltip>().text = "Go to the next song in your Playlist";
            NextButton.GetComponent<UiTooltip>().alternateText = "Go to the next song in your Playlist";
            Component.DestroyImmediate(NextButton.GetComponentInChildren<Text>());
            NextButtonButton = NextButton.GetComponent<Button>();
            NextButtonButton.name = "MC_NextSong";
            NextButtonButton.onClick = new Button.ButtonClickedEvent();
            NextButtonButton.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(MediaControl.NextTrack)));
            NextButton.SetActive(MelonPrefs.GetBool("MControl", "ShowAtStartup"));

            if (MelonPrefs.GetBool("MControl", "MoveMediaVRCPlus")) {
                prbrt.localPosition = new Vector2(-420f, 2240f);
                plbrt.localPosition = new Vector2(-140f, 2240f);
                stbrt.localPosition = new Vector2(140f, 2240f);
                nxbrt.localPosition = new Vector2(420f, 2240f);
            } else {
                prbrt.localPosition = new Vector2(-140f, 2240f);
                plbrt.localPosition = new Vector2(140f, 2240f);
                stbrt.localPosition = new Vector2(420f, 2240f);
                nxbrt.localPosition = new Vector2(700f, 2240f);
            }

            MelonLogger.Log("Constructing Media Control Buttons: Done! Halting Init Coroutine.");
            
            yield break;
        }
        public override void OnModSettingsApplied() {
            if (MelonPrefs.GetBool("MControl", "MoveMediaVRCPlus")) {
                PrevButton.GetComponent<RectTransform>().localPosition = new Vector2(-420f, 2240f);
                PlayButton.GetComponent<RectTransform>().localPosition = new Vector2(-140f, 2240f);
                StopButton.GetComponent<RectTransform>().localPosition = new Vector2(140f, 2240f);
                NextButton.GetComponent<RectTransform>().localPosition = new Vector2(420f, 2240f);
            } else {
                PrevButton.GetComponent<RectTransform>().localPosition = new Vector2(-140f, 2240f);
                PlayButton.GetComponent<RectTransform>().localPosition = new Vector2(140f, 2240f);
                StopButton.GetComponent<RectTransform>().localPosition = new Vector2(420f, 2240f);
                NextButton.GetComponent<RectTransform>().localPosition = new Vector2(700f, 2240f);
            }
        }
        private void ToggleButtons() {
            PrevButton.SetActive(!PrevButton.activeSelf);
            PlayButton.SetActive(!PlayButton.activeSelf);
            StopButton.SetActive(!StopButton.activeSelf);
            NextButton.SetActive(!NextButton.activeSelf);
        }
        private static ResourceBundle Buttons;
        private static GameObject BaseButton;
        private static Transform parentMenu;
        private static GameObject ShowButton;
        private static Button ShowButtonButton;
        private static GameObject PrevButton;
        private static Button PrevButtonButton;
        private static GameObject PlayButton;
        private static Button PlayButtonButton;
        private static GameObject StopButton;
        private static Button StopButtonButton;
        private static GameObject NextButton;
        private static Button NextButtonButton;
    }
}
