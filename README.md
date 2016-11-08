# Unity3D MVC

Basic scripts for creating Unity games following MVC patterns. The concept is based on the article [Unity with MVC: How to Level Up Your Game Development](https://www.toptal.com/unity-unity3d/unity-with-mvc-how-to-level-up-your-game-development) written by Eduardo Dias.

I've made some changes to fit my needs. Those are the scripts I use when starting a new Unity project. And here is how it's structured on the scene hierarchy:

![](screenshots/1.png)

## Minimum Requirements

Unity version 5.3.5f1, released 15 Mar 2016.

## Linking the scripts

On the root of "Scripts" folder you find the base scripts to link to those game objects of the screenshot above:

- GlobalStorage (GlobalStorage.cs)
- Application (Application.cs)
  - Model (ModelContainer.cs)
  - View (ViewContainer.cs)
  - Controller (ControllerContainer.cs)

## The containers

The game objects "Model", "View", "Controller" are containers for any other scripts you create inheriting the MvcBehaviour class, which is mandatorily a model, a view or a controller.

Here is an example of how you would add controllers to the Controller Container:

![](screenshots/2.png)

![](screenshots/3.png)

Note that TimeController, DoorController, AlertController and InvestmentAreaBarsController are all derived from MvcBehaviour too. And those scripts are attached to their related game objects on scene - children of the Controller game object.

## Separation in Models, Views and Controllers

In summary, _Models_ hold the data structure and current values, _Views_ hold the references of game objects on scene and _Controllers_ control how the objects will behave on scene, using the data from models and the references from views.

## The GlobalStorage - Passing values/objets through scenes

Now you might be asking: "If my game has more than one scene, do I need to crate a MVC structure in all scenes? If so, do I have to reinitialize the game objects everytime? Then, how to I keep the state of my game through the scenes?"

Yes, you need to create the same structure on all scenes, and yes, you lose the data from the scripts on your game objects.

That's where the GlobalStorage comes in. It has a singleton implementation, so even if you place the GlobalStorage on all scenes, it will alawys keep only one instance (deleting the others). And due to its _DontDestroyOnLoad_ behavior, it'll be available on all scenes. The GlobalStorage will not only store your variables. It'll also show them on Unity's Inspector Window, so you can keep track of everything that is being stored.

## GlobalStorage usage is really simple

To save some value on the storage:
```
GlobalStorage.Save("name", "John Doe");     // Saving strings.
GlobalStorage.Save("age", 28);              // Saving integers, positive and negative.
GlobalStorage.Save("experience", 473.32);   // Faving doubles/floats, positive and negative.
GlobalStorage.Save("isRunning", true);      // Saving booleans, true or false.
GlobalStorage.Save("status", playerStatus); // Saving objects. In this case, playerStatus is an instance of PlayerStatus class.
```

To load some value from the storage:
```
var name = GlobalStorage.Load<string>("name");             // Note that we need to cast
var age = GlobalStorage.Load<int>("age");                  // the type of the object
var experience = GlobalStorage.Load<double>("experience"); // being recovered from the
var isRunning = GlobalStorage.Load<bool>("isRunning");     // storage. That's how the script
var status =  GlobalStorage.Load<PlayerStatus>("status");  // knows how to treat the value.
```

To delete some value from the storage:
```
GlobalStorage.Delete("name");
GlobalStorage.Delete("age");
GlobalStorage.Delete("experience");
GlobalStorage.Delete("isRunning");
GlobalStorage.Delete("status");
```

## License

The MIT License  
<http://victor.mit-license.org>
