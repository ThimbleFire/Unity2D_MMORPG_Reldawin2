namespace AlwaysEast
{
    public enum Packet
    {
        Crash,

        //On connection give a basic welcome response
        ConnectionOK,

        Account_Login_Query,
        Account_Login_Success,
        Account_Login_Fail,
        Account_Create_Query,
        Account_Create_Fail,
        Account_Create_Success,
        
        //Request map dimensions, seed, player position
        RequestSeed,

        //Let the server know the player has moved tile
        SavePositionToServer,

        //Let the server know to tell other clients where the player entity is moving to
        AnnounceMovementToNearbyPlayers,

        PingTest,
        DoesUserExist,

        //When a client connects, create an entity for them.
        OtherPlayerCharacterLoggedIn,

        //Ask the server who else is logged in (called on login)
        OtherPlayerCharacterListRequest,

        Load_Chunk,
        Load_Doodads,

        //Ask the server to subscribe the client to an interact
        StartInteract,
        YieldInteract,
        StopInteract,

        ToggleRunning,
        ToggleSwimming,
        Disconnect
    };
}
