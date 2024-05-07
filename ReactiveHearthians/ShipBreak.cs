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

        public void Start()
        {
            GlobalMessenger.AddListener("ShipHullBreach", OnShipHullBreach);
        }

        public void OnShipHullBreach()
        {
            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Ship hull breached!", MessageType.Success);

            Vector3 shipPosition = Locator.GetShipBody().transform.position;
            float reactRadius = 25;

            // Chert
            if (Vector3.Distance(shipPosition, HugModStuff.Instance.Chert_Standard.transform.position) <= reactRadius)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_SHIP_BREACH", true);
            }

            // All the villagers
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
