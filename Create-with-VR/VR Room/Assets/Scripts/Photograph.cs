namespace VARLab.UnityVRLearn.OldSchoolCamera
{
	using System.Collections;
	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.Assertions;
	
	[RequireComponent(typeof(Rigidbody))]
	public class Photograph : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The region of the photograph that the picture will be rendered to")]
		private GameObject photoFilm;
	
	
		private Rigidbody photoRigidbody;
		
		// false if the photograph has detached from the camera (see splitFromCamera), true otherwise
		private bool isAttached;
		void Awake()
		{
			Assert.IsNotNull(photoFilm, "VARLab.UnityVRLearn.OldSchoolCamera.Photograph.photoFilm must be set!");
			
			isAttached = true;
			photoRigidbody = GetComponent<Rigidbody>();
		}
		
		// Register that this photograph is not attached to the camera
		public void detach()
		{
			isAttached = false;
		}
		
		// Stop considering this photograph as linked to the camera it came from
		public void splitFromCamera()
		{
			Transform photoPosition = GetComponent<Transform>();
			photoPosition.SetParent(null, true);
			
			photoRigidbody.isKinematic = false;
			
			detach();
		}
		
		public bool getIsAttached()
		{
			return isAttached;
		}
		
		public void setPicture(Texture2D picture)
		{
			photoFilm.GetComponent<Renderer>().material.mainTexture = picture;
		}
	}

}
