# Updates

## 2.0.0
- Idk I'm loosing my fucking sanity :3

## 1.5.0
- uhhh... yeah?

## 1.4.0
- Reworked how modules work for LethalSex-Core v2.0.0.
- Disabled the camera leaning module due to bad code.

## 1.3.1
- Added more config options for the fake items.
- Fixed camera leaning [bug](https://github.com/IgnoredSoul/LethalSex/issues/2).

## 1.3.0 - The "InsanityRemastered Reimagined" Update >:3
- Fake scrap will spawn around the player at random intervals. Picking the scrap up can cause random effects.
- [W.I.P] New monster called "stalker" takes after the flowerman. He will follow the player around when reaching insanity level 1 and will not despawn after then. :)
- More config to the leaning module.

## 1.2.0 - Fuck.
- Bug fixes
- Removed the testing UI. <sub>*(I forgor 💀)*</sub>
- Added a new config toggle for the lean module. I know some people would prefer not to have it.

## 1.1.3 - I am slow
- Bug fixes.

## 1.1.2
- Init release.

<details>
  <summary><strong>Do not use this cause I could't be fucked remaking the config shit and making this took so fucking long :3</summary>

	# Config
	The config area is confusing and sometimes don't supply a discription, read this before complaining in the [thread](https://discord.com/channels/1168655651455639582/1210095873875247144) :)

	> [!IMPORTANT]
	> When writing the config, make sure the values are in order as its stated below.
	>
	> Make sure the value has the correct type "f", "i", "s", "b" so it can know what the value is supposed to be.

	<details><summary>Post Processing</summary>
	<div align=center>

	### Default
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | This module adds multiple post processing effect applied to the player. |
	| [Priority](https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Quick-start.html#post-process-volume) | i:1 | Defines this volume’s order in the stack. The higher this number is, the higher this volume is placed in the stack. This means that Unity runs this volume before volumes in the stack that have a lower Priority number. |
	<hr/>

	### [Chromatic Aberation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/post-processing-chromatic-aberration.html)
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | Chromatic Aberration effect splits color along boundaries in an image into their red, green, and blue channels |
	| Lvl | i:2 | Change the insanity level when the effect applies. |
	| ApplyVal | f:1.0 | When applying the effect, what override value should be applied. |
	| ApplySpeed | f:20.0 | When applying the effect, how long should it take till its complete. |
	| ResetVal | f:0.0 | When removing the effect, what override value should be applied. |
	| ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |
	<hr/>

	### [Lens Distortion](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/Post-Processing-Lens-Distortion.html)
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | Lens Distortion effect distorts the final rendered picture to simulate the shape of a real-world camera lens. |
	| Lvl | i:2 | Change the insanity level when the effect applies.
	| ApplyVal | f:0.55 | When applying the effect, what override value should be applied. |
	| ApplySpeed | f:20.0 | When applying the effect, how long should it take till its complete. |
	| ResetVal | f:0.0 | When removing the effect, what override value should be applied. |
	| ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |
	<hr/>

	### [Film Grain](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/Post-Processing-Film-Grain.html)
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | The Film Grain effect simulates the random optical texture of photographic film, usually caused by small particles being present on the physical film. |
	| Lvl | i:1 | Change the insanity level when the effect applies.
	| ApplyVal | f:1.6 | When applying the effect, what override value should be applied. |
	| ApplySpeed | f:15.0 | When applying the effect, how long should it take till its complete. |
	| ResetVal | f:0.0 | When removing the effect, what override value should be applied. |
	| ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |
	<hr/>

	### [Vignette](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/post-processing-vignette.html)
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | Vignetting is the term for the darkening and/or desaturating towards the edges of an image compared to the center. |
	| Lvl | i:1 | Change the insanity level when the effect applies.
	| ApplyVal | f:0.5 | When applying the effect, what override value should be applied. |
	| ApplySpeed | f:25.0 | When applying the effect, how long should it take till its complete. |
	| ResetVal | f:0.1 | When removing the effect, what override value should be applied. |
	| ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |
	| Opacity | f:0.5 | Controls the intensity / visibility of the vignette effect applied to the scene. |
	| Smoothness | f:25.0 | The higher the value, the smoother the vignette border |
	<hr/>

	### [Depth of Field](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/post-processing-depth-of-field.html)
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | Depth Of Field component applies a depth of field effect, which simulates the focus properties of a camera lens. |
	| Lvl | i:3 | Change the insanity level when the effect applies.
	| Start ApplyVal | f:5.0 | When applying the effect, what override value should be applied. |
	| Start ApplySpeed | f:25.0 | When applying the effect, how long should it take till its complete. |
	| Start ResetVal | f:2000.0 | When removing the effect, what override value should be applied. |
	| Start ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |
	| End ApplyVal | f:25.0 | When applying the effect, what override value should be applied. |
	| End ApplySpeed | f:20.0 | When applying the effect, how long should it take till its complete. |
	| End ResetVal | f:2000.0 | When removing the effect, what override value should be applied. |
	| End ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |

	### [Color Adjustments](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/Post-Processing-Color-Adjustments.html)
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | Use this effect to tweak the overall tone, brightness, and contrast of the final rendered image |
	| Lvl | i:3 | Change the insanity level when the effect applies.
	| ApplyVal | f:-64.0 | When applying the effect, what override value should be applied. |
	| ApplySpeed | f:25.0 | When applying the effect, how long should it take till its complete. |
	| ResetVal | f:0.0 | When removing the effect, what override value should be applied. |
	| ResetSpeed | f:0.75 | When removing the effect, how long should it take till its complete. |

	</div></details><br/><br/>

	<details><summary>Insanity Options</summary>
	<div align=center>

	### Default
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| MaxInsanity | i:65 | This sets the max insanity the player can get to. |
	| Lvl1 | i:35 | The amount of insanity the player needs to reach before level 1 is applied. |
	| Lvl2 | i:45 | The amount of insanity the player needs to reach before level 2 is applied. |
	| Lvl3 | i:65 | The amount of insanity the player needs to reach before level 3 is applied. |
	<hr/>
	</div></details><br/><br/>

	<details><summary>Camera Leaning</summary>
	<div align=center>

	### Default
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | When the player turns, the camera can lean into the direction / opposite direction. |
	| Max | f:35.0 | The maximum allowed leaning the camera can do. It will not rotate further than the maximum allowed value. |
	| Threshold | f:35.0 | Only apply when the mouse has reached speed over the threshold. |
	| Reset | f:5.0 | Time it takes for the camera to reset its rotation. |
	| Invert | b:false | Inverts the way the camera leans. |
	<hr/>

	### Extra
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Default | f:0.05 | The default amount of leaning applied to the camera. |
	| Sprint | f:0.05 | When sprinting, add this to the default multiplier for harsher leaning. |
	| Stamina | f:25.0 | When the player is below the stamina threshold, add this to the default multiplier for harsher leaning. |
	| Fear | f:0.05 | When in fear, add this to the default multiplier for harsher leaning. |
	| Insanity | f:0.05 | When the player is insane, add this to the default multiplier for harsher leaning. |
	| InsanityVal | f:15.0 | When the player has reached or over the threshold of insanity, apply the Insanity multiplier. |
	| Day | f:0.05 | When the time of day is over the set Daymode, add this to the default multiplier for harsher leaning. |
	| Daymode | i:2 | When the day has reached or over the desired DayMode, apply the Day multiplier. |

	</div></details><br/><br/>

	<details><summary>Fake Items</summary>
	<div align=center>

	### Default
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | Fake items will spawn around the map that can apply random effects to the player when they try to pick it up. |
	| Min | i:1 | Minumum amount of items that will spawn. |
	| Max | i:3 | Maximum amount of items that will spawn. |
	| Radius | f:25.0 | The distance from around the player can spawn from. |
	<hr/>

	### Extra
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Respawning | b:true | Allow items to respawn. |
	| Delay | f:70.0 | The delay from which items will spawn. |
	| Log | b:false | Log all scrap and whitelisted scrap into the ingame console provided by LethalSex-Core. |
	<hr/>

	### Blacklist
	| Name       | Default  | Description                             | Example |
	|------------|----------|-----------------------------------------|---------|
	| Blacklist | | Adding the name of an item will blacklist it from the spawnable fake items. | BigBolt, PickleJar, Airhorn

	</div></details><br/><br/>

	<details><summary>Camera Shake</summary>
	<div align=center>

	### Default
	| Name       | Default  | Description                             |
	|------------|----------|-----------------------------------------|
	| Enabled | b:true | This "simulates" shaking of the player when they're scared. |
	<hr/>
	</div></details><br/><br/>

</details>