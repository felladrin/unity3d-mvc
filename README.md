# Unity3D MVC

Basic scripts for creating Unity games following MVC patterns. The concept is based on the article [Unity with MVC: How to Level Up Your Game Development](https://www.toptal.com/unity-unity3d/unity-with-mvc-how-to-level-up-your-game-development) written by Eduardo Dias.

I've made some changes to fit my needs. Those are the scripts I use when starting a new Unity project. And here is how it's structured on the scene hierarchy:

![screenshot](screenshot.png)

## Linking the scripts

On the root of "Scripts" folder you find the base scripts to link to those game objects of the screenshot above:

- GlobalStorage (GlobalStorage.cs)
- Application (Application.cs)
- -- Model (ModelContainer.cs)
- -- View (ViewContainer.cs)
- -- Controller (ControllerContainer.cs)


## The containers

The game objects "Model", "View", "Controller" are containers for any other scripts you create inheriting the MvcBehaviour class, which is mandatorily a model, a view or a controller. (Examples coming soon)
