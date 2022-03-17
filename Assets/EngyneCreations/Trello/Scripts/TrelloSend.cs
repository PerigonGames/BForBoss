/*
 * TrelloSend.cs
 * Script that holds keys and allows to send Trello cards.
 *  
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using System;
using UnityEngine;

namespace Trello
{
    public class TrelloSend : MonoBehaviour
    {
        /// <summary>
        /// Sends a given Trello card using the authorization settings.
        /// </summary>
        /// <param name="card">Trello card to send.</param>
        /// <param name="list">Trello list to upload card to</param>
        /// <param name="board">Overrides default board.</param>
        /// <param name="onComplete">Callback to be called after Trello card is made (or on failure to create)</param>
        public static void SendNewCard(TrelloCard card, string list, string board = null, Action<bool> onComplete = null)
        {
            Send_Internal(card, list, board, onComplete);
        }

        private static async void Send_Internal(TrelloCard card, string list, string board = null, Action<bool> onComplete = null)
        {
            TrelloAPI api = new TrelloAPI(() => onComplete?.Invoke(false));
            
            await api.SetCurrentBoard(board);
            card.listId = await api.SetCurrentList(list);
            await api.UploadCard(card);
            onComplete?.Invoke(true);
        }
    }
}