public interface IndoorLocationManager
{
	void Start (string appId, string appToken, string locationId);

	bool IsInsideLocation ();

	double GetX ();

	double GetY ();
}
