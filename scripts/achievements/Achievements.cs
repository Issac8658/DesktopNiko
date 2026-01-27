using System;
using System.Collections.Generic;
using Godot;

public partial class AchievementsController : Node
{

    private List<Achievements.Achievement> AchievementsList = [];

	public bool IsAchievementExist(string AchievementId)
    {
        bool achievementExist(Achievements.Achievement achievement) { return achievement.Id == AchievementId; }
        return AchievementsList.Exists(achievementExist);
    }

    public Achievements.Achievement? GetAchievementById(string AchievementId)
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

    public bool IsAchievementTaked(string AchievementId)
    {
        var achievement = GetAchievementById(AchievementId);
        if (achievement is not null)
        {
            return achievement.Value.Taked;
        }
        return false;
    }

    public void TakeAchievement(string AchievementId)
    {
        for (int i = 0; i < AchievementsList.Count; i++)
        {
            if (AchievementsList[i].Id == AchievementId)
            {
                var achievement = AchievementsList[i];
                achievement.Taked = true;
                AchievementsList[i] = achievement;
                break;
            }
        }
    }
}

namespace Achievements
{
    public struct Achievement(string AchievementId, Texture2D AchievementIcon, string AchievementTitle = "NO_TITLE", string AchievementDesc = "")
    {
		public bool Taked = false;

        public string Id = AchievementId;
		public Texture2D Icon = AchievementIcon;
		public string Title = AchievementTitle;
		public string Description = AchievementDesc;
    }
}