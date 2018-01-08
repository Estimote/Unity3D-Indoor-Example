using System.Runtime.InteropServices;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

#endif

class iOSIndoorLocationManager : IndoorLocationManager
{
	[DllImport ("__Internal")]
	private static extern void _Start (string appId, string appToken, string locationId);

	[DllImport ("__Internal")]
	private static extern bool _IsInsideLocation ();

	[DllImport ("__Internal")]
	private static extern double _GetX ();

	[DllImport ("__Internal")]
	private static extern double _GetY ();

	public void Start (string appId, string appToken, string locationId)
	{
		_Start (appId, appToken, locationId);
	}

	public bool IsInsideLocation ()
	{
		return _IsInsideLocation ();
	}

	public double GetX ()
	{
		return _GetX ();
	}

	public double GetY ()
	{
		return _GetY ();
	}

	/* Post-build hook to modify the Xcode project for Indoor SDK */

	#if UNITY_EDITOR

	[PostProcessBuildAttribute (1)]
	public static void OnPostprocessBuild (BuildTarget platform, string pathToBuiltProject)
	{
		if (platform == BuildTarget.iOS) {
			string pbxProjectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

			PBXProject pbxProject = new PBXProject ();
			pbxProject.ReadFromFile (pbxProjectPath);

			string target = pbxProject.TargetGuidByName ("Unity-iPhone");

			// Estimote Indoor SDK is built without Bitcode, so we have to disable it
			pbxProject.SetBuildProperty (target, "ENABLE_BITCODE", "NO");

			pbxProject.WriteToFile (pbxProjectPath);
		}
	}

	#endif
}
