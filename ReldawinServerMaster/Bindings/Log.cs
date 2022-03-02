namespace Bindings
{
    internal class Log
    {
        public const int MAX_PLAYERS = 10;
        public const int BUFFER_PLAYERS = 8;

        // DB Errors
        public const string DatabaseOffline = "[DB] failed to connect";
        public const string DatabaseNullException = "[DB] Unexpectedly returned null.";
        public const string DatabaseUserCrash = "[DB] User {0} crashed, probably due to a network or db error. ";

        public const string DatabaseUsernameMismatch = "[DB] No player account with this name was found.";
        public const string DatabasePasswordMismatch = "[DB] Bad password.";
        public const string DatabaseAccountAlreadyExists = "[DB] Name already taken";
        public const string DatabaseAccountCreated = "[DB] Account Created!";

        // Confirmation in server
        public const string SERVER_LOBBY_JOIN = "player {0} waiting in the lobby";
        public const string SERVER_LOGIN_SUCCESS = "{0} logged in.";
        public const string SERVER_DISCONNECT_SAFELY = "Connection from {0} has been safely terminated.";
        public const string SERVER_CREATE_ACCOUNT_QUERY = "Attempting to create account under username {0}";
    }
}