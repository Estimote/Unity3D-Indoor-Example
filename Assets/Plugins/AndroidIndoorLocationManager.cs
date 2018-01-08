using UnityEngine;

class AndroidIndoorLocationManager : IndoorLocationManager
{
	private OnPositionUpdateListener positionListener;

	public void Start (string appId, string appToken, string locationId)
	{
		Debug.Log ("AndroidIndoorLocationManager Start");

		AndroidJNIHelper.debug = true;

		AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

		AndroidJavaObject cloudCredentials = new AndroidJavaObject ("com.estimote.cloud_plugin.common.EstimoteCloudCredentials", appId, appToken);

		AndroidJavaObject cloudManagerFactory = new AndroidJavaObject ("com.estimote.indoorsdk_module.cloud.IndoorCloudManagerFactory");
		AndroidJavaObject cloudManager = cloudManagerFactory.Call<AndroidJavaObject> ("create", activity, cloudCredentials);

		Debug.Log ("AndroidIndoorLocationManager setting up CloudManagerCallback");

		positionListener = new OnPositionUpdateListener ();
		cloudManager.Call ("getLocation", locationId, new CloudManagerCallback (activity, cloudCredentials, positionListener));
	}

	public bool IsInsideLocation ()
	{
		return positionListener.IsInsideLocation;
	}

	public double GetX ()
	{
		return positionListener.LastKnownX;
	}

	public double GetY ()
	{
		return positionListener.LastKnownY;
	}

	// ...

	class CloudManagerCallback : AndroidJavaProxy
	{
		private AndroidJavaObject activity;
		private AndroidJavaObject cloudCredentials;
		private OnPositionUpdateListener positionListener;

		public CloudManagerCallback (AndroidJavaObject activity, AndroidJavaObject cloudCredentials, OnPositionUpdateListener positionListener)
			: base ("com.estimote.indoorsdk_module.cloud.CloudCallback")
		{
			this.activity = activity;
			this.cloudCredentials = cloudCredentials;
			this.positionListener = positionListener;
		}

		public void success (AndroidJavaObject location)
		{
			AndroidJavaObject indoorManagerBuilder = new AndroidJavaObject ("com.estimote.indoorsdk.IndoorLocationManagerBuilder", activity, location, cloudCredentials);
			AndroidJavaObject defaultScannerBuilder = indoorManagerBuilder.Call<AndroidJavaObject> ("withDefaultScanner");
			AndroidJavaObject indoorManager = defaultScannerBuilder.Call<AndroidJavaObject> ("build");

			indoorManager.Call ("setOnPositionUpdateListener", positionListener);
			indoorManager.Call ("startPositioning");
		}

		public void failure (AndroidJavaObject estimoteCloudException)
		{
			Debug.LogError ("CloudManagerCallback failure: " + estimoteCloudException);
		}
	}

	class OnPositionUpdateListener : AndroidJavaProxy
	{
		public bool IsInsideLocation { get; set; }

		public double LastKnownX { get; set; }

		public double LastKnownY { get; set; }

		public OnPositionUpdateListener () : base ("com.estimote.indoorsdk_module.algorithm.OnPositionUpdateListener")
		{
		}

		public void onPositionUpdate (AndroidJavaObject position)
		{
			IsInsideLocation = true;
			LastKnownX = position.Call<double> ("getX");
			LastKnownY = position.Call<double> ("getY");
		}

		public void onPositionOutsideLocation ()
		{
			IsInsideLocation = false;
		}
	}
}
