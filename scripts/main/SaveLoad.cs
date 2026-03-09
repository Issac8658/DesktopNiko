using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SaveLoad : Node
{
    const string SAVE_FILE_PATH = "user://NikoMemories.cfg";

    public readonly string[] legacy_verisons = ["1.1.4"];

    private readonly Dictionary<string, Dictionary<string, string>> VARS_TO_SAVE = new() { // var from ValuesContainer.cs, name in save file
        // vars from ValuesContainer.cs to save
        {"Main", new()
            {
                {"TotalTime", "TotalTime"},
                {"Clicks", "Clicks"},
                {"Version", "SaveVersion"}
            }
        },
        {"TWM", new()
            {
                {"MouseSoundEnabled", "MouseSound"},
                {"ShowSavingIcon", "SavingIcon"},
                {"ShowAchievements", "ShowAchievements"},
                {"DoEvents", "DoEvents"},
                {"Language", "Language"},
                {"NikoAlwaysOnTop", "NikoAlwaysOnTop"},
                {"WindowsAlwaysOnTop", "WindowsAlwaysOnTop"},
                {"ShowTaskbarIcon", "TaskbarIcon"},
                {"CurrentTheme", "Theme"},
                {"DonedEvents", "DonedEvents"}
            }
        },
        {"NikoStates", new()
            {
                {"CurrentMeowSoundId", "MeowSoundId"},
                {"SnapToBottom", "SnapToBottom"},
                {"PeacfulMode", "PeacfulMode"},
                {"IdleFacepic", "IdleFacepic"},
                {"SpeakFacepic", "SpeakFacepic"},
                {"ScareFacepic", "ScareFacepic"},
                {"ScareSpeakFacepic", "ScareSpeakFacepic"},
                {"NikoScale", "NikoScale"},
                {"NikoTimeToSleep", "NikoTimeToSleep"},
                {"CurrentSkin", "Skin"}
            }
        },
    };

    private static readonly Dictionary<string, Dictionary<string, string>> LEGACY_VARS_TO_SAVE = new() { // var from ValuesContainer.cs, name in save file
        // vars from ValuesContainer.cs to save
        {"Main", new()
            {
                {"Clicks", "Clicks"},
                {"CurrentMeowSoundId", "MeowSoundId"},
                {"TotalTime", "TotalTime"},
                {"NikoScale", "NikoScale"},
                {"Version", "SaveVersion"},
                {"NikoTimeToSleep", "SleepTime"},
                {"DonedEvents", "DonedEvents"}
            }
        },
        {"Settings", new()
            {
                {"MouseSoundEnabled", "MouseSound"},
                {"ShowSavingIcon", "SavingIcon"},
                {"ShowAchievements", "ShowAchievements"},
                {"DoEvents", "DoEvents"},
                {"Language", "Language"},
                {"NikoAlwaysOnTop", "NikoAlwaysOnTop"},
                {"WindowsAlwaysOnTop", "WindowsAlwaysOnTop"},
                {"ShowTaskbarIcon", "TaskbarIcon"},
                {"CurrentTheme", "Theme"},
                {"SnapToBottom", "SnapToBottom"},
                {"PeacfulMode", "PeacfulMode"},
                {"IdleFacepic", "IdleFacepic"},
                {"SpeakFacepic", "SpeakFacepic"},
                {"ScareFacepic", "ScareFacepic"},
                {"ScareSpeakFacepic", "ScareSpeakFacepic"},
                {"CurrentSkin", "Skin"}
            }
        },
    };

    private ValuesContainer _valuesContainer;
    private AchievementsController _achievementsController;

    public override void _Ready()
    {
        _valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
        _achievementsController = GetNode<AchievementsController>("/root/AchievementsController");
    }

    // Dont call Save and Load functions from other scripts directly, use GlobalController functions pls
    public void Save()
    {
        ConfigFile saveFile = new();
        foreach (var section in VARS_TO_SAVE)
        {
            foreach (var varPair in section.Value)
            {
                var varToSave = _valuesContainer.Get(varPair.Key);
                saveFile.SetValue(section.Key, varPair.Value, varToSave);
            }
        }
        // uncommon vars saving
        saveFile.SetValue("NikoStates", "NikoPosition", GetWindow().Position);
        List<string> TakedAchievements = [];
        foreach (Achievement achievement in _achievementsController.GetAchievementsList())
            if (_achievementsController.IsAchievementTaked(achievement.Id))
                TakedAchievements.Add(achievement.Id);
        saveFile.SetValue("Main", "TakedAchievements", TakedAchievements.ToArray());
        // achievements saving here

        saveFile.Save(SAVE_FILE_PATH);
    }

    public void Load()
    {
        ConfigFile saveFile = new();
        Error err = saveFile.Load(SAVE_FILE_PATH);
        if (err != Error.Ok)
        {
            GD.PrintErr("Failed to load save file: " + err);
            return;
        }

        string version = saveFile.GetValue("Main", "SaveVersion").AsString();
        GD.Print("Save version: " + version);
        if (legacy_verisons.Contains(version))
        {
            GD.Print("Trying to load legacy save...");
            BackupSave($"user://{version}_backup.cfg");
            foreach (var section in LEGACY_VARS_TO_SAVE)
            {
                foreach (var varPair in section.Value)
                {
                    if (saveFile.HasSectionKey(section.Key, varPair.Value))
                    {
                        var value = saveFile.GetValue(section.Key, varPair.Value);
                        _valuesContainer.Set(varPair.Key, value);
                    }
                }
            }

            // uncommon vars loading
            if (saveFile.HasSectionKey("Main", "NikoPosition"))
            {
                GetWindow().Position = saveFile.GetValue("Main", "NikoPosition").AsVector2I();
            }
            if (saveFile.HasSectionKey("Main", "Achievements"))
            {
                string[] TakedAchievementIds = saveFile.GetValue("Main", "Achievements").AsStringArray();
                foreach (string achievementId in TakedAchievementIds)
                {
                    _achievementsController.TakeAchievement(achievementId, false);
                }
            }
        }
        else
        {
            GD.Print("Loading save...");
            foreach (var section in VARS_TO_SAVE)
            {
                foreach (var varPair in section.Value)
                {
                    if (saveFile.HasSectionKey(section.Key, varPair.Value))
                    {
                        var value = saveFile.GetValue(section.Key, varPair.Value);
                        _valuesContainer.Set(varPair.Key, value);
                    }
                }
            }

            // uncommon vars loading
            if (saveFile.HasSectionKey("NikoStates", "NikoPosition"))
            {
                GetWindow().Position = saveFile.GetValue("NikoStates", "NikoPosition").AsVector2I();
            }
            if (saveFile.HasSectionKey("Main", "TakedAchievements"))
            {
                string[] TakedAchievementIds = saveFile.GetValue("Main", "TakedAchievements").AsStringArray();
                foreach (string achievementId in TakedAchievementIds)
                {
                    _achievementsController.TakeAchievement(achievementId, false);
                }
            }
        }
        
    }

    private void BackupSave(string NewName)
    {
        DirAccess UserFolder = DirAccess.Open("user://");
        if (UserFolder.FileExists(SAVE_FILE_PATH))
        {
            UserFolder.Copy(SAVE_FILE_PATH, NewName);
        }
    }
}