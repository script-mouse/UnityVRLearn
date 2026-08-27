namespace VARLab.UnityVRLearn.BouncyBall 
{
	using System.Collections;
	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.Assertions;




	
	public class Ball : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The audio file that plays when this object collides with anything else")]
		private AudioClip bounceAudioClip;
		
		//Audio Source that plays sound when the ball hits anything
		private AudioSource bounceSource;
		
		private void Start()
		{
			bounceSource = GetComponent<AudioSource>();
			Assert.IsNotNull(bounceSource, "Objects with the VARLab.UnityVRLearn.BouncyBall.Ball script component must also have an AudioSource component!");
		}
		
		void OnCollisionEnter(Collision other)
		{
			bounceSource.PlayOneShot(bounceAudioClip, System.Math.Min(
				Unity.Mathematics.math.log2(other.relativeVelocity.magnitude) / 3.0f,
				1.0f
				)
			);
		}
		
		private void Awake()
		{
			Assert.IsNotNull(bounceAudioClip, "VARLab.UnityVRLearn.BouncyBall.Ball.bounceAudioClip must be set!");
		}
	}
}