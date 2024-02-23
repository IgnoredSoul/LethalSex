using LethalSex_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class FakeItem : LethalClass
    {
        // When the ship lands, make new gameobject and add component
        public override void OnShipLand()
        {
            FakeItemHandler = new GameObject("FakeItemHandler");
            FakeItemHandler.AddComponent<FakeItem>();
        }

        private GameObject FakeItemHandler;
        private List<GameObject> Fakes = new();

        // Start coroutinue
        private void Start() => StartCoroutine(HandleItem());

        // Do loop shit
        private IEnumerator HandleItem()
        {
            while (true)
            {
                // If the player is inside the factory
                if (LocalPlayer.PlayerController.isInsideFactory)
                {
                    // Spawn new objects
                    SpawnNewFakes();

                    // Wait for 30 - 70 seconds
                    yield return new WaitForSeconds(NumberUtils.Next(30, 70));

                    // Delete fakes
                    foreach (GameObject fake in Fakes) Destroy(fake);
                }

                // Loop
                yield return null;
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
        public override void OnGrabObject(GrabbableObject obj)
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
        public override void OnPlayerDie() => Destroy(FakeItemHandler);

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}