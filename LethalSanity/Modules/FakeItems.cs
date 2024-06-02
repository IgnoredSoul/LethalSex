using System;
using System.Linq;
using UnityEngine;
using LethalSex_Core;
using System.Collections;
using LethalSex_Core.Modules;
using System.Collections.Generic;

namespace LethalSanity.Modules
{
    internal class FakeItems : LethalModule
    {
        /// <summary>
        /// Instance of the class
        /// </summary>
        public static FakeItems instance { get; private set; }

        /// <summary>
        /// Instance of the monobehaviour
        /// </summary>
        public static FakeItems component { get; private set; }

        /// <summary>
        /// GameObject that handles the monobehaviour
        /// </summary>
        internal GameObject FakeItemsHandler;

        /// <summary>
        /// List of all spawned fake items
        /// </summary>
        private List<GameObject> Fakes = new();

        /// <summary>
        /// List of all allowed items
        /// </summary>
        private List<SpawnableItemWithRarity> Whitelist = new();

        private Coroutine HandleItemsCor { get; set; }

        /// <summary>
        /// When registering the module, check the config to see if the user wants this module disabled
        /// </summary>
        public override void OnRegister() => instance = this;

        /// <summary>
        /// When the ship lands
        /// </summary>
        public override void OnShipLand()
        {
            component = (FakeItemsHandler = new GameObject("LS-FakeItemHandler")).GetOrAddComponent<FakeItems>();

            FakeItemsHandler.transform.parent = FakeItemsHandler.transform.Find("System");

            /*            // Add all scrap
                        Whitelist.AddRange(RoundManager.Instance.currentLevel.spawnableScrap);

                        if ((bool)Config.config["FI_EX"]["Log"])
                            ConsoleManager.Log($"Whitelist Before: [{string.Join(", ", Whitelist.Select(obj => obj.spawnableItem.name.ToLower()))}]", "FakeItems", Color.magenta);

                        ConsoleManager.Log($"Blacklist: {string.Join(", ", Config.config["FI_BL"].ToList().Select(b => b))}");

                        for (int i = 0; i < Whitelist.Count; i++)
                        {
                            // Check if the item's name exists in the second list
                            if (Config.config["FI_BL"].ToArray().Contains(Whitelist[i].spawnableItem.name.ToLower()))
                            {
                                // Remove the item from the first list
                                Whitelist.RemoveAt(i);

                                // Decrement i to account for the removed item
                                i--;
                            }
                        }

                        if ((bool)Config.config["FI_EX"]["Log"])
                            ConsoleManager.Log($"Whitelist After: [{string.Join(", ", Whitelist.ToList().Select(obj => obj.spawnableItem.name.ToLower()))}]", "FakeItems", Color.magenta);*/
        }

        private IEnumerator HandleItems()
        {
            // Do loop shit
            while (true)
            {
                // Firstly check if the player is inside the factory
                if (LocalPlayer.PlayerController.isInsideFactory)
                {
                    // Then check if the player wants the items to respawn
                    if ((bool)Config.config["FI_EX"]["Respawning"])
                    {
                        // Spawn new objects
                        SpawnNewFakes();

                        float min = (float)Config.config["FI_EX"]["Delay"] - NumberUtils.NextF(5f);
                        float max = (float)Config.config["FI_EX"]["Delay"] + NumberUtils.NextF(5f);

                        // Wait for (CONFIG.SPAWNDELAY ± rnd(5f)) seconds
                        yield return new WaitForSeconds(Mathf.Clamp(NumberUtils.NextF(min, max), 0, max));

                        // Delete fakes
                        foreach (GameObject fake in Fakes) Destroy(fake);
                    }

                    // Else just spawn and stop
                    else
                    {
                        // Spawn new objects
                        SpawnNewFakes();

                        // Break loop
                        yield break;
                    }
                }
                // Else we wait till the player is inside
                else
                {
                    yield return new WaitForSeconds(5);
                }
            }
        }

        // Create new fake scrap
        private void SpawnNewFakes()
        {
            Extensions.TryCatch(() =>
            {
                // Spawn (CONFIG.MIN) to (CONFIG.MAX) items
                for (int i = 0; i < NumberUtils.Next((int)Config.config["FI"]["Min"], (int)Config.config["FI"]["Max"]); i++)
                {
                    // Choose a random scrap
                    SpawnableItemWithRarity scrap = Whitelist[NumberUtils.NextL(Whitelist.Count)];

                    // Choose a random position
                    Vector3 position = RoundManager.Instance.GetRandomNavMeshPositionInRadiusSpherical(LocalPlayer.Player.transform.position, (float)Config.config["FI"]["Radius"]) + Vector3.up * scrap.spawnableItem.verticalOffset;

                    // Instantiate new object
                    GameObject itemObject = Instantiate(scrap.spawnableItem.spawnPrefab, position, Quaternion.identity);

                    // Put "Fake_" in front
                    itemObject.name = $"Fake_{itemObject.name}_{NumberUtils.NextF(1000f)}";
                    Fakes.Add(itemObject);
                }
            }, "Error in 'FakeItems', 'SpawnNewFakes'");
        }

        // When the object is grabbed
        public override void OnGrabObject(GrabbableObject obj)
        {
            // Check if item is an fake
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
        public override void OnLocalPlayerDie() => Destroy(FakeItemsHandler);

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}