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
        
  //      public static int musicState = 0; 
  //      // 1战前, 每次播放完更换一个新的战前bgm继续播放； 2交战，从头播放，反复播放循环节。3接近结束（任意一方80%消灭），从头播放，反复播放循环节。 4之前阶段的音乐不再循环，要将其播放到结尾，之后变为0，切换回游戏原本bgm。
  //      public static int musicCnt = 0;
  //      public static bool isOverriding = false;
  //      public static int currentGroup = -1;
  //      public static int lastMusic = -1; //用于fadeOut
  //      public static int currentMusic = -1;
  //      public static int nextMusic = -1;
  //      public static float fadeOutTime = 0;
  //      public static float fadeInTime = 0;
		//public static string[] index2NameMap = new string[] { "pre_1", "loop_1", "fin_1", "pre_2", "loop_2", "pre_3", "loop_3", "fin_3", "pre_4", "loop_4", "fin_4", "pre_5", "loop_5", "fin_5", "pre_6", "loop_6", "fin_6",
  //      "Track7", "Track9", "Track10", "Track39", "TrackA1", "TrackE1", "TrackU1", "TrackU2"};
  //      public static Dictionary<string, int> name2IndexMap = new Dictionary<string, int>();
		//public static AssetBundle bgmAB;
  //      public static List<AudioSource> battleMusics = new List<AudioSource>();
  //      public static List<float> musicBPM = new List<float> { 100, 176, 180, 180, 180, 200, 200 };
  //      public static int beforeBattleBgmBeginNum = 17;
  //      public static bool nextPlayFinishMusic = false;
        
         
  //      public static void InitAudioSources()
  //      {
  //          if (!Configs.enableBattleBGM) return;
  //          Transform BGMParent = GameObject.Find("Audios/BGM").transform;
  //          GameObject oriAudioSource = GameObject.Find("Audios/BGM/universe-1");
  //          bgmAB = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DSP_Battle.battlebgm"));
  //          if (bgmAB == null)
  //          {
  //              Configs.enableBattleBGM = false;
  //              return;
  //          }
  //          musicCnt = index2NameMap.Length;

  //          for (int i = 0; i < index2NameMap.Length; i++)
  //          {
		//		GameObject battleTestObj = GameObject.Instantiate(oriAudioSource, BGMParent);
  //              battleTestObj.name = index2NameMap[i];
  //              AudioSource bgm = battleTestObj.GetComponent<AudioSource>();
  //              bgm.clip = bgmAB.LoadAsset<AudioClip>(index2NameMap[i]);
  //              name2IndexMap.Add(index2NameMap[i], i);
  //              if(index2NameMap[i] == "fin_1")
  //                  name2IndexMap.Add("fin_2", i);
  //              battleMusics.Add(bgm);
  //          }

		//}

  //      public static void InitWhenLoad()
  //      {
  //          if (!Configs.enableBattleBGM) return;
  //          if (currentMusic >= 0 && currentMusic < musicCnt)
  //              battleMusics[currentMusic].Stop();
  //          if (lastMusic >= 0 && lastMusic < musicCnt)
  //              battleMusics[lastMusic].Stop();

  //          currentGroup = -1;
  //          lastMusic = -1;
  //          currentMusic = -1;
  //          nextMusic = -1;
  //          fadeOutTime = 0;
  //          fadeInTime = 0;
  //          isOverriding = false;
  //          nextPlayFinishMusic = false;
  //      }

  //      public static void BGMLogicUpdate()
  //      {
  //          if (!Configs.enableBattleBGM) return;

  //          //bgm监测与选择逻辑
  //          if (Configs.nextWaveState <= 1) // 战斗结束后播放
  //          {
  //              if (nextPlayFinishMusic)
  //              {
  //                  if (currentGroup < 0 || currentMusic < 0)
  //                  {
  //                      nextPlayFinishMusic = false;
  //                      nextMusic = name2IndexMap["fin_" + currentGroup.ToString()];
  //                  }
  //                  else if (atBarEnd(battleMusics[currentMusic].time,musicBPM[currentGroup])) // 尽可能在小节末尾衔接结束的bgm
  //                  {
  //                      nextPlayFinishMusic = false;
  //                      nextMusic = name2IndexMap["fin_" + currentGroup.ToString()];
  //                  }
  //              }
  //              else if (currentMusic >= 0 && battleMusics[currentMusic].time >= battleMusics[currentMusic].clip.length - 0.05f) // 战斗结束的音乐播放完毕，初始化
  //              {
  //                  lastMusic = -1;
  //                  currentMusic = -1;
  //                  nextMusic = -1;
  //                  fadeOutTime = 0;
  //                  fadeInTime = 0;
  //                  isOverriding = false;
  //                  BGMController.Playback(0, 1, 1);
  //              }
  //          }
  //          else if (Configs.nextWaveState == 2) // 虫洞生成时播放
  //          {
  //              if (!isOverriding) //如果之前是游戏本身的bgm，则开始战斗bgm时随机一个group
  //              {
  //                  currentGroup = Utils.RandInt(1, 7);
  //              }
  //              if (currentMusic < 0) //如果没在播放bgm
  //              {
  //                  nextMusic = Utils.RandInt(beforeBattleBgmBeginNum, battleMusics.Count);
  //              }
  //              else if (battleMusics[currentMusic].time >= battleMusics[currentMusic].clip.length - 3f) // 如果在播放，且播放完了，换随机的下一首
  //              {
  //                  nextMusic = Utils.RandInt(beforeBattleBgmBeginNum, battleMusics.Count);
  //                  if (nextMusic == currentMusic)
  //                  {
  //                      nextMusic = currentMusic + 1 < musicCnt ? currentMusic + 1 : currentMusic - 1;
  //                  }
  //              }
  //          }
  //          else if (Configs.nextWaveState == 3) // 在战斗中
  //          {
  //              if (currentGroup < 0)
  //              {
  //                  System.Random rand = new System.Random();
  //                  currentGroup = rand.Next(1, 7);
  //              }
  //              if (currentMusic >= beforeBattleBgmBeginNum || currentMusic < 0) // 当前播放的是虫洞刷新的bgm，或者是没播放。则下一个播放战斗中的循环节之前的音频
  //              {
  //                  nextMusic = name2IndexMap["pre_" + currentGroup.ToString()];
  //              }
  //              else if (index2NameMap[currentMusic][0] == 'p' && battleMusics[currentMusic].time >= battleMusics[currentMusic].clip.length - 0.05f) // 已处在播放战斗中的音频中， 如果循环节之前的音频播放完毕，则跳转到循环节
  //              {
  //                  nextMusic = name2IndexMap["loop_" + currentGroup.ToString()];
  //              }
  //          }

  //          //启动或更新bgm
  //          if (currentMusic != nextMusic)
  //              PlayNext();

  //          //bgm淡入淡出逻辑
  //          if(isOverriding)
  //          {
  //              if (fadeOutTime > 0 && lastMusic >= 0 && lastMusic < musicCnt)
  //              {
  //                  battleMusics[lastMusic].volume -= 0.01666667f / fadeOutTime;
  //                  if (battleMusics[lastMusic].volume <= 0)
  //                  {
  //                      fadeOutTime = 0;
  //                      battleMusics[lastMusic].volume = 0;
  //                      battleMusics[lastMusic].loop = false;
  //                  }
  //              }
  //              else if (fadeOutTime <= 0 && lastMusic >= 0 && lastMusic < musicCnt)
  //              {
  //                  battleMusics[lastMusic].volume = 0;
  //                  battleMusics[lastMusic].loop = false;
  //                  battleMusics[lastMusic].Stop();
  //              }

  //              if (fadeInTime > 0 && currentMusic >= 0 && currentMusic < musicCnt)
  //              {
  //                  battleMusics[currentMusic].volume += 0.01666667f / fadeInTime;
  //                  if (battleMusics[currentMusic].volume >= VFAudio.audioVolume * VFAudio.musicVolume * 0.8f)
  //                  {
  //                      fadeOutTime = 0;
  //                      battleMusics[currentMusic].volume = VFAudio.audioVolume * VFAudio.musicVolume * 0.8f;
  //                  }
  //              }
  //              else if(fadeInTime <= 0 && currentMusic >= 0 && currentMusic < musicCnt)
  //              {
  //                  AudioSource cur = battleMusics[currentMusic];
  //                  cur.volume = VFAudio.audioVolume * VFAudio.musicVolume * 0.8f;
  //              }
  //          }
  //      }

  //      [HarmonyPostfix]
  //      [HarmonyPatch(typeof(GameData), "GameTick")]
  //      public static void GameData_GameTick(ref GameData __instance, long time)
  //      {
  //          if (time % 5 == 0 && isOverriding) MuteGameOriBgm();
  //      }




		//[HarmonyPostfix]
		//[HarmonyPatch(typeof(BGMController), "Playback")]
		//public static void PlaybackPostPatch(int bgmIndex, float fadeOutTime, float fadeInTime, EPlaybackOrigin origin = EPlaybackOrigin.Begin, float offset = 0f)
		//{
  //          if (Configs.enableBattleBGM && isOverriding)
  //          {
  //              if (!BGMController.HasBGM(bgmIndex))
  //              {
  //                  bgmIndex = 0;
  //              }
  //              if(BGMController.instance!=null && BGMController.instance.musics!=null && bgmIndex>=0 && bgmIndex< BGMController.instance.musics.Length && BGMController.instance.musics[bgmIndex]!=null)
  //                  BGMController.instance.musics[bgmIndex].Stop();
  //          }
		//}

  //      [HarmonyPostfix]
  //      [HarmonyPatch(typeof(BGMController), "UpdateLogic")]
  //      public static void BGMPassFilterPatch()
  //      {
  //          if (Configs.enableBattleBGM && isOverriding && UIGame.viewMode == EViewMode.Starmap)
  //              VFListener.SetPassFilter(100);
  //      }

  //      public static void PlayNext(float fade = 0f)
  //      {
  //          if (nextMusic >= 0 && nextMusic < musicCnt)
  //          {
  //              //如果之前是原本游戏的BGM，设置淡出
  //              if (lastMusic < 0 && BGMController.instance != null && BGMController.instance.musics != null && BGMController.HasBGM(BGMController.instance.playbackIndex))
  //              {
  //                  BGMController.instance.fadeOutTimes[BGMController.instance.playbackIndex] = 1f;
  //                  BGMController.instance.volsTar[BGMController.instance.playbackIndex] = 0;
  //              }

  //              fadeOutTime = fade;
  //              fadeInTime = fade;
  //              isOverriding = true;
  //              lastMusic = currentMusic;
  //              currentMusic = nextMusic;
  //              AudioSource bgm = battleMusics[nextMusic];
  //              bgm.time = currentMusic>=beforeBattleBgmBeginNum? 0.3f:0;
  //              bgm.Play();
  //              bgm.loop = index2NameMap[currentMusic][0] == 'l';
  //              bgm.volume = 0;
  //          }
  //      }

  //      public static void SetWaveFinished()
  //      {
  //          if(currentMusic>=0 && currentMusic<musicCnt)
  //          {
  //              battleMusics[currentMusic].loop = false;
  //          }
  //          nextPlayFinishMusic = true;
  //      }

  //      public static void MuteGameOriBgm()
  //      {
  //          if (lastMusic < 0 && BGMController.instance != null && BGMController.instance.musics != null && BGMController.HasBGM(BGMController.instance.playbackIndex))
  //          {
  //              BGMController.instance.fadeOutTimes[BGMController.instance.playbackIndex] = 0f;
  //              BGMController.instance.volsTar[BGMController.instance.playbackIndex] = 0;
  //          }
  //      }

  //      public static bool atBarEnd(float time, float bpm)
  //      {
  //          float barLength = 60f / bpm * 4;
  //          float dif = time / barLength - (int)(time / barLength);
  //          return (dif <= 0.02f || dif >= barLength - 0.03f);
  //      }
        
    }
}
