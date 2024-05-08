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
using UnityEngine.InputSystem.HID;

namespace ReactiveHearthians
{
    public class ProbeStuff : MonoBehaviour
    {
        public static ProbeStuff Instance;
        public void Awake()
        {
            Instance = this;
        }

        public float chertProbeLowestDistance = 9999999;
        public OrbitalProbeLaunchController launchController;
        public OWRigidbody probe;
        public bool chertProbeSpotted;

        public void Update()
        {
            if (ReactiveHearthians.Instance.loadedScene == "vanilla")
            {
                if (TimeLoop.GetMinutesElapsed() <= 1f)
                {
                    if (launchController == null)
                    {
                        launchController = Resources.FindObjectsOfTypeAll<OrbitalProbeLaunchController>().First();
                    }
                    if (probe == null)
                    {
                        probe = launchController._probeBody;
                    }

                    if (chertProbeSpotted == false)
                    {
                        float chertProbeDist = Vector3.Distance(probe.transform.position, HugModStuff.Instance.Chert_Standard.transform.position);

                        // Probe is in range for testing
                        if (chertProbeDist < 240)
                        {
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Probe is in range for testing; drawing a raycast.", MessageType.Success);

                            // Copied raycast code
                            int layerMask = OWLayerMask.groundMask;

                            Vector3 chertPos = HugModStuff.Instance.Chert_Standard.transform.position;
                            Vector3 probePos = probe.transform.position;

                            RaycastHit hit;
                            Physics.Raycast(chertPos, probePos - chertPos, out hit, 240, layerMask);
                            Debug.DrawRay(chertPos, probePos - chertPos * hit.distance, Color.yellow);

                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Raycast hit this thing: " + hit.collider.ToString(), MessageType.Success);

                            if (hit.collider.ToString() == "Structure_NOM_Probe_Renderer (UnityEngine.MeshCollider)")
                            {
                                ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert has spotted the probe!", MessageType.Success);
                                chertProbeSpotted = true;
                                DialogueConditionManager.SharedInstance.SetConditionState("RH_CHERT_PROBE_SPOTTED", true);
                            }
                        }
                    }
                }
            }
        }
    }
}
