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
5. Copy MonoBehaviourExtended.cs, LitJson.dll, NotificarePushLib.cs and Singleton.cs to Plugins. (For now these files are in the example project.)
6. Copy NotificarePushLibUnity.h and NotificarePushLibUnity.m to Plugins
7. In Unity create an empty GameObject and add NotificarePushLib.cs as a component.
8. Set the number of subscribers to at least 1 in the Inspector.
9. Create a (C#) script that subclasses NotificareMonoBehaviour. (Don't know if this is going to work for UnityScript.)
10. Override any of the virtual methods defined in the NotificareMonoBehaviour class.
11. Create another empty GameObject and add the script you just created.
12. In the Inspector, set Notificare Push Lib to the GameObject that holds NotificarePushLib.cs.
13. Select the GameObject with NotificarePushLib.cs in the Hierarchy and set Element 0 under Subscribers to the GameObject that holds the script you created.
14. Build the project and open it in XCode.
15. Currently, Notificare.plist, NotificareTags.plist and DefaultTheme.bundle aren't being copied by Unity. Copy them to Libraries>Plugins>iOS>notificare-push-lib-1.6.0.
16. Go to the project's Build Phases, open Copy Bundle Resources and make sure DefaultTheme.bundle is listed only once (delete any extra entries).
17. Check the [iOS Implementation Guide](http://docs.notifica.re/sdk/implementation/), follow the instructions under Manual Installation and add any missing frameworks.
18. Should be ready to run now.

References
---------------
- Unity
	1. [Native Plugins](http://docs.unity3d.com/Manual/NativePlugins.html)
	2. [Building Plugins for iOS](http://docs.unity3d.com/Manual/PluginsForIOS.html)
	3. [Low-level Native Plugin Interface](http://docs.unity3d.com/Manual/NativePluginInterface.html)
	4. [WRITING PLUGINS](https://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/writing-plugins)
	5. [NotificationServices](http://docs.unity3d.com/ScriptReference/iOS.NotificationServices.html)
	6. [Singleton](http://wiki.unity3d.com/index.php/Singleton)
- Mono
	1. [Embedding Mono](http://www.mono-project.com/docs/advanced/embedding/)
	2.  [Interop with Native Libraries](http://www.mono-project.com/docs/advanced/pinvoke/)
	3. [Marshaling Data with Platform Invoke](https://msdn.microsoft.com/en-US/library/fzhhdwae(v=vs.110).aspx)
- LitJSON
	1. [Project](https://lbv.github.io/litjson/)
	2. [GitHub](https://github.com/lbv/litjson/releases)
	3. [Documentation](https://lbv.github.io/litjson/docs/quickstart.html)
 