using System;
using System.Collections.Generic;
using Godot;

public partial class AchievementsController : Node
{
	[Signal] public delegate void AchievementTakedEventHandler(string AchievementId);

	private List<Achievement> AchievementsList = [];
	private List<bool> AchievementTakedList = [];

	private static AchievementsController StaticNode = null;

    public override void _Ready()
    {
		StaticNode = this;
    }

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
			return AchievementTakedList[achievementId];
		else
			GD.PushWarning($"Unknown achievement \"{AchievementId}\"!");
		return false;
	}

	public void TakeAchievement(string AchievementId, bool Notification = true)
	{
		for (int i = 0; i < AchievementsList.Count; i++)
		{
			if (AchievementsList[i].Id == AchievementId)
			{
				if (!AchievementTakedList[i])
				{
					AchievementTakedList[i] = true;
					if (Notification)
						EmitSignal("AchievementTaked", AchievementId);
				}
				break;
			}
		}
	}

	public void RegisterAchievement(Achievement Achievement)
	{
		if (!IsAchievementExist(Achievement.Id))
		{
			AchievementsList.Add(Achievement);
			AchievementTakedList.Add(false);
		}
		else
			GD.PushWarning($"Achievement with id {Achievement.Id} already registered!");
	}

	public Achievement[] GetAchievementsList()
	{
		return [.. AchievementsList];
	}

	public static bool IsAchievementExistStatic(string AchievementId) => StaticNode.IsAchievementExist(AchievementId);
	public static Achievement GetAchievementByIdStatic(string AchievementId) => StaticNode.GetAchievementById(AchievementId);
	public static int GetAchievementIndexByIdStatic(string AchievementId) => StaticNode.GetAchievementIndexById(AchievementId);
	public static bool IsAchievementTakedStatic(string AchievementId) => StaticNode.IsAchievementTaked(AchievementId);
	public static void TakeAchievementStatic(string AchievementId, bool Notification = true) => StaticNode.TakeAchievement(AchievementId, Notification);
	public static void RegisterAchievementStatic(Achievement Achievement) => StaticNode.RegisterAchievement(Achievement);
	public static Achievement[] GetAchievementsListStatic() => StaticNode.GetAchievementsList();
}