using UnityEngine;

public class GlobalStorage : MonoBehaviour
{
	// Here you declare any other variables you
	// want to make accessible through all scenes.
	// For example: Health, Score, Kills.

	#region Singleton definition
	static GlobalStorage instance;

	public static GlobalStorage Instance {
		get { return instance; }
	}

	void GrantSingleInstance() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}
	#endregion

	void Awake ()
	{
		GrantSingleInstance ();
		DontDestroyOnLoad (gameObject);
	}
}