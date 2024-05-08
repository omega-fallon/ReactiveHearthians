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

                    float chertProbeDist = Vector3.Distance(probe.transform.position, HugModStuff.Instance.Chert_Standard.transform.position);
                    float emberProbeDist = Vector3.Distance(probe.transform.position, ReactiveHearthians.Instance.Ember_Twin.transform.position);
                    float chertEmberDist = Vector3.Distance(HugModStuff.Instance.Chert_Standard.transform.position, ReactiveHearthians.Instance.Ember_Twin.transform.position);

                    // Getting a new lowest distance
                    if (chertProbeDist < chertProbeLowestDistance)
                    {
                        chertProbeLowestDistance = chertProbeDist;
                        //ReactiveHearthians.Instance.ModHelper.Console.WriteLine("New lowest distance! Chert and the Probe are " + chertProbeLowestDistance.ToString() + " meters away.", MessageType.Success);
                    }

                    // Probe is in range for testing
                    if (chertProbeSpotted == false && chertProbeDist < 170)
                    {
                        ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Probe is in range for testing.", MessageType.Success);

                        if (emberProbeDist < chertEmberDist)
                        {
                            // The probe is closer to Ember Twin than Chert is to Ember Twin; this means the probe is below Chert's line of sight. Do nothing.
                        }
                        else
                        {
                            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("Chert has spotted the probe!", MessageType.Success);
                            chertProbeSpotted = true;
                        }
                    }
                }
            }
        }
    }
}
