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
    public class DeathTypeTracker : MonoBehaviour
    {
        public static DeathTypeTracker Instance;
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            GlobalMessenger<DeathType>.AddListener("PlayerDeath", DeathTracker);
            GlobalMessenger.AddListener("GhostKillPlayer", OnNeckSnap);
        }

        public void OnNeckSnap()
        {
            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player got their neck snapped.", MessageType.Success);
            PlayerData.SetPersistentCondition("PLAYER_NECK_SNAPPED", true);
        }

        public void DeathTracker(DeathType deathType)
        {
            switch (deathType)
            {
                case DeathType.Default:
                    break;
                case DeathType.Impact:
                    break;
                case DeathType.Asphyxiation:
                    if (Locator.GetPlayerSuit().IsWearingSuit() != true && PlayerState.IsCameraUnderwater())
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player drowned.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_DROWNED", true);
                    }
                    else if (Locator.GetPlayerSuit().IsWearingSuit() != true)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player forgot their space suit.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_FORGOT_SPACE_SUIT", true);
                    }
                    else if (Locator.GetPlayerSuit().IsWearingSuit() == true)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player ran out of oxygen.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_RAN_OUT_OF_O2", true);
                    }
                    break;
                case DeathType.Energy:
                    if (ReactiveHearthians.Instance.loadedScene == "vanilla" && Vector3.Distance(Locator.GetPlayerBody().GetPosition(), ReactiveHearthians.Instance.Giants_Deep.transform.position) <= 500)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player got electrocuted by the core of Giant's Deep.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_ELECTROCUTED_GD_CORE", true);
                    }
                    break;
                // Dying inside the Ash Twin Project
                case DeathType.Supernova:
                    if (Locator.GetPlayerSectorDetector().IsWithinSector(Sector.Name.TimeLoopDevice))
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player died inside of the ATP due to the supernova.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_DIED_INSIDE_ATP", true);
                    }
                    break;
                case DeathType.Digestion:
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player got eaten by an anglerfish.", MessageType.Success);
                    PlayerData.SetPersistentCondition("PLAYER_DIED_ANGLERFISH", true);
                    break;
                case DeathType.BigBang:
                    break;
                case DeathType.Crushed:
                    if (ReactiveHearthians.Instance.loadedScene == "vanilla" && Vector3.Distance(Locator.GetPlayerBody().GetPosition(), ReactiveHearthians.Instance.Ember_Twin.transform.position) <= 170)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player was crushed by sand on Ember Twin.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_CRUSHED_BY_SAND", true);
                    }
                    break;
                case DeathType.Meditation:
                    break;
                // Escaping death
                case DeathType.TimeLoop:
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player escaped the supernova.", MessageType.Success);
                    PlayerData.SetPersistentCondition("PLAYER_ESCAPED_SUPERNOVA", true);

                    if (Locator.GetPlayerSectorDetector().IsWithinSector(Sector.Name.TimeLoopDevice))
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player escaped the supernova via ATP.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_ESCAPED_SUPERNOVA_ATP", true);
                    }
                    else if (Locator.GetRingWorldController() != null && Locator.GetRingWorldController().isPlayerInside)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player escaped the supernova via the Stranger.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_ESCAPED_SUPERNOVA_STRANGER", true);
                    }
                    else
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player escaped the supernova by flying away.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_ESCAPED_SUPERNOVA_OUTOFRANGE", true);
                    }
                    break;
                case DeathType.Lava:
                    if (ReactiveHearthians.Instance.loadedScene == "vanilla" && Vector3.Distance(Locator.GetPlayerBody().GetPosition(), ReactiveHearthians.Instance.Hollows_Lantern.transform.position) <= 100)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player died to lava on Hollow's Lantern.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_DIED_TO_LAVA_HL", true);
                    }
                    break;
                case DeathType.BlackHole:
                    break;
                case DeathType.Dream:
                    // Code for neck snap is handled above
                    break;
                case DeathType.DreamExplosion:
                    ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player used the prototype artifact.", MessageType.Success);
                    PlayerData.SetPersistentCondition("PLAYER_DIED_TO_PROTOTYPE_LANTERN", true);
                    break;
                case DeathType.CrushedByElevator:
                    if (ReactiveHearthians.Instance.loadedScene == "vanilla" && Vector3.Distance(Locator.GetPlayerBody().GetPosition(), HugModStuff.Instance.Slate_Standard.transform.position) <= 15)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Player was crushed by the launch elevator.", MessageType.Success);
                        PlayerData.SetPersistentCondition("PLAYER_CRUSHED_BY_LAUNCH_ELEVATOR", true);
                    }
                    break;
                
            }
        }
    }
}
