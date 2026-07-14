using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node
{
    private enum _sceneState
    {
        idle,
        loadScene,
        getScenes
    }

    private _sceneState _currentSceneState = _sceneState.idle;

    private Player _player;

    public Node currentScene;
    public Node oldScene = null;
    private List<ZoneLoadArea> _loadZones = new List<ZoneLoadArea>();
    private List<ZoneLoadArea> _oldLoadZones = new List<ZoneLoadArea>();
    private string sceneToLoad = null;
    
    

    public override void _Ready()
    {
        GetLoadZones();
        PrepNextScene();
        GD.Print("We should not be here twice");
        _player = currentScene.GetNode<CharacterBody2D>("Player") as Player;

        PrintSceneInfo();
        
    }

    public override void _Process(double delta)
    {
        switch (_currentSceneState)
        {
            case _sceneState.loadScene:
                // Loadscene
                GoToScene();
                _currentSceneState = _sceneState.getScenes;
                break;
            case _sceneState.getScenes:
                // GetScenes
                GetLoadZones();
                PrepNextScene();
                PrintSceneInfo();
                _currentSceneState = _sceneState.idle;
                break;
            default:
                break;
        }
    }


    private void GetLoadZones()
    {
        Viewport root = GetTree().Root;
        currentScene = root.GetChild(-1);
        var loadZonesNode = currentScene.GetNode<Node2D>("LoadZones");
        bool loadZonesIsEmpty = _loadZones.Count == 0;
        List<ZoneLoadArea> tempList = new List<ZoneLoadArea>();

        foreach (Node child in loadZonesNode.GetChildren())
        {
            if (child is ZoneLoadArea area)
            {
                ZoneLoadArea loadArea = child as ZoneLoadArea;
                area.BodyEntered += (body) => OnBodyEnteredZoneLoadArea(body, area);
                if (loadZonesIsEmpty)
                {
                    _loadZones.Add(area);
                    continue;
                }
                else
                {
                    tempList.Add(area);
                    continue;
                }
            }
        }
        if (!loadZonesIsEmpty)
        {
            // _oldLoadZones = _loadZones;
            // _loadZones = tempList;
            (_oldLoadZones, _loadZones) = (_loadZones, tempList);
            return;
        }
        return;
    }

    // Not working as intended TODO
    private void PrepNextScene()
    {
        foreach (ZoneLoadArea zone in _loadZones)
        {
            if (!_oldLoadZones.Contains(zone))
            {
                ResourceLoader.LoadThreadedRequest(zone.SceneToLoad);
            }
        }

        foreach (ZoneLoadArea zone in _oldLoadZones)
        {
            if (!_loadZones.Contains(zone))
            {
                _ = ResourceLoader.LoadThreadedGet(zone.SceneToLoad);
            }
        }
    }

    private void GoToScene()
    {
        oldScene = currentScene;
        var sceneToGo = (PackedScene)ResourceLoader.LoadThreadedGet(sceneToLoad);
        GetTree().ChangeSceneToPacked(sceneToGo);
    }

    public void OnBodyEnteredZoneLoadArea(Node2D body, ZoneLoadArea area)
    {
        if (!body.IsInGroup("player"))
        {
            return;
        }
        if (area.SceneToLoad == null) return;

        // set SceneToLoad to new scene
        // Switch state to loadScene
        sceneToLoad = area.SceneToLoad;
        _currentSceneState = _sceneState.loadScene;
        return;

    }

    private void PrintSceneInfo()
    {
        GD.Print("========================");
        GD.Print("Scene Info:");
        GD.Print("========================");
        GD.Print("");
        GD.Print("Current Scene");
        GD.Print(currentScene.Name);
        GD.Print("Old Scene");
        // GD.Print(oldScene);
        GD.Print("Load Zones:");
        foreach (ZoneLoadArea scene in _loadZones)
        {
            GD.Print($"LoadZone: {scene.Name}");
        }
        GD.Print("Scenes to Load:");
        foreach (ZoneLoadArea scene in _loadZones)
        {
            string path = scene.SceneToLoad;
            if (path.StartsWith("uid://"))
            {
                long id = ResourceUid.TextToId(path);
                if (ResourceUid.HasId(id))
                    path = ResourceUid.GetIdPath(id);
            }
            GD.Print($"Scene to load: {path}");
        }
        
        GD.Print("========================");
        GD.Print("Player Info:");
        // GD.Print($"Player Name: {_player.Name}");
        // GD.Print($"Player Speed: {_player.BaseSpeed}");
        GD.Print("========================");
        
    }

}
