using UnityEngine;

namespace LiveLarson.Camera.CameraFade
{
	public class CameraFade : MonoBehaviour
	{
		public void OnPostRender()
		{
			CameraFadeSystem.OnPostRenderUpdate();
		}
	}
}