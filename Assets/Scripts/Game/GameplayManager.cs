using ClansWars.Game;
using ClansWars.Network;
using ClansWars.Player;
using ClansWars.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class GameplayManager : MonoBehaviour
{
    [Inject]
    private MapsContainer _mapsContainer;

    [SerializeField]
    private Map _currentMap;

    [SerializeField]
    private NetworkPlayersInput _networkPlayersInput;

    [SerializeField]
    private PlayerState _playerPrefab;

    private void Start()
    {
        InitializeMap();
        InitializePlayers();
    }

    private void InitializeMap()
    {
        _currentMap = Instantiate(_mapsContainer.Maps[0], Vector3.zero, Quaternion.identity, null);
    }

    private void InitializePlayers()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
            {
                InitializeNetworkPlayer(NetworkManager.Singleton.ConnectedClientsIds[i], _currentMap.PlayerSpawnPositions[i].position);
            }
        }
        else if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
        {
            InitializeLocalPlayer();
        }
    }

    private void InitializeNetworkPlayer(ulong playerId, Vector3 position)
    {
        PlayerState _newPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity, null);
        _newPlayer.NetworkObject.SpawnWithOwnership(playerId);
        _newPlayer.PlayerId.Value = playerId;
        _networkPlayersInput.AddPlayersInputStates(_newPlayer);
    }

    private void InitializeLocalPlayer()
    {
        PlayerState _newPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity, null);
    }
}
