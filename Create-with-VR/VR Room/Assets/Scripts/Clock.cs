namespace VARLab.UnityVRLearn.Clock {
	using System.Collections;
	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.Assertions;


	public class Clock : MonoBehaviour
	{
		#region Hands
		
		[SerializeField]
		[Tooltip("Reference to the second (unit of time) hand of this clock")]
		private Transform secondHand;
		
		[SerializeField]
		[Tooltip("Reference to the minute hand of this clock")]
		private Transform minuteHand;
		
		[SerializeField]
		[Tooltip("Reference to the hour hand of this clock")]
		private Transform hourHand;
		
		#endregion Hands
		
		// Update is called once per frame
		void Update()
		{
			System.DateTime currentTime = System.DateTime.Now;
			
			/*
			Store the angle that the second hand should be rotated. This is calculated as
			(currentTime.Second * 360) / 60, simplified to (currentTime.Second * 6) to avoid
			intermediate calculations that invite floating point error
			*/
			float secondAngle = (currentTime.Second * 6);

			// Convert the rotation from an Euler angle to a quaternion
			secondHand.localRotation = Quaternion.Euler(
				new Vector3(secondAngle, 0, 0)
			);
			
			/* Store the angle that the minute hand should be rotated. This is calculated as
			(currentTime.Minute * 360) / 60 + (currentTime.Second * 360) / (60  * 60), simplified to 
			(currentTime.Minute * 6) + (currentTime.Second / 10.0) to avoid
			intermediate calculations that invite floating point error
			*/
			float minuteAngle = ((float) (currentTime.Minute * 6)) + (((float) currentTime.Second) / ((float) 10.0));
		
			//Convert the rotation from an Euler angle to a quaternion
			minuteHand.localRotation = Quaternion.Euler(
				new Vector3(minuteAngle, 0, 0)
			);
			
			// DateTime stores the hour in 24 hour format, so we convert to a 12 hour format
			int clockTime = currentTime.Hour % 12;
			
			/* Store the angle that the hour hand should be rotated. This is calculated as
			(clockTime * 360) / 12 + (currentTime.Minute * 360) / (60 * 12) + (currentTime.Second * 360) / (60  * 60 * 12), simplified to 
			(clockTime * 30) + ((currentTime.Minute * 6) + (currentTime.Second / 10.0)) / 12.0 to allow reuse of
			the minuteAngle value calculated earlier
			*/
			float hourAngle = ((float) (clockTime * 30)) + (minuteAngle / ((float) 12.0));
			
			//Convert the rotation from an Euler angle to a quaternion
			hourHand.localRotation = Quaternion.Euler(
				new Vector3(hourAngle, 0, 0)
			);
			
			
			
		}
		
		private void Awake()
		{
			Assert.IsNotNull(secondHand, "Clock.secondHand must be set!");
			Assert.IsNotNull(minuteHand, "Clock.minuteHand must be set!");
			Assert.IsNotNull(hourHand, "Clock.hourHand must be set!");
			

		}
	}
}
