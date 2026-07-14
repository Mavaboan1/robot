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

    public Node currentScene;
    private List<Node> _loadZones = new List<Node>();
        
    private string sceneToLoad = null;

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        currentScene = root.GetChild(-1);
        var loadZones = currentScene.GetNode<Node2D>("LoadZones");

        foreach (Node child in loadZones.GetChildren())
        {
            if (child is ZoneLoadArea area)
            {
                area.BodyEntered += (body) => OnBodyEnteredZoneLoadArea(body, area);
                _loadZones.Add(area);
                GD.Print("Area assigned");
                GD.Print(area.Name);
            }
        }
        
    }

    public override void _Process(double delta)
    {
        switch (_currentSceneState)
        {
            case _sceneState.loadScene:
                // Loadscene
                break;
            case _sceneState.getScenes:
                // GetScenes
                GetLoadZones(currentScene);
                break;
            default:
                break;
        }
    }


    private void GetLoadZones(Node currentScene)
    {
        return;
    }

    public void OnBodyEnteredZoneLoadArea(Node2D body, ZoneLoadArea area)
    {
        if (body.IsInGroup("player"))
        {
            // set SceneToLoad to new scene
            // Switch state to loadScene
            sceneToLoad = area.SceneToLoad;
            _currentSceneState = _sceneState.loadScene;
            return;
        }
        ;
        if (area.SceneToLoad == null) return;
    }

    private void PrintSceneInfo()
    {
        
    }

}
