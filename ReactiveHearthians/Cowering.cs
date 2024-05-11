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
    public class Cowering : MonoBehaviour
    {
        public static Cowering Instance;
        public void Awake()
        {
            Instance = this;
        }

        public CowerAnimTriggerVolume cower_volume_mica;
        public CowerAnimTriggerVolume cower_volume_rutile;
        public CowerAnimTriggerVolume cower_volume_porphy;

        public List<CowerAnimTriggerVolume> cower_volumes;

        public void Start()
        {
            GlobalMessenger.AddListener("TriggerSupernova", MakeAllCower_Supernova);
            GlobalMessenger<DeathType>.AddListener("PlayerDeath", MakeAllCower_Death);
            GlobalMessenger<string, bool>.AddListener("DialogueConditionChanged", MakeMicaCower);

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                AllCower = false;
                boomTime = 1360;

                if (loadScene == OWScene.SolarSystem)
                {
                    // Specific cower volumes
                    cower_volume_mica = GameObject.Find("Villager_HEA_Mica/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();
                    cower_volume_rutile = GameObject.Find("Villager_HEA_Rutile/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();
                    cower_volume_porphy = GameObject.Find("Villager_HEA_Porphy/CowerAnimTrigger").GetComponent<CowerAnimTriggerVolume>();

                    StartCoroutine(WaitAGoshDarnedFrame());
                }
            };
        }

        private IEnumerator WaitAGoshDarnedFrame()
        {
            yield return null;

            // Getting all the cower_volumes into a list. This MUST be done here and not on Start(), for some reason
            cower_volumes = Resources.FindObjectsOfTypeAll<CowerAnimTriggerVolume>().ToList();
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
        public float boomTime = 1360;
        private void MakeAllCower_Supernova()
        {
            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("MakeAllCower_Supernova triggered.", MessageType.Success);

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
            ReactiveHearthians.Instance.ModHelper.Console.WriteLine("MakeAllCower_Death triggered.", MessageType.Success);

            if (AllCower)
            {
                // Do nothing; function has already been run before
            }
            else if (deathType == DeathType.TimeLoop || deathType == DeathType.Meditation)
            {
                // Do nothing, this is the "purple lines at the edge of the screen" death and meditation death respectively
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

        public IEnumerator Coweroutine(Animator animator, float time_min, float time_max=9999)
        {
            if (HugModStuff.Instance.hugApi != null)
            {
                GameObject current = animator.gameObject, huggable = null;
                while (huggable == null && current != null)
                {
                    if (HugModStuff.Instance.huggables.Contains(current)) huggable = current;
                    current = current.transform.parent?.gameObject;
                }
                if (huggable != null) HugModStuff.Instance.hugApi.SetAnimationTrigger(huggable, (int)HugTrigger.None);
            }

            while (TimeLoop.GetSecondsElapsed() < time_min) yield return null;
            animator.SetTrigger("ProbeDodge");
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Cower 2") && !animator.GetCurrentAnimatorStateInfo(1).IsName("Cower 2")) yield return null;
            var n = animator.GetCurrentAnimatorStateInfo(0).IsName("Cower 2") ? 0 : 1;
            while (TimeLoop.GetSecondsElapsed() < time_max)
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
    }
}
