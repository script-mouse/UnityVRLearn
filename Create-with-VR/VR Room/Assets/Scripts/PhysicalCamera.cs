namespace VARLab.UnityVRLearn.OldSchoolCamera
{
	
	using System.Collections;
	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.Assertions;
	
	[RequireComponent(typeof(Animation))]
	public class PhysicalCamera : MonoBehaviour
	{

		#region Private Fields Accessible from the Editor
		
		[SerializeField]
		[Tooltip("The object that should be printed by the camera when a picture is taken")]
		private GameObject photoPrefab;

		[SerializeField]
		[Tooltip("The copy of the photo prefab that should be used for the camera printing animation")]
		private GameObject printingPhoto;
		
		[SerializeField]
		[Tooltip("The Render Texture that this camera should use to make new Render Textures to write to when taking a photograph")]
		private RenderTexture photoRenderTextureExample;
		
		[SerializeField]
		[Tooltip("The Camera that will be used to take the photographs")]
		private Camera photoCamera;
		
		
		[SerializeField]
		[Tooltip("The offset from the camera that printed photographs should stop printing")]
		private Vector3 printedOffset;
		
		[SerializeField]
		[Tooltip("The offset from the camera that the printing animation should begin at")]
		private Vector3 printBeginning;
		
		#endregion Private Fields Accessible from the Editor


		#region Private Fields
		// The animation of a photo printing from the camera
		private Animation printingAnimation;
		
		// The photograph that was last printed by the camera
		private GameObject previousPhotograph;
		
		// The Descriptor for the Render Texture that is actually written to when taking a photograph
		private RenderTextureDescriptor photoRenderTextureDescriptor;
		
		// The Textures containing the pictures the camera took but has not finished printing
		private Queue<Texture2D> waitingTextures;
		
		#endregion Private Fields
		
		void Awake() 
		{
			Assert.IsNotNull(printingPhoto, "VARLab.UnityVRLearn.OldSchoolCamera.PhysicalCamera.printingPhoto must be set!");
			Assert.IsNotNull(photoRenderTextureExample, "VARLab.UnityVRLearn.OldSchoolCamera.PhysicalCamera.photoRenderTextureExample must be set!");
			Assert.IsNotNull(photoCamera, "VARLab.UnityVRLearn.OldSchoolCamera.PhysicalCamera.photoCamera must be set!");
			
			
			printingAnimation = GetComponent<Animation>();
			
			
			AnimationClip printingClip = printingAnimation.clip;
			
			if(!(printingClip))
			{
				Debug.LogWarning("No Animation Clip was assigned to this animation, creating a new one in VARLab.UnityVRLearn.OldSchoolCamera.PhysicalCamera.");
				printingClip = new AnimationClip();
				
				printingClip.name = "PrintPhotograph";
				printingClip.legacy = true;
				
				printingAnimation.clip = printingClip;
			}
			
			// Double check we are in legacy mode, if we are not then adding curves directly would not work
			if(!(printingClip.legacy))
			{
				Debug.LogWarning("printingClip must be in legacy mode but it is not, the VARLab.UnityVRLearn.OldSchoolCamera.PhysicalCamera will attempt to convert it to legacy mode!");
			}
			printingClip.wrapMode = WrapMode.Once;
			
			// Default values chosen using editor, subject to change
			AnimationCurve xMovement = AnimationCurve.Linear(0.0f, printBeginning.x, 1.0f, printedOffset.x);
			AnimationCurve yMovement = AnimationCurve.Linear(0.0f, printBeginning.y, 1.0f, printedOffset.y);
			AnimationCurve zMovement = AnimationCurve.Linear(0.0f, printBeginning.z, 1.0f, printedOffset.z);
			
			// Since this may change in different versions and is used multiple times, put it in a variable;
			string photoPath = "Camera_Photograph";
			
			printingClip.SetCurve(photoPath, typeof(Transform), "m_LocalPosition.x", xMovement);
			printingClip.SetCurve(photoPath, typeof(Transform), "m_LocalPosition.y", yMovement);
			printingClip.SetCurve(photoPath, typeof(Transform), "m_LocalPosition.z", zMovement);
			
			photoRenderTextureDescriptor = photoRenderTextureExample.descriptor;
			
			waitingTextures = new Queue<Texture2D>();
			
		}
		
		// Play the animation of a photo printing from the camera,
		// the picture object is created afterwards using AnimationEvents
		public void printPicture() 
		{
			RenderTexture oldActiveRenderTexture = RenderTexture.active;
			RenderTexture oldCameraRenderTexture = photoCamera.targetTexture;
			
			RenderTexture.active = RenderTexture.GetTemporary(photoRenderTextureDescriptor);
			photoCamera.targetTexture = RenderTexture.active;
			
			photoCamera.Render();

			int textureWidth = RenderTexture.active.width;
			int textureHeight = RenderTexture.active.height;
			
			Texture2D picture = new Texture2D(textureWidth, textureHeight);
			picture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
			picture.Apply();
			
			waitingTextures.Enqueue(picture);
			
			photoCamera.targetTexture = oldCameraRenderTexture;
			
			RenderTexture.ReleaseTemporary(RenderTexture.active);
			RenderTexture.active = oldActiveRenderTexture;
			
			printingPhoto.SetActive(true);
			printingAnimation.PlayQueued("PrintPhotograph");
		}
		
		// Create the photo object, as a physical object to be printed
		private void createPicture() 
		{
			
			Transform cameraTransform = GetComponent<Transform>();
			
			Vector3 photoPosition = cameraTransform.position;
			
			// If there's ever any reason to change the x offset relative to the parent
			// go ahead and set up the code for doing that
			photoPosition += cameraTransform.right * printedOffset.x;
			
			// The photograph should start in the photo slot
			photoPosition += cameraTransform.up * printedOffset.y;
			
			// After printing, the photo should be right outside of the camera
			photoPosition += cameraTransform.forward * printedOffset.z;
			
			previousPhotograph = Instantiate(photoPrefab, photoPosition, cameraTransform.rotation, cameraTransform);
			previousPhotograph.GetComponent<Photograph>().setPicture(waitingTextures.Dequeue());
			
			printingPhoto.SetActive(false);
		}
		
		// When the printing animation begins, make sure the previous photograph won't get in the way
		public void setupPrinting()
		{
			// Detach the previous photo object, if it exists and is still attached
			if(previousPhotograph)
			{
				Photograph previousPhotoScript = previousPhotograph.GetComponent<Photograph>();
				if(previousPhotoScript.getIsAttached())
				{
					previousPhotoScript.splitFromCamera();
				}
			}
			
			printingPhoto.GetComponent<Photograph>().setPicture(waitingTextures.Peek());
		}
		
		
	}
}
