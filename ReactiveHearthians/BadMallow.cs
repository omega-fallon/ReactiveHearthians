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
    public class BadMallow : MonoBehaviour
    {
        public static BadMallow Instance;

        public Campfire slatefire;
        public Campfire riebeckfire;
        public Campfire eskerfire;
        public Campfire chertfire;
        public Campfire feldsparfire;

        public bool slatefire_roasting;
        public bool riebeckfire_roasting;
        public bool eskerfire_roasting;
        public bool chertfire_roasting;
        public bool feldsparfire_roasting;

        public float slatefire_badmallow_lastate;
        public float riebeckfire_badmallow_lastate;
        public float eskerfire_badmallow_lastate;
        public float chertfire_badmallow_lastate;
        public float feldsparfire_badmallow_lastate;

        public List<BadMarshmallowCan> badcans;

        public void Start()
        {
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                // Makes the badmallow list
                badcans = Resources.FindObjectsOfTypeAll<BadMarshmallowCan>().ToList();

                if (loadScene == OWScene.SolarSystem)
                {
                    // Campfires people are sat near
                    slatefire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    riebeckfire = GameObject.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Interactables_Crossroads/VisibleFrom_BH/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    eskerfire = GameObject.Find("Moon_Body/Sector_THM/Interactables_THM/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    chertfire = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Lakebed_VisibleFrom_Far/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    feldsparfire = GameObject.Find("DB_PioneerDimension_Body/Sector_PioneerDimension/Interactables_PioneerDimension/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                }
            };

            GlobalMessenger<Campfire>.AddListener("EnterRoastingMode", EnterRoastingMode);
            GlobalMessenger<Campfire>.AddListener("ExitRoastingMode", ExitRoastingMode);
            GlobalMessenger<float>.AddListener("EatMarshmallow", EatMarshmallow);
            GlobalMessenger.AddListener("EnableBigHeadMode", BigHeadMode);
        }

        // Big head mode variable
        public void BigHeadMode()
        {
            DialogueConditionManager.SharedInstance.SetConditionState("BIG_HEAD_MODE", true);
        }

        // C-a-m-p-f-i-r-e f-u-n-c funcs
        public void EnterRoastingMode(Campfire campfire)
        {
            if (campfire == slatefire)
            {
                slatefire_roasting = true;
            }
            else if (campfire == eskerfire)
            {
                eskerfire_roasting = true;
            }
            else if (campfire == riebeckfire)
            {
                riebeckfire_roasting = true;
            }
            else if (campfire == chertfire)
            {
                chertfire_roasting = true;
            }
            else if (campfire == feldsparfire)
            {
                feldsparfire_roasting = true;
            }
        }
        public void ExitRoastingMode(Campfire campfire)
        {
            slatefire_roasting = false;
            eskerfire_roasting = false;
            riebeckfire_roasting = false;
            chertfire_roasting = false;
            feldsparfire_roasting = false;
        }

        public void EatMarshmallow(float toastedFraction)
        {
            if (badcans.Any(can => can._pickedUp) && toastedFraction < 1f)
            {
                if (slatefire_roasting)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_ATE_BAD_MALLOW", true);
                    slatefire_badmallow_lastate = TimeLoop.GetSecondsElapsed();
                }
                else if (eskerfire_roasting)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_ATE_BAD_MALLOW", true);
                    eskerfire_badmallow_lastate = TimeLoop.GetSecondsElapsed();
                }
                else if (riebeckfire_roasting)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_ATE_BAD_MALLOW", true);
                    riebeckfire_badmallow_lastate = TimeLoop.GetSecondsElapsed();
                }
                else if (chertfire_roasting)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ATE_BAD_MALLOW", true);
                    chertfire_badmallow_lastate = TimeLoop.GetSecondsElapsed();
                }
                else if (feldsparfire_roasting)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_ATE_BAD_MALLOW", true);
                    feldsparfire_badmallow_lastate = TimeLoop.GetSecondsElapsed();
                }
            }
        }

    }
}
