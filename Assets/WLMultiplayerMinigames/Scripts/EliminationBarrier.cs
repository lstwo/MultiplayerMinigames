using ModWobblyLife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<ModPlayerCharacter>())
        {
            var character = other.GetComponentInParent<ModPlayerCharacter>();
            ModdedGameMode.Instance.EliminatePlayer(character.GetPlayerController());
            character.Kill();
        }
    }
}
