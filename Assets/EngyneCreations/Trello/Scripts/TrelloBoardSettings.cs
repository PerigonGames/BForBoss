using System;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Trello
{
	public class TrelloBoardSettings : ScriptableObject
	{
		[Header("API Key and Token needed for Interacting with Trello")]
		public string APIKey;
		public string APIToken;

		private static TrelloBoardSettings _instance;

		public static TrelloBoardSettings Instance
		{
			get
			{
				LoadSettings();
				return _instance;
			}
		}

		public static void LoadSettings()
		{
			_instance = FindOrCreateInstance();
		}

		private static TrelloBoardSettings FindOrCreateInstance()
		{
			TrelloBoardSettings instance = null;
			instance = Resources.Load<TrelloBoardSettings>("TrelloBoard");
			instance = instance ? instance : Resources.LoadAll<TrelloBoardSettings>(string.Empty).FirstOrDefault();
			instance = instance ? instance : CreateAndSave<TrelloBoardSettings>();

			if (instance == null)
			{
				throw new Exception("Could not find or create settings for Trello API");
			}

			return instance;
		}
		
		private static T CreateAndSave<T>() where T : ScriptableObject
		{
			T instance = CreateInstance<T>();
#if UNITY_EDITOR
			//Saving during Awake() will crash Unity, delay saving until next editor frame
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.delayCall += () => SaveAsset(instance);
			}
			else
			{
				SaveAsset(instance);
			}
#endif
			return instance;
		}
		
#if UNITY_EDITOR
		private static void SaveAsset<T>(T obj) where T : ScriptableObject
		{

			string dirName = "Assets/Resources";
			if (!Directory.Exists(dirName))
			{
				Directory.CreateDirectory(dirName);
			}
			AssetDatabase.CreateAsset(obj, "Assets/Resources/TrelloBoard.asset");
			AssetDatabase.SaveAssets();
		}
#endif
	}
}
