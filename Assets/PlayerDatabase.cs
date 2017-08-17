﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class PlayerDatabase
{
	public enum Position
	{
		QB,
		RB,
		WR,
		TE,
		K,
		DEF
	}

	public enum NFLTeam
	{
		// NFC
		MIN,
		GB,
		DET,
		CHI,

		SF,
		SEA,
		ARI,
		LAR, // Rams

		DAL,
		PHI,
		WAS,
		NYG,

		NO,
		ATL,
		CAR,
		TB,

		// AFC
		DEN,
		OAK,
		KC,
		LAC, // Chargers

		CLE,
		CIN,
		PIT,
		BAL,

		IND,
		JAC,
		TEN,
		HOU,

		NE,
		BUF,
		MIA,
		NYJ,

		FA, // Free agent

		TotalNFLTeams
	}
	
	public class PlayerData
	{
		public string playerName;
		public uint overallRank;
		public NFLTeam nflTeam;
		public Position position;
		public uint byeWeek;
	}

	// Database
	List<PlayerData> qbList;
	List<PlayerData> rbList;
	List<PlayerData> wrList;
	List<PlayerData> teList;
	List<PlayerData> kList;
	List<PlayerData> defList;

	List<PlayerData> searchResults;

	public PlayerDatabase()
	{
		qbList = new List<PlayerData>();
		rbList = new List<PlayerData>();
		wrList = new List<PlayerData>();
		teList = new List<PlayerData>();
		kList = new List<PlayerData>();
		defList = new List<PlayerData>();

		searchResults = new List<PlayerData>();
	}

	public void ImportPlayerData()
	{
		// Grab the file with the player data
		Debug.Log("Importing Contract Players...");
		string filename = "/" + (DateTime.Now.Year) + "DraftOverallRankings.csv";
		string filepath = Application.persistentDataPath + filename;

		// No Importing of contracts if the file does not exist
		if (!File.Exists(filepath))
		{
			Debug.Log(filepath + " does not exist!");
			return;
		}

		StreamReader reader = new StreamReader(filepath);
		while (!reader.EndOfStream)
		{
			// Rank : Player Name : Team : Position : Bye Week
			string playerLine = reader.ReadLine();
			string[] playerFields = playerLine.Split(',');
			playerFields[3] = new String(playerFields[3].Where(c => (c < '0' || c > '9')).ToArray());
			Debug.Log(playerFields[0] + " : " + playerFields[3]);

			PlayerData newData = new PlayerData();
			newData.overallRank = Convert.ToUInt32(playerFields[0]);
			newData.playerName = playerFields[1];
			newData.nflTeam = GetNFLTeam(playerFields[2]);
			newData.byeWeek = Convert.ToUInt32(playerFields[4]);

			switch (playerFields[3])
			{
				case "QB":
					newData.position = Position.QB;
					qbList.Add(newData);
					break;
				case "RB":
					newData.position = Position.RB;
					rbList.Add(newData);
					break;
				case "WR":
					newData.position = Position.WR;
					wrList.Add(newData);
					break;
				case "TE":
					newData.position = Position.TE;
					teList.Add(newData);
					break;
				case "K":
					newData.position = Position.K;
					kList.Add(newData);
					break;
				case "DST":
					newData.position = Position.DEF;
					defList.Add(newData);
					break;
			}
		}

		Debug.Log(wrList.Count);
	}

	public PlayerData PickPlayerFromDatabase(string playerName)
	{
		// Get the number of matches
		int numMatches = NumMatchingPlayers(playerName);

		// Single match found, remove from database and return the player
		if (numMatches == 1)
		{
			PlayerData foundPlayer = searchResults.First();

			// Remove the player from the database
			switch (foundPlayer.position)
			{
				case Position.QB:
					qbList.Remove(foundPlayer);
					break;
				case Position.RB:
					rbList.Remove(foundPlayer);
					break;
				case Position.WR:
					wrList.Remove(foundPlayer);
					break;
				case Position.TE:
					teList.Remove(foundPlayer);
					break;
				case Position.K:
					kList.Remove(foundPlayer);
					break;
				case Position.DEF:
					defList.Remove(foundPlayer);
					break;
			}

			// Return the player
			return foundPlayer;
		}
		else
		{
			PlayerData newPlayer = new PlayerData();
			newPlayer.playerName = playerName;
			newPlayer.overallRank = 500;
			newPlayer.nflTeam = NFLTeam.FA;
			newPlayer.position = Position.DEF;
			newPlayer.byeWeek = 0;
			return newPlayer;
		}
	}

	// Gets the nfl team enum from the team string
	public NFLTeam GetNFLTeam(string teamString)
	{
		foreach(NFLTeam team in Enum.GetValues(typeof(NFLTeam)))
		{
			if(team.ToString() == teamString)
			{
				return team;
			}
		}

		return NFLTeam.TotalNFLTeams;
	}

	public int NumMatchingPlayers(string playerToFind)
	{
		searchResults.Clear();

		// Player is an empty string
		if(playerToFind.Length == 0)
		{
			return 0;
		}

		// Look through the QBs
		foreach (PlayerData player in qbList)
		{
			if(player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the RBs
		foreach (PlayerData player in rbList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the WRs
		foreach (PlayerData player in wrList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the TEs
		foreach (PlayerData player in teList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the Ks
		foreach (PlayerData player in kList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the DEFs
		foreach (PlayerData player in defList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		return searchResults.Count;
	}

	public string GetSearchResult()
	{
		if(searchResults.Count > 1 || searchResults.Count == 0)
		{
			return string.Empty;
		}
		else
		{
			return searchResults.First().playerName;
		}
	}
}