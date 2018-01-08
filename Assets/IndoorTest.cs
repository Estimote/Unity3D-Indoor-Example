using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class IndoorTest : MonoBehaviour
{

	public Text text;

	public IndoorLocationManager indoorManager = IndoorLocationManagerFactory.Create ();

	void Start ()
	{
		indoorManager.Start ("<APP ID>", "<APP TOKEN>", "<LOCATION IDENTIFIER>");
	}

	void Update ()
	{
		if (indoorManager.IsInsideLocation ()) {
			double x = indoorManager.GetX ();
			double y = indoorManager.GetY ();

			text.text = string.Format ("x = {0:0.00}, y = {1:0.00}", x, y);
		} else {
			text.text = "outside location";
		}
	}
}
