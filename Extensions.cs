using System;
using UnityEngine;
using GameNetcodeStuff;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

namespace LethalSex
{
    internal static class Extensions
    {
        internal static GameObject EasyInstantiate(GameObject target, GameObject parent) => UnityEngine.Object.Instantiate(ReturnIfActive(target), ReturnIfActive(parent).transform);
        internal static GameObject ReturnIfActive(this GameObject gameObject) => gameObject == null ? null : gameObject;

        internal static T GetOrAddComponent<T>(this GameObject gameObject, bool Log = false) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (!component)
            {
                component = gameObject.AddComponent<T>();
                if (Log) Main.mls.LogInfo($"Added {component.name}");
            }

            return component;
        }

        internal static GameObject GetLocalPlayer()
        {
            return StartOfRound.Instance?.localPlayerController?.gameObject ?? null;
        }
        internal static PlayerControllerB GetLocalPlayerController()
        {
            return StartOfRound.Instance?.localPlayerController ?? null;
        }

        private static Dictionary<string, CancellationTokenSource> ctsDict = new Dictionary<string, CancellationTokenSource>();
        internal static async void SmoothIncrementValue(string ActionName, Action<float> action, float start, float target, float duration)
        {
            // Cancel the task if it's already running
            if (ctsDict.ContainsKey(ActionName))
            {
                ctsDict[ActionName].Cancel();
                ctsDict[ActionName].Dispose();
                LogHandler.Warn($"Cancelling smooth increment task: {ActionName}");
                ctsDict.Remove(ActionName);
            }

            // Create a new cancellation token source for the task
            ctsDict.Add(ActionName, new CancellationTokenSource());

            // Start the task
            Task smoothIncrementTask = SmoothIncrementValueTask((value) =>
            {
                action(value);
            }, start, target, duration, ctsDict[ActionName].Token);

            // Wait for the task to complete (or be canceled)
            await smoothIncrementTask;

            // Clean up the CancellationTokenSource and remove action from dict
            ctsDict.Remove(ActionName);
        }
        private static async Task SmoothIncrementValueTask(Action<float> action, float start, float target, float duration, CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;
            float currentValue;

            while (elapsedTime < duration)
            {
                // Check for cancellation
                if (cancellationToken.IsCancellationRequested)
                {
                    // Perform cleanup or any necessary actions before exiting
                    return;
                }

                currentValue = Mathf.Lerp(start, target, elapsedTime / duration);
                action(currentValue);

                elapsedTime += Time.deltaTime;

                await Task.Yield();
            }

            currentValue = target;
            action(currentValue);
        }
    }
}
