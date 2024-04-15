// THANKS TO: Xen, Lutias Kokopelli, Ixrec, viovayo, Magister Dragon

// TODO LIST:
// Hearthians turn to face the boom and seem horrified
// change Riebeck’s introductory message that he starts a conversation with if the player has told them about Solanum (sort of how the message Chert greets you with changes if you tell them about the time loop or they realize that the sun is going to blow)
// Also make Riebeck acknowledge that nearby chunks of BH have fallen throughout the loop
// Hug mod compat
// More Solanum interactions
// Angry Mica animation?
// Characters react to you dying in front of them
// Some characters have dialogue for you being suited up
// Tell Hal about Solanum?
// THE OUTSIDER - Tell Hal about the Friend?
// ASTRAL CODEC - For any mod that introduces a new species, add a dialogue option for them to the addendum of the AC

// DONE LIST
// option to tell Riebeck about the Stranger
// let the player talk with gabbro about turning off the loop
// Tell Gabbro that you're gonna go to the eye
// Tell Gabbro about the Stranger
// Tell Feldspar you landed on the Sun Station and went inside the Interloper
// Gabbro needs to react to you holding the AWC if you're holding it and they know what it is
// Have Mica cower when distraught
// Mica's Wrath!
// Tell Arkose about the Interloper being the source of all ghost matter
// ASTRAL CODEC - Tell Arkose about being able to see ghost matter
// Add scroll objects to "cool items" list
// THE VISION - Add vision torch to "cool items" list
// THE OUTSIDER - Tell Gabbro about talking to "Friend"
// ASTRAL CODEC - Tell Riebeck about finding the Lingering Chime
// THE OUTSIDER - Compat for Dream variable

using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                volumes = Resources.FindObjectsOfTypeAll<CowerAnimTriggerVolume>().ToList();
            };

            GlobalMessenger.AddListener("EnterConversation", OnEnterConversation);
            GlobalMessenger.AddListener("TriggerSupernova", MakeAllCower);
            GlobalMessenger<string, bool>.AddListener("DialogueConditionChanged", MakeMicaCower);
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

        // Patching for Mica's wrath
        [HarmonyPatch]
        public class MyPatchClass2
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(DestructionVolume), nameof(DestructionVolume.VanishModelRocketShip))]
            public static void OnModelRocketShipDestroyed_Postfix()
            {
                if (TimeLoop.GetSecondsElapsed() < 1330)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("MICAS_WRATH", true);
                }
            }
        }

        // Hearthians cowering
        private List<CowerAnimTriggerVolume> volumes;

        private void MakeMicaCower(string name, bool state)
        {
            if (name == "MODELROCKETKID_RH_DISTRAUGHT" && state)
            {
                var volume = GameObject.Find("Villager_HEA_Mica/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();
                volume.StartCoroutine(Coweroutine(volume._animator, 970));
                volumes.Remove(volume);
            }
        }

        private void MakeAllCower()
        {
            foreach (var volume in volumes) volume.StartCoroutine(Coweroutine(volume._animator, 1330));
            StartCoroutine(Banjoroutine(1330));
        }

        private IEnumerator Coweroutine(Animator animator, int time)
        {
            while (TimeLoop.GetSecondsElapsed() < time) yield return null;
            animator.SetTrigger("ProbeDodge");
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Cower 2") && !animator.GetCurrentAnimatorStateInfo(1).IsName("Cower 2")) yield return null;
            var n = animator.GetCurrentAnimatorStateInfo(0).IsName("Cower 2") ? 0 : 1;
            while (true)
            {
                var info = animator.GetCurrentAnimatorStateInfo(n);
                if (info.normalizedTime * info.length >= 0.2f) animator.CrossFade("Cower 2", 0.2f, n, -0.2f);
                yield return null;
            }
        }

        private IEnumerator Banjoroutine(int time)
        {
            var banjo = GameObject.Find("AudioSource_BanjoTuning");
            while (banjo != null && TimeLoop.GetSecondsElapsed() < time) yield return null;
            if (banjo != null) banjo.SetActive(false);
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
                DialogueConditionManager.SharedInstance.SetConditionState("RH_VILLAGEDAY", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_VILLAGEDAY", false);
            }

            // This variable is set true when a large amount of rubble has fallen into Brittle Hollow - UNUSED
            if (TimeLoop.GetSecondsElapsed() >= 660)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_BRITTLEBROKEN", true);
            }

            // This variable is set true around when the Sun turns red. This figure is set to 16:10. It's sooner than when Chert changes, but at this point the red sun peaks above the horizon on the village, which is what I'm actually using this variable for.
            if (TimeLoop.GetSecondsElapsed() >= 970)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_REDSUN", true);
            }

            // This variable is set true once End Times begins playing
            if (TimeLoop.GetSecondsElapsed() >= 1235)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ENDTIMES", true);
            }

            // This variable is set true when the supernova's boom occurs at 22:10.
            if (TimeLoop.GetSecondsElapsed() >= 1330)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_BOOM", true);
            }

            // This variable is set true if the player is wearing a space suit (either the training or the regular one)
            if (Locator.GetPlayerSuit().IsWearingSuit())
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SUITED", true);
            }

            // This variable is set true if the player is wearing the training suit
            if (Locator.GetPlayerSuit().IsTrainingSuit())
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TRAINING_SUITED", true);
            }

            // This variable is set true if the ATP is deactivated
            var TheMountain = UnityEngine.Object.FindObjectOfType<TimeLoopCoreController>();
            if ((TheMountain._warpCoreSocket.IsSocketOccupied() && TheMountain._warpCoreSocket.GetWarpCoreType() == WarpCoreType.Vessel) == false)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ATPDOWN", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ATPDOWN", false);
            }

            // STRANGER //
            // This variable is set true if the player knows of the Strangelings' connection to the Eye.
            if (Locator.GetShipLogManager().IsFactRevealed("IP_ZONE_2_X2") || Locator.GetShipLogManager().IsFactRevealed("IP_ZONE_2_STORY_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_STORY_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_STORY_X2"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_EYE", true);
            }

            // This variable is set true if the player has visited the dream world.
            if (Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_1"]._state >= ShipLogEntry.State.Explored || Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_2"]._state >= ShipLogEntry.State.Explored || Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_3"]._state >= ShipLogEntry.State.Explored) {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_DREAMWORLD", true);
            }
            else if (ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null && Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_HOME_X1"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_DREAMWORLD", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_DREAMWORLD", false);
            }

            // This variable is set true if the player knows the dream world is a simulation.
            if (Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_1_RULE_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_RULE_X2") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_3_RULE_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_3_STORY_X2"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_DREAM_IS_CODE", true);
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
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                        }
                        // The item is a broken warp core
                        else if ((warpCore._wcType == WarpCoreType.VesselBroken) || (warpCore._wcType == WarpCoreType.SimpleBroken))
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
                        }
                        // The item is some other kind of warp core
                        else
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                        }
                    }
                    // The item is one of the Stranger's lanterns
                    else if (item._type == ItemType.DreamLantern)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", true);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a Nomai scroll or vision torch
                    else if (item._type == ItemType.Scroll || item._type == ItemType.VisionTorch)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    else
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
                    }
                }
                // Holding nothing
                else
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
                }
            }
            catch
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
            }

            // This variable is set to true if the player has something new to say about the Stranger to Gabbro
            if (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_RING") == false || DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_INHABITANTS") == false || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_EYE") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_EYE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_LANTERN") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGERLANTERNHELD") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_DREAMWORLD") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD_CODE") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_DREAM_IS_CODE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_PRISONER") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && Locator.GetShipLogManager().IsFactRevealed("IP_SARCOPHAGUS_X5") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_FRIEND") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null && Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_HOME_X1")))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_STRANGER_SOMETHINGNEW", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_STRANGER_SOMETHINGNEW", false);
            }
        }
    }
}
