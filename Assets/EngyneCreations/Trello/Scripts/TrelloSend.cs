/*
 * TrelloSend.cs
 * Script that holds keys and allows to send Trello cards.
 *  
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using UnityEngine;

namespace Trello
{
    public class TrelloSend : MonoBehaviour
    {
        private const string DEFAULT_BOARD = "Level Design Board 1";
        
        /// <summary>
        /// Sends a given Trello card using the authorization settings.
        /// </summary>
        /// <param name="card">Trello card to send.</param>
        /// <param name="list">Trello list to upload card to</param>
        /// <param name="board">Overrides default board.</param>
        public static void SendNewCard(TrelloCard card, string list, string board = null)
        {
            Send_Internal(card, list, board);
        }

        private static async void Send_Internal(TrelloCard card, string list, string board = null)
        {
            TrelloAPI api = new TrelloAPI();
            
            await api.SetCurrentBoard(string.IsNullOrEmpty(board) ? DEFAULT_BOARD : board);
            card.listId = await api.SetCurrentList(list);
            await api.UploadCard(card);
        }
    }
}