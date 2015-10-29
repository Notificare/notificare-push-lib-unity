Notificare Unity Plugin
=================
Wrapper for using Notificare with Unity.

Currently supports Notificare Push and Monetize services. Project is currently in development, focusing on iOS first, and not guaranteed to work. Current version is untested.

How To Use (iOS)
----------------
1. Create an empty Unity Project targeting iOS.
2. If not present, create a Plugins folder in Assets.
3. Copy iOS Push Lib to Plugins.
4. Fill out Notificare.plist in the iOS Push Lib folder.
5. Copy MonoBehaviourExtended.cs, Newtonsoft.Json.dll, NotificarePushLib.cs and Singleton.cs to Plugins. (For now these files are in the example project.)
6. Copy NotificarePushLibUnity.h and NotificarePushLibUnity.m to Plugins
7. In Unity, under Player Settings, set Api Compatibility Level to .NET 2.0. (Might be necessary for Newtonsoft Json.NET. Have to verify this.)
8. In Unity create an empty GameObject and add NotificarePushLib.cs as a component.
9. Set the number of subscribers to at least 1 in the Inspector.
10. Create a (C#) script that subclasses NotificareMonoBehaviour. (Don't know if this is going to work for UnityScript.)
11. Override any of the virtual methods defined in the NotificareMonoBehaviour class.
12. Create another empty GameObject and add the script you just created.
13. In the Inspector, set Notificare Push Lib to the GameObject that holds NotificarePushLib.cs.
14. Select the GameObject with NotificarePushLib.cs in the Hierarchy and set Element 0 under Subscribers to the GameObject that holds the script you created.
15. This is where the example is at currently.
16. Building, XCode and running coming up. Also see the guide for iOS.