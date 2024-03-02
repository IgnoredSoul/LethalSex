using LethalSex_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class FakeItem : LethalClass
    {
        public static FakeItem Module { get; private set; } = null!;

        /// <summary>
        /// Assign Module to this instance.
        /// </summary>
/*        protected override void OnRegister()
        {
            // Unregister this module if disabled in the config.
            if (!Config.FI_ToggleModule) { ManualUnregister(); return; }

            base.OnRegister();
            Module = this;
        }*/

        /// <summary>
        /// If the module unregisters, unload everything.
        /// </summary>
        protected override void Unregister()
        {
            // Try to destroy the object, if it cannot doesn't matter cause it doesnt exist lol.
            Extensions.TryDestroy(FakeItemHandler);

            base.Unregister();
            Module = null;
        }

        /// <summary>
        /// When the ship lands, make new gameobject and add component
        /// </summary>
        protected override void OnShipLand()
        {
            FakeItemHandler = new GameObject("LS-FakeItemHandler");
            FakeItemHandler.AddComponent<FakeItem>();
        }

        private GameObject FakeItemHandler;
        private List<GameObject> Fakes = new();

        // Start coroutinue
        private void Start() => StartCoroutine(HandleItem());

        /// <summary>
        /// Handle fake item spawning and item spawn delay
        /// </summary>
        /// <returns></returns>
        private IEnumerator HandleItem()
        {
            // Wait till the player is inside the factory
            if (!LocalPlayer.PlayerController.isInsideFactory) yield return null;

            // Wait for (CONFIG.SPAWNDELAY ± rnd(5)) seconds
            yield return new WaitForSeconds(NumberUtils.Next(Mathf.Max(0, Config.FI_SpawnDelay - NumberUtils.Next(5)), Mathf.Max(0, Config.FI_SpawnDelay + NumberUtils.Next(5))));

            // If respawning enabled
            if (Config.FI_RespawnDelay)
            {
                while (true)
                {
                    // If the player is inside the factory just in case
                    if (LocalPlayer.PlayerController.isInsideFactory)
                    {
                        // Spawn new objects
                        SpawnNewFakes();

                        // Wait for (CONFIG.SPAWNDELAY ± rnd(5)) seconds
                        yield return new WaitForSeconds(NumberUtils.Next(Mathf.Max(0, Config.FI_SpawnDelay - NumberUtils.Next(5)), Mathf.Max(0, Config.FI_SpawnDelay + NumberUtils.Next(5))));

                        // Delete fakes
                        foreach (GameObject fake in Fakes) Destroy(fake);
                    }

                    // Loop
                    yield return null;
                }
            }
            else
            {
                while (true)
                {
                    // If the player is inside the factory just in case
                    if (LocalPlayer.PlayerController.isInsideFactory)
                    {
                        // Spawn new objects
                        SpawnNewFakes();

                        yield break;
                    }

                    // Loop
                    yield return null;
                }
            }
        }

        // Create new fake scrap
        private void SpawnNewFakes()
        {
            try
            {
                // Chose random about of scrap
                for (int i = 0; i < NumberUtils.Next(1, 3); i++)
                {
                    // Choose a random scrap
                    SpawnableItemWithRarity scrap = RoundManager.Instance.currentLevel.spawnableScrap[NumberUtils.NextL(RoundManager.Instance.currentLevel.spawnableScrap.Count)];

                    // Choose a random position
                    Vector3 position = RoundManager.Instance.GetRandomNavMeshPositionInRadiusSpherical(LocalPlayer.Player.transform.position, 25) + Vector3.up * scrap.spawnableItem.verticalOffset;

                    // Instantiate new object
                    GameObject itemObject = Instantiate(scrap.spawnableItem.spawnPrefab, position, Quaternion.identity);

                    // Put "Fake_" in front
                    itemObject.name = $"Fake_{itemObject.name}_{NumberUtils.Next(1000f)}";
                    Fakes.Add(itemObject);
                }
            }
            catch (System.Exception e) { ConsoleManager.Log(e); }
        }

        // When the object is grabbed
        protected override void OnGrabObject(GrabbableObject obj)
        {
            // Check if item is an fake
            ConsoleManager.Log(obj.name.StartsWith("Fake"));
            if (!obj.name.StartsWith("Fake")) return;

            if (NumberUtils.Chance(40)) { LocalPlayer.PlayerController.DropBlood(); SoundManager.Instance.earsRingingTimer = 0.15f; }
            else if (NumberUtils.Chance(25)) { SoundManager.Instance.PlayAmbientSound(false, false); SoundManager.Instance.earsRingingTimer = 0.5f; }
            else if (NumberUtils.Chance(10)) { LocalPlayer.PlayerController.DamagePlayer(25); SoundManager.Instance.earsRingingTimer = 0.85f; }
            else if (NumberUtils.Chance(1)) LocalPlayer.PlayerController.KillPlayer(LocalPlayer.Player.transform.forward);

            LocalPlayer.Insanity += 10f;
            LocalPlayer.PlayerController.JumpToFearLevel(0.5f);

            Destroy(obj.gameObject);
        }

        // When the player dies. Destroy gameobject
        protected override void OnPlayerDie() => Destroy(FakeItemHandler);

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}