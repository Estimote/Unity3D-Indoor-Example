using UnityEngine;

public class IndoorLocationManagerFactory
{
	public static IndoorLocationManager Create ()
	{
		switch (Application.platform) {
		case RuntimePlatform.Android:
			return new AndroidIndoorLocationManager ();
		case RuntimePlatform.IPhonePlayer:
			return new iOSIndoorLocationManager ();
		default:
			return new MockIndoorLocationManager ();
		}
	}
}
