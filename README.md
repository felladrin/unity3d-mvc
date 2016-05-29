# Unity3D MVC

Basic scripts for creating Unity games following MVC patterns. The concept is based on the article [Unity with MVC: How to Level Up Your Game Development](https://www.toptal.com/unity-unity3d/unity-with-mvc-how-to-level-up-your-game-development) written by Eduardo Dias.

I've made some changes to fit my needs. Those are the scripts I use when starting a new Unity project. And here is how it's structured on the scene hierarchy:

![](screenshots/1.png)

## Linking the scripts

On the root of "Scripts" folder you find the base scripts to link to those game objects of the screenshot above:

- GlobalStorage (GlobalStorage.cs)
- Application (Application.cs)
- -- Model (ModelContainer.cs)
- -- View (ViewContainer.cs)
- -- Controller (ControllerContainer.cs)

## The containers

The game objects "Model", "View", "Controller" are containers for any other scripts you create inheriting the MvcBehaviour class, which is mandatorily a model, a view or a controller.

Here is an example of how you would add controllers to the Controller Container:

![](screenshots/2.png)

![](screenshots/3.png)

Note that TimeController, DoorController, AlertController and InvestmentAreaBarsController are all derived from MvcBehaviour too. And those scripts are attached to their related game objects on scene - children of the Controller game object.

## Separation in Models, Views and Controllers

In summary, _Models_ hold the data structure, _Views_ hold the references of game objects on scene and _Controllers_ control how the objects will behave on scene, using the data from models and the references from views.