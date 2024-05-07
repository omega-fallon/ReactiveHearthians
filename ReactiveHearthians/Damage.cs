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
    public class Damage : MonoBehaviour
    {
        public static Damage Instance;

        // Hazard volumes
        public HazardVolume hazardvolume_slatefire;
        public float hazardvolume_slatefire_lasttouched;

        public HazardVolume hazardvolume_riebeckfire;
        public float hazardvolume_riebeckfire_lasttouched;

        public HazardVolume hazardvolume_eskerfire;
        public float hazardvolume_eskerfire_lasttouched;

        public HazardVolume hazardvolume_chertfire;
        public float hazardvolume_chertfire_lasttouched;

        public HazardVolume hazardvolume_feldsparfire;
        public float hazardvolume_feldsparfire_lasttouched;

        public DarkMatterVolume darkmattervolume_arkose;

        // Last damaged
        public float arkose_last_damaged;
        public float chert_last_damaged;
        public float esker_last_damaged;
        public float feldspar_last_damaged;
        public float gabbro_last_damaged;
        public float galena_last_damaged;
        public float gneiss_last_damaged;
        public float gossan_last_damaged;
        public float hal_last_damaged;
        public float hornfels_last_damaged;
        public float marl_last_damaged;
        public float mica_last_damaged;
        public float moraine_last_damaged;
        public float porphy_last_damaged;
        public float riebeck_last_damaged;
        public float rutile_last_damaged;
        public float slate_last_damaged;
        public float spinel_last_damaged;
        public float tektite_last_damaged;
        public float tephra_last_damaged;
        public float tuff_last_damaged;

        public void Start()
        {
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene == OWScene.SolarSystem)
                {
                    hazardvolume_slatefire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_riebeckfire = GameObject.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Interactables_Crossroads/VisibleFrom_BH/Prefab_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_eskerfire = GameObject.Find("Moon_Body/Sector_THM/Interactables_THM/Effects_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_chertfire = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Lakebed_VisibleFrom_Far/Prefab_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_feldsparfire = GameObject.Find("DB_PioneerDimension_Body/Sector_PioneerDimension/Interactables_PioneerDimension/Prefab_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();

                    darkmattervolume_arkose = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/DarkMatterVolume").GetComponent<DarkMatterVolume>();
                }
            };
        }

        // Harmony patches
        [HarmonyPatch]
        public class MyPatchClass
        {
            // Patching for characters reacting to you taking damage near them.
            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlayerAnimController), nameof(PlayerAnimController.OnInstantDamage))]
            public static void FallDamage_PostFix(InstantDamageType damageType)
            {
                Vector3 playerPosition = Locator.GetPlayerBody().transform.position;
                float reactRadius = 10;

                // Player is in Timber Hearth
                if (ReactiveHearthians.Instance.InSector_TimberHearth)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Timber Hearth.", MessageType.Success);

                    // Updates these positions
                    ReactiveHearthians.Instance.GetVariableNPCPositions();

                    // Impact damage
                    if (damageType == InstantDamageType.Impact)
                    {
                        // Test characters who are off by themselves and have no chance of overlapping first, using else ifs. Doing this to avoid doing unnecessary checks (if you're close to Tektite for example, you're 100% nowhere near anybody else. No need to check for anyone else. 
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Tektite_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_IMPACT_DAMAGE", true);
                            Damage.Instance.tektite_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, HugModStuff.Instance.Tuff_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_TUFF_IMPACT_DAMAGE", true);
                            Damage.Instance.tuff_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, HugModStuff.Instance.Gossan_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_GOSSAN_IMPACT_DAMAGE", true);
                            Damage.Instance.gossan_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, HugModStuff.Instance.Arkose_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_ARKOSE_IMPACT_DAMAGE", true);
                            Damage.Instance.arkose_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, HugModStuff.Instance.Moraine_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_MORAINE_IMPACT_DAMAGE", true);
                            Damage.Instance.moraine_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, HugModStuff.Instance.Slate_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_IMPACT_DAMAGE", true);
                            Damage.Instance.slate_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        // Non-loner NPCs
                        else
                        {
                            // Tephra
                            if (ReactiveHearthians.Instance.tephraPosition == "STANDARD" && Vector3.Distance(playerPosition, HugModStuff.Instance.Tephra_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", true);
                                Damage.Instance.tephra_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.tephraPosition == "HAS" && Vector3.Distance(playerPosition, HugModStuff.Instance.Tephra_HAS.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", true);
                                Damage.Instance.tephra_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.tephraPosition == "POST_OBSERVATORY" && Vector3.Distance(playerPosition, HugModStuff.Instance.Tephra_PostObservatory.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", true);
                                Damage.Instance.tephra_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Galena
                            if (ReactiveHearthians.Instance.galenaPosition == "STANDARD" && Vector3.Distance(playerPosition, HugModStuff.Instance.Galena_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_GALENA_IMPACT_DAMAGE", true);
                                Damage.Instance.galena_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.galenaPosition == "HAS" && Vector3.Distance(playerPosition, HugModStuff.Instance.Galena_HAS.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_GALENA_IMPACT_DAMAGE", true);
                                Damage.Instance.galena_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Hornfels
                            if (ReactiveHearthians.Instance.hornfelsPosition == "STANDARD" && Vector3.Distance(playerPosition, HugModStuff.Instance.Hornfels_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HORNFELS_IMPACT_DAMAGE", true);
                                Damage.Instance.hornfels_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.hornfelsPosition == "DOWNSTAIRS" && Vector3.Distance(playerPosition, HugModStuff.Instance.Hornfels_Downstairs.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HORNFELS_IMPACT_DAMAGE", true);
                                Damage.Instance.hornfels_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Hal
                            if (ReactiveHearthians.Instance.halPosition == "STANDARD" && Vector3.Distance(playerPosition, HugModStuff.Instance.Hal_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HAL_IMPACT_DAMAGE", true);
                                Damage.Instance.hal_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.halPosition == "OUTSIDE" && Vector3.Distance(playerPosition, HugModStuff.Instance.Hal_Outside.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HAL_IMPACT_DAMAGE", true);
                                Damage.Instance.hal_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Everybody else: Gneiss, Marl, Mica, Porphy, Rutile, Spinel
                            if (Vector3.Distance(playerPosition, HugModStuff.Instance.Gneiss_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_GNEISS_IMPACT_DAMAGE", true);
                                Damage.Instance.gneiss_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, HugModStuff.Instance.Marl_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_MARL_IMPACT_DAMAGE", true);
                                Damage.Instance.marl_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, HugModStuff.Instance.Mica_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_MICA_IMPACT_DAMAGE", true);
                                Damage.Instance.mica_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, HugModStuff.Instance.Porphy_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_PORPHY_IMPACT_DAMAGE", true);
                                Damage.Instance.porphy_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, HugModStuff.Instance.Rutile_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_RUTILE_IMPACT_DAMAGE", true);
                                Damage.Instance.rutile_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, HugModStuff.Instance.Spinel_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_SPINEL_IMPACT_DAMAGE", true);
                                Damage.Instance.spinel_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                        }
                    }
                    else if (damageType == InstantDamageType.Puncture)
                    {
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Tektite_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_PUNCTURE_DAMAGE", true);
                            Damage.Instance.tektite_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on the Attlerock
                else if (ReactiveHearthians.Instance.InSector_TimberMoon)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on the Attlerock.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Esker_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_IMPACT_DAMAGE", true);
                            Damage.Instance.esker_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on Ember Twin
                else if (ReactiveHearthians.Instance.InSector_CaveTwin)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Ember Twin.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Chert_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_IMPACT_DAMAGE", true);
                            Damage.Instance.chert_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on Brittle Hollow
                else if (ReactiveHearthians.Instance.InSector_BrittleHollow)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Brittle Hollow.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Riebeck_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_IMPACT_DAMAGE", true);
                            Damage.Instance.riebeck_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on Gabbro's Island
                else if (ReactiveHearthians.Instance.InSector_GiantsDeep)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Gabbro's Island.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Gabbro_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_IMPACT_DAMAGE", true);
                            Damage.Instance.gabbro_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is in Feldspar's dimension
                else if (ReactiveHearthians.Instance.InSector_PioneerDimension)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken in Feldspar's dimension.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, HugModStuff.Instance.Feldspar_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_IMPACT_DAMAGE", true);
                            Damage.Instance.feldspar_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                else
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken elsewhere.", MessageType.Success);
                }

                // Pick up Chert mod check
                if (ReactiveHearthians.Instance.ModHelper.Interaction.TryGetMod("orclecle.PickUpChert") != null && damageType == InstantDamageType.Impact)
                {
                    if (Vector3.Distance(playerPosition, HugModStuff.Instance.Chert_Standard.transform.position) <= reactRadius)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_IMPACT_DAMAGE", true);
                        Damage.Instance.chert_last_damaged = TimeLoop.GetSecondsElapsed();
                    }
                }
            }

            // Patching for entering a hazard detector
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HazardDetector), nameof(HazardDetector.OnVolumeAdded))]
            public static void DamageVolume_PostFix(EffectVolume eVolume, HazardDetector __instance)
            {
                if (__instance.CompareTag("PlayerDetector"))
                {
                    // Campfire & ghost matter damage
                    if (eVolume == Damage.Instance.hazardvolume_slatefire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_FIRE_DAMAGED", true);
                        Damage.Instance.hazardvolume_slatefire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == Damage.Instance.hazardvolume_riebeckfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_FIRE_DAMAGED", true);
                        Damage.Instance.hazardvolume_riebeckfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == Damage.Instance.hazardvolume_eskerfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_FIRE_DAMAGED", true);
                        Damage.Instance.hazardvolume_eskerfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == Damage.Instance.hazardvolume_chertfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_FIRE_DAMAGED", true);
                        Damage.Instance.hazardvolume_chertfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == Damage.Instance.hazardvolume_feldsparfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_FIRE_DAMAGED", true);
                        Damage.Instance.hazardvolume_feldsparfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == Damage.Instance.darkmattervolume_arkose)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_ARKOSE_DARKMATTER_DAMAGED", true);
                    }
                    else
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine(eVolume.ToString(), MessageType.Success);
                    }
                }
            }
        }
    }
}
