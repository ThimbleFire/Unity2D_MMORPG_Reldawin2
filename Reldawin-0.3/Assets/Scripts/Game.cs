using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlwaysEast
{
    public class Game
    {
        public static int dbID = 0;
        public static string password;
        public static string username;
        public static bool rememberUsernameOnMainMenu = false;


        public static void Save() {
            PlayerPrefs.SetString( "username", username );
            PlayerPrefs.SetString( "password", password );
            PlayerPrefs.SetInt( "dbID", dbID );
            PlayerPrefs.SetInt( "rememberUsernameOnMainMenu", rememberUsernameOnMainMenu == true ? 1 : 0 );
        }
        public static void Load() {
            username = PlayerPrefs.GetString( "username" );
            password = PlayerPrefs.GetString( "password" );
            dbID = ( ushort )PlayerPrefs.GetInt( "dbID" );
            rememberUsernameOnMainMenu = PlayerPrefs.GetInt( "rememberUsernameOnMainMenu" ) == 1 ? true : false;
        }

        public static void SetupApplicationGlobals() {
            Application.targetFrameRate = 60;
        }
    }
}