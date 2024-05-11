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
    public class SlideReelBurning : MonoBehaviour
    {
        public static SlideReelBurning Instance;
        public void Awake()
        {
            Instance = this;
        }

        [HarmonyPatch]
        public class MyPatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Campfire), nameof(Campfire.OnPressInteract))]
            public static void CampfireOnPressInteract_Prefix(Campfire __instance)
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Campfire interacted with.", MessageType.Success);
                if (__instance._state != Campfire.State.LIT)
                {
                    // pass
                }
                else if (__instance._dropSlideReelMode)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Slide reel burned at name ("+__instance.ToString()+")", MessageType.Success);
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Slide reel burned at position (" + __instance.transform.position.ToString() + ")", MessageType.Success);

                    float chertDist = Vector3.Distance(__instance.transform.position, HugModStuff.Instance.Chert_Standard.transform.position);

                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("ChertDist is "+chertDist.ToString(), MessageType.Success);

                    if (chertDist <= 15)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Slide reel burned near Chert.", MessageType.Success);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_SLIDEREEL_BURNED", true);
                    }
                }
            }
        }
    }
}
