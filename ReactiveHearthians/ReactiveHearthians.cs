// THANKS TO: Xen, Lutias Kokopelli, Ixrec, viovayo, Magister Dragon

// TODO LIST:
// Hearthians turn to face the boom and seem horrified
// change Riebeck’s introductory message that he starts a conversation with if the player has told them about Solanum (sort of how the message Chert greets you with changes if you tell them about the time loop or they realize that the sun is going to blow)
// Also make Riebeck acknowledge that nearby chunks of BH have fallen throughout the loop
// Hug mod compat
// More Solanum interactions
// Have Mica cower when distraught

// DONE LIST
// option to tell Riebeck about the Stranger
// let the player talk with gabbro about turning off the loop
// Tell Gabbro that you're gonna go to the eye
// Tell Gabbro about the Stranger
// Tell Feldspar you landed on the Sun Station and went inside the Interloper
// Gabbro needs to react to you holding the AWC if you're holding it and they know what it is

using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ReactiveHearthians
{
    public class ReactiveHearthians : ModBehaviour
    {
        public static ReactiveHearthians Instance;
        public static INewHorizons newHorizons;
        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Instance = this;
        }

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(ReactiveHearthians)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
                nameYourVariableWhatever = GameObject.Find("SolarSystemRoot").GetComponent<TimeLoop>();
                nameYourVariableWhatever2 = GameObject.Find("SolarSystemRoot").GetComponent<TimeLoop>();
            };

            GlobalMessenger.AddListener("EnterConversation", OnEnterConversation);
        }

        // Special patching for Gabbro.
        [HarmonyPatch]
        public class MyPatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(GabbroDialogueSwapper), nameof(GabbroDialogueSwapper.Start))]
            public static void GabbroDialogueSwapper_Postfix()
            {
                ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_All.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", GameObject.Find("GabbroIsland_Body"));

                if (TimeLoop.GetLoopCount() == 1)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_1.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", GameObject.Find("GabbroIsland_Body"));
                }
                else if (TimeLoop.GetLoopCount() == 2)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_2.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", GameObject.Find("GabbroIsland_Body"));
                }
                else
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_3.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", GameObject.Find("GabbroIsland_Body"));
                }
            }
        }

        // Patching for Hearthians cowering
        private TimeLoop nameYourVariableWhatever;
        private TimeLoop nameYourVariableWhatever2;
        public void Update()
        {
            if (nameYourVariableWhatever != null)
            {
                if (TimeLoop.GetSecondsElapsed() >= 1330)
                {
                    MakeAllCower();
                    nameYourVariableWhatever = null;
                }

                if (nameYourVariableWhatever2 != null)
                {
                    if (TimeLoop.GetSecondsElapsed() >= 970 && DialogueConditionManager.SharedInstance.GetConditionState("MODELROCKETKID_DISTRAUGHT"))
                    {
                        MakeMicaCower();
                        nameYourVariableWhatever2 = null;
                    }
                }
            }
        }
        private void MakeAllCower()
        {
            GameObject.Find("AudioSource_BanjoTuning").SetActive(false);
            foreach (var volume in Resources.FindObjectsOfTypeAll<CowerAnimTriggerVolume>())
            {
                var animator = volume._animator;
                animator.SetTrigger("ProbeDodge");
                volume.StartCoroutine(FYeahCoroutines(animator));
            }
        }
        // TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Mica/CowerAnimTrigger
        private void MakeMicaCower()
        {
            var volume = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Mica/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();

            var animator = volume._animator;
            animator.SetTrigger("ProbeDodge");
            volume.StartCoroutine(FYeahCoroutines(animator));
        }

        private IEnumerator FYeahCoroutines(Animator animator)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Cower 2") && !animator.GetCurrentAnimatorStateInfo(1).IsName("Cower 2")) yield return null;
            var n = animator.GetCurrentAnimatorStateInfo(0).IsName("Cower 2") ? 0 : 1;
            while (true)
            {
                animator.CrossFade("Cower 2", 0.1f, n);
                yield return null;
            }
        }

        // Dialogue variables
        public void OnEnterConversation()
        {
            // Loop variables. //
            if (TimeLoop.GetLoopCount() == 1)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("LOOP_COUNT_IS_1", true);
            }
            else if (TimeLoop.GetLoopCount() == 2)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("LOOP_COUNT_IS_2", true);
            }
            else if (TimeLoop.GetLoopCount() == 3)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("LOOP_COUNT_IS_3", true);
            }
            else if (TimeLoop.GetLoopCount() >= 4)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("LOOP_COUNT_GOE_4", true);
            }

            // Time-in-loop variables //
            // This variable is set true when the Sun is over the Timber Hearth village. Timestamps provided by Lutias Kokopelli.
            if ((TimeLoop.GetSecondsElapsed() >= 103 && TimeLoop.GetSecondsElapsed() <= 307) || (TimeLoop.GetSecondsElapsed() >= 519 && TimeLoop.GetSecondsElapsed() <= 723) || (TimeLoop.GetSecondsElapsed() >= 935 && TimeLoop.GetSecondsElapsed() <= 1139))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_VILLAGEDAY", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_VILLAGEDAY", false);
            }

            // This variable is set true when a large amount of rubble has fallen into Brittle Hollow - UNUSED
            if (TimeLoop.GetSecondsElapsed() >= 660)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_BRITTLEBROKEN", true);
            }

            // This variable is set true around when the Sun turns red. This figure is set to 16:10. It's sooner than when Chert changes, but at this point the red sun peaks above the horizon on the village, which is what I'm actually using this variable for.
            if (TimeLoop.GetSecondsElapsed() >= 970)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_REDSUN", true);
            }

            // This variable is set true once End Times begins playing
            if (TimeLoop.GetSecondsElapsed() >= 1235)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_ENDTIMES", true);
            }

            // This variable is set true when the supernova's boom occurs at 22:10.
            if (TimeLoop.GetSecondsElapsed() >= 1330)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_BOOM", true);
            }

            // This variable is set true if the ATP is deactivated
            var TheMountain = UnityEngine.Object.FindObjectOfType<TimeLoopCoreController>();
            if ((TheMountain._warpCoreSocket.IsSocketOccupied() && TheMountain._warpCoreSocket.GetWarpCoreType() == WarpCoreType.Vessel) == false)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_ATPDOWN", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_ATPDOWN", false);
            }

            // STRANGER //
            // This variable is set true if the player knows of the Strangelings' connection to the Eye.
            if (Locator.GetShipLogManager().IsFactRevealed("IP_ZONE_2_X2") || Locator.GetShipLogManager().IsFactRevealed("IP_ZONE_2_STORY_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_STORY_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_STORY_X2"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGER_EYE", true);
            }

            // This variable is set true if the player has visited the dream world.
            if (Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_1"]._state >= ShipLogEntry.State.Explored || Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_2"]._state >= ShipLogEntry.State.Explored || Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_3"]._state >= ShipLogEntry.State.Explored) {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGER_DREAMWORLD", true);
            }

            // This variable is set to true if the player has something new to say about the Stranger to Gabbro
            if (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_RING") == false || DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_INHABITANTS") == false || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_EYE") == false && DialogueConditionManager.SharedInstance.GetConditionState("CUSTOM_CONDITION_STRANGER_EYE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_LANTERN") == false && DialogueConditionManager.SharedInstance.GetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == false && DialogueConditionManager.SharedInstance.GetConditionState("CUSTOM_CONDITION_STRANGER_DREAMWORLD") == true)|| (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_PRISONER") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && Locator.GetShipLogManager().IsFactRevealed("IP_SARCOPHAGUS_X5") == true))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_GABBRO_STRANGER_SOMETHINGNEW", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_GABBRO_STRANGER_SOMETHINGNEW", false);
            }

            // Sets variables depending on what (if anything) the player is holding. //
            try
            {
                if (Locator.GetToolModeSwapper().GetToolMode() != ToolMode.None)
                {
                    var item = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
                    // The item is a warp core
                    if (item._type == ItemType.WarpCore)
                    {
                        var warpCore = item as WarpCoreItem;

                        // The item is the Advanced Warp Core
                        if (warpCore._wcType == WarpCoreType.Vessel)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", true);
                        }
                        // The item is a broken warp core
                        else if ((warpCore._wcType == WarpCoreType.VesselBroken) || (warpCore._wcType == WarpCoreType.SimpleBroken))
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", false);
                        }
                        // The item is some other kind of warp core
                        else
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", true);
                        }
                    }
                    // The item is one of the Stranger's lanterns
                    else if (item._type == ItemType.Lantern || item._type == ItemType.DreamLantern)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", true);
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", true);
                    }
                    else
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", false);
                    }
                }
                // Holding nothing
                else
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", false);
                }
            }
            catch
            {
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_AWCHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_WARPCOREHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_STRANGERLANTERNHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("CUSTOM_CONDITION_COOLTHINGHELD", false);
            }
        }
    }
}
