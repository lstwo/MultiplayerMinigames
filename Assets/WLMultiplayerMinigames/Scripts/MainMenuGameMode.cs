using ModWobblyLife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGameMode : ModdedGameMode
{
    [Header("Main Menu Game Mode")]
    public ModPlayerCharacterSpawnPoint hostSpawnPoint;

    public override ModPlayerCharacterSpawnPoint GetPlayerSpawnPoint(ModPlayerController playerController)
    {
        if (playerController.modNetworkObject.IsServer())
        {
            return hostSpawnPoint;
        }
        else
        {
            return defaultSpawnPoint;
        }
    }
}
