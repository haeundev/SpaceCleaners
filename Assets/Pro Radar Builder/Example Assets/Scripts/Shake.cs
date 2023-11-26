using UnityEngine;
using System.Collections;
public class Shake : MonoBehaviour
{
    [TextArea]
    public string text;
	//public CameraEffectsManager _CameraEffectsManager;
	public CharacterMotion characterMotion;
	private Quaternion originRotation;
	public float shake_decay;
	public float intensity;
	public float shake_intensity;
	public float HoldShake;
	private bool doShake;

	
	public void DoShake(){
		if(shake_intensity == 0)
		{
		originRotation = transform.localRotation;
		shake_intensity = intensity;
			//Invoke ("TheShake",0);
		}
	}

	public void FixedUpdate(){

		if (characterMotion.speed > 3f && characterMotion.speed < 5 && !doShake)
		{
			doShake = true;
			DoShake();
		}
		
		if (shake_intensity > 0) {

			if(Time.timeScale != 0)
			{
			transform.rotation = new Quaternion (
				originRotation.x + Random.Range (-shake_intensity, shake_intensity),
				originRotation.y + Random.Range (-shake_intensity, shake_intensity),
				originRotation.z + Random.Range (-shake_intensity, shake_intensity),
				originRotation.w + Random.Range (-shake_intensity, shake_intensity));
			shake_intensity -= shake_decay;
			//_CameraEffectsManager.initiliseCameraMotionBlur = true;
			}

			else{
				HoldShake = shake_intensity;
				shake_intensity = 0;
			
			}

		}

		else 
		{
			shake_intensity = 0;
			doShake = false;
			//_CameraEffectsManager.initiliseCameraMotionBlur = false;

		}

		if(Time.timeScale != 0 && HoldShake > 0)
		{

			shake_intensity = HoldShake;
			HoldShake = 0;


		}


	}

}
