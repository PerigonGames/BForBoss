using System.Text;
using Trello;
using UnityEditor;
using UnityEngine;

namespace Perigon.Utility
{
    public static class LevelDesignFeedbackSender
    {
        private static string _cardName;
        
        
        public static void SendFeedback(string name, string feedback, Texture2D image)
        {
            _cardName = name;
            TrelloCard card = new TrelloCard
            {
                name = name,
                desc = GenerateDescription(feedback),
                attachment = new TrelloCard.Attachment(image, "Attachment.jpg")
            };
            TrelloSend.SendNewCard(card, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, null, OnFeedbackSent);
        }

        private static void OnFeedbackSent(bool success)
        {
            string message = $"The Trello Card {_cardName} was {(success ? "successfully" : "unsuccessfully")} created";
            EditorUtility.DisplayDialog("Level Design Feedback Window", message, "Sounds good");
        }
        
        private static string GenerateDescription(string feedback)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("#Feedback");
            stringBuilder.AppendLine("___");
            stringBuilder.AppendLine("###User Description");
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine(feedback);
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine("___");

            if (Camera.main != null)
            {
                stringBuilder.AppendLine("###Additional Details");
                stringBuilder.AppendLine($"Main Camera Position : {Camera.main.transform.position}");
            }

            return stringBuilder.ToString();
        }
    }
}