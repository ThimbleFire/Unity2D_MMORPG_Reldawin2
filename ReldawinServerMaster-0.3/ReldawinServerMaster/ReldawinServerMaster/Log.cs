namespace Bindings
{
    internal class Log
    {
        public const int BUFFER_PLAYERS = 8;
        public const string DatabaseAccountAlreadyExists = "[DB] Name already taken";
        public const string DatabaseAccountCreated = "[DB] Account Created!";
        public const string DatabaseNullException = "[DB] Unexpectedly returned null.";
        // DB Errors
        public const string DatabaseOffline = "[DB] failed to connect";

        public const string DatabasePasswordMismatch = "[DB] Bad password.";
        public const string DatabaseUserCrash = "[DB] User {0} crashed, probably due to a network or db error. ";
        public const string DatabaseUsernameMismatch = "[DB] No player account with this name was found.";
        public const int MAX_PLAYERS = 10;
        public const string SERVER_CREATE_ACCOUNT_QUERY = "Attempting to create account under username {0}";

        public const string SERVER_DISCONNECT_SAFELY = "Connection from {0} has been safely terminated.";

        // Confirmation in server
        public const string SERVER_LOBBY_JOIN = "player {0} waiting in the lobby";

        public const string SERVER_LOGIN_SUCCESS = "{0} logged in.";
    }
}