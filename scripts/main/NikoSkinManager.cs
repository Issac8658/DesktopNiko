using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.NativeInterop;
using Microsoft.VisualBasic;

public partial class NikoSkinManager : Node
{
	private const string SKINS_FOLDER_PATH = "res://niko/";
	private const string DEFAULT_SKIN_ID = "niko_default";
	private const string SKIN_PATH_OVERRIDE = "skin://";
	private const string SKIN_CONF_NAME = "skin.cfg";
	private const string DEFAULT_SPRITE_ID = "default";
	public static readonly List<string> RequiaredSkinStates = [
		DEFAULT_SPRITE_ID,
		"normal",
		"speak",
		"shock",
		"surprised",
		"cry",
		"cool",
		"sad",
		"cry",
		"super_cry",
		"eat1",
		"eat2",
		"look_left",
		"look_right",
		"huh",
		"pancakes",
		"sleep",
        "sleepy"
	];
	private List<Skin> SkinsList = [];
	//private Dictionary<string, Texture2D> PreloadedSkin = [];
	private Dictionary<string, Dictionary<string, Texture2D>> _loadedSprites = [];

	private ValuesContainer _valuesContainer;


	public override void _Ready()
	{
		_valuesContainer = GetNode("/root/ValuesContainer") as ValuesContainer;

		RegisterSkins(SKINS_FOLDER_PATH);
		DirAccess UserFolder = DirAccess.Open("user://");
		if (!UserFolder.DirExists("skins"))
			UserFolder.MakeDir("skins");
		else
			RegisterSkins("user://skins/");

		SetSkin(DEFAULT_SKIN_ID);
	}

	public void SetSkin(string SkinId)
	{
		Skin? MaybeSkinToLoad = GetSkinFromId(SkinId);
		if (MaybeSkinToLoad is Skin)
		{
			_valuesContainer.CurrentSkin = SkinId;
		}
	}
	
	public Texture2D GetCurrentSkinSprite(string SpriteId)
	{
		return LoadSkinSpriteFromId(_valuesContainer.CurrentSkin, SpriteId);
	}

	public bool SkinIsExist(string SkinId)
	{
		foreach (Skin skin in SkinsList)
			if (skin.Id == SkinId)
				return true;
		return false;
	}

	public Skin[] GetSkinsList()
	{
		return [.. SkinsList];
	}

	public Texture2D LoadSkinSprite(Skin skin, string spriteId)
	{
		return LoadSkinSpriteFromId(skin.Id, spriteId);
	}

	public Texture2D LoadSkinSpriteFromId(string skinId, string spriteId)
	{
		Texture2D sprite;
		if (_loadedSprites.ContainsKey(skinId))
		{
			if (_loadedSprites[skinId].TryGetValue(spriteId, out Texture2D loadedSprite))
				return loadedSprite;
			else
			{
				Skin skin = (Skin)GetSkinFromId(skinId);

				if (skin.Sprites.TryGetValue(spriteId, out string SpritePath))
					sprite = LoadTexture(SpritePath);
				else
					sprite = LoadTexture(skin.Sprites[DEFAULT_SPRITE_ID]);
					GD.Print("Skin sprite \"" + spriteId + "\" isn't exist in skin \"" + skinId + "\"");

				_loadedSprites[skinId][spriteId] = sprite;
			}
		}
		else
		{
			Dictionary<string, Texture2D> SkinTextures = [];
			Skin skin = (Skin)GetSkinFromId(skinId);

			if (skin.Sprites.TryGetValue(spriteId, out string SpritePath))
				sprite = LoadTexture(SpritePath);
			else
				sprite = LoadTexture(skin.Sprites[DEFAULT_SPRITE_ID]);
				GD.Print("Skin sprite \"" + spriteId + "\" isn't exist in skin \"" + skinId + "\"");

			SkinTextures[spriteId] = sprite;
			_loadedSprites[skinId] = SkinTextures;
		}
		return sprite;
	}

	public bool SpriteIsLoaded(string SkinId, string SpriteId)
	{
		if (_loadedSprites.TryGetValue(SkinId, out Dictionary<string, Texture2D> sprites))
			if (sprites.ContainsKey(SpriteId))
				return true;
		return false;
	}
	public Skin? GetSkinFromId(string SkinId)
	{
		foreach (Skin skin in SkinsList)
			if (skin.Id == SkinId)
				return skin;
		
		GD.Print($"Skin \"{SkinId}\" is invalid or does not exist");
		return null;
	}

	public static bool PathIsBuiltIn(string Path)
	{
		return Path.Contains("res://");
	}

	private void RegisterSkins(string SkinsFolderPaths)
	{
		GD.Print($"Registering skins from {SkinsFolderPaths}");
		DirAccess SkinsFolder = DirAccess.Open(SkinsFolderPaths);
		foreach (string SkinId in SkinsFolder.GetDirectories())
		{
			RegisterSkin(SkinsFolder.GetCurrentDir() + "/" + SkinId);
		}
	}

	private static Texture2D LoadTexture(string Path)
	{
		Texture2D texture;
		if (PathIsBuiltIn(Path))
		{
			texture = GD.Load(Path) as Texture2D;
		}
		else
		{ 
			Image image = Image.LoadFromFile(Path);
			texture = ImageTexture.CreateFromImage(image);
		}
		return texture;
	}

	private void RegisterSkin(string SkinPath)
	{
		DirAccess SkinDir = DirAccess.Open(SkinPath);
		SkinPath = SkinDir.GetCurrentDir(); // for remove "/" at end if it exist
		string[] SkinFiles = SkinDir.GetFiles();
		if (SkinFiles.Contains(SKIN_CONF_NAME))
		{
			string SkinId = SkinDir.GetCurrentDir().Split("/").Last();
			GD.Print($"Registering skin \"{SkinId}\"...");

			ConfigFile SkinConfig = new();
			SkinConfig.Load(SkinPath + "/" + SKIN_CONF_NAME);

			if (SkinConfig.HasSection("Info") && SkinConfig.HasSection("Data") && SkinConfig.HasSection("Sprites"))
			{
				Dictionary<string, string> SkinSprites = [];
				foreach (string SpriteId in SkinConfig.GetSectionKeys("Sprites"))
				{
					SkinSprites.Add(SpriteId, ((string)SkinConfig.GetValue("Sprites", SpriteId)).Replace(SKIN_PATH_OVERRIDE, SkinPath + "/"));
				}
				if (SkinSprites.ContainsKey("default"))
				{
					string SkinName = (string)SkinConfig.GetValue("Info", "Name", "NO_NAME");
					string SkinDesc = (string)SkinConfig.GetValue("Info", "Description", "");
					string SkinAuthor = (string)SkinConfig.GetValue("Info", "Author", "Unknown");
					string SkinVersion = (string)SkinConfig.GetValue("Info", "Version", "Unknown");
					string SkinForVersion = (string)SkinConfig.GetValue("Info", "ForVersion", "Unknown");
					string SkinComment = (string)SkinConfig.GetValue("Info", "Comment", "");
					string[] SkinTags = (string[])SkinConfig.GetValue("Info", "Tags", "[]");

					float SkinScale = (float)SkinConfig.GetValue("Data", "Scale", 1.0);
					Vector2[] SkinCollisionPolygon = (Vector2[])SkinConfig.GetValue("Data", "CollisionPolygon", "[0,0, 100,0 ,100,100, 0,100]");
					if (!SkinConfig.HasSectionKey("Data", "CollisionPolygon"))
						GD.Print($"Collision polygon in \"{SkinId}\" doesn't exist! Using skin is not recommended");

					Dictionary<string, string> SkinExtraSprites = [];
					if (SkinConfig.HasSection("ExtraSprites"))
					{
						foreach (string SpriteId in SkinConfig.GetSectionKeys("ExtraSprites"))
						{
							SkinExtraSprites.Add(SpriteId, ((string)SkinConfig.GetValue("ExtraSprites", SpriteId)).Replace(SKIN_PATH_OVERRIDE, SkinPath + "/"));
						}
					}

					Skin RegisteredSkin = new(SkinId, SkinPath, SkinSprites, SkinExtraSprites, SkinScale, SkinCollisionPolygon, SkinName, SkinDesc, SkinAuthor, SkinVersion, SkinForVersion, SkinComment, SkinTags);
					SkinsList.Add(RegisteredSkin);
					GD.Print($"Skin \"{SkinId}\" registered");
				}
				else
					GD.Print($"Skin \"{SkinId}\" doesn't have \"default\" sprite!(in config)");
			}
			else
				GD.Print($"Skin \"{SkinId}\" config file is invalid!");
		}
	}


	public struct Skin(string SkinId, string SkinPath, Dictionary<string, string> SkinSprites, Dictionary<string, string> SkinExtraSprites, float SkinScale, Vector2[] SkinCollisionPolygon, string SkinName, string SkinDesc, string SkinAuthor, string SkinVersion, string SkinForVersion, string SkinComment, string[] SkinTags)
	{
		public string Id = SkinId;
		public string Dir = SkinPath;
		public Dictionary<string, string> Sprites = SkinSprites;
		public Dictionary<string, string> ExtraSprites = SkinExtraSprites;
		public float Scale = SkinScale;
		public Vector2[] CollisionPolygon = SkinCollisionPolygon;
		public string Name = SkinName;
		public string Description = SkinDesc;
		public string Author = SkinAuthor;
		public string Version = SkinVersion;
		public string ForVersion = SkinForVersion;
		public string Comment = SkinComment;
		public string[] Tags = SkinTags;

		public readonly bool IsValid()
		{
			foreach (string RequiaredState in RequiaredSkinStates)
				if (!Sprites.ContainsKey(RequiaredState))
					return false;
			return true;
		}

	}
}
