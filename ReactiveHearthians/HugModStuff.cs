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
    public class HugModStuff : MonoBehaviour
    {
        public static HugModStuff Instance;

        public IHugModApi hugApi;
        public GameObject[] huggables;

        // Huggable people
        public GameObject Arkose_Standard;

        public GameObject Chert_Standard;
        public GameObject Chert_Eye;

        public GameObject Esker_Standard;
        public GameObject Esker_Eye;

        public GameObject Feldspar_Standard;
        public GameObject Feldspar_Eye;

        public GameObject Gabbro_Standard;
        public GameObject Gabbro_Eye;

        public GameObject Galena_Standard;
        public GameObject Galena_HAS;

        public GameObject Gneiss_Standard;

        public GameObject Gossan_Standard;

        public GameObject Hal_Standard;
        public GameObject Hal_Outside;

        public GameObject Hornfels_Standard;
        public GameObject Hornfels_Downstairs;

        public GameObject Marl_Standard;

        public GameObject Mica_Standard;

        public GameObject Moraine_Standard;

        public GameObject Porphy_Standard;

        public GameObject Prisoner_Eye_Choice;
        public GameObject Prisoner_Eye_Campfire;

        public GameObject Riebeck_Standard;
        public GameObject Riebeck_Eye;

        public GameObject Rutile_Standard;

        public GameObject Slate_Standard;

        public GameObject Solanum_Eye;

        public GameObject Spinel_Standard;

        public GameObject Tektite_Standard;

        public GameObject Tephra_Standard;
        public GameObject Tephra_HAS;
        public GameObject Tephra_PostObservatory;

        public GameObject Tuff_Standard;

        public void Start()
        {
            // Hug API
            hugApi = ReactiveHearthians.Instance.ModHelper.Interaction.TryGetModApi<IHugModApi>("VioVayo.HugMod");

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene == OWScene.SolarSystem)
                {
                    // NPC objects
                    Arkose_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_UpperVillage/Characters_UpperVillage/Villager_HEA_Arkose_GhostMatter");

                    Chert_Standard = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert/");

                    Esker_Standard = GameObject.Find("Moon_Body/Sector_THM/Characters_THM/Villager_HEA_Esker/");

                    Feldspar_Standard = GameObject.Find("DB_PioneerDimension_Body/Sector_PioneerDimension/Interactables_PioneerDimension/Pioneer_Characters/Traveller_HEA_Feldspar/");

                    Gabbro_Standard = GameObject.Find("Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/");

                    Galena_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Kids_PreGame/Villager_HEA_Galena");
                    Galena_HAS = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Kids_Hidden/Villager_HEA_Galena (1)");

                    Gneiss_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Gneiss");

                    Gossan_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_UpperVillage/Characters_UpperVillage/Villager_HEA_Gossan");

                    Hal_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Characters_Observatory/Character_HEA_Hal_Museum");
                    Hal_Outside = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Characters_Observatory/Character_HEA_Hal_Outside");

                    Hornfels_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Characters_Observatory/Villager_HEA_Hornfels");
                    Hornfels_Downstairs = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Characters_Observatory/Villager_HEA_Hornfels (1)");

                    Marl_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Marl");

                    Mica_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Mica");

                    Moraine_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_UpperVillage/Characters_UpperVillage/Villager_HEA_Moraine");

                    Porphy_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Porphy");

                    Riebeck_Standard = GameObject.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Characters_Crossroads/Traveller_HEA_Riebeck");

                    Rutile_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Rutile");

                    Slate_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_StartingCamp/Characters_StartingCamp/Villager_HEA_Slate");

                    Spinel_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Spinel");

                    Tektite_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_ImpactCrater/Characters_ImpactCrater/Villager_HEA_Tektite_2");

                    Tephra_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Kids_PreGame/Villager_HEA_Tephra");
                    Tephra_HAS = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Kids_Hidden/Villager_HEA_Tephra (1)");
                    Tephra_PostObservatory = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_VillageCemetery/Characters_VillageCemetery/Villager_HEA_Tephra_PostObservatory");

                    Tuff_Standard = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_ZeroGCave/Characters_ZeroGCave/Villager_HEA_Tuff");

                    // Everything that gets done a frame later goes here:
                    StopCoroutine(WaitAGoshDarnedFrame_Eye());
                    StartCoroutine(WaitAGoshDarnedFrame_SolarSystem());
                }
                else if (loadScene == OWScene.EyeOfTheUniverse)
                {
                    // Huggables
                    Chert_Eye = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Chert/Traveller_HEA_Chert/");

                    Esker_Eye = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Esker/Villager_HEA_Esker/");

                    Feldspar_Eye = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Feldspar/Traveller_HEA_Feldspar/");

                    Gabbro_Eye = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Gabbro/Traveller_HEA_Gabbro/");

                    Prisoner_Eye_Choice = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Prisoner/PrisonerRoot_Choice/Prisoner_Choice/");
                    Prisoner_Eye_Campfire = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Prisoner/PrisonerRoot_Campfire/Prisoner_Campfire/");

                    Riebeck_Eye = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Riebeck/Traveller_HEA_Riebeck/");

                    Solanum_Eye = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Solanum/");

                    // Everything that gets done a frame later goes here:
                    StopCoroutine(WaitAGoshDarnedFrame_SolarSystem());
                    StartCoroutine(WaitAGoshDarnedFrame_Eye());
                }
            };


            GlobalMessenger.AddListener("TriggerMemoryUplink", StatueLinked);
        }

        // Single-frame delay coroutine
        private IEnumerator WaitAGoshDarnedFrame_SolarSystem()
        {
            yield return null;

            if (hugApi != null)
            {
                huggables = hugApi.GetAllHuggables();

                // Setting watchers
                if (Arkose_Standard != null)
                {
                    hugApi.OnHugStart(Arkose_Standard, () => { Person_Hug("ARKOSE"); });
                }

                if (Chert_Standard != null)
                {
                    hugApi.OnHugStart(Chert_Standard, () => { Person_Hug("CHERT"); });
                }

                if (Esker_Standard != null)
                {
                    hugApi.OnHugStart(Esker_Standard, () => { Person_Hug("ESKER"); });
                }

                if (Feldspar_Standard != null)
                {
                    hugApi.OnHugStart(Feldspar_Standard, () => { Person_Hug("FELDSPAR"); });
                }

                if (Gabbro_Standard != null)
                {
                    hugApi.OnHugStart(Gabbro_Standard, () => { Person_Hug("GABBRO"); });
                }

                if (Galena_Standard != null)
                {
                    hugApi.OnHugStart(Galena_Standard, () => { Person_Hug("GALENA"); });
                }
                if (Galena_HAS != null)
                {
                    hugApi.OnHugStart(Galena_HAS, () => { Person_Hug("GALENA"); });
                }

                if (Gneiss_Standard != null)
                {
                    hugApi.OnHugStart(Gneiss_Standard, () => { Person_Hug("GNEISS"); });
                }

                if (Gossan_Standard != null)
                {
                    hugApi.OnHugStart(Gossan_Standard, () => { Person_Hug("GOSSAN"); });
                }

                if (Hal_Standard != null)
                {
                    hugApi.OnHugStart(Hal_Standard, () => { Person_Hug("HAL"); });
                }
                if (Hal_Outside != null)
                {
                    hugApi.OnHugStart(Hal_Outside, () => { Person_Hug("HAL"); });
                }

                if (Hornfels_Standard != null)
                {
                    hugApi.OnHugStart(Hornfels_Standard, () => { Person_Hug("HORNFELS"); });
                }
                if (Hornfels_Downstairs != null)
                {
                    hugApi.OnHugStart(Hornfels_Downstairs, () => { Person_Hug("HORNFELS"); });
                }

                if (Marl_Standard != null)
                {
                    hugApi.OnHugStart(Marl_Standard, () => { Person_Hug("MARL"); });
                }

                if (Mica_Standard != null)
                {
                    hugApi.OnHugStart(Mica_Standard, () => { Person_Hug("MICA"); });
                }

                if (Moraine_Standard != null)
                {
                    hugApi.OnHugStart(Moraine_Standard, () => { Person_Hug("MORAINE"); });
                }

                if (Porphy_Standard != null)
                {
                    hugApi.OnHugStart(Porphy_Standard, () => { Person_Hug("PORPHY"); });
                }

                if (Riebeck_Standard != null)
                {
                    hugApi.OnHugStart(Riebeck_Standard, () => { Person_Hug("RIEBECK"); });
                }

                if (Rutile_Standard != null)
                {
                    hugApi.OnHugStart(Rutile_Standard, () => { Person_Hug("RUTILE"); });
                }

                if (Slate_Standard != null)
                {
                    hugApi.OnHugStart(Slate_Standard, () => { Person_Hug("SLATE"); });
                }

                if (Spinel_Standard != null)
                {
                    hugApi.OnHugStart(Spinel_Standard, () => { Person_Hug("SPINEL"); });
                }

                if (Tektite_Standard != null)
                {
                    hugApi.OnHugStart(Tektite_Standard, () => { Person_Hug("TEKTITE"); });
                }

                if (Tephra_Standard != null)
                {
                    hugApi.OnHugStart(Tephra_Standard, () => { Person_Hug("TEPHRA"); });
                }
                if (Tephra_HAS != null)
                {
                    hugApi.OnHugStart(Tephra_HAS, () => { Person_Hug("TEPHRA"); });
                }
                if (Tephra_PostObservatory != null)
                {
                    hugApi.OnHugStart(Tephra_PostObservatory, () => { Person_Hug("TEPHRA"); });
                }

                if (Tuff_Standard != null)
                {
                    hugApi.OnHugStart(Tuff_Standard, () => { Person_Hug("TUFF"); });
                }
            }
        }
        private IEnumerator WaitAGoshDarnedFrame_Eye()
        {
            yield return null;

            if (hugApi != null)
            {
                huggables = hugApi.GetAllHuggables();

                // Setting watchers
                if (Chert_Eye != null)
                {
                    hugApi.OnHugStart(Chert_Eye, () => { Person_Hug("CHERT"); });
                }

                if (Esker_Eye != null)
                {
                    hugApi.OnHugStart(Esker_Eye, () => { Person_Hug("ESKER"); });
                }

                if (Feldspar_Eye != null)
                {
                    hugApi.OnHugStart(Feldspar_Eye, () => { Person_Hug("FELDSPAR"); });
                }

                if (Gabbro_Eye != null)
                {
                    hugApi.OnHugStart(Gabbro_Eye, () => { Person_Hug("GABBRO"); });
                }

                if (Prisoner_Eye_Choice != null)
                {
                    hugApi.OnHugStart(Prisoner_Eye_Choice, () => { Person_Hug("PRISONER_CHOICE"); });
                }
                if (Prisoner_Eye_Campfire != null)
                {
                    hugApi.OnHugStart(Prisoner_Eye_Campfire, () => { Person_Hug("PRISONER_CAMPFIRE"); });
                }

                if (Riebeck_Eye != null)
                {
                    hugApi.OnHugStart(Riebeck_Eye, () => { Person_Hug("RIEBECK"); });
                }

                if (Solanum_Eye != null)
                {
                    hugApi.OnHugStart(Solanum_Eye, () => { Person_Hug("SOLANUM"); });
                }
            }
        }

        // Hug mod method
        public void Person_Hug(string person)
        {
            if (TimeLoop.GetSecondsElapsed() >= Cowering.Instance.boomTime)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_" + person + "_HUGGED_SUPERNOVA", true);
            }

            if (person == "CHERT")
            {
                if (TimeLoop.GetMinutesElapsed() >= 20.5f)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S4", true);
                }
                else if (TimeLoop.GetMinutesElapsed() >= 17f)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S3", true);
                }
                else if (TimeLoop.GetMinutesElapsed() >= 11f)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S2", true);
                }
                else
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S1", true);
                }
            }

            DialogueConditionManager.SharedInstance.SetConditionState("RH_" + person + "_HUGGED", true);
        }

        // Statue linkup variable
        public void StatueLinked()
        {
            PlayerData.SetPersistentCondition("RH_NOMAI_STATUE_LINKED", true);

            // Rechecking Hal & Tephra, just in case
            if (hugApi != null)
            {
                if (Hal_Outside == null)
                {
                    Hal_Outside = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Characters_Observatory/Character_HEA_Hal_Outside");
                    hugApi.OnHugStart(Hal_Outside, () => { Person_Hug("HAL"); });
                }
                if (Tephra_PostObservatory == null)
                {
                    Tephra_PostObservatory = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_VillageCemetery/Characters_VillageCemetery/Villager_HEA_Tephra_PostObservatory");
                    hugApi.OnHugStart(Tephra_PostObservatory, () => { Person_Hug("TEPHRA"); });
                }
            }
        }
    }
}
