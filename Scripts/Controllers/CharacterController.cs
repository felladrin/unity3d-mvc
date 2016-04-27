public class CharacterController : MvcBehaviour
{
	CharacterModel model;
	CharacterView view;

	void Start ()
	{
		model = App.Model.Character;
		view = App.View.Character;
	}

	// Implment here all other actions a character
	// can execute or receive, for example:
	// ApplyStatsBonus. TakeHit, PlayEmote...

	// From here (the controller) you can access
	// the CharacterModel through 'model' variable
	// and CharacterView through 'view' variable
}
