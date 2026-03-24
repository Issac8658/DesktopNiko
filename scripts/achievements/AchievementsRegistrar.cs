using Godot;
using System;
using System.Collections.Generic;

public partial class AchievementsRegistrar : Node
{
	private AchievementsController _achCon;
	private readonly Dictionary<string, Variant[]> Achievements = new(){ // Name, description, icon, is description hidden before unlock
		{"ten_clicks", 					["achievements.ten_clicks",						"res://sprites/achievements/ten_clicks.png",					false]},
		{"one_hundred_clicks", 			["achievements.one_hundred_clicks",				"res://sprites/achievements/one_hundred_clicks.png",			false]},
		{"one_thousand_clicks",			["achievements.one_thousand_clicks",			"res://sprites/achievements/one_thousand_clicks.png",			false]},
		{"one_hundred_thousand_clicks", ["achievements.one_hundred_thousand_clicks",	"res://sprites/achievements/one_hundred_thousand_clicks.png",	false]},
		{"one_million_clicks", 			["achievements.one_million_clicks",				"res://sprites/achievements/one_million_clicks.png",			false]},
		{"one_billion_clicks", 			["achievements.one_billion_clicks",				"res://sprites/achievements/glitched2.png",						false]},
		{"im_scared", 					["achievements.im_scared",						"res://sprites/achievements/im_scared.png",						false]},
		{"what_you_do", 				["achievements.what_you_do",					"res://sprites/achievements/what_you_doing.png",				false]},
		{"sweet_dreams", 				["achievements.sweet_dreams",					"res://sprites/achievements/sweet_dreams.png",					false]},
		{"little_win", 					["achievements.little_win",						"res://sprites/achievements/little_win.png",					false]},
		{"master_of_tennis", 			["achievements.master_of_tennis",				"res://sprites/achievements/master_of_tennis.png",				false]},
		{"hacked_a_machine", 			["achievements.hacked_a_machine",				"res://sprites/achievements/hacked_a_machine.png",				false]},
		{"time_to_dinner", 				["achievements.time_to_dinner",					"res://sprites/achievements/time_to_dinner.png",				false]},
		{"few_squares", 				["achievements.few_squares",					"res://sprites/achievements/glitched2.png",				false]},
		{"worst_friend", 				["achievements.worst_friend",					"res://sprites/achievements/glitched.png",						true]},
		{"hello_secret", 				["achievements.hello_secret",					"res://sprites/achievements/unknown.png",						true]}
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_achCon = GetNode<AchievementsController>("/root/AchievementsController");

		foreach (KeyValuePair<string, Variant[]> AchievementData in Achievements)
		{
			_achCon.RegisterAchievement(new()
			{
				Id = AchievementData.Key,
				Title = (string)AchievementData.Value[0] + ".title",
				Description = (string)AchievementData.Value[0] + ".description",
				Icon = GD.Load<Texture2D>((string)AchievementData.Value[1]),
				Hidden = (bool)AchievementData.Value[2]
			});
		}
	}
}
