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
		
		public static TrelloBoardSettings LoadSettings()
		{
			return FindOrCreateInstance();
		}

		private static TrelloBoardSettings FindOrCreateInstance()
		{
			TrelloBoardSettings settings = null;
			settings = Resources.Load<TrelloBoardSettings>("TrelloBoard") ?? CreateAndSave<TrelloBoardSettings>();
			
			if (settings == null)
			{
				throw new Exception("Could not find or create settings for Trello API");
			}
			
			return settings;
		}
		
		private static T CreateAndSave<T>() where T : ScriptableObject
		{
			T settings = CreateInstance<T>();
#if UNITY_EDITOR
			//Saving during Awake() will crash Unity, delay saving until next editor frame
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.delayCall += () => SaveAsset(settings);
			}
			else
			{
				SaveAsset(settings);
			}
#endif
			return settings;
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
