/*
 * TrelloCard.cs
 * Base class for a Trello card.
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using UnityEngine;

namespace Trello
{
    public class TrelloCard
    {
        public struct Attachment
        {
            private byte[] _fileSource;
            private string _fileName;

            public byte[] FileSource => _fileSource;
            public string FileName => _fileName;

            public Attachment(byte[] fileSource, string fileName)
            {
                _fileSource = fileSource;
                _fileName = fileName;
            }

            public Attachment(Texture2D jpeg, string fileName)
            {
                _fileSource = jpeg.EncodeToJPG();
                _fileName = fileName;
            }

            public bool IsValid()
            {
                return _fileSource != null && !string.IsNullOrEmpty(_fileName);
            }
        }
        
        public string name = "";
        public string desc = "";
        public string due = "null";
        public string listId = "";
        public string urlSource = "null";
        public Attachment attachment;

        /// <summary>
        /// Base class for a Trello card.
        /// </summary>
        public TrelloCard() 
        {
        }

        /// <summary>
        /// Check if TrelloCard is Valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(name);
        }


    }
}