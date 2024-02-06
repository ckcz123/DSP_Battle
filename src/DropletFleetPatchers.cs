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
    public class DropletFleetPatchers
    {
		public static int dropletId = 9511;

		public static UIButton[] dropletFleetTypeButtons = new UIButton[2];
		public static ECraftSize dropletSize = (ECraftSize)9;

        /// <summary>
        /// 允许水滴放入
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMechaFighterEntry), "OnFighterButtonClick")]
        public static void OnFighterButtonClickPostPatch()
        {

        }

        /// <summary>
        /// 允许水滴放入
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMechaFighterEntry), "HandPutOne")]
        public static void HandPutOnePostPatch(ref UIMechaFighterEntry __instance)
        {

        }


        /// <summary>
        /// 允许水滴放入
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModuleFleet), "AddFighterToPort")]
        public static void AddFighterToPortPostPatch(ref ModuleFleet __instance, ref bool __result, int fighterIndex, int itemId)
        {
			ref ModuleFighter ptr = ref __instance.fighters[fighterIndex];
			if (ptr.count > 0)
			{
				return;
			}
			FleetProto fleetProto = LDB.fleets.Select(__instance.protoId);
			if (fleetProto == null && __instance.protoId != 6 && __instance.protoId != 7)
			{
				__result = false;
				return;
			}
			ItemProto itemProto = LDB.items.Select(itemId);
			if (itemProto == null)
			{
				__result = false;
				return;
			}
			
			if (__instance.protoId >=6 && __instance.protoId <= 7)
			{
				if (itemId == dropletId)
				{
					ptr.itemId = itemId;
					ptr.count = 1;
					__result = true;
					return;
				}
				else
                {
					UIRealtimeTip.Popup("只能放入水滴".Translate(), true, 0);
					__result = false;
					return;
                }
			}
			
			return;
		}

		/// <summary>
		/// 为编辑队形新增（单个）水滴队形
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(UIMechaWindow), "OpenConfigPanel")]
		public static void OpenConfigPanelPostPatch(ref UIMechaWindow __instance)
		{
			var _this = __instance;
			int num = _this.mecha.groundCombatModule.moduleFleets.Length;
			if (_this.fleetConfigIndex >= num)
			{
				UIButton uibutton = UnityEngine.Object.Instantiate<UIButton>(_this.spaceFleetTypeButton, _this.spaceFleetTypeButton.transform.parent);
				int i = 4;
				uibutton.data = i;
				RectTransform rectTransform = uibutton.transform as RectTransform;
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + (float)(i % 2 * 65), rectTransform.anchoredPosition.y - (float)(i / 2 * 20));
				uibutton.gameObject.SetActive(true);
				uibutton.onClick += OnDropletFleetTypeButtonClick;
				dropletFleetTypeButtons[0] = uibutton;
				if (true)
				{
					Sprite iconSprite = Resources.Load<Sprite>("Assets/DSPBattle/dropletInFleetConfig");
					Image image = uibutton.GetComponentsInChildren<Image>()[1];
					image.sprite = iconSprite;
					image.enabled = (iconSprite != null);
				}
				_this.fleetConfigGroupRectTrans.sizeDelta = new Vector2(194, 73); // 194,54
			}
			else
            {
				_this.fleetConfigGroupRectTrans.sizeDelta = new Vector2(194, 54);
			}
		}

		/// <summary>
		/// 关闭时一并销毁obj
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(UIMechaWindow), "CloseConfigPanel")]
		public static void CloseConfigPanelPostPatch(ref UIMechaWindow __instance)
        {
			for (int i = 0; i < dropletFleetTypeButtons.Length; i++)
			{
				UIButton uibutton = dropletFleetTypeButtons[i];
				if (uibutton != null)
				{
					uibutton.onClick -= OnDropletFleetTypeButtonClick;
                    UnityEngine.Object.Destroy(uibutton.gameObject);
				}
			}
			__instance.fleetConfigGroupRectTrans.gameObject.SetActive(false);
			__instance.fleetConfigFrame = 0;
		}

		public static void OnDropletFleetTypeButtonClick(int obj)
        {
			var _this = UIRoot.instance.uiGame.mechaWindow;
			if (_this.fleetTabIndex == 1)
			{
				ChangeFleetConfigToDroplet(ref _this.mecha.spaceCombatModule, _this.fleetConfigIndex - _this.mecha.groundCombatModule.moduleFleets.Length, 6, _this.mecha.fighterStorage, _this.mecha.player);
			}
			_this.fleetConfigIndex = -1;
		}

		public static void ChangeFleetConfigToDroplet(ref CombatModuleComponent _this, int fleetIndex, int newConfigId, StorageComponent storage, Player player)
        {
			if (_this.isSpace)
			{
				ref ModuleFleet ptr = ref _this.moduleFleets[fleetIndex];
				if (ptr.protoId != newConfigId)
				{
					for (int i = 0; i < ptr.fighters.Length; i++)
					{
						if (ptr.fighters[i].count > 0)
						{
							int num = 0;
							int num2 = 0;
							ptr.TakeFighterFromPort(i, ref num, ref num2);
							if (num > 0 && num2 > 0)
							{
								int num4;
								int num3 = storage.AddItemStacked(num, num2, 0, out num4);
								num2 -= num3;
								if (num2 > 0 && _this.entityId <= 0 && _this.entityId == 0)
								{
									player.TryAddItemToPackage(num, num2, 0, true, 0, false);
								}
							}
						}
					}
					ptr = CreateDropletModuleFleet(newConfigId);
					//ptr.SetItemId(-1, ItemProto.kFighterSpaceSmallIds[0], ECraftSize.Small);
					ptr.SetItemId(-1, dropletId, ECraftSize.Large);
					return;
				}
			}
		}

		public static ModuleFleet CreateDropletModuleFleet(int newConfigId)
        {
			FleetProto fleetProto = LDB.fleets.Select(5);
			if (fleetProto == null)
			{
				return default(ModuleFleet);
			}
			PrefabDesc prefabDesc = fleetProto.prefabDesc;
			FleetPortDesc[] fleetPorts2 = prefabDesc.fleetPorts;
			FleetPortDesc fleetPortDesc = fleetPorts2[1];
			Utils.Log($"{fleetPortDesc.rowInUI}/{fleetPortDesc.colInUI}. pos x y z {fleetPortDesc.pose.position.x}/{fleetPortDesc.pose.position.y}/{fleetPortDesc.pose.position.z}. rot {fleetPortDesc.pose.rotation}");

			ModuleFleet moduleFleet;
			moduleFleet.fleetAstroId = 0;
			moduleFleet.fleetId = 0;
			moduleFleet.protoId = newConfigId;
			moduleFleet.inCommand = false;
			moduleFleet.fleetEnabled = true;
			moduleFleet.fighters = new ModuleFighter[1];
			for (int i = 0; i < 1; i++)
			{
				ModuleFighter[] array = moduleFleet.fighters;
				array[i].itemId = dropletId;
				array[i].size = dropletSize; // 需要patch一些东西，为什么不直接用large呢？因为如果是large，放入不是水滴的舰队时，AddFighterToPort返回false，不仅会在其patch里触发只能放入水滴的提示，还会在返回后错误地触发只能放入中型舰的提示。
				array[i].rowInUI = fleetPortDesc.rowInUI;
				array[i].colInUI = fleetPortDesc.colInUI;
				array[i].formDesc.formPos = fleetPortDesc.pose.position;
				array[i].formDesc.formRot = fleetPortDesc.pose.rotation;
			}
			return moduleFleet;
		}

		/// <summary>
		/// 为了让size是不在enum里面列出的small和large的情况下
		/// </summary>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(UIMechaFighterEntry), "SetTrans")]
		public static void SetTransPostPatch(ref UIMechaFighterEntry __instance)
        {
			var _this = __instance;
			_this.rectTrans.anchoredPosition = new Vector2((float)(6 + _this.fighter.colInUI * 33), (float)(-6 - _this.fighter.rowInUI * 24));
			RectTransform rectTransform = _this.fighterIconSprite.transform as RectTransform;
			ECraftSize size = _this.fighter.size;
			if (size != dropletSize)
			{
				return;
			}
			_this.rectTrans.sizeDelta = new Vector2(62f, 46f);
			rectTransform = (_this.fighterIconSprite.transform as RectTransform);
			rectTransform.anchoredPosition = new Vector2(10f, 0f);
			rectTransform.sizeDelta = new Vector2(40f, 40f);
		}
	}
}
