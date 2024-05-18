using HarmonyLib;
using HugMod;
using NAudio.CoreAudioApi;
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
    public class DialogueManage : MonoBehaviour
    {
        public static DialogueManage Instance;
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            GlobalMessenger.AddListener("EnterConversation", OnEnterConversation);
            GlobalMessenger.AddListener("ExitConversation", OnExitConversation);
            GlobalMessenger<string, bool>.AddListener("DialogueConditionChanged", GabbroFlagsWatcher);
        }

        
        [HarmonyPatch]
        public class MyPatchClass
        {
            // Solanum convo counter
            [HarmonyPostfix]
            [HarmonyPatch(typeof(NomaiConversationManager), nameof(NomaiConversationManager.OnFinishDialogue))]
            public static void NomaiConversationManager_PostFix()
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player talked to Solanum.", MessageType.Success);

                // Checking for the Vision completeness
                if (ReactiveHearthians.Instance.ModHelper.Interaction.TryGetMod("hearth1an.TheVision") != null && Locator.GetShipLogManager().IsFactRevealed("SOLANUM_PROJECTION_COMPLETE"))
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("The Vision's plotline is in progress; do not count Solanum interactions.", MessageType.Success);
                }
                else
                {
                    // Load, increment, then save the counter to a file
                    int solanumSpokenConversationCount = ReactiveHearthians.Instance.ModHelper.Storage.Load<int>("my_number.json");
                    if (solanumSpokenConversationCount == 0 && PlayerData.GetPersistentCondition("MET_SOLANUM"))
                    {
                        solanumSpokenConversationCount += 1;
                    }
                    solanumSpokenConversationCount += 1;
                    ReactiveHearthians.Instance.ModHelper.Storage.Save(solanumSpokenConversationCount, "my_number.json");

                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Solanum conversation count is: " + solanumSpokenConversationCount.ToString(), MessageType.Success);

                    // Set dialogue variables
                    if (solanumSpokenConversationCount >= 2)
                    {
                        PlayerData.SetPersistentCondition("RH_SOLANUM_TALKED_TWICE", true);
                    }
                    if (solanumSpokenConversationCount >= 3)
                    {
                        PlayerData.SetPersistentCondition("RH_SOLANUM_TALKED_THRICE", true);
                    }
                    if (solanumSpokenConversationCount >= 5)
                    {
                        PlayerData.SetPersistentCondition("RH_SOLANUM_TALKED_FIVE_TIMES", true);
                    }
                    if (solanumSpokenConversationCount >= 10)
                    {
                        PlayerData.SetPersistentCondition("RH_SOLANUM_TALKED_TEN_TIMES", true);
                    }
                }
            }

            // Patching for ending a conversation with a specific character
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.EndConversation))]
            public static void ConversationEnd_PostFix(CharacterDialogueTree __instance)
            {
                string name = __instance._xmlCharacterDialogueAsset.name.ToString();
                // use this patch in the future if necessary
            }
        }


        // Resetting dialogue variables
        public void OnExitConversation()
        {
            // Resetting all hug & damage variables to false
            string[] people = { "ARKOSE", "CHERT", "ESKER", "FELDSPAR", "GABBRO", "GALENA", "GNEISS", "GOSSAN", "HAL", "HORNFELS", "MARL", "MICA", "MORAINE", "PORPHY", "RIEBECK", "RUTILE", "SLATE", "SPINEL", "TEKTITE", "TEPHRA", "TUFF", "PRISONER_CHOICE", "PRISONER_CAMPFIRE", "SOLANUM" };
            foreach (string person in people)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_" + person + "_HUGGED", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_" + person + "_IMPACT_DAMAGE", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_" + person + "_PUNCTURE_DAMAGE", false);
            }

            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S4", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S3", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S2", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S1", false);

            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_PROBE_SPOTTED", false);
        }

        // Dialogue variables
        public void OnEnterConversation()
        {
            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Setting conversation variables…", MessageType.Success);

            // Installed mod variables. //
            DialogueConditionManager.SharedInstance.SetConditionState("ASTRAL_CODEC", ReactiveHearthians.Instance.Astral_Codec_Installed);
            DialogueConditionManager.SharedInstance.SetConditionState("THE_OUTSIDER", ReactiveHearthians.Instance.Outsider_Installed);

            DialogueConditionManager.SharedInstance.SetConditionState("PLAY_AS_GABBRO", ReactiveHearthians.Instance.Play_As_Gabbro_Installed);
            DialogueConditionManager.SharedInstance.SetConditionState("NOPLAY_AS_GABBRO", !ReactiveHearthians.Instance.Play_As_Gabbro_Installed);

            if (PlayerData.GetShipLogFactSave("HN_POD_RESOLUTION") != null && PlayerData.GetShipLogFactSave("HN_POD_RESOLUTION").revealOrder > -1)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("HearthsNeighborComplete", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("HearthsNeighborComplete", false);
            }

            // Nomai statue skip variable //
            if (PlayerData.GetPersistentCondition("RH_NOMAI_STATUE_LINKED") || TimeLoop.GetLoopCount() > 1)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_NOMAI_STATUE_NOT_LINKED", false);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_NOMAI_STATUE_NOT_LINKED", true);
            }

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
            // Chert phase variable
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S1", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S2", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S3", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S4", false);

            if (TimeLoop.GetMinutesElapsed() >= 20.5f)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S4", true);
            }
            else if (TimeLoop.GetMinutesElapsed() >= 17f)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S3", true);
            }
            else if (TimeLoop.GetMinutesElapsed() >= 11f)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S2", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_S1", true);
            }

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

            // This variable is set true when the supernova's boom occurs (normally 22:10)
            if (TimeLoop.GetSecondsElapsed() >= Cowering.Instance.boomTime)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_BOOM", true);
            }

            // Solanum met, setting to a standard variable //
            if (PlayerData.GetPersistentCondition("MET_SOLANUM"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SOLANUM_MET", true);
            }

            // Campfire recency checks //
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_SLATE_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_SLATE_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - Damage.Instance.hazardvolume_slatefire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_RIEBECK_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_RIEBECK_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - Damage.Instance.hazardvolume_riebeckfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_ESKER_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_ESKER_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - Damage.Instance.hazardvolume_eskerfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_CHERT_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_CHERT_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - Damage.Instance.hazardvolume_chertfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_FELDSPAR_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_FELDSPAR_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - Damage.Instance.hazardvolume_feldsparfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_FIRE_DAMAGED_RECENT", false);
            }

            // Resetting badmallow dialogue, we do this differently than above but it's effectively the same thing //
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_SLATE_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - BadMallow.Instance.slatefire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_RIEBECK_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - BadMallow.Instance.riebeckfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_ESKER_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - BadMallow.Instance.eskerfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_CHERT_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - BadMallow.Instance.chertfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_FELDSPAR_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - BadMallow.Instance.feldsparfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_ATE_BAD_MALLOW", false);
            }

            // Resetting damage dialogue variables!! Add this!! //
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.arkose_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ARKOSE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.chert_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.esker_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.feldspar_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.gabbro_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.galena_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GALENA_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.gneiss_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GNEISS_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.gossan_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GOSSAN_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.hal_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_HAL_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.hornfels_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_HORNFELS_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.marl_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_MARL_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.mica_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_MICA_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.moraine_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_MORAINE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.porphy_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_PORPHY_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.riebeck_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.rutile_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RUTILE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.slate_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.spinel_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SPINEL_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.tektite_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_IMPACT_DAMAGE", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_PUNCTURE_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.tephra_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - Damage.Instance.tuff_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TUFF_IMPACT_DAMAGE", false);
            }

            // Ship nearby variables //
            if (ReactiveHearthians.Instance.loadedScene == "vanilla" && Vector3.Distance(Locator.GetShipBody().GetPosition(), HugModStuff.Instance.Porphy_Standard.transform.position) <= 50)
            {
                // fill in
            }

            // Misc variables //
            // This variable is set true if the ATP is deactivated
            if (ReactiveHearthians.Instance.loadedScene == "vanilla" && ReactiveHearthians.Instance.TheMountain != null && !(ReactiveHearthians.Instance.TheMountain._warpCoreSocket.IsSocketOccupied() && ReactiveHearthians.Instance.TheMountain._warpCoreSocket.GetWarpCoreType() == WarpCoreType.Vessel))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ATPDOWN", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ATPDOWN", false);
            }

            // This variable is set true if the player is wearing a space suit (either the training or the regular one)
            if (Locator.GetPlayerSuit().IsWearingSuit())
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SUITED", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SUITED", false);
            }

            // This variable is set true if the player is wearing the training suit
            if (Locator.GetPlayerSuit().IsTrainingSuit())
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TRAINING_SUITED", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TRAINING_SUITED", false);
            }

            // This variable is set true if the player is wearing the normal suit before getting the launch codes (possible via a tricky geyser maneuver)
            if (TimeLoop.GetLoopCount() == 1 && Locator.GetPlayerSuit().IsWearingSuit() && Locator.GetPlayerSuit().IsTrainingSuit() == false && PlayerData.GetPersistentCondition("LAUNCH_CODES_GIVEN") == false)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SUITED_EARLY", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SUITED_EARLY", false);
            }

            // This variable is set true if the player has taken off before getting the launch codes (see above)
            if (TimeLoop.GetLoopCount() == 1 && DialogueConditionManager.SharedInstance.GetConditionState("Space") && PlayerData.GetPersistentCondition("LAUNCH_CODES_GIVEN") == false)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_L1_SKIP_DONE", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_L1_SKIP_DONE", false);
            }

            // This variable is set true if the player is standing on Porphy's pot.
            if (false)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_PORPHY_POT_STAND", false);
            }

            // This variable is set true if Tephra is positioned next to Galena.
            if (ReactiveHearthians.Instance.tephraPosition == "STANDARD" && ReactiveHearthians.Instance.galenaPosition == "STANDARD")
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_WITH_GALENA", false);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_WITH_GALENA", true);
            }

            // This variable is set true if the Hide and Seek game is available to be played.
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_TEPHRA_WITH_GALENA") && DialogueConditionManager.SharedInstance.GetConditionState("BeginHideAndSeek") == false)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_HIDEANDSEEK_AVAILABLE", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_HIDEANDSEEK_AVAILABLE", false);
            }

            // STRANGER //
            // This variable is set true if the player knows of the Strangelings' connection to the Eye.
            if (Locator.GetShipLogManager().IsFactRevealed("IP_ZONE_2_X2") || Locator.GetShipLogManager().IsFactRevealed("IP_ZONE_2_STORY_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_STORY_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_STORY_X2"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_EYE", true);
            }

            // This variable is set true if the player has visited the dream world.
            if (Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_1"]._state >= ShipLogEntry.State.Explored || Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_2"]._state >= ShipLogEntry.State.Explored || Locator.GetShipLogManager()._entryDict["IP_DREAM_ZONE_3"]._state >= ShipLogEntry.State.Explored)
            {
                PlayerData.SetPersistentCondition("DREAMWORLD_EVER_BEEN", true);
            }
            else if (ReactiveHearthians.Instance.ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null && Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_HOME_X1"))
            {
                PlayerData.SetPersistentCondition("DREAMWORLD_EVER_BEEN", true);
            }

            // This variable is set true if the player knows the dream world is a simulation.
            if (Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_1_RULE_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_RULE_X2") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_3_RULE_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_3_STORY_X2"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_DREAM_IS_CODE", true);
            }

            // Chert position variables //
            // Resetting these first
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_EMBER_TWIN", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_ASH_TWIN", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_TIMBER_HEARTH", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_ATTLEROCK", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_BRITTLE_HOLLOW", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_HOLLOW_LANTERN", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_GIANTS_DEEP", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_DARK_BRAMBLE", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_QUANTUM_MOON", false);

            if (ReactiveHearthians.Instance.loadedScene == "vanilla")
            {
                if (ReactiveHearthians.Instance.ModHelper.Interaction.TryGetMod("orclecle.PickUpChert") != null)
                {
                    GameObject chertObject = GameObject.Find("Traveller_HEA_Chert");
                    OWRigidbody chertBody = chertObject.GetAttachedOWRigidbody();

                    // Setting a variable based on which body Chert is on
                    switch (chertBody.ToString())
                    {
                        case "CaveTwin_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Ember Twin.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_EMBER_TWIN", true);
                            break;
                        case "TowerTwin_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Ash Twin.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_ASH_TWIN", true);
                            break;
                        case "TimberHearth_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Timber Hearth.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_TIMBER_HEARTH", true);
                            break;
                        case "Moon_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on the Attlerock.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_ATTLEROCK", true);
                            break;
                        case "BrittleHollow_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Brittle Hollow.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_BRITTLE_HOLLOW", true);
                            break;
                        case "VolcanicMoon_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Hollow's Lantern.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_HOLLOW_LANTERN", true);
                            break;
                        case "GabbroIsland_Body (OWRigidbody)":
                        case "StatueIsland_Body (OWRigidbody)":
                        case "ConstructionYardIsland_Body (OWRigidbody)":
                        case "QuantumIsland_Body (OWRigidbody)":
                        case "BrambleIsland_Body (OWRigidbody)":
                        case "GiantsDeep_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Giant's Deep.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_GIANTS_DEEP", true);
                            break;
                        case "DarkBramble_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on Dark Bramble.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_DARK_BRAMBLE", true);
                            break;
                        case "QuantumMoon_Body (OWRigidbody)":
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is on the Quantum Moon.", MessageType.Success);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_QUANTUM_MOON", true);
                            break;
                        default:
                            if (chertBody.ToString().Contains("DB_") && chertBody.ToString().Contains("Dimension") && chertBody.ToString().Contains("_Body"))
                            {
                                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert is in Dark Bramble.", MessageType.Success);
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_DARK_BRAMBLE", true);
                            }
                            else
                            {
                                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("No variable set. Chert is on " + chertBody.ToString(), MessageType.Success);
                            }
                            break;
                    }
                }
                else
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("PickUpChert not installed; Chert is on Ember Twin.", MessageType.Success);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ON_EMBER_TWIN", true);
                }
            }
            else
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Not in the vanilla scene; Chert's position doesn't matter.", MessageType.Success);
            }

            // Sets variables depending on what (if anything) the player is holding. //
            DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERTHELD", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);

            try
            {
                // The player is holding an item
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
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                        }
                        // The item is a broken warp core
                        else if ((warpCore._wcType == WarpCoreType.VesselBroken) || (warpCore._wcType == WarpCoreType.SimpleBroken))
                        {
                            // Nothing
                        }
                        // The item is some other kind of warp core
                        else
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                        }
                    }
                    // The item is one of the Stranger's lanterns
                    else if (item._type == ItemType.DreamLantern)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", true);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a vision torch
                    else if (item._type == ItemType.VisionTorch)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a Nomai scroll
                    else if (item._type == ItemType.Scroll)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is one of the stones used to talk with Solanum
                    else if (item._type == ItemType.ConversationStone)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a slide reel
                    else if (item._type == ItemType.SlideReel)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", true);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is Chert
                    else if (item.ToString() == "Traveller_HEA_Chert (PickUpChert.ChertItem)")
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERTHELD", true);
                    }
                    // The item is something else
                    else
                    {
                        // Nothing
                    }
                }
                // The player is not holding an item
                else
                {
                    // Nothing
                }
            }
            catch
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Couldn't set variables for held item.", MessageType.Error);
            }

            GabbroStranger();

            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Done setting conversation variables.", MessageType.Success);
        }

        // Updates Gabbro conversation completion flags mid-conversation
        public void GabbroFlagsWatcher(string str, bool bl)
        {
            switch (str)
            {
                case "GABBRO_RH_STRANGER_RING":
                case "GABBRO_RH_STRANGER_INHABITANTS":
                case "GABBRO_RH_STRANGER_EYE":
                case "GABBRO_RH_STRANGER_DREAMWORLD":
                case "GABBRO_RH_STRANGER_DREAMWORLD_CODE":
                case "GABBRO_RH_STRANGER_PRISONER":
                case "GABBRO_RH_STRANGER_LANTERN":
                case "GABBRO_RH_STRANGER_FRIEND":
                    GabbroStranger();
                    break;
            }
        }

        public void GabbroStranger()
        {
            // This variable is set to true if the player has something new to say about the Stranger to Gabbro
            if (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_RING") == false || DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_INHABITANTS") == false || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_EYE") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_EYE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_LANTERN") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGERLANTERNHELD") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == false && PlayerData.GetPersistentCondition("DREAMWORLD_EVER_BEEN")) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD_CODE") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_DREAM_IS_CODE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_PRISONER") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && Locator.GetShipLogManager().IsFactRevealed("IP_SARCOPHAGUS_X5") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_FRIEND") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && ReactiveHearthians.Instance.ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null && Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_HOME_X1")))
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
