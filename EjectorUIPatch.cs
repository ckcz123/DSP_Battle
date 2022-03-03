using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class EjectorUIPatch
    {
        public static int curEjectorPlanetId = -1;
        public static int curEjectorEntityId = -1;
        public static bool needToRefreshTarget = false;
        public static bool curEjectorIsCannon = false;
        public static EnemyShip curTarget = null;

        public static Text SolarSailAmoutLabel = null; //原本显示的是太阳帆总数的标题，现在要改成“剩余生命值”之类的
        public static Text EjectCycleLabel = null; //原本是显示弹射周期的标题，现在要改成“射速”字样

        public static GameObject orbitalPickerObj = null;
        public static GameObject remainEnemyShipsLabelObj = null;
        public static GameObject remainEnemyShipsValueObj = null;
        public static Text remainEnemyShipsLabel = null;
        public static Text remainEnemyShipsValue = null;

        public static void InitGameObjects()
        {
            try
            {
                GameObject SolarSailAmoutLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/detail-texts/label (5)");
                SolarSailAmoutLabel = SolarSailAmoutLabelObj.GetComponent<Text>();
                GameObject EjectCycleLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/detail-texts/label (4)");
                EjectCycleLabel = EjectCycleLabelObj.GetComponent<Text>();

                //下面初始化一组显示剩余敌机总数的UI
                GameObject parent = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window");
                GameObject EjectCycleValueObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/detail-texts/value (4)");
                remainEnemyShipsLabelObj = GameObject.Instantiate(EjectCycleLabelObj);
                remainEnemyShipsLabelObj.name = "remain-enemy-label";
                remainEnemyShipsLabelObj.transform.SetParent(parent.transform, false);
                remainEnemyShipsLabelObj.transform.localPosition = new Vector3(-170, -113, 0);
                remainEnemyShipsLabel = remainEnemyShipsLabelObj.GetComponent<Text>();

                remainEnemyShipsValueObj = GameObject.Instantiate(EjectCycleValueObj);
                remainEnemyShipsValueObj.name = "remain-enemy-value";
                remainEnemyShipsValueObj.transform.SetParent(parent.transform, false);
                remainEnemyShipsValueObj.transform.localPosition = new Vector3(-15, -113, 0);
                remainEnemyShipsValue = remainEnemyShipsValueObj.GetComponent<Text>();


                orbitalPickerObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/orbit-picker");
            }
            catch (Exception)
            {
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEjectorWindow), "OnEjectorIdChange")]
        public static void OnEjectorIdChangePostPatch(UIEjectorWindow __instance)
        {
            ShareUICurEjectorInfos(__instance);
        }


        public static void ShareUICurEjectorInfos(UIEjectorWindow __instance)
        {
            if(SolarSailAmoutLabel == null)
            {
                InitGameObjects();
            }
            try
            {
                curEjectorIsCannon = true;
                EjectorComponent ejectorComponent = __instance.factorySystem.ejectorPool[__instance.ejectorId];
                int planetId = ejectorComponent.planetId;
                PlanetFactory factory = GameMain.galaxy.stars[planetId / 100 - 1].planets[planetId % 100 - 1].factory;
                int gmProtoId = factory.entityPool[ejectorComponent.entityId].protoId;
                if (gmProtoId == 2311) //原版弹射器不进行修改
                {
                    curTarget = null;
                    curEjectorIsCannon = false;
                    return;
                }
                curEjectorPlanetId = ejectorComponent.planetId;
                curEjectorEntityId = ejectorComponent.entityId; //二者均相符时，代表是同一个建筑
                needToRefreshTarget = true;//在ejector选择目标时，如果needToRefreshTarget，则将选择的目标刷新传递过来，供UI显示所需属性，同时将此项设置为false，不重复刷新
            }
            catch (Exception)
            {
                curEjectorIsCannon = false;
            }
            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEjectorWindow), "_OnUpdate")]
        public static void DisplayTargetShipHp(ref UIEjectorWindow __instance)
        {
            if (curEjectorIsCannon)
            {
                if (curTarget != null && curTarget.state == EnemyShip.State.active)// && EnemyShips.ships.ContainsKey(curTarget.shipIndex)
                {
                    SolarSailAmoutLabel.text = "目标生命值".Translate();
                    __instance.valueText6.text = curTarget.hp.ToString();
                }
                else
                {
                    curTarget = null;
                    needToRefreshTarget = true;
                    SolarSailAmoutLabel.text = "无攻击目标".Translate();
                    __instance.valueText6.text = "-";
                }
                orbitalPickerObj.SetActive(false);
                EjectCycleLabel.text = "射速".Translate();
                remainEnemyShipsLabel.text = "剩余敌舰".Translate();
                remainEnemyShipsValue.text = EnemyShips.ships.Count.ToString();
            }
            else
            {
                SolarSailAmoutLabel.text = "太阳帆总数".Translate();
                orbitalPickerObj.SetActive(false);
                EjectCycleLabel.text = "弹射周期".Translate();
                remainEnemyShipsLabel.text = "";
                remainEnemyShipsValue.text = "";
            }
        }


    }
}
