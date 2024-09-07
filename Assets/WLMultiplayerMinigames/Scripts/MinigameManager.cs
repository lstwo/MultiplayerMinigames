using ModWobblyLife;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Tymski;
using UMod;
using UnityEngine;

public class MinigameManager : ModScriptBehaviour
{
    public static bool isPaused = false;

    private static MinigameManager _instance;
    public static MinigameManager Instance
    {
        get { return _instance; }
    }

    public SceneReference[] minigameScenes;
    public SceneReference lobbyScene;

    public TextMeshProUGUI winText;

    [HideInInspector]
    public List<ModPlayerController> playerStates_Keys = new List<ModPlayerController>();

    [HideInInspector]
    public List<PlayerState> playerStates_Values = new List<PlayerState>();

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(_instance.gameObject);
        DontDestroyOnLoad(gameObject);
        _instance = this;

    }
    public static void SetPlayerState(ModPlayerController controller, PlayerState state)
    {
        if(_instance.playerStates_Keys.Contains(controller))
            _instance.playerStates_Values[_instance.playerStates_Keys.IndexOf(controller)] = state;
        else
        {
            _instance.playerStates_Keys.Add(controller);
            _instance.playerStates_Values.Add(state);
        }
    }

    public static PlayerState GetPlayerState(ModPlayerController controller)
    {
        if (_instance.playerStates_Keys.Contains(controller))
            return _instance.playerStates_Values[_instance.playerStates_Keys.IndexOf(controller)];
        else
            return PlayerState.NotFound;
    }

    public static void ResetManager()
    {
        _instance.playerStates_Keys.Clear();
        _instance.playerStates_Values.Clear();
    }

    public static void LoadMinigame(int index)
    {
        var scene = _instance.minigameScenes[index];
        ModdedGameMode.Instance.networkingManager.ServerLoadScene(scene);
    }

    public static void CheckPlayerCount()
    {
        if(isPaused) return;

        ModPlayerController winner = null;

        if(ModdedGameMode.Instance.minigameType == ModdedGameMode.MinigameType.Elimination)
        {
            winner = CheckPlayerCount_Elimination();
        }
        else if(ModdedGameMode.Instance.minigameType == ModdedGameMode.MinigameType.Qualification)
        {
            winner = CheckPlayerCount_Qualification();
        }
    }

    public static ModPlayerController CheckPlayerCount_Elimination()
    {
        var normalCount = 0;
        ModPlayerController normalPlayer = null;

        foreach (var player in ModInstance.Instance.GetModPlayerControllers())
        {
            var state = GetPlayerState(player);

            if (state == PlayerState.Normal)
            {
                normalCount++;

                if (normalCount > 1)
                {
                    return null;
                }

                normalPlayer = player;
            }
        }

        if(normalCount == 0)
        {
            ResetManager();
            ModdedGameMode.Instance.networkingManager.ServerLoadScene("MainScene");
        }

        if (normalPlayer == null)
        {
            return null;
        }

        ModdedGameMode.Instance.networkingManager.ServerPlayerWon(normalPlayer);

        return normalPlayer;
    }

    public static ModPlayerController CheckPlayerCount_Qualification()
    {
        var normalCount = 0;
        ModPlayerController normalPlayer = null;

        foreach (var player in ModInstance.Instance.GetModPlayerControllers())
        {
            var state = GetPlayerState(player);

            if (state == PlayerState.Normal)
            {
                normalCount++;

                if (normalCount > 1)
                {
                    return null;
                }

                normalPlayer = player;
            }
        }

        return normalPlayer;
    }

    public override void OnModUnload()
    {
        Destroy(gameObject);
    }
}

public enum PlayerState
{
    NotFound,
    Normal,
    Eliminated,
    Qualified
}
