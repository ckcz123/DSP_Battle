using HarmonyLib;
using MoreMegaStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    public class UIEscMenuPatch
    {
        public static GameObject EnableVoidInvasionButtonObj = null;
        public static UIButton EnableVoidInvasionUIBtn;
        public static Text EnableVoidInvasionText;
        public static string mainMenuLogoObjPath = "UI Root/Overlay Canvas/Main Menu/dsp-logo"; // RawImage
        public static string escMenuLogoObjPath = "UI Root/Overlay Canvas/In Game/Esc Menu/logo"; // RawImage

        public static void Init()
        {
            if(EnableVoidInvasionButtonObj == null && Configs.enableVoidInvasionUpdate)
            {
                GameObject oriButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Esc Menu/button-group/button (1)");
                GameObject parentObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Esc Menu/button-group");
                EnableVoidInvasionButtonObj = GameObject.Instantiate(oriButton, parentObj.transform);
                EnableVoidInvasionButtonObj.name = "button (7)";
                EnableVoidInvasionButtonObj.transform.localScale = Vector3.one;
                EnableVoidInvasionButtonObj.transform.localPosition = new Vector3(0, 366, 0);
                EnableVoidInvasionText = EnableVoidInvasionButtonObj.GetComponentInChildren<Text>();
                EnableVoidInvasionText.text = "开启虚空入侵".Translate();

                //EnableVoidInvasionButtonObj.RebindButtonOnClick(() => { AssaultController.TryEnableVoidInvasion(); });
                EnableVoidInvasionButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                EnableVoidInvasionButtonObj.GetComponent<Button>().onClick.AddListener(AssaultController.TryEnableVoidInvasion);

                EnableVoidInvasionUIBtn = EnableVoidInvasionButtonObj.GetComponent<UIButton>();
                EnableVoidInvasionUIBtn.button = EnableVoidInvasionButtonObj.GetComponent<Button>();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEscMenu), "_OnUpdate")]
        public static void UIEscMenuOnUpdate()
        {
            if (Configs.enableVoidInvasionUpdate)
            {
                if (!GameMain.data.gameDesc.isCombatMode)
                {
                    EnableVoidInvasionUIBtn.gameObject.SetActive(false);
                    //EnableVoidInvasionText.text = "非黑雾模式".Translate();
                    //EnableVoidInvasionUIBtn.button.interactable = false;
                }
                else if (AssaultController.voidInvasionEnabled)
                {
                    EnableVoidInvasionUIBtn.gameObject.SetActive(true);
                    EnableVoidInvasionText.text = "已开启虚空入侵".Translate();
                    EnableVoidInvasionUIBtn.button.interactable = false;
                }
                else
                {
                    EnableVoidInvasionUIBtn.gameObject.SetActive(true);
                    EnableVoidInvasionText.text = "开启虚空入侵".Translate();
                    EnableVoidInvasionUIBtn.button.interactable = true;
                }
            }
        }
    }
}
