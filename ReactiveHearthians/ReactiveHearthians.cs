// THANKS TO: Xen, Lutias Kokopelli, Ixrec, viovayo, Magister Dragon

// TODO LIST:
// Change Riebeck’s introductory message that they start a conversation with if the player has told them about Solanum (sort of how the message Chert greets you with changes if you tell them about the time loop or they realize that the sun is going to blow)
// Also make Riebeck acknowledge that nearby chunks of BH have fallen throughout the loop
// More Solanum interactions
// Angry Mica animation?
// Tell Hal about Solanum?
// THE OUTSIDER - Tell Hal about the Friend?
// Maybe add dialogue for reaching somewhere without your ship???
// Characters react to you standing on top of them
// Porphy reacts to you standing on their pot
// Fix ATP pairing readout if you haven't paired to the statue (and also fix the one statue)
// Add dialogue for slide reel burning
// Add damage dialogue
// Chert wakes you up if you're sleeping at their camp once they go to their third phase
// Pick up Chert dialogue - NOT DOING THIS
// Add dialogue for the Probe landing near NPCs
// Add dialogue for the probe passing by Chert
// Add dialogue for the ship exploding near NPCs

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
// Make Slate an exception and potentially Rutile exceptions to the cowering code given certain conditions
// Some characters have dialogue for you being suited up
// Dialogue for if you're wearing the regular space suit talking to Hornfels/Hal on the first loop
// Dialogue for standing on someone's campfire
// Dialogue for getting damaged by the ghost matter near Arkose
// Bad Mallow dialogue, being high dialogue for Esker
// Add slide reels to interesting items
// Hug mod compat
// Hug dialogue for Gabbro
// Characters react to you dying in front of them
// Change Gabbro dialogue on first loop if you do the geyser skip
// Fix node redirects
// Have Chert's drumming turn chaotic and meloncholy at phases 3 and 4 respectively (kind of accomplished)

// ASTRAL CODEC ADDENDUMS
// - Hearth's Neighbor [done]
// - Magisterium [done]
// - Band Together [done]

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

        // Various publics
        public GameObject Gabbro_Island;
        public GameObject Ember_Twin;
        public GameObject EyeOfTheUniverse;

        public TimeLoopCoreController TheMountain;

        // Mods installed
        public bool Outsider_Installed;
        public bool Astral_Codec_Installed;
        public bool Play_As_Gabbro_Installed;

        // Sectors inside of bodies
        public bool InSector_TimberHearth;
        public bool InSector_BrittleHollow;
        public bool InSector_PioneerDimension;
        public bool InSector_TimberMoon;
        public bool InSector_CaveTwin;
        public bool InSector_GiantsDeep;

        // Positions (broadly speaking) of variable position NPCs
        public string halPosition;
        public string hornfelsPosition;
        public string tephraPosition;
        public string galenaPosition;

        // Loaded scene
        public string loadedScene;

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(ReactiveHearthians)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            // Mods loaded
            if (ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null)
            {
                ModHelper.Console.WriteLine("Outsider installed!", MessageType.Success);
                Outsider_Installed = true;
            }
            else
            {
                Outsider_Installed = false;
            }

            if (ModHelper.Interaction.TryGetMod("Walker.AstralCodex") != null)
            {
                ModHelper.Console.WriteLine("Astral Codec installed!", MessageType.Success);
                Astral_Codec_Installed = true;
            }
            else
            {
                Astral_Codec_Installed = false;
            }

            if (ModHelper.Interaction.TryGetMod("xen.PlayAsGabbro") != null)
            {
                ModHelper.Console.WriteLine("Play as Gabbro installed!", MessageType.Success);
                Play_As_Gabbro_Installed = true;
            }
            else
            {
                Play_As_Gabbro_Installed = false;
            }

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                // Reset these variables
                loadedScene = "unknown";

                // Code dependent on which scene we're loading into
                if (loadScene == OWScene.SolarSystem) 
                {
                    ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
                    loadedScene = "vanilla";

                    // Sets this to true by default
                    InSector_TimberHearth = true;

                    // AC addendums (will move to a separate mod at some point)
                    if (Astral_Codec_Installed)
                    {
                        //newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Codec_Addendum.xml")), "{ pathToExistingDialogue: \"Sector/Station/CodecDispenser/Core/CodecAddendumDialogue\" }", GameObject.Find("LingeringChime_Body"));
                    }

                    // Bodies
                    Gabbro_Island = GameObject.Find("GabbroIsland_Body");
                    Ember_Twin = GameObject.Find("CaveTwin_Body");

                    // ATP
                    TheMountain = UnityEngine.Object.FindObjectOfType<TimeLoopCoreController>();
                }
                else if (loadScene == OWScene.EyeOfTheUniverse)
                {
                    ModHelper.Console.WriteLine("Loaded into the Eye!", MessageType.Success);
                    loadedScene = "eye";

                    EyeOfTheUniverse = GameObject.Find("EyeOfTheUniverse_Body");

                    // DIALOGUE PATCHING HERE TO AVOID ERRORS
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Chert_Eye.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Chert/Traveller_HEA_Chert/ConversationZone_Chert\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Esker_Eye.xml")), "{ pathToExistingDialogue: \"Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Esker/Villager_HEA_Esker/ConversationZone_Esker\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Feldspar_Eye.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Feldspar/Traveller_HEA_Feldspar/ConversationZone_Feldspar\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_Eye.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Gabbro/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Riebeck_Eye.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Riebeck/Traveller_HEA_Riebeck/ConversationZone_Riebeck\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Solanum_Eye.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Solanum/Character_NOM_Solanum/ConversationZone\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Prisoner_Eye_Choice.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Prisoner/PrisonerRoot_Choice/Prisoner_Choice/ConversationVolume_Prisoner\" }", EyeOfTheUniverse);
                    newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Prisoner_Eye_Campfire.xml")), "{ pathToExistingDialogue: \"EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/Campsite/Prisoner/PrisonerRoot_Campfire/Prisoner_Campfire/ConversationVolume_Prisoner\" }", EyeOfTheUniverse);
                }
            };

            GlobalMessenger.AddListener("EnterDreamWorld", DreamWorldBeen);
            GlobalMessenger.AddListener("EnterTimeLoopCentral", AshTwinFix);
        }

        // Harmony patches
        [HarmonyPatch]
        public class MyPatchClass
        {
            // Special patching for Gabbro.
            [HarmonyPostfix]
            [HarmonyPatch(typeof(GabbroDialogueSwapper), nameof(GabbroDialogueSwapper.Start))]
            public static void GabbroDialogueSwapper_Postfix()
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("GabbroDialogueSwapper postfix has been run.", MessageType.Success);

                ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_All.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", ReactiveHearthians.Instance.Gabbro_Island);

                if (TimeLoop.GetLoopCount() == 1)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_1.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", ReactiveHearthians.Instance.Gabbro_Island);
                }
                else if (TimeLoop.GetLoopCount() == 2)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_2.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", ReactiveHearthians.Instance.Gabbro_Island);
                }
                else
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Gabbro_3.xml")), "{ pathToExistingDialogue: \"Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro\" }", ReactiveHearthians.Instance.Gabbro_Island);
                }
            }

            // Special patching for Chert
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ChertDialogueSwapper), nameof(ChertDialogueSwapper.OnStartConversation))]
            public static void ChertDialogueSwapper_Postfix()
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("ChertDialogueSwapper postfix has been run.", MessageType.Success);

                if (TimeLoop.GetMinutesElapsed() >= 20.5f)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Chert_4.xml")), "{ pathToExistingDialogue: \"CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert/ConversationZone_Chert\" }", ReactiveHearthians.Instance.Ember_Twin);
                }
                else if (TimeLoop.GetMinutesElapsed() >= 17f)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Chert_3.xml")), "{ pathToExistingDialogue: \"CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert/ConversationZone_Chert\" }", ReactiveHearthians.Instance.Ember_Twin);
                }
                else if (TimeLoop.GetMinutesElapsed() >= 11f)
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Chert_2.xml")), "{ pathToExistingDialogue: \"CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert/ConversationZone_Chert\" }", ReactiveHearthians.Instance.Ember_Twin);
                }
                else
                {
                    ReactiveHearthians.newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ReactiveHearthians.Instance.ModHelper.Manifest.ModFolderPath, "planets/text/Chert_1.xml")), "{ pathToExistingDialogue: \"CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert/ConversationZone_Chert\" }", ReactiveHearthians.Instance.Ember_Twin);
                }
            }

            // Patching for Mica's wrath
            [HarmonyPostfix]
            [HarmonyPatch(typeof(DestructionVolume), nameof(DestructionVolume.VanishModelRocketShip))]
            public static void OnModelRocketShipDestroyed_Postfix()
            {
                if (TimeLoop.GetSecondsElapsed() < Cowering.Instance.boomTime)
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("MICAS_WRATH", true);
                }
            }

            // Sets variables for which planet the player is on, as needed
            // SectorEnter fires for every sector entered; it's possible to be in multiple sectors, I think? Don't add an else.
            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlayerSectorDetector), nameof(PlayerSectorDetector.OnAddSector))]
            public static void SectorEnter(Sector sector)
            {
                //ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player has entered sector \""+sector.gameObject.name+"\"", MessageType.Success);

                if (sector.gameObject.name == "Sector_TH")
                {
                    ReactiveHearthians.Instance.InSector_TimberHearth = true;
                }
                else if (sector.gameObject.name == "Sector_THM")
                {
                    ReactiveHearthians.Instance.InSector_TimberMoon = true;
                }
                else if (sector.gameObject.name == "Sector_CaveTwin")
                {
                    ReactiveHearthians.Instance.InSector_CaveTwin = true;
                }
                else if (sector.gameObject.name == "Sector_BH")
                {
                    ReactiveHearthians.Instance.InSector_BrittleHollow = true;
                }
                else if (sector.gameObject.name == "Sector_GabbroIsland")
                {
                    ReactiveHearthians.Instance.InSector_GiantsDeep = true;
                }
                else if (sector.gameObject.name == "Sector_PioneerDimension")
                {
                    ReactiveHearthians.Instance.InSector_PioneerDimension = true;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlayerSectorDetector), nameof(PlayerSectorDetector.OnRemoveSector))]
            public static void SectorExit(Sector sector)
            {
                //ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player has exited sector \""+sector.gameObject.name+ "\"", MessageType.Success);

                if (sector.gameObject.name == "Sector_TH")
                {
                    ReactiveHearthians.Instance.InSector_TimberHearth = false;
                }
                else if (sector.gameObject.name == "Sector_THM")
                {
                    ReactiveHearthians.Instance.InSector_TimberMoon = false;
                }
                else if (sector.gameObject.name == "Sector_CaveTwin")
                {
                    ReactiveHearthians.Instance.InSector_CaveTwin = false;
                }
                else if (sector.gameObject.name == "Sector_BH")
                {
                    ReactiveHearthians.Instance.InSector_BrittleHollow = false;
                }
                else if (sector.gameObject.name == "Sector_GabbroIsland")
                {
                    ReactiveHearthians.Instance.InSector_GiantsDeep = false;
                }
                else if (sector.gameObject.name == "Sector_PioneerDimension")
                {
                    ReactiveHearthians.Instance.InSector_PioneerDimension = false;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(OrbitalProbeLaunchController), nameof(OrbitalProbeLaunchController.Awake))]
            public static void OPC_Angle_Lock()
            {
                // fill in
            }
        }

        // Determines the positions of Hal, Hornfels, Tephra, and Galena.
        public void GetVariableNPCPositions()
        {
            // Hal and Hornfels
            if (TimeLoop.GetLoopCount() == 1)
            {
                hornfelsPosition = "STANDARD";

                if (PlayerData.GetPersistentCondition("RH_NOMAI_STATUE_LINKED"))
                {
                    halPosition = "OUTSIDE";
                }
            }
            else
            {
                hornfelsPosition = "DOWNSTAIRS";
                halPosition = "STANDARD";
            }

            // Tephra and Galena
            if (TimeLoop.GetLoopCount() == 1 && DialogueConditionManager.SharedInstance.GetConditionState("RH_NOMAI_STATUE_LINKED"))
            {
                tephraPosition = "POST_OBSERVATORY";
                galenaPosition = "STANDARD";
            }
            else if (DialogueConditionManager.SharedInstance.GetConditionState("BeginHideAndSeek") || DialogueConditionManager.SharedInstance.GetConditionState("EndHideAndSeek") == false)
            {
                tephraPosition = "HAS";
                galenaPosition = "HAS";
            }
            else
            {
                tephraPosition = "STANDARD";
                galenaPosition = "STANDARD";
            }
        }

        // Dream world been to detection
        public void DreamWorldBeen()
        {
            PlayerData.SetPersistentCondition("DREAMWORLD_EVER_BEEN", true);
        }

        // Statue readout & glow fix
        public void AshTwinFix()
        {
            // Add this!
        }
    }
}
