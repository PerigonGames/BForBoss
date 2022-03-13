﻿/*
 * TrelloAPI.cs
 * Interact directly with the Trello API using MiniJSON and uploads cards. 
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using MiniJSON;
using UnityEngine.Networking;

namespace Trello 
{
	public class TrelloAPI 
	{
		private const string KEY = "b5d764e33331c58cfad14460e32def9f";
		private const string TOKEN = "d9f41075b982196531f134319f960281ec1024df209d16e8b1c01d4bc7ae4dc8";
		private const string DEFAULT_BOARD = "Level Design Board 1";
		
		private const string MEMBER_BASE_URL = "https://api.trello.com/1/members/me";
		private const string BOARD_BASE_URL = "https://api.trello.com/1/boards/";
		private const string LIST_BASE_URL = "https://api.trello.com/1/lists/";
		private const string CARD_BASE_URL = "https://api.trello.com/1/cards/";
		
		private List<object> _boards;
		private List<object> _lists;
		private string _currentBoardId = null;
		private string _currentListId = null;


		/// <summary>
		/// Generate new Trello API instance.
		/// </summary>
		public TrelloAPI()
		{
		}
		
		/// <summary>
		/// Sets the given board to search for lists in.
		/// </summary>
		/// <param name="name">Name of the board.</param>
		public async Task SetCurrentBoard(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new TrelloException("There are no boards available. Either the user does not have access to a board or PopulateBoards() wasn't called.");
			}

			await PopulateBoards();
			
			string currentBoardId = GetBoardId(name);

			if (!string.IsNullOrEmpty(currentBoardId))
			{
				_currentBoardId = currentBoardId;
				return;
			}
			
			//if board name does not exist, create new Trello board with given name
			await CreateNewBoard(name);
			await PopulateBoards();

			currentBoardId = GetBoardId(name);

			if (string.IsNullOrEmpty(currentBoardId))
			{
				throw new TrelloException("A board with the name " + name + " was not found.");
			}

			_currentBoardId = currentBoardId;
		}
		
		/// <summary>
		/// Sets the given list to upload cards to.
		/// </summary>
		/// <param name="name">Name of the list.</param>
		public async Task<string> SetCurrentList(string name)
		{
			await PopulateLists();
			
			if (_lists == null || string.IsNullOrEmpty(name))
			{
				throw new TrelloException("There are no lists available. Either the board does not contain lists or PopulateLists() wasn't called.");
			}
			
			string currentListId = GetListId(name);

			if (!string.IsNullOrEmpty(currentListId))
			{
				_currentListId = currentListId;
				return currentListId;
			}
			
			//if list name does not exist, create new Trello List with given name
			await CreateNewList(name);
			await PopulateLists();

			currentListId = GetListId(name);

			if (string.IsNullOrEmpty(currentListId))
			{
				throw new TrelloException("A list with the name " + name + " was not found.");
			}
			
			_currentListId = currentListId;
			return currentListId;
		}
		
		/// <summary>
		/// Uploads a given TrelloCard object to the Trello server.
		/// </summary>
		/// <param name="card">Trello card to upload.</param>
		/// <exception cref="TrelloException"></exception>
		public async Task UploadCard(TrelloCard card) 
		{
			if (!card.IsValid())
			{
				throw new TrelloException("Invalid Trello Card, unable to upload");
			}
			
			WWWForm post = new WWWForm();
			post.AddField("name", card.name);
			post.AddField("desc", card.desc);
			post.AddField("due", card.due);
			post.AddField("idList", card.listId);
			post.AddField("urlSource", card.urlSource);
			if (card.attachment.IsValid())
			{
				TrelloCard.Attachment attachment = card.attachment;
				post.AddBinaryData("fileSource", attachment.FileSource, attachment.FileName);
			}

			using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}?key={1}&token={2}", CARD_BASE_URL, KEY, TOKEN), post))
			{
				await request.SendWebRequest();

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
		
		/// <summary>
		/// Download the list of available boards for the user and store them.
		/// </summary>
		/// <returns>Downloaded boards.</returns>
		private async Task PopulateBoards()
		{
			using (UnityWebRequest request = UnityWebRequest.Get(string.Format("{0}?key={1}&token={2}&boards=all", MEMBER_BASE_URL, KEY, TOKEN)))
			{
				await request.SendWebRequest();

				_boards = null;
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
		/// Download all the lists of the selected board and store them.
		/// </summary>
		/// <returns>Downloaded list.</returns>
		private async Task PopulateLists()
		{
			_lists = null;
			
			if (_currentBoardId == null)
			{
				throw new TrelloException("Cannot retrieve the lists, there isn't a selected board yet.");
			}

			using (UnityWebRequest request = UnityWebRequest.Get(string.Format("{0}{1}?key={2}&token={3}&lists=all", BOARD_BASE_URL, _currentBoardId, KEY, TOKEN)))
			{
				await request.SendWebRequest();

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

		private async Task CreateNewBoard(string boardName)
		{
			if (string.IsNullOrEmpty(boardName))
			{
				throw new TrelloException("Unable to create the new board");
			}
			
			WWWForm post = new WWWForm();
			post.AddField("name",boardName);

			using (UnityWebRequest request = UnityWebRequest.Post(string.Format(string.Format("{0}?key={1}&token={2}", BOARD_BASE_URL, KEY, TOKEN)), post))
			{
				await request.SendWebRequest();

				if (request.result != UnityWebRequest.Result.Success)
				{
					throw new Exception($"Unable to create new Trello Board : {request.error}");
				}
			}
		}
		
		private async Task CreateNewList(string listName)
		{
			if (string.IsNullOrEmpty(listName))
			{
				throw new TrelloException("Unable to name the new list.");
			}

			WWWForm post = new WWWForm();
			post.AddField("name", listName);
			post.AddField("idBoard", _currentBoardId);

			using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}?key={1}&token={2}", LIST_BASE_URL, KEY, TOKEN), post))
			{
				await request.SendWebRequest();
				
				if (request.result != UnityWebRequest.Result.Success)
				{
					throw new TrelloException($"Unable to create new Trello List : {request.error}");
				}
				
			}
		}

		private string GetBoardId(string boardName)
		{
			if (string.IsNullOrEmpty(boardName))
			{
				return null;
			}
			
			for (int i = 0, count = _boards.Count; i < count; i++)
			{
				var board = (Dictionary<string, object>)_boards[i];
				if ((string)board["name"] == boardName)
				{
					return (string)board["id"];
				}
			}

			return null;
		}

		private string GetListId(string listName)
		{
			if (string.IsNullOrEmpty(listName))
			{
				return null;
			}
			
			for (int i = 0, count = _lists.Count; i < count; i++)
			{
				var list = (Dictionary<string, object>)_lists[i];
				
				if ((string)list["name"] == listName)
				{
					return (string)list["id"];
				}
			}

			return null;
		}
	}
}
public static class AsyncOperationExtensionMethods
{
	public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
	{
		var tcs = new TaskCompletionSource<object>();
		asyncOp.completed += obj => { tcs.SetResult(null); };
		return ((Task)tcs.Task).GetAwaiter();
	}
}