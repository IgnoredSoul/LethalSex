using LethalSex_Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace LethalSanity.Modules
{
    internal class StalkerHallucination : LethalClass
    {
        public static StalkerHallucination Module { get; private set; }

        /// <summary>
        /// Assign Module to this instance.
        /// </summary>
/*        protected override void OnRegister()
        {
            // Unregister this module if disabled in the config.
            if (!Config.SH_ToggleModule) { ManualUnregister(); return; }

            base.OnRegister();
            Module = this;
        }*/

        /// <summary>
        /// If the module unregisters, unload everything.
        /// </summary>
        protected override void Unregister()
        {
            // Try to destroy the object, if it cannot doesn't matter cause it doesnt exist lol.
            Extensions.TryDestroy(stalker);

            base.Unregister();
            Module = null;
        }

        protected override void OnShipLand()
        {
            stalker = Instantiate(Resources.FindObjectsOfTypeAll<FlowermanAI>()[0].gameObject);
            Destroy(stalker.GetComponent<FlowermanAI>());
            stalker.AddComponent<StalkerHallucination>();
        }

        private NavMeshAgent agent { get; set; }

        private GameObject[] aiNodes { get; set; }
        internal bool runningAway { get; set; }

        public GameObject stalker { get; private set; }
        private Transform turnCompass { get; set; }

        internal Animator animator { get; set; }
        internal Vector3 previousPosition { get; set; }

        private void Start()
        {
            stalker = gameObject;
            stalker.name = "LS-Stalker";
            agent = gameObject.GetOrAddComponent<NavMeshAgent>();
            agent.stoppingDistance = NumberUtils.Next(2f, 5f);
            aiNodes = GameObject.FindGameObjectsWithTag("AINode");
            transform.position = aiNodes[NumberUtils.NextL(aiNodes.Length)].transform.position;
            animator = gameObject.GetComponentInChildren<Animator>();
            turnCompass = gameObject.transform.Find("FlowermanModel/TurnCompass");
        }

        private Vector3 ChooseFarthestNodeFromPosition()
        {
            // Filter objects within the specified range then order them by distance from the stalker and is further distance than the player.
            // Hopefully making the stalker always to run away in the opposite direction from the player.
            List<GameObject> a = (List<GameObject>)aiNodes.Where(obj =>
            {
                float stalkerDist = Vector3.Distance(obj.transform.position, stalker.transform.position);
                float playerDist = Vector3.Distance(obj.transform.position, LocalPlayer.Player.transform.position);
                return (stalkerDist >= 20f && stalkerDist <= 30f) && stalkerDist > playerDist;
            }).OrderBy(obj => Vector3.Distance(obj.transform.position, stalker.transform.position));
            return a.Count > 0 ? a[NumberUtils.Next(a.Count)].transform.position : Vector3.zero;
        }

        internal void LookAt()
        {
            turnCompass.LookAt(LocalPlayer.PlayerController.gameplayCamera.transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, turnCompass.eulerAngles.y, 0f)), 30f * Time.deltaTime);
        }

        internal void FollowPlayer()
        {
            try
            {
                agent.speed = Mathf.Clamp(Vector3.Distance(transform.position, LocalPlayer.Player.transform.position), 0f, 10f);
                agent.SetDestination(LocalPlayer.Player.transform.position);
            }
            catch (Exception arg)
            {
                ConsoleManager.Log(arg);
            }
        }

        internal void RunAway()
        {
            agent.SetDestination(ChooseFarthestNodeFromPosition());
        }

        internal IEnumerator EvadeTimer()
        {
            this.runningAway = true;
            yield return new WaitForSecondsRealtime(NumberUtils.Next(Config.SH_StalkDelay - 3f, Config.SH_StalkDelay + 3f));
            this.runningAway = false;
            yield break;
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}