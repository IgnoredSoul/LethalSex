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
        public override void OnShipLand()
        {
            if (!Config.SH_ToggleStalker) return;
            stalker = Instantiate(Resources.FindObjectsOfTypeAll<FlowermanAI>()[0].gameObject);
            Destroy(stalker.GetComponent<FlowermanAI>());
            stalker.AddComponent<StalkerHallucination>();
        }

        public static StalkerHallucination instance { get; private set; }

        private NavMeshAgent agent { get; set; }

        private GameObject[] aiNodes { get; set; }
        private bool runningAway { get; set; }

        public GameObject stalker { get; private set; }
        private Transform turnCompass { get; set; }

        private Animator animator { get; set; }
        private Vector3 previousPosition { get; set; }

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
            instance = this;
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

        private void LookAt()
        {
            turnCompass.LookAt(LocalPlayer.PlayerController.gameplayCamera.transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, turnCompass.eulerAngles.y, 0f)), 30f * Time.deltaTime);
        }

        private void FollowPlayer()
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

        private void RunAway()
        {
            agent.SetDestination(ChooseFarthestNodeFromPosition());
        }

        private IEnumerator EvadeTimer()
        {
            this.runningAway = true;
            yield return new WaitForSecondsRealtime(NumberUtils.Next(Config.SH_StalkDelay - 3f, Config.SH_StalkDelay + 3f));
            this.runningAway = false;
            yield break;
        }

        public void Update()
        {
            LookAt();
            if (LocalPlayer.PlayerController.HasLineOfSightToPosition(base.transform.position + Vector3.up * 0.5f, 30f, 60, -1f))
            {
                animator.SetBool("anger", true);
                animator.SetBool("sneak", false);
                animator.SetFloat("speedMultiplier", Vector3.ClampMagnitude(transform.position - previousPosition, 1f).sqrMagnitude / (Time.deltaTime * 2f));
                RunAway();
                if (!runningAway)
                {
                    StartCoroutine(EvadeTimer());
                }
            }
            else if (!runningAway)
            {
                animator.SetBool("anger", false);
                animator.SetBool("sneak", true);
                FollowPlayer();
                animator.SetFloat("speedMultiplier", Vector3.ClampMagnitude(transform.position - previousPosition, 1f).sqrMagnitude / (Time.deltaTime * 4f));
            }
            previousPosition = transform.position;
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}