using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class NikoSkinManager : Node
{
    private const string SKINS_FOLDER_PATH = "res://niko/";
    public static readonly List<string> RequiaredSkinStates = [
        "default",
        "normal",
        "speak",
        "shock",
        "surprised",
        "cry",
        "cool",
        "sad",
        "cry",
        "supercry",
        "eat1",
        "eat2",
        "look_left",
        "look_right",
        "huh",
        "pancakes"
    ];
    private List<Skin> SkinsList = [];
    //private Dictionary<string, Texture2D> PreloadedSkin = [];
    private Dictionary<string, Texture2D> CurrentSkin = [];

    public override void _Ready()
    {
        LoadSkins(SKINS_FOLDER_PATH);
        DirAccess UserFolder = DirAccess.Open("user://");
        if (!UserFolder.DirExists("skins"))
            UserFolder.MakeDir("skins");
        else
            LoadSkins("user://skins/");
    }

    public void LoadSkin(string SkinId)
    {
        Skin? MaybeSkinToLoad = GetSkinFromId(SkinId);
        if (MaybeSkinToLoad is Skin SkinToLoad)
        {
            foreach (string State in SkinToLoad.States)
            {
                CurrentSkin[State] = GD.Load(SkinToLoad.Dir + State + ".png") as Texture2D;
            }
        }
    }
    public Skin? GetSkinFromId(string SkinId)
    {
        foreach (Skin skin in SkinsList)
            if (skin.Id == SkinId)
                if (skin.IsValid())
                    return skin;
        
        GD.PushWarning($"Skin \"{SkinId}\" is invalid or does not exist");
        return null;
    }

    private void LoadSkins(string SkinsFolderPaths)
    {
        GD.Print($"Loading skins from {SkinsFolderPaths}");
        DirAccess SkinsFolder = DirAccess.Open(SkinsFolderPaths);
        foreach (string SkinId in SkinsFolder.GetDirectories())
        {
            string SkinPath = SkinsFolderPaths + SkinId;
                    
            GD.Print($"Finded \"{SkinId}\" skin, validation...");

            bool CanUse = true;
            List<string> States = [];
            DirAccess SkinDir = DirAccess.Open(SkinPath);

            foreach (string FileName in SkinDir.GetFiles())
            {
                if (SkinDir.FileExists(FileName))
                {
                    string[] StateFile = FileName.Split(".");
                    string StateName = StateFile[0];
                    string StateFormat = StateFile[1];
                    if (StateFormat == "png")
                        States.Add(StateName);
                }
            }

            if (!States.Contains("default"))
            {
                CanUse = false;
                GD.Print($"     Skin \"{SkinId}\" is INVALID");
            }

            if (CanUse)
            {
                GD.Print("     Loaded skin from " + SkinPath);
            }

            SkinsList.Add(new(SkinId, "Skin Name", "Skin Desc", SkinPath, States.ToArray()));
        }
    }


    public struct Skin(string SkinId, string SkinName, string SkinDesc, string SkinPath, string[] SkinStates)
    {
        public string Id = SkinId;
        public string Name = SkinName;
        public string Description = SkinDesc;
        public string Dir = SkinPath;
        public string[] States = SkinStates;

        public readonly bool IsValid()
        {
            foreach (string RequiaredState in RequiaredSkinStates)
                if (!States.Contains(RequiaredState))
                    return false;
            return true;
        }

    }
}