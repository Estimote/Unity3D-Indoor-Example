class MockIndoorLocationManager : IndoorLocationManager
{
	public void Start (string appId, string appToken, string locationId)
	{
	}

	public bool IsInsideLocation ()
	{
		return false;
	}

	public double GetX ()
	{
		return 1.23;
	}

	public double GetY ()
	{
		return 4.56;
	}
}
