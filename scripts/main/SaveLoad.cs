using Godot;
using System;
using System.Collections.Generic;

public partial class SaveLoad : Node
{
    const string SAVE_FILE_PATH = "user://NikoMemories.cfg";

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

    public override void _Ready()
    {
        _valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
    }

    // Dont call Save and Load functions from other scripts directly, use GlobalController functions pls
    public void Save()
    {
        EmitSignal("SaveStarted");
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
        saveFile.SetValue("Main", "SaveVersion", ProjectSettings.GetSetting("application/config/save_version"));
        // achievements saving here

        saveFile.Save(SAVE_FILE_PATH);
        EmitSignal("SaveEnded");
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


        GD.Print("Save version: " + saveFile.GetValue("Main", "SaveVersion").AsString());
        if (saveFile.GetValue("Main", "SaveVersion").AsString().Contains('.'))
        {
            GD.Print("Trying to load legacy save...");
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
            // achievements loading here
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
            // achievements loading here
        }
        
    }

    
    // Save-load script here
}