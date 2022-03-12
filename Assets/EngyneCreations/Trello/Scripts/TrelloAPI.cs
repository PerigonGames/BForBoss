/*
 * TrelloAPI.cs
 * Interact directly with the Trello API using MiniJSON and uploads cards. 
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using UnityEngine.Networking;

namespace Trello 
{
	public class TrelloAPI 
	{
		private const string MEMBER_BASE_URL = "https://api.trello.com/1/members/me";
		private const string BOARD_BASE_URL = "https://api.trello.com/1/boards/";
		private const string LIST_BASE_URL = "https://api.trello.com/1/lists/";
		private const string CARD_BASE_URL = "https://api.trello.com/1/cards/";
		
		private string _token;
		private string _key;
		private List<object> _boards;
		private List<object> _lists;
		private string _currentBoardId = null;
		private string _currentListId = null;


		/// <summary>
		/// Generate new Trello API instance.
		/// </summary>
		/// <param name="key">Trello API key, keep it private.</param>
		/// <param name="token">Trello API token, keep it private.</param>
		public TrelloAPI(string key, string token)
		{
			_key = key;
			_token = token;
		}

		/// <summary>
		/// Download the list of available boards for the user and store them.
		/// </summary>
		/// <returns>Downloaded boards.</returns>
		public IEnumerator PopulateBoards()
		{
			_boards = null;

			using (UnityWebRequest request = UnityWebRequest.Get(string.Format("{0}?key={1}&token={2}&boards=all", MEMBER_BASE_URL, _key, _token)))
			{
				yield return request.SendWebRequest();
			
				if (request.result == UnityWebRequest.Result.Success && request.downloadHandler != null)
				{
					var dict = Json.Deserialize(request.downloadHandler.text) as Dictionary<string,object>;
					_boards = (List<object>)dict["boards"];
				}
				else
				{
					throw new TrelloException($"Unable to populate board from Trello : {request.error}");
				}
				
			}
		}
		
		/// <summary>
		/// Sets the given board to search for lists in.
		/// </summary>
		/// <param name="name">Name of the board.</param>
		public void SetCurrentBoard(string name)
		{
			if (_boards == null)
			{
				throw new TrelloException("There are no boards available. Either the user does not have access to a board or PopulateBoards() wasn't called.");
			}
			
			for (int i = 0; i < _boards.Count; i++)
			{
				var board = (Dictionary<string, object>)_boards[i];
				if ((string)board["name"] == name)
				{
					_currentBoardId = (string)board["id"];
					return;
				}
			}
			
			_currentBoardId = null;
			throw new TrelloException("A board with the name " + name + " was not found.");
		}

		/// <summary>
		/// Download all the lists of the selected board and store them.
		/// </summary>
		/// <returns>Downloaded list.</returns>
		public IEnumerator PopulateLists()
		{
			_lists = null;
			
			if (_currentBoardId == null)
			{
				throw new TrelloException("Cannot retreive the lists, there isn't a selected board yet.");
			}

			using (UnityWebRequest request = UnityWebRequest.Get(string.Format("{0}{1}?key={2}&token={3}&lists=all", BOARD_BASE_URL, _currentBoardId, _key, _token)))
			{
				yield return request.SendWebRequest();

				if (request.result == UnityWebRequest.Result.Success && request.downloadHandler != null)
				{
					var dict = Json.Deserialize(request.downloadHandler.text) as Dictionary<string,object>;
					_lists = (List<object>)dict["lists"];
				}
				else
				{
					throw new TrelloException($"Unable to retrieve the lists : {request.error}");
				}
			}
		}

		/// <summary>
		/// Sets the given list to upload cards to.
		/// </summary>
		/// <param name="name">Name of the list.</param>
		public void SetCurrentList(string name)
		{
			if (_lists == null)
			{
				throw new TrelloException("There are no lists available. Either the board does not contain lists or PopulateLists() wasn't called.");
			}

			for (int i = 0; i < _lists.Count; i++)
			{
				var list = (Dictionary<string, object>)_lists[i];
				if ((string)list["name"] == name)
				{
					_currentListId = (string)list["id"];
					return;
				}
			}
			
			_currentListId = null;
			throw new TrelloException("A list with the name " + name + " was not found.");
		}

		/// <summary>
		/// Creates a new Trell List for current Board ID
		/// </summary>
		/// <param name="name">name of the Trello List</param>
		/// <returns></returns>
		/// <exception cref="TrelloException"></exception>
		public IEnumerator CreateNewList(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new TrelloException("Unable to name the new list.");
			}

			WWWForm post = new WWWForm();
			post.AddField("name", name);
			post.AddField("idBoard", _currentBoardId);

			using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}?key={1}&token={2}", LIST_BASE_URL, _key, _token), post))
			{
				yield return request.SendWebRequest();
				
				if (request.result != UnityWebRequest.Result.Success)
				{
					throw new TrelloException($"Unable to create new Trello List : {request.error}");
				}
				
			}
		}
		
		/// <summary>
		/// Returns the selected Trello list id.
		/// </summary>
		/// <returns>The list id.</returns>
		public string GetCurrentListId()
		{
			if (_currentListId == null)
			{
				throw new TrelloException("A list has not been selected. Call SetCurrentList() first.");
			}
			return _currentListId;
		}
		
		/// <summary>
		/// Uploads a given TrelloCard object to the Trello server.
		/// </summary>
		/// <param name="card">Trello card to upload.</param>
		/// <exception cref="TrelloException"></exception>
		public IEnumerator UploadCard(TrelloCard card) 
		{
			if (!card.IsValid())
			{
				throw new TrelloException("Invalid Trello Card, unable to upload");
			}
			
			WWWForm post = new WWWForm();
			post.AddField("name", card.name);
			post.AddField("desc", card.desc);
			post.AddField("due", card.due);
			post.AddField("idList", card.idList);
			post.AddField("urlSource", card.urlSource);
			if (card.fileSource != null && card.fileName != null) 
			{
				post.AddBinaryData("fileSource", card.fileSource, card.fileName);
			}

			using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}?key={1}&token={2}", CARD_BASE_URL, _key, _token), post))
			{
				yield return request.SendWebRequest();

				if (request.result == UnityWebRequest.Result.Success)
				{
					Debug.Log($"Trello Card \"{card.name}\" was successfully uploaded");
				}
				else
				{
					throw new TrelloException($"Unable to upload Trello Card: {request.error}");
				}
			}
		}
	}
}