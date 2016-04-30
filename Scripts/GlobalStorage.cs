using UnityEngine;

public class GlobalStorage : MonoBehaviour
{
	// Here you declare any other variables you
	// want to make accessible through all scenes.
	// For example: Health, Score, Kills.

	#region Singleton definition
	public static GlobalStorage Instance { get; private set; }

	void GrantSingleInstance ()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy (gameObject);
	}
	#endregion

	void Awake ()
	{
		GrantSingleInstance ();
		DontDestroyOnLoad (gameObject);
	}
}