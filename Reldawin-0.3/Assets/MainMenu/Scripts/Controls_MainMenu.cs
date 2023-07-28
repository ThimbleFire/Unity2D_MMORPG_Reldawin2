using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AlwaysEast
{
    public class Controls_MainMenu : SceneBehaviour
    {
        [SerializeField] private EventSystem system;
        [SerializeField] private GameObject username;
        [SerializeField] private GameObject password;
        [SerializeField] private Toggle rememberUsername;
        [SerializeField] private TMPro.TMP_Text txtErrorLog;
        [SerializeField] private GameObject creationWindow;

        public void OnBtnLoginClicked() {

            string usrname = username.GetComponent<TMPro.TMP_InputField>().text;
            string pword = password.GetComponent<TMPro.TMP_InputField>().text;

            ClientTCP.SendLoginAttemptQuery( usrname, pword );
        }

        public void OnBtnCreateAccountClicked() {
            creationWindow.SetActive( true );
            gameObject.SetActive( false );
        }

        public void OnToggleRememberUsername( Toggle toggleComponent ) {
            Game.rememberUsernameOnMainMenu = toggleComponent.isOn;
        }

        private void OnNetworkConnectedToLobby() {
            if( Game.rememberUsernameOnMainMenu ) {
                username.GetComponent<TMPro.TMP_InputField>().text = Game.username;
                rememberUsername.isOn = true;
            } else
                rememberUsername.isOn = false;
        }

        private void OnNetworkLoginFail( params object[] args ) {
            txtErrorLog.enabled = true;
            txtErrorLog.text = ( string )args[0];
        }

        private void OnNetworkLoginSuccess( params object[] args ) {
            // store the users name on their computer
            if( rememberUsername.isOn )
                Game.username = username.GetComponent<TMPro.TMP_InputField>().text;
            else
                Game.username = string.Empty;

            Game.Save();

            SceneManager.LoadScene( 1 );
        }

        protected override void Awake() {
            base.Awake();

            Game.SetupApplicationGlobals();
            Game.Load();

            NetworkConnection.OnConnectedEvent += OnNetworkConnectedToLobby;
            EventProcessor.AddInstructionParams( Packet.Account_Login_Fail, OnNetworkLoginFail );
            EventProcessor.AddInstructionParams( Packet.Account_Login_Success, OnNetworkLoginSuccess );
        }

        public void Update() {
            if( Input.GetKeyDown( KeyCode.Tab ) ) {
                if( system.currentSelectedGameObject == username ) {
                    system.SetSelectedGameObject( password, new BaseEventData( system ) );
                    return;
                }
                if( system.currentSelectedGameObject == password ) {
                    system.SetSelectedGameObject( username, new BaseEventData( system ) );
                    return;
                }
            }
        }

        private void OnDestroy() {
            NetworkConnection.OnConnectedEvent -= OnNetworkConnectedToLobby;
            EventProcessor.RemoveInstructionParams( Packet.Account_Login_Fail );
            EventProcessor.RemoveInstructionParams( Packet.Account_Login_Success );
        }
    }
}