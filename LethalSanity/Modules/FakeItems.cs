using LethalSex_Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class FakeItems : LethalClass
    {
        public static FakeItems Module { get; private set; }

        protected override void OnRegister()
        {
            if (!Config.config.FI.Enabled || Config.config.FI.Min > Config.config.FI.Max) Unregister();
        }

        // When the ship lands, make new gameobject and add component
        protected override void OnShipLand()
        {
            Module = (FakeItemsHandler = new GameObject("LS-FakeItemHandler")).GetOrAddComponent<FakeItems>();
            FakeItemsHandler.transform.parent = FakeItemsHandler.transform.Find("System");

            // Add all scrap
            Whitelist.AddRange(RoundManager.Instance.currentLevel.spawnableScrap);

            if (Config.config.FI_BL == null) return;

            if (Config.config.FI_EX.Log)
                ConsoleManager.Log($"Whitelist Before: [{string.Join(", ", Whitelist.ToList().Select(obj => obj.spawnableItem.name.ToLower()))}]", "FakeItems", Color.magenta);

            LethalSex_Core.Main.mls.LogInfo(Config.config.FI_BL.Length);

            for (int i = 0; i < Whitelist.Count; i++)
            {
                // Check if the item's name exists in the second list
                if (Config.config.FI_BL.Contains(Whitelist[i].spawnableItem.name.ToLower()))
                {
                    // Remove the item from the first list
                    Whitelist.RemoveAt(i);

                    // Decrement i to account for the removed item
                    i--;
                }
            }

            if (Config.config.config.FI_EX.Log)
                ConsoleManager.Log($"Whitelist After: [{string.Join(", ", Whitelist.ToList().Select(obj => obj.spawnableItem.name.ToLower()))}]", "FakeItems", Color.magenta);
        }

        internal GameObject FakeItemsHandler;
        private List<GameObject> Fakes = new();
        private List<SpawnableItemWithRarity> Whitelist = new();

        // Start coroutinue
        private void Start() => StartCoroutine(HandleItems());

        private IEnumerator HandleItems()
        {
            // Do loop shit
            while (true)
            {
                // Firstly check if the player is inside the factory
                if (LocalPlayer.PlayerController.isInsideFactory)
                {
                    // Then check if the player wants the items to respawn
                    if (Config.config.FI_EX.Extra.Respawning)
                    {
                        // Spawn new objects
                        SpawnNewFakes();

                        // Wait for (CONFIG.SPAWNDELAY - rnd(5f)) to (CONFIG.SPAWNDELAY + rnd(5f)) seconds
                        yield return new WaitForSeconds(NumberUtils.NextF(Config.config.FI_EX.Delay - NumberUtils.NextF(5f), Config.config.FI_EX.Delay + NumberUtils.NextF(5f)));

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
            try
            {
                // Spawn (CONFIG.MIN) to (CONFIG.MAX) items
                for (int i = 0; i < NumberUtils.Next((int)Config.config.FI.Min, (int)Config.config.FI.Max); i++)
                {
                    // Choose a random scrap
                    SpawnableItemWithRarity scrap = Whitelist[NumberUtils.NextL(Whitelist.Count)];

                    // Choose a random position
                    Vector3 position = RoundManager.Instance.GetRandomNavMeshPositionInRadiusSpherical(LocalPlayer.Player.transform.position, (float)Config.config.FI.Radius) + Vector3.up * scrap.spawnableItem.verticalOffset;

                    // Instantiate new object
                    GameObject itemObject = Instantiate(scrap.spawnableItem.spawnPrefab, position, Quaternion.identity);

                    // Put "Fake_" in front
                    itemObject.name = $"Fake_{itemObject.name}_{NumberUtils.NextF(1000f)}";
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
        protected override void OnPlayerDie() => Destroy(FakeItemsHandler);

        private void OnDestroy() => base.Destroy();

        private void OnDisable() => base.Disable();

        private void OnEnable() => base.Enable();
    }
}