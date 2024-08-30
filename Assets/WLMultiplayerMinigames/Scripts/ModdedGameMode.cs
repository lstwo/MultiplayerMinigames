using ModWobblyLife;
using System.Collections.Generic;
using UnityEngine;

public class ModdedGameMode : ModFreemodeGamemode
{
    public static ModdedGameMode Instance;

    [Header("Modded Game Mode")]
    public bool isMinigame = true;

    [Space(5)]
    public ModPlayerCharacterSpawnPoint eliminatedSpawnPoint;
    public ModPlayerCharacterSpawnPoint defaultSpawnPoint;
    public NetworkingManager networkingManager;

    [Space(5)]
    public bool allowNormalRespawning = false;
    public bool allowEliminatedRespawning = false;

    public void EliminatePlayer(ModPlayerController playerController)
    {
        MinigameManager.SetPlayerState(playerController, PlayerState.Eliminated);
        UpdatePlayerState(playerController);
        MinigameManager.CheckPlayerCount();
    }

    public override ModPlayerCharacterSpawnPoint GetPlayerSpawnPoint(ModPlayerController playerController)
    {
        if (MinigameManager.GetPlayerState(playerController) == PlayerState.Eliminated)
        {
            return eliminatedSpawnPoint;
        } 
        else
        {
            defaultSpawnPoint.transform.position += Vector3.up * 1.5f;
            return defaultSpawnPoint;
        }
    }

    protected override void OnSpawnedPlayerController(ModPlayerController playerController)
    {
        base.OnSpawnedPlayerController(playerController);
        UpdatePlayerState(playerController);
        playerController.ServerSetAllowedCustomClothingAbilities(false);
    }

    protected override void ModAwake()
    {
        base.ModAwake();

        Instance = this;
    }

    private void UpdatePlayerState(ModPlayerController controller)
    {
        var state = MinigameManager.GetPlayerState(controller);

        if (state == PlayerState.NotFound)
        {
            MinigameManager.SetPlayerState(controller, PlayerState.Normal);
            Debug.Log("asdffag");
        }
        else
        {
            if (state == PlayerState.Normal)
            {
                controller.SetAllowedToRespawn(allowNormalRespawning);
            }
            else if (state == PlayerState.Eliminated)
            {
                controller.SetAllowedToRespawn(allowEliminatedRespawning);
            }
        }
    }
}