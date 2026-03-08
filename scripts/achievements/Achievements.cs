using System;
using System.Collections.Generic;
using Achievements;
using Godot;

public partial class AchievementsController : Node
{
	[Signal] public delegate void AchievementTakedEventHandler(string AchievementId);

	private List<Achievement> AchievementsList = [];
	private List<bool> AchievementTakedList = [];

	public bool IsAchievementExist(string AchievementId)
	{
		bool achievementExist(Achievement achievement) { return achievement.Id == AchievementId; }
		return AchievementsList.Exists(achievementExist);
	}

	public Achievement? GetAchievementById(string AchievementId)
	{
		foreach (var achievement in AchievementsList)
		{
			if (achievement.Id == AchievementId)
			{
				return achievement;
			}
		}
		return null;
	}
	public int? GetAchievementIndexById(string AchievementId)
	{
		for (int i = 0; i < AchievementsList.Count; i++)
		{
			Achievement achievement = AchievementsList[i];
			if (achievement.Id == AchievementId)
			{
				return i;
			}
		}
		return null;
	}

	public bool IsAchievementTaked(string AchievementId)
	{
		int? achievement = GetAchievementIndexById(AchievementId);
		if (achievement is int achievementId)
		{
			return AchievementTakedList[achievementId];
		}
		return false;
	}

	public void TakeAchievement(string AchievementId)
	{
		for (int i = 0; i < AchievementsList.Count; i++)
		{
			if (AchievementsList[i].Id == AchievementId)
			{
				AchievementTakedList[i] = true;
				break;
			}
		}
	}

	public void RegisterAchievement(Achievement achievement)
	{
		if (!IsAchievementExist(achievement.Id))
		{
			AchievementsList.Add(achievement);
			AchievementTakedList.Add(false);
		}
		else
			GD.PushWarning($"Achievement with id {achievement.Id} already registered!");
	}
}

namespace Achievements
{
	public struct Achievement(string AchievementId, Texture2D AchievementIcon, string AchievementTitle = "NO_TITLE", string AchievementDesc = "")
	{
		public string Id = AchievementId;
		public Texture2D Icon = AchievementIcon;
		public string Title = AchievementTitle;
		public string Description = AchievementDesc;
	}
}
