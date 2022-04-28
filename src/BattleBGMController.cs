using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;


namespace DSP_Battle
{
    public class BattleBGMController
    {
        /*
        public static int musicState = 0; 
        // 1战前, 每次播放完更换一个新的战前bgm继续播放； 2交战，从头播放，反复播放循环节。3接近结束（任意一方80%消灭），从头播放，反复播放循环节。 4之前阶段的音乐不再循环，要将其播放到结尾，之后变为0，切换回游戏原本bgm。
        public static int musicCnt = 0;
        public static bool isOverriding = false;
        public static int currentGroup = -1;
        public static int lastMusic = -1; //用于fadeOut
        public static int currentMusic = -1;
        public static int nextMusic = -1;
        public static float fadeOutTime = 0;
        public static float fadeInTime = 0;
		public static string[] index2NameMap = new string[] { "Track 17" };
        public static Dictionary<string, int> name2IndexMap = new Dictionary<string, int>();
		public static AssetBundle bgmAB;
        public static List<AudioSource> battleMusics = new List<AudioSource>();
        public static List<float> musicLoopBegin = new List<float> { 5.11f };
        public static List<float> musicLoopEnd = new List<float> { 101.1f };
        
//DSP_Battle.BattleBGMController.musicLoopBegin[0] = 5.21f;
//DSP_Battle.BattleBGMController.musicLoopEnd[0] = 101.2f;
         
        public static void InitAudioSources()
        {
            if (!Configs.enableBattleBGM) return;
            Transform BGMParent = GameObject.Find("Audios/BGM").transform;
            GameObject oriAudioSource = GameObject.Find("Audios/BGM/universe-1");
			bgmAB = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DSP_Battle.battlebgm"));
            if(bgmAB == null)
            {
                Configs.enableBattleBGM = false;
                return;
            }
            musicCnt = index2NameMap.Length;

            for (int i = 0; i < index2NameMap.Length; i++)
            {
				GameObject battleTestObj = GameObject.Instantiate(oriAudioSource, BGMParent);
                battleTestObj.name = index2NameMap[i];
                AudioSource bgm = battleTestObj.GetComponent<AudioSource>();
				bgm.clip = bgmAB.LoadAsset<AudioClip>(index2NameMap[i]);
                name2IndexMap.Add(index2NameMap[i], i);
				battleMusics.Add(bgm);
			}

		}

        public static void InitWhenLoad()
        {
            if (!Configs.enableBattleBGM) return;
            if (currentMusic >= 0 && currentMusic < musicCnt)
                battleMusics[currentMusic].Stop();
            if (lastMusic >= 0 && lastMusic < musicCnt)
                battleMusics[lastMusic].Stop();

            currentGroup = -1;
            lastMusic = -1;
            currentMusic = -1;
            nextMusic = -1;
            fadeOutTime = 0;
            fadeInTime = 0;
            isOverriding = false;
        }

        public static void BGMLogicUpdate()
        {
            if (!Configs.enableBattleBGM) return;
            //bgm循环与终止逻辑
            if (isOverriding) 
            {
                if (currentMusic >= 0 && currentMusic < battleMusics.Count && currentMusic == nextMusic)
                {
                    if (Configs.nextWaveState > 0 && Configs.nextWaveFrameIndex - GameMain.instance.timei <= 3600 * 5 && battleMusics[currentMusic].time >= musicLoopEnd[currentMusic])
                    {
                        LoopCurrent();
                    }
                    //如果bgm没有被强制从中间循环，那么设定bgm结束
                    else if (Configs.nextWaveState<=2 && battleMusics[currentMusic].time >= battleMusics[currentMusic].clip.length - 0.05f ) 
                    {
                        if(lastMusic>=0 && lastMusic<musicCnt)
                            battleMusics[lastMusic].Stop();

                        
                        battleMusics[currentMusic].Stop();

                        lastMusic = -1;
                        currentMusic = -1;
                        nextMusic = -1;
                        fadeOutTime = 0;
                        fadeInTime = 0;

                        //如果此时也没在战斗状态，虫洞也还没刷新，则彻底退出战斗bgm状态，并且播放游戏原本的bgm
                        if (Configs.nextWaveState <= 1 && BGMController.instance!=null) 
                        {
                            currentGroup = -1;
                            isOverriding = false;
                            BGMController.Playback(0, 1, 1);
                        }
                    }
                }
            }
            
            //bgm监测与选择逻辑
            if (Configs.nextWaveState > 0 && Configs.nextWaveFrameIndex - GameMain.instance.timei <= 3600*5)
            {
                if(!isOverriding) //如果之前是游戏本身的bgm，则开始战斗bgm时随机一个group
                {
                    currentGroup = Utils.RandInt(0, 3);
                }
                if (currentMusic < 0) //如果没在播放bgm
                {
                    nextMusic = 0; //需要更改
                }
            }

            //启动或更新bgm
            if (currentMusic != nextMusic)
                PlayNext();

            //bgm淡入淡出逻辑
            if(isOverriding)
            {
                if (fadeOutTime > 0 && lastMusic >= 0 && lastMusic < musicCnt)
                {
                    battleMusics[lastMusic].volume -= 0.01666667f / fadeOutTime;
                    if (battleMusics[lastMusic].volume <= 0)
                    {
                        fadeOutTime = 0;
                        battleMusics[lastMusic].volume = 0;
                        battleMusics[lastMusic].loop = false;
                    }
                }
                else if (fadeOutTime <= 0 && lastMusic >= 0 && lastMusic < musicCnt)
                {
                    battleMusics[lastMusic].volume = 0;
                    battleMusics[lastMusic].loop = false;
                    battleMusics[lastMusic].Stop();
                }

                if (fadeInTime > 0 && currentMusic >= 0 && currentMusic < musicCnt)
                {
                    battleMusics[currentMusic].volume += 0.01666667f / fadeInTime;
                    if (battleMusics[currentMusic].volume >= VFAudio.audioVolume * VFAudio.musicVolume)
                    {
                        fadeOutTime = 0;
                        battleMusics[currentMusic].volume = VFAudio.audioVolume * VFAudio.musicVolume;
                    }
                }
                else if(fadeInTime <= 0 && currentMusic >= 0 && currentMusic < musicCnt)
                {
                    AudioSource cur = battleMusics[currentMusic];
                    cur.volume = VFAudio.audioVolume * VFAudio.musicVolume;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {

        }




		[HarmonyPostfix]
		[HarmonyPatch(typeof(BGMController), "Playback")]
		public static void PlaybackPostPatch(int bgmIndex, float fadeOutTime, float fadeInTime, EPlaybackOrigin origin = EPlaybackOrigin.Begin, float offset = 0f)
		{
            if (Configs.enableBattleBGM && isOverriding)
            {
                if (!BGMController.HasBGM(bgmIndex))
                {
                    bgmIndex = 0;
                }
                if(BGMController.instance!=null && BGMController.instance.musics!=null && bgmIndex>=0 && bgmIndex< BGMController.instance.musics.Length && BGMController.instance.musics[bgmIndex]!=null)
                    BGMController.instance.musics[bgmIndex].Stop();
            }
		}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BGMController), "UpdateLogic")]
        public static void BGMPassFilterPatch()
        {
            if (Configs.enableBattleBGM && isOverriding && UIGame.viewMode == EViewMode.Starmap)
                VFListener.SetPassFilter(100);
        }

        public static void PlayNext()
        {
            if (nextMusic >= 0 && nextMusic < musicCnt)
            {
                //如果之前是原本游戏的BGM，设置淡出
                if (lastMusic < 0 && BGMController.instance != null && BGMController.instance.musics != null && BGMController.HasBGM(BGMController.instance.playbackIndex))
                {
                    BGMController.instance.fadeOutTimes[BGMController.instance.playbackIndex] = 1f;
                    BGMController.instance.volsTar[BGMController.instance.playbackIndex] = 0;
                }

                fadeOutTime = 0.2f;
                fadeInTime = 0.2f;
                isOverriding = true;
                lastMusic = currentMusic;
                currentMusic = nextMusic;
                AudioSource bgm = battleMusics[nextMusic];
                bgm.time = 0;
                bgm.Play();
                bgm.loop = false;
                bgm.volume = 0;
            }
        }

        public static void LoopCurrent()
        {
            if (currentMusic >= 0 && currentMusic < musicCnt)
            {
                AudioSource bgm = battleMusics[currentMusic];
                bgm.time = musicLoopBegin[currentMusic];
                bgm.Play();
                bgm.loop = false;
            }
        }

        public static void SetWaveFinished()
        {
            if(currentMusic>=0 && currentMusic<musicCnt)
            {
                battleMusics[currentMusic].loop = false;
            }
        }
        */
    }
}
