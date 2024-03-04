using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;
using static LethalSex_Core.Extensions;

namespace LethalSanity
{
    internal class Config
    {
        private ConfigFile _config { get; set; }
        internal static JObject config { get; private set; }

        public Config() => Init();

        private void Init()
        {
            // =====================================================[ Create / Read ]===================================================== \\
            _config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalSanity.cfg"), true, new(Main.modGUID, Main.modName, Main.modVersion));

            // ==========================================================[ Info ]========================================================== \\
            _config.Bind("!Info!", "READ", "https://github.com/IgnoredSoul/LethalSex", "Yes some descriptions do get cut off, so please visit the github repo for more information!"); /* Info / Warning I guess. */

            // ====================================================[ Post Processing ]==================================================== \\
            JObject PostProcessing = CreateEntry((entry) =>
            {
                InsertList
                (
                    values: _config.Bind
                    (
                        section: "Post Processing",
                        key: "Post Processing Config",
                        defaultValue: "true, 0",
                        description: "1) Toggle on and off the post processing module. This will completely remove insanity visual effects from the mod.\n2) Changing the priority means to override every other post processing effect. This includes other mods and base game. Increasing means higher priority, decreasing means less priority."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "Priority"],
                    dest: entry,
                    key: "cfg"
                );

                InsertList
                (
                    _config.Bind
                    (
                        section: "Post Processing",
                        key: "Chromatic Aberation",
                        defaultValue: "true, 1.0, 20.0, 0.0, 0.75",
                        description: "To even understand what the fuck is happening you've got to read the documentation / code on my github..."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "PutVal", "PutSpeed", "TakeVal", "TakeSpeed"],
                    dest: entry,
                    key: "ChrAb"
                );

                InsertList
                (
                    _config.Bind
                    (
                        section: "Post Processing",
                        key: "Lens Distortion",
                        defaultValue: "true, 0.55, 27.0, 0.0, 0.75",
                        description: "To even understand what the fuck is happening you've got to read the documentation / code on my github..."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "PutVal", "PutSpeed", "TakeVal", "TakeSpeed"],
                    dest: entry,
                    key: "LensDist"
                );

                InsertList
                (
                    _config.Bind
                    (
                        section: "Post Processing",
                        key: "Film Grain",
                        defaultValue: "true, 1.6, 15.0, 0.0, 0.75",
                        description: "To even understand what the fuck is happening you've got to read the documentation / code on my github..."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "PutVal", "PutSpeed", "TakeVal", "TakeSpeed"],
                    dest: entry,
                    key: "FilmGrain"
                );

                InsertList
                (
                    _config.Bind
                    (
                        section: "Post Processing",
                        key: "Vignette",
                        defaultValue: "true, 0.5, 25.0, 0.1, 0.75, 0.5, 25",
                        description: "To even understand what the fuck is happening you've got to read the documentation / code on my github..."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "PutVal", "PutSpeed", "TakeVal", "TakeSpeed", "Opacity", "Smoothness"],
                    dest: entry,
                    key: "Vignette"
                );

                InsertList
                (
                    _config.Bind
                    (
                        section: "Post Processing",
                        key: "Depth Of Field",
                        defaultValue: "true, 5.0, 25.0, 2000.0, 0.75, 25.0, 20.0, 2000.0, 0.75",
                        description: "To even understand what the fuck is happening you've got to read the documentation / code on my github..."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "SPutVal", "SPutSpeed", "STakeVal", "STakeSpeed", "EPutVal", "EPutSpeed", "ETakeVal", "ETakeSpeed"],
                    dest: entry,
                    key: "DOP"
                );

                InsertList
                (
                    _config.Bind
                    (
                        section: "Post Processing",
                        key: "Color Adjustments",
                        defaultValue: "true, -64.0, 25.0, 0.0, 0.75",
                        description: "To even understand what the fuck is happening you've got to read the documentation / code on my github..."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "PutVal", "PutSpeed", "TakeVal", "TakeSpeed"],
                    dest: entry,
                    key: "CA"
                );
            });

            // ====================================================[ Insanity Options ]==================================================== \\

            JObject InsanityOptions = CreateEntry((entry) =>
            {
                InsertList
                (
                    values: _config.Bind
                    (
                        section: "Insanity",
                        key: "Insanity Options",
                        defaultValue: "65.0, 35.0, 45.0, 65.0",
                        description: "1) Set max insanity.\n2) Change insanity levels for when insanity modules kick in."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["MaxInsanity", "Lvl1", "Lvl2", "Lvl3"],
                    dest: entry,
                    key: "cfg"
                );
            });

            // =====================================================[ Camera Leaning ]===================================================== \\

            JObject CameraLeaning = CreateEntry((entry) =>
            {
                InsertList
                (
                    values: _config.Bind
                    (
                        section: "Camera Leaning",
                        key: "Camera Leaning Config",
                        defaultValue: "true, 35.0, 35.0, 5",
                        description: "1) Toggle on and off the lean module. This will completely remove leaning from the mod.\n2) Max amount of lean the camera can lean to.\n3) How fast the mouse has to move before the lean is applied.\n4) How fast the lerp should take"
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "Max", "Threshold", "Reset"],
                    dest: entry,
                    key: "cfg"
                );
            });

            // =======================================================[ Fake Items ]======================================================= \\

            JObject FakeItems = CreateEntry((entry) =>
            {
                InsertList
                (
                    values: _config.Bind
                    (
                        section: "Fake Items",
                        key: "Fake Items Config",
                        defaultValue: "true, 1, 3, 25.0",
                        description: "1) Toggle on and off the fake items module. This will completely remove fake item hallucinations from the mod.\n2) Minimum amount of fake items that will spawn.\n3) Maximum amount of fake items that will spawn.\n4) Radius the item will spawn in."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "Min", "Max", "Radius"],
                    dest: entry,
                    key: "cfg"
                );

                InsertList
                (
                    values: _config.Bind
                    (
                        section: "Fake Items",
                        key: "Fake Items Extra",
                        defaultValue: "true, 70, false",
                        description: "1)Allow items to respawn.\n2) Time between respawns ± rnd(5.0f).\n3) Log all items to in-game console when ship has landed."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Respawning", "Delay", "Log"],
                    dest: entry,
                    key: "Extra"
                );

                InsertArray
                (
                    items: _config.Bind
                    (
                        section: "Fake Items",
                        key: "Blacklist",
                        defaultValue: "",
                        description: "1) To blacklist items, type the exact name out. (Case-insensitive)"
                    ).Value.Replace(" ", "").Split(','),
                    dest: entry,
                    key: "Blacklist"
                );
            });

            // ======================================================[ Camera Shake ]====================================================== \\

            JObject CameraShake = CreateEntry((entry) =>
            {
                InsertList
                (
                    values: _config.Bind
                    (
                        section: "Camera Shake",
                        key: "Camera Shake Config",
                        defaultValue: "true, 0.05, 0.15",
                        description: "1) Toggle on and off the camera shake module. This will completely remove any added camera shaking from the mod.\n2) Wobble amount when hitting lvl2 and lvl3 insanity."
                    ).Value.Replace(" ", "").Split(','),
                    keys: ["Enabled", "Lvl2", "Lvl3"],
                    dest: entry,
                    key: "cfg"
                );
            });

            // =================================================[ Stalker Hallucination ]================================================= \\
            //SH_ToggleStalker = config.Bind("Stalker", "Toggle Stalker Module", true, "Toggle on and off the stalker module. This will completely remove the insanity creature *'(stalker)'* from the mod.").Value;

            // ========================================================[ Combine ]======================================================== \\

            config = JsonPackage([PostProcessing, InsanityOptions, CameraLeaning, FakeItems, CameraShake], ["PP", "IO", "CL", "FI", "CS"]);

            Debug.Log("\n" + config);
        }
    }
}