﻿namespace Bindings
{
    public enum Packet
    {
        Crash,
        ConnectionOK,
        Account_Login_Query,
        Account_Login_Success,
        Account_Login_Fail,
        Account_Create_Query,
        Account_Create_Fail,
        Account_Create_Success,
        RequestSpawn,
        SavePositionToServer,
        AnnounceMovementToNearbyPlayers,
        PingTest,
        DoesUserExist,
        OtherPlayerCharacterLoggedIn,
        OtherPlayerCharacterListRequest,
        Load_Chunk,
        Load_Doodads,
        StartInteract,
        YieldInteract,
        StopInteract,
        ToggleRunning,
        ToggleSwimming,
        Disconnect
    };
}