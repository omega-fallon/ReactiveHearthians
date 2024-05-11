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
    public class ShipBreak : MonoBehaviour
    {
        public static ShipBreak Instance;
        public void Awake()
        {
            Instance = this;
        }

        [HarmonyPatch]
        public class MyPatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ShipEjectionSystem), nameof(ShipEjectionSystem.OnPressInteract))]
            public static void ShipEject_PostFix(ShipEjectionSystem __instance)
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Eject button interacted with!", MessageType.Success);

                if (__instance._cockpitModule.isDetached)
                {
                    // pass
                }
                if (__instance._ejectPrimed) {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_SHIP_EJECTED", true);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ShipDamageController), nameof(ShipDamageController.Explode))]
            public static void ShipExplode_PostFix(ShipDamageController __instance)
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Ship has exploded!", MessageType.Success);

                // Proximity to people - fix this!
                if (Vector3.Distance(__instance.transform.position, HugModStuff.Instance.Porphy_Standard.transform.position) <= 150)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_VILLAGE_SHIP_EXPLODED", true);
                }

                // Cower code
                foreach (CowerAnimTriggerVolume cower_volume in Cowering.Instance.cower_volumes)
                {
                    if (cower_volume.gameObject.activeInHierarchy)
                    {
                        cower_volume.StartCoroutine(Cowering.Instance.Coweroutine(cower_volume._animator, 0, TimeLoop.GetSecondsElapsed()+5));
                    }
                }
            }
        }

        public void Start()
        {
            GlobalMessenger.AddListener("ShipHullBreach", OnShipHullBreach);
        }

        public void OnShipHullBreach()
        {
            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Ship hull breached!", MessageType.Success);

            Vector3 shipPosition = Locator.GetShipBody().transform.position;
            float reactRadius = 25;

            DialogueConditionManager.SharedInstance.SetConditionState("RH_SHIP_BREACH", true);

            // Chert
            if (Vector3.Distance(shipPosition, HugModStuff.Instance.Chert_Standard.transform.position) <= reactRadius)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_SHIP_BREACH", true);
            }

            // All the villagers - FIX THIS
            if (Vector3.Distance(shipPosition, HugModStuff.Instance.Porphy_Standard.transform.position) <= reactRadius)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_VILLAGE_SHIP_BREACH", true);
            }

            // Riebeck
            if (Vector3.Distance(shipPosition, HugModStuff.Instance.Riebeck_Standard.transform.position) <= reactRadius)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_SHIP_BREACH", true);
            }

            // Gabbro
            if (Vector3.Distance(shipPosition, HugModStuff.Instance.Gabbro_Standard.transform.position) <= reactRadius)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_SHIP_BREACH", true);
            }

            // Feldspar
            if (Vector3.Distance(shipPosition, HugModStuff.Instance.Feldspar_Standard.transform.position) <= reactRadius)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_SHIP_BREACH", true);
            }
        }
    }
}
