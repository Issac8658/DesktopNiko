using System;
using System.Collections.Generic;
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

	public Achievement GetAchievementById(string AchievementId)
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
	public int GetAchievementIndexById(string AchievementId)
	{
		for (int i = 0; i < AchievementsList.Count; i++)
		{
			Achievement achievement = AchievementsList[i];
			if (achievement.Id == AchievementId)
			{
				return i;
			}
		}
		return -1;
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

	public void TakeAchievement(string AchievementId, bool notification = true)
	{
		for (int i = 0; i < AchievementsList.Count; i++)
		{
			if (AchievementsList[i].Id == AchievementId)
			{
				if (!AchievementTakedList[i])
				{
					AchievementTakedList[i] = true;
					if (notification)
						EmitSignal("AchievementTaked", AchievementId);
				}
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

	public Achievement[] GetAchievementsList()
	{
		return [.. AchievementsList];
	}
}
