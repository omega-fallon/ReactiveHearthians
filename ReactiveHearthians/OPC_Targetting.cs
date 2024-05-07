using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReactiveHearthians.planets
{
    public class OPC_Targetting : MonoBehaviour
    {
        private bool _hasHitplayer;
        private OWRigidbody player;
        private OrbitalProbeLaunchController launchController;
        private OWRigidbody probe;

        public void Awake()
        {
            LoadManager.OnCompleteSceneLoad += OnSceneLoad;
        }

        void OnSceneLoad(OWScene oldScene, OWScene newScene)
        {
            _hasHitplayer = false;
        }

        public void FixedUpdate()
        {
            if (LoadManager.GetCurrentScene() != OWScene.SolarSystem)
            {
                return;
            }

            if (_hasHitplayer)
            {
                return;
            }

            if (launchController == null)
            {
                launchController = Resources.FindObjectsOfTypeAll<OrbitalProbeLaunchController>().First();
            }

            if (!launchController._hasLaunchedProbe)
            {
                return;
            }

            if (player == null)
            {
                player = Locator.GetPlayerBody();
            }

            if (probe == null)
            {
                probe = launchController._probeBody;
                var align = probe.gameObject.AddComponent<AlignWithTargetBody>();
                align.SetTargetBody(Locator.GetPlayerBody());
                align.SetUsePhysicsToRotate(true);
                align.SetLocalAlignmentAxis(new Vector3(-1, 0, 0));
            }

            var approachSpeed = probe.GetVelocity().magnitude - player.GetVelocity().magnitude;

            var newSpeed = Mathf.Max(500, player.GetVelocity().magnitude + 500f);

            probe.SetVelocity(probe.transform.TransformDirection(new Vector3(-1, 0, 0) * newSpeed));

            if (Vector3.Distance(player.GetPosition(), probe.GetPosition()) <= 50f)
            {
                ReactiveHearthians.Instance.ModHelper.Console.WriteLine($"HIT player!");
                _hasHitplayer = true;
                probe.GetComponent<AlignWithTargetBody>().enabled = false;
            }
        }
    }
}
