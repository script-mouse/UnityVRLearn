namespace VARLab.UnityVRLearn.Teleportation
{
	using System.Collections;
	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.Assertions;

	using UnityEngine.XR.Interaction.Toolkit;

	public class FadeTeleportPlayer
		: TeleportPlayer
	{
		[SerializeField]
		[Tooltip("The Canvas that should be used for fading in and out when teleporting")]
		private FadeCanvas fadeCanvas;
		
		void Awake()
		{
			Assert.IsNotNull(fadeCanvas, "VARLab.UnityVRLearn.Teleportation.FadeTeleportation.fadeCanvas must be set!");	
		}
		

		// Same behavior as TeleportAnchorWithFade.cs
		public void FadeTeleport()
		{
			StartCoroutine(FadeSequence());
		}
			
        private IEnumerator FadeSequence()
		{
			fadeCanvas.QuickFadeIn();

			yield return fadeCanvas.CurrentRoutine;
			base.Teleport();

			fadeCanvas.QuickFadeOut();
		}
    }
}
