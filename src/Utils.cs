using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace DSP_Battle
{
    public class Utils
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> randSeed =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        static Random rd = new Random();

        public static int Rand()
        {
            return randSeed.Value.Next();
        }


        public static VectorLF3 RandPosDelta()
        {
            return new VectorLF3(randSeed.Value.NextDouble() - 0.5, randSeed.Value.NextDouble() - 0.5, randSeed.Value.NextDouble() - 0.5);
        }

        public static VectorLF3 RandPosDelta(ref int Seed)
        {
            System.Random rand = new System.Random(Seed);
            Seed = RandNext();
            return new VectorLF3(rand.NextDouble() - 0.5, rand.NextDouble() - 0.5, rand.NextDouble() - 0.5);
        }

        public static int RandInt(int min, int max)
        {
            return randSeed.Value.Next(min, max);
        }

        public static int RandNext()
        {
            return randSeed.Value.Next();
        }

        public static double RandDouble()
        {
            return randSeed.Value.NextDouble();
        }


        public static double RandDoubleBySeedDelta(int delta)
        {
            return RandDouble();
        }

        public static int RandIntBySeedDelta(int min, int max, int delta)
        {
            return RandInt(min, max);
        }

        public static void Check(int num = 0, string str = "check ")
        {
            DspBattlePlugin.logger.LogInfo(str + num.ToString());
        }

        public static void Log(string str, int perFrame = 1, int isWarning = 0)
        {
            if (GameMain.instance.timei % perFrame == 0)
            {
                if (isWarning > 0)
                {
                    DspBattlePlugin.logger.LogWarning(str);
                }
                else
                {
                    DspBattlePlugin.logger.LogInfo(str);
                }
            }
        }

        public static string KMGFormat(long num)
        {
            if(num >= 1000000000000000)
            {
                return (num * 1.0 / 1000000000000000).ToString("G3") + " P";
            }
            else if(num >= 1000000000000)
            {
                return (num * 1.0 / 1000000000000).ToString("G3") + " T";
            }
            else if (num >= 1000000000)
            {
                return (num * 1.0 / 1000000000).ToString("G3") + " G";
            }
            else if (num >= 1000000)
            {
                return (num * 1.0 / 1000000).ToString("G3") + " M";
            }
            else if (num >= 1000)
            {
                return (num * 1.0 / 1000).ToString("G3") + " k";
            }
            else
            {
                return num.ToString();
            }
        }

        public static void UIItemUp(int itemId, int upCount, float forceWidth = 300, int forceItemCount = -1)
        {
            if (GameMain.mainPlayer == null)
            {
                return;
            }
            if (UIRoot.instance == null)
            {
                return;
            }
            if (upCount <= 0)
            {
                return;
            }
            UIItemup itemupTips = UIRoot.instance.uiGame.itemupTips;
            int itemCount = GameMain.mainPlayer.package.GetItemCount(itemId);
            if (forceItemCount >= 0) itemCount = forceItemCount;
            bool flag;
            UIItemupNode uiitemupNode = itemupTips.CreateNode(itemId, out flag);
            SetItemupNodeData(ref uiitemupNode, itemId, upCount, itemCount - upCount, itemCount, forceWidth);
            //uiitemupNode.SetData(itemId, upCount, itemCount - upCount, itemCount);
            if (!flag)
            {
                itemupTips.itemups.Insert(0, uiitemupNode);
                UIRoot.instance.uiGame.itemupTips.transform.SetAsLastSibling();
            }
            if (!itemupTips.active)
            {
                itemupTips._Open();
            }
        }

        public static void SetItemupNodeData(ref UIItemupNode _this, int _itemId, int _getCnt, int _prevCnt, int _totalCnt, float forceWidth)
        {
            if (_this.fadeOutCalled)
            {
                Assert.CannotBeReached();
                return;
            }
            bool flag = _itemId != _this.itemId;
            _this.itemId = _itemId;
            _this.getCount += _getCnt;
            _this.prevCount = _prevCnt;
            _this.totalCount = _totalCnt;
            _this.displayCount = (float)_this.prevCount;
            ItemProto itemProto = LDB.items.Select(_itemId);
            if (itemProto == null)
            {
                _this._Close();
                return;
            }

            _this.itemIconImage.sprite = itemProto.iconSprite;
            _this.getNumText.text = "+ " + _this.getCount.ToString();
            _this.itemNameText.text = itemProto.name;
            if (_this.displayCount > 0f)
            {
                _this.totalNumText.text = _this.displayCount.ToString("#,##0");
            }
            else
            {
                _this.totalNumText.text = "0";
            }
            _this.sizeTween.to = new Vector2(forceWidth, _this.sizeTween.to.y);

            if (flag)
            {
                for (int i = 0; i < _this.tweeners.Length; i++)
                {
                    _this.tweeners[i].Play0To1();
                }
            }
            else
            {
                _this.tweeners[1].Play0To1();
                _this.tweeners[1].normalizedTime = 0.3f;
                _this.tweeners[2].Play0To1();
                _this.tweeners[2].normalizedTime = 0.5f;
            }
            _this.time = (flag ? 0f : 0.3f);
        }

    }
}
