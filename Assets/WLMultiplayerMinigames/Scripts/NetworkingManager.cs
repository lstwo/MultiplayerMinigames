using ModWobblyLife;
using ModWobblyLife.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetworkingManager : ModNetworkBehaviour
{
    private byte RPC_LOAD_SCENE;
    private byte RPC_PLAYER_WIN;

    protected override void ModRegisterRPCs(ModNetworkObject modNetworkObject)
    {
        base.ModRegisterRPCs(modNetworkObject);

        RPC_LOAD_SCENE = modNetworkObject.RegisterRPC(ClientLoadScene);
        RPC_PLAYER_WIN = modNetworkObject.RegisterRPC(ClientPlayerWon);
    }

    public void ServerLoadScene(string name)
    {
        if (modNetworkObject == null || !modNetworkObject.IsServer()) return;

        modNetworkObject.SendRPC(RPC_LOAD_SCENE, ModRPCRecievers.Others, name);

        ModScenes.Load(name);
    }

    private void ClientLoadScene(ModNetworkReader reader, ModRPCInfo info)
    {
        ModScenes.Load(reader.ReadString());
    }

    public void ServerPlayerWon(ModPlayerController player)
    {
        if (modNetworkObject == null || !modNetworkObject.IsServer()) return;

        modNetworkObject.SendRPC(RPC_PLAYER_WIN, ModRPCRecievers.All, player.modNetworkObject.GetNetworkID());
    }

    private void ClientPlayerWon(ModNetworkReader reader, ModRPCInfo info)
    {
        var player = ModInstance.Instance.GetModPlayerControllerByNetworkid(reader.ReadUInt32());
        StartCoroutine(PlayerWinCoroutine(player));
    }

    public IEnumerator PlayerWinCoroutine(ModPlayerController player)
    {
        MinigameManager.isPaused = true;
        MinigameManager.Instance.winText.text = $"Player {RemoveHtmlTags(player.GetPlayerName())} won";
        MinigameManager.Instance.winText.gameObject.SetActive(true);

        var wait = new WaitForSeconds(10);
        yield return wait;

        MinigameManager.Instance.winText.gameObject.SetActive(false);
        MinigameManager.isPaused = false;

        if(player.modNetworkObject.IsServer())
        {
            MinigameManager.ResetManager();
            ModdedGameMode.Instance.networkingManager.ServerLoadScene(MinigameManager.Instance.lobbyScene);
        }
    }

    public static string RemoveHtmlTags(string text)
    {
        List<int> openTagIndexes = Regex.Matches(text, "<").Cast<Match>().Select(m => m.Index).ToList();
        List<int> closeTagIndexes = Regex.Matches(text, ">").Cast<Match>().Select(m => m.Index).ToList();
        if (closeTagIndexes.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            int previousIndex = 0;
            foreach (int closeTagIndex in closeTagIndexes)
            {
                var openTagsSubset = openTagIndexes.Where(x => x >= previousIndex && x < closeTagIndex);
                if (openTagsSubset.Count() > 0 && closeTagIndex - openTagsSubset.Max() > 1)
                {
                    sb.Append(text.Substring(previousIndex, openTagsSubset.Max() - previousIndex));
                }
                else
                {
                    sb.Append(text.Substring(previousIndex, closeTagIndex - previousIndex + 1));
                }
                previousIndex = closeTagIndex + 1;
            }
            if (closeTagIndexes.Max() < text.Length)
            {
                sb.Append(text.Substring(closeTagIndexes.Max() + 1));
            }
            return sb.ToString();
        }
        else
        {
            return text;
        }
    }
}
