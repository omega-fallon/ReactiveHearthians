using HarmonyLib;
using HugMod;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ReactiveHearthians
{
    public class BlackHoleTracker : MonoBehaviour
    {
        public static BlackHoleTracker Instance;
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            GlobalMessenger.AddListener("PlayerEnterBlackHole", OnEnterBlackHole);
        }

        public void OnEnterBlackHole()
        {
            if (ReactiveHearthians.Instance.loadedScene == "vanilla")
            {
                float dist = Vector3.Distance(Locator.GetPlayerBody().GetPosition(), GameObject.Find("BrittleHollow_Body").transform.position);
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Distance to Brittle Hollow's center: "+dist.ToString(), MessageType.Success);
                if (dist <= 75)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Warped from Brittle Hollow to White Hole.", MessageType.Success);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_BH_TO_WH_WARP_THISLOOP", true);
                }
            }
        }

        [HarmonyPatch]
        public class MyPatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(NomaiWarpPlatform), nameof(NomaiWarpPlatform.ReceiveWarpedBody))]
            public static void NomaiWarpPlatform_Postfix(NomaiWarpPlatform __instance)
            {
                if (__instance.GetFrequency().ToString() == "BrittleHollowPolar")
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Warped from White Hole Station to Brittle Hollow.", MessageType.Success);
                    if (DialogueConditionManager.SharedInstance.GetConditionState("RH_BH_TO_WH_WARP_THISLOOP"))
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Round trip from Brittle Hollow complete!", MessageType.Success);
                        PlayerData.SetPersistentCondition("RH_SUCCESSFUL_RETURN_TO_BH", true);
                    }
                }
            }
        }
    }
}
