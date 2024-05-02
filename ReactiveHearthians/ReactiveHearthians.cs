﻿// THANKS TO: Xen, Lutias Kokopelli, Ixrec, viovayo, Magister Dragon

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
// Have Chert's drumming turn chaotic and meloncholy at phases 3 and 4 respectively
// Chert wakes you up if you're sleeping at their camp once they go to their third phase

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

// ASTRAL CODEC ADDENDUMS
// - Hearth's Neighbor [done]
// - Magisterium [done]
// - Band Together [done]

using HarmonyLib;
using HugMod;
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

        // Various publics
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

        public CowerAnimTriggerVolume cower_volume_mica;
        public CowerAnimTriggerVolume cower_volume_rutile;
        public CowerAnimTriggerVolume cower_volume_porphy;

        public GameObject Gabbro_Island;
        public GameObject Ember_Twin;
        public GameObject EyeOfTheUniverse;

        public List<BadMarshmallowCan> badcans;
        private List<CowerAnimTriggerVolume> cower_volumes;

        public IHugModApi hugApi;

        public GameObject[] huggables;

        public TimeLoopCoreController TheMountain;

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

        // Positions (broadly speaking) of variable position NPCs
        public string halPosition;
        public string hornfelsPosition;
        public string tephraPosition;
        public string galenaPosition;

        // Audio
        public AudioClip ChertMusic_3;
        public AudioClip ChertMusic_4;
        public bool Chert3Swap_Done;
        public bool Chert4Swap_Done;

        private void Update()
        {
            var audioTable = Locator.GetAudioManager()._audioLibraryDict;

            if (Chert4Swap_Done == false && TimeLoop.GetMinutesElapsed() >= 20.5f)
            {
                ModHelper.Console.WriteLine("Chert's music changed to version 4.", MessageType.Success);
                Chert4Swap_Done = true;
                audioTable[(int)AudioType.TravelerChert] = new AudioLibrary.AudioEntry(AudioType.TravelerChert, new[] { ChertMusic_4 });
            }
            else if (Chert3Swap_Done == false && TimeLoop.GetMinutesElapsed() >= 17f)
            {
                ModHelper.Console.WriteLine("Chert's music changed to version 3.", MessageType.Success);
                Chert3Swap_Done = true;
                audioTable[(int)AudioType.TravelerChert] = new AudioLibrary.AudioEntry(AudioType.TravelerChert, new[] { ChertMusic_3 });
            }
            else if (TimeLoop.GetMinutesElapsed() >= 11f)
            {
                // Nothing
            }
            else
            {
                // Nothing
            }
        }

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(ReactiveHearthians)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            // Hug API
            hugApi = ModHelper.Interaction.TryGetModApi<IHugModApi>("VioVayo.HugMod");

            // Mods loaded
            if (ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null)
            {
                Outsider_Installed = true;
            }
            else
            {
                Outsider_Installed = false;
            }

            if (ModHelper.Interaction.TryGetMod("Walker.AstralCodex") != null)
            {
                Astral_Codec_Installed = true;
            }
            else
            {
                Astral_Codec_Installed = false;
            }

            if (ModHelper.Interaction.TryGetMod("xen.PlayAsGabbro") != null)
            {
                Play_As_Gabbro_Installed = true;
            }
            else
            {
                Play_As_Gabbro_Installed = false;
            }

            // Getting Chert's custom music files
            ChertMusic_3 = ModHelper.Assets.GetAudio("planets/music/Chert_UpSemitone.wav");
            ChertMusic_4 = ModHelper.Assets.GetAudio("planets/music/Chert_DownSemitone.wav");

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                // Makes the badmallow list
                badcans = Resources.FindObjectsOfTypeAll<BadMarshmallowCan>().ToList();

                // Reset these variables
                AllCower = false;
                boomTime = 1360;
                Chert4Swap_Done = false;
                Chert3Swap_Done = false;

                // The below code only runs on loading into the vanilla solar system
                if (loadScene == OWScene.SolarSystem) 
                {
                    ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);

                    // Sets this to true by default
                    InSector_TimberHearth = true;

                    // AC addendums (will move to a separate mod at some point)
                    if (Astral_Codec_Installed)
                    {
                        newHorizons.CreateDialogueFromXML(null, File.ReadAllText(Path.Combine(ModHelper.Manifest.ModFolderPath, "planets/text/Codec_Addendum.xml")), "{ pathToExistingDialogue: \"Sector/Station/CodecDispenser/Core/CodecAddendumDialogue\" }", GameObject.Find("LingeringChime_Body"));
                    }

                    // Campfires people are sat near
                    slatefire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    riebeckfire = GameObject.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Interactables_Crossroads/VisibleFrom_BH/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    eskerfire = GameObject.Find("Moon_Body/Sector_THM/Interactables_THM/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    chertfire = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Lakebed_VisibleFrom_Far/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();
                    feldsparfire = GameObject.Find("DB_PioneerDimension_Body/Sector_PioneerDimension/Interactables_PioneerDimension/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();

                    hazardvolume_slatefire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_riebeckfire = GameObject.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Interactables_Crossroads/VisibleFrom_BH/Prefab_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_eskerfire = GameObject.Find("Moon_Body/Sector_THM/Interactables_THM/Effects_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_chertfire = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Lakebed_VisibleFrom_Far/Prefab_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();
                    hazardvolume_feldsparfire = GameObject.Find("DB_PioneerDimension_Body/Sector_PioneerDimension/Interactables_PioneerDimension/Prefab_HEA_Campfire/HeatHazardVolume").GetComponent<HazardVolume>();

                    darkmattervolume_arkose = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/DarkMatterVolume").GetComponent<DarkMatterVolume>();

                    // Specific cower volumes
                    cower_volume_mica = GameObject.Find("Villager_HEA_Mica/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();
                    cower_volume_rutile = GameObject.Find("Villager_HEA_Rutile/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();
                    cower_volume_porphy = GameObject.Find("Villager_HEA_Porphy/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();

                    // Bodies
                    Gabbro_Island = GameObject.Find("GabbroIsland_Body");
                    Ember_Twin = GameObject.Find("CaveTwin_Body");

                    // ATP
                    TheMountain = UnityEngine.Object.FindObjectOfType<TimeLoopCoreController>();

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
                    ModHelper.Console.WriteLine("Loaded into the Eye!", MessageType.Success);

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

            GlobalMessenger.AddListener("EnterConversation", OnEnterConversation);
            GlobalMessenger.AddListener("ExitConversation", OnExitConversation);
            GlobalMessenger<string, bool>.AddListener("DialogueConditionChanged", MakeMicaCower);
            GlobalMessenger.AddListener("EnterDreamWorld", DreamWorldBeen);
            GlobalMessenger.AddListener("TriggerMemoryUplink", StatueLinked);
            GlobalMessenger.AddListener("EnterTimeLoopCentral", AshTwinFix);
            GlobalMessenger<Campfire>.AddListener("EnterRoastingMode", EnterRoastingMode);
            GlobalMessenger<Campfire>.AddListener("ExitRoastingMode", ExitRoastingMode);
            GlobalMessenger<float>.AddListener("EatMarshmallow", EatMarshmallow);
            GlobalMessenger.AddListener("EnableBigHeadMode", BigHeadMode);
            GlobalMessenger.AddListener("TriggerSupernova", MakeAllCower_Supernova);
            GlobalMessenger<DeathType>.AddListener("PlayerDeath", MakeAllCower_Death);
        }

        // Single-frame delay coroutine
        private IEnumerator WaitAGoshDarnedFrame_SolarSystem()
        {
            yield return null;

            // Getting all the cower_volumes into a list. This MUST be done here and not on Start(), for some reason
            cower_volumes = Resources.FindObjectsOfTypeAll<CowerAnimTriggerVolume>().ToList();

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
            if (TimeLoop.GetSecondsElapsed() >= boomTime)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_"+person+"_HUGGED_SUPERNOVA", true);
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

            DialogueConditionManager.SharedInstance.SetConditionState("RH_"+person+"_HUGGED", true);
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
                if (TimeLoop.GetSecondsElapsed() < 1330)
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
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Tektite_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.tektite_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Tuff_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_TUFF_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.tuff_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Gossan_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_GOSSAN_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.gossan_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Arkose_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_ARKOSE_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.arkose_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Moraine_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_MORAINE_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.moraine_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        else if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Slate_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.slate_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                        // Non-loner NPCs
                        else
                        {
                            // Tephra
                            if (ReactiveHearthians.Instance.tephraPosition == "STANDARD" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Tephra_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.tephra_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.tephraPosition == "HAS" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Tephra_HAS.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.tephra_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.tephraPosition == "POST_OBSERVATORY" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Tephra_PostObservatory.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.tephra_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Galena
                            if (ReactiveHearthians.Instance.galenaPosition == "STANDARD" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Galena_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_GALENA_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.galena_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.galenaPosition == "HAS" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Galena_HAS.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_GALENA_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.galena_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Hornfels
                            if (ReactiveHearthians.Instance.hornfelsPosition == "STANDARD" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Hornfels_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HORNFELS_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.hornfels_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.hornfelsPosition == "DOWNSTAIRS" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Hornfels_Downstairs.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HORNFELS_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.hornfels_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Hal
                            if (ReactiveHearthians.Instance.halPosition == "STANDARD" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Hal_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HAL_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.hal_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            else if (ReactiveHearthians.Instance.halPosition == "OUTSIDE" && Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Hal_Outside.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_HAL_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.hal_last_damaged = TimeLoop.GetSecondsElapsed();
                            }

                            // Everybody else: Gneiss, Marl, Mica, Porphy, Rutile, Spinel
                            if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Gneiss_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_GNEISS_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.gneiss_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Marl_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_MARL_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.marl_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Mica_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_MICA_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.mica_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Porphy_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_PORPHY_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.porphy_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Rutile_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_RUTILE_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.rutile_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                            if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Spinel_Standard.transform.position) <= reactRadius)
                            {
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_SPINEL_IMPACT_DAMAGE", true);
                                ReactiveHearthians.Instance.spinel_last_damaged = TimeLoop.GetSecondsElapsed();
                            }
                        }
                    }
                    else if (damageType == InstantDamageType.Puncture)
                    {
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Tektite_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_PUNCTURE_DAMAGE", true);
                            ReactiveHearthians.Instance.tektite_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on the Attlerock
                else if (ReactiveHearthians.Instance.InSector_TimberMoon)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on the Attlerock.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Esker_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.esker_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on Ember Twin
                else if (ReactiveHearthians.Instance.InSector_CaveTwin)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Ember Twin.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Chert_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.chert_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on Brittle Hollow
                else if (ReactiveHearthians.Instance.InSector_BrittleHollow)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Brittle Hollow.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Riebeck_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.riebeck_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is on Gabbro's Island
                else if (ReactiveHearthians.Instance.InSector_GiantsDeep)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken on Gabbro's Island.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Gabbro_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.gabbro_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                // Player is in Feldspar's dimension
                else if (ReactiveHearthians.Instance.InSector_PioneerDimension)
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken in Feldspar's dimension.", MessageType.Success);
                    if (damageType == InstantDamageType.Impact)
                    {
                        if (Vector3.Distance(playerPosition, ReactiveHearthians.Instance.Feldspar_Standard.transform.position) <= reactRadius)
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_IMPACT_DAMAGE", true);
                            ReactiveHearthians.Instance.feldspar_last_damaged = TimeLoop.GetSecondsElapsed();
                        }
                    }
                }
                else
                {
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Damage was taken elsewhere.", MessageType.Success);
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
                    if (eVolume == ReactiveHearthians.Instance.hazardvolume_slatefire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_FIRE_DAMAGED", true);
                        ReactiveHearthians.Instance.hazardvolume_slatefire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == ReactiveHearthians.Instance.hazardvolume_riebeckfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_FIRE_DAMAGED", true);
                        ReactiveHearthians.Instance.hazardvolume_riebeckfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == ReactiveHearthians.Instance.hazardvolume_eskerfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_FIRE_DAMAGED", true);
                        ReactiveHearthians.Instance.hazardvolume_eskerfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == ReactiveHearthians.Instance.hazardvolume_chertfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_FIRE_DAMAGED", true);
                        ReactiveHearthians.Instance.hazardvolume_chertfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == ReactiveHearthians.Instance.hazardvolume_feldsparfire)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_FIRE_DAMAGED", true);
                        ReactiveHearthians.Instance.hazardvolume_feldsparfire_lasttouched = TimeLoop.GetSecondsElapsed();
                    }
                    else if (eVolume == ReactiveHearthians.Instance.darkmattervolume_arkose)
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

        // Hearthians cowering //
        private void MakeMicaCower(string name, bool state)
        {
            if (name == "MODELROCKETKID_RH_DISTRAUGHT" && state)
            {
                cower_volume_mica.StartCoroutine(Coweroutine(cower_volume_mica._animator, 970));
            }
        }

        public bool AllCower;
        public float boomTime;
        private void MakeAllCower_Supernova()
        {
            ModHelper.Console.WriteLine("MakeAllCower_Supernova triggered.", MessageType.Success);
            
            if (AllCower)
            {
                // Do nothing; function has already been run before
            }
            else
            {
                // Removing certain people from the function,
                if (DialogueConditionManager.SharedInstance.GetConditionState("MODELROCKETKID_RH_DISTRAUGHT"))
                {
                    // Remove Mica; they are already cowering //
                    cower_volumes.Remove(cower_volume_mica);
                }
                else if (DialogueConditionManager.SharedInstance.GetConditionState("RUTILE_RH_DISTRAUGHT"))
                {
                    // Remove Rutile; they were informed of the supernova beforehand and is calm (ignore the variable name being called 'distraught') //
                    cower_volumes.Remove(cower_volume_rutile);
                }
                else if (DialogueConditionManager.SharedInstance.GetConditionState("PORPHY_RH_DISTRAUGHT"))
                {
                    // Remove Porphy; same as above but for Porphy //
                    cower_volumes.Remove(cower_volume_porphy);
                }

                boomTime = TimeLoop.GetSecondsElapsed() + 10;

                // Iterates through each cower_volume and runs the coweroutine on each
                foreach (CowerAnimTriggerVolume cower_volume in cower_volumes)
                {
                    if (cower_volume.gameObject.activeInHierarchy)
                    {
                        cower_volume.StartCoroutine(Coweroutine(cower_volume._animator, boomTime));
                    }
                }

                // Gets rid of Gneiss' banjo sound
                StartCoroutine(Banjoroutine(boomTime));

                // Sets a flag so this function is run only once.
                AllCower = true;
            }
        }
        private void MakeAllCower_Death(DeathType deathType)
        {
            ModHelper.Console.WriteLine("MakeAllCower_Death triggered.", MessageType.Success);

            if (AllCower)
            {
                // Do nothing; function has already been run before
            }
            else
            {
                // Iterates through each cower_volume and runs the coweroutine on each
                foreach (CowerAnimTriggerVolume cower_volume in cower_volumes)
                {
                    if (cower_volume.gameObject.activeInHierarchy)
                    {
                        cower_volume.StartCoroutine(Coweroutine(cower_volume._animator, 0));
                    }
                }

                // Gets rid of Gneiss' banjo sound
                StartCoroutine(Banjoroutine(0));

                // Sets a flag so this function is run only once.
                AllCower = true;
            }
        }

        private IEnumerator Coweroutine(Animator animator, float time)
        {
            if (hugApi != null)
            {
                GameObject current = animator.gameObject, huggable = null;
                while (huggable == null && current != null)
                {
                    if (huggables.Contains(current)) huggable = current;
                    current = current.transform.parent?.gameObject;
                }
                if (huggable != null) hugApi.SetAnimationTrigger(huggable, (int)HugTrigger.None);
            }

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

        private IEnumerator Banjoroutine(float time)
        {
            var banjo = GameObject.Find("AudioSource_BanjoTuning");
            while (banjo != null && TimeLoop.GetSecondsElapsed() < time) yield return null;
            if (banjo != null) banjo.SetActive(false);
        }

        // Dream world been to detection
        public void DreamWorldBeen()
        {
            PlayerData.SetPersistentCondition("DREAMWORLD_EVER_BEEN", true);
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

        // Statue readout & glow fix
        public void AshTwinFix()
        {
            // Add this!
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

        // Resetting dialogue variables
        public void OnExitConversation()
        {
            // Resetting all hug & damage variables to false
            string[] people = { "ARKOSE", "CHERT", "ESKER", "FELDSPAR", "GABBRO", "GALENA", "GNEISS", "GOSSAN", "HAL", "HORNFELS", "MARL", "MICA", "MORAINE", "PORPHY", "RIEBECK", "RUTILE", "SLATE", "SPINEL", "TEKTITE", "TEPHRA", "TUFF", "PRISONER_CHOICE", "PRISONER_CAMPFIRE", "SOLANUM" };
            foreach (string person in people)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_"+person+"_HUGGED", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_"+person+"_IMPACT_DAMAGE", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_"+person+"_PUNCTURE_DAMAGE", false);
            }

            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S4", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S3", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S2", false);
            DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_HUGGED_S1", false);
        }

        // Dialogue variables
        public void OnEnterConversation()
        {
            // Installed mod variables. //
            DialogueConditionManager.SharedInstance.SetConditionState("ASTRAL_CODEC", Astral_Codec_Installed);
            DialogueConditionManager.SharedInstance.SetConditionState("THE_OUTSIDER", Outsider_Installed);

            DialogueConditionManager.SharedInstance.SetConditionState("PLAY_AS_GABBRO", Play_As_Gabbro_Installed);
            DialogueConditionManager.SharedInstance.SetConditionState("NOPLAY_AS_GABBRO", !Play_As_Gabbro_Installed);

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
            if (TimeLoop.GetSecondsElapsed() >= boomTime)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_BOOM", true);
            }

            // Campfire recency checks //
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_SLATE_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_SLATE_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - hazardvolume_slatefire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_RIEBECK_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_RIEBECK_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - hazardvolume_riebeckfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_ESKER_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_ESKER_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - hazardvolume_eskerfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_CHERT_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_CHERT_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - hazardvolume_chertfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_FIRE_DAMAGED_RECENT", false);
            }

            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_FELDSPAR_FIRE_DIALOGUE_THISLOOP") != true && DialogueConditionManager.SharedInstance.GetConditionState("RH_FELDSPAR_FIRE_DAMAGED") && TimeLoop.GetSecondsElapsed() - hazardvolume_feldsparfire_lasttouched <= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_FIRE_DAMAGED_RECENT", true);
            }
            else
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_FIRE_DAMAGED_RECENT", false);
            }

            // Resetting badmallow dialogue, we do this differently than above but it's effectively the same thing //
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_SLATE_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - slatefire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_RIEBECK_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - riebeckfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_ESKER_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - eskerfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_CHERT_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - chertfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_ATE_BAD_MALLOW", false);
            }
            if (DialogueConditionManager.SharedInstance.GetConditionState("RH_FELDSPAR_ATE_BAD_MALLOW_DIALOGUE_THIS_LOOP") || TimeLoop.GetSecondsElapsed() - feldsparfire_badmallow_lastate >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_ATE_BAD_MALLOW", false);
            }

            // Resetting damage dialogue variables!! Add this!! //
            if (TimeLoop.GetSecondsElapsed() - arkose_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ARKOSE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - chert_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - esker_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_ESKER_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - feldspar_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_FELDSPAR_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - gabbro_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GABBRO_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - galena_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GALENA_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - gneiss_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GNEISS_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - gossan_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_GOSSAN_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - hal_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_HAL_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - hornfels_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_HORNFELS_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - marl_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_MARL_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - mica_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_MICA_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - moraine_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_MORAINE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - porphy_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_PORPHY_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - riebeck_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RIEBECK_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - rutile_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_RUTILE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - slate_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLATE_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - spinel_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SPINEL_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - tektite_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_IMPACT_DAMAGE", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEKTITE_PUNCTURE_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - tephra_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TEPHRA_IMPACT_DAMAGE", false);
            }
            if (TimeLoop.GetSecondsElapsed() - tuff_last_damaged >= 10)
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_TUFF_IMPACT_DAMAGE", false);
            }

            // Misc variables //
            // This variable is set true if the ATP is deactivated
            if (TheMountain != null && (TheMountain._warpCoreSocket.IsSocketOccupied() && TheMountain._warpCoreSocket.GetWarpCoreType() == WarpCoreType.Vessel) == false)
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
            if (tephraPosition == "STANDARD" && galenaPosition == "STANDARD")
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
            else if (ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null && Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_HOME_X1"))
            {
                PlayerData.SetPersistentCondition("DREAMWORLD_EVER_BEEN", true);
            }

            // This variable is set true if the player knows the dream world is a simulation.
            if (Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_1_RULE_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_2_RULE_X2") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_3_RULE_X1") || Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_3_STORY_X2"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGER_DREAM_IS_CODE", true);
            }

            // Sets variables depending on what (if anything) the player is holding. //
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
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                        }
                        // The item is a broken warp core
                        else if ((warpCore._wcType == WarpCoreType.VesselBroken) || (warpCore._wcType == WarpCoreType.SimpleBroken))
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
                        }
                        // The item is some other kind of warp core
                        else
                        {
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", true);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                            DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                        }
                    }
                    // The item is one of the Stranger's lanterns
                    else if (item._type == ItemType.DreamLantern)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", true);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a vision torch
                    else if (item._type == ItemType.VisionTorch)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a Nomai scroll
                    else if (item._type == ItemType.Scroll)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is a slide reel
                    else if (item._type == ItemType.SlideReel)
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", true);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", true);
                    }
                    // The item is something else
                    else
                    {
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                        DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
                    }
                }
                // The player is not holding an item
                else
                {
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                    DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
                }
            }
            catch
            {
                DialogueConditionManager.SharedInstance.SetConditionState("RH_AWCHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_WARPCOREHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_STRANGERLANTERNHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_SLIDEREELHELD", false);
                DialogueConditionManager.SharedInstance.SetConditionState("RH_COOLTHINGHELD", false);
            }

            // This variable is set to true if the player has something new to say about the Stranger to Gabbro
            if (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_RING") == false || DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_INHABITANTS") == false || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_EYE") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_EYE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_LANTERN") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGERLANTERNHELD") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == false && PlayerData.GetPersistentCondition("DREAMWORLD_EVER_BEEN")) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD_CODE") == false && DialogueConditionManager.SharedInstance.GetConditionState("RH_STRANGER_DREAM_IS_CODE") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_PRISONER") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && Locator.GetShipLogManager().IsFactRevealed("IP_SARCOPHAGUS_X5") == true) || (DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_FRIEND") == false && DialogueConditionManager.SharedInstance.GetConditionState("GABBRO_RH_STRANGER_DREAMWORLD") == true && ModHelper.Interaction.TryGetMod("SBtT.TheOutsider") != null && Locator.GetShipLogManager().IsFactRevealed("IP_DREAM_HOME_X1")))
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
