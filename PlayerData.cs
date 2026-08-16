using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerData : Node
{
    private Player _player;

    public int playerHealth;
    public HashSet<int> knownCreatures;
    public List<int> knownCreatureList;
    private Node _currentScene;

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        _currentScene = root.GetChild(-1);
        _player = _currentScene.GetNode<CharacterBody2D>("Player") as Player;
    }

    public void GetPlayerData()
    {
        knownCreatures = _player.KnownCreatures;
        knownCreatureList = _player.KnownCreatureList;
    }


    public Player SavePlayerData()
    {
        Player newPlayer = _player;
        // newPlayer.KnownCreatures = _player.KnownCreatures;
        knownCreatureList = _player.KnownCreatureList;
        return null;
    }


}
