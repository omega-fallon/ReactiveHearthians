using HarmonyLib;
using HugMod;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ReactiveHearthians
{
    public class ChertMusicSwapper : MonoBehaviour
    {
        public static ChertMusicSwapper Instance;
        public void Awake()
        {
            Instance = this;
        }

        // Audio
        public AudioClip ChertMusic_3;
        public AudioClip ChertMusic_4;
        public bool Chert3Swap_Done;
        public bool Chert4Swap_Done;

        public void Update()
        {
            if (ReactiveHearthians.Instance.loadedScene == "vanilla")
            {
                // Audio swapping
                var audioTable = Locator.GetAudioManager()._audioLibraryDict;

                if (Chert4Swap_Done == false && TimeLoop.GetMinutesElapsed() >= 20.5f)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert's music changed to version 4.", OWML.Common.MessageType.Success);
                    Chert4Swap_Done = true;
                    audioTable[(int)AudioType.TravelerChert] = new AudioLibrary.AudioEntry(AudioType.TravelerChert, new[] { ChertMusic_4 });
                }
                else if (Chert3Swap_Done == false && TimeLoop.GetMinutesElapsed() >= 17f)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert's music changed to version 3.", OWML.Common.MessageType.Success);
                    Chert3Swap_Done = true;
                    audioTable[(int)AudioType.TravelerChert] = new AudioLibrary.AudioEntry(AudioType.TravelerChert, new[] { ChertMusic_3 });
                }
                else if (TimeLoop.GetMinutesElapsed() >= 11f)
                {
                    // Nothing
                }
                else
                {
                    // Nothing
                }
            }
        }

        public void Start()
        {
            // Getting Chert's custom music files
            ChertMusic_3 = ReactiveHearthians.Instance.ModHelper.Assets.GetAudio("planets/music/Chert_UpSemitone.wav");
            ChertMusic_4 = ReactiveHearthians.Instance.ModHelper.Assets.GetAudio("planets/music/Chert_DownSemitone.wav");

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                Chert4Swap_Done = false;
                Chert3Swap_Done = false;
            };
        }
    }
}
