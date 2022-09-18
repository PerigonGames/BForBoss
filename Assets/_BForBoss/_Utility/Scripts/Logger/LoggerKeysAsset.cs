using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Perigon.Utility
{
    [FilePath("Perigon/Logger.asset", FilePathAttribute.Location.ProjectFolder)]
    public class LoggerKeysAsset : ScriptableSingleton<LoggerKeysAsset>
    {
        [SerializeField] private List<string> _loggingKeys = new List<string>();

        public List<string> LoggingKeys => _loggingKeys;

        public void AddKey(string key)
        {
            _loggingKeys.Add(key);
            Save(true);
        }

        public void RemoveKey(string key)
        {
            _loggingKeys.Remove(key);
            Save(true);
        }
    }
}
