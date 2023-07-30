using AlwaysEast;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace AlwaysEast
{
    public class Controls_MainMenu_CreateAccount : SceneBehaviour
    {
        [SerializeField] private EventSystem system;
        [SerializeField] private GameObject username;
        [SerializeField] private GameObject password;
        [SerializeField] private TMPro.TMP_Text txtErrorLog;
        [SerializeField] private Button btnCreateAccount;
        [SerializeField] private GameObject mainMenuWindow;

        public void OnCreateAccountClicked() {
            using PacketBuffer buffer = new PacketBuffer( Packet.Account_Create_Query );
            buffer.WriteString( username.GetComponent<TMPro.TMP_InputField>().text );
            buffer.WriteString( password.GetComponent<TMPro.TMP_InputField>().text );
            ClientTCP.SendData( buffer.ToArray() );
        }

        public void OnIpfUsernameCharChanged( TMPro.TMP_InputField ipf ) {
            using PacketBuffer buffer = new PacketBuffer( Packet.DoesUserExist );
            buffer.WriteString( ipf.GetComponent<TMPro.TMP_InputField>().text );
            ClientTCP.SendData( buffer.ToArray() );
        }

        public void OnBtnReturnToMainMenuClicked() {
            mainMenuWindow.SetActive( true );
            gameObject.SetActive( false );
        }

        private void OnNetworkQueryUsernameResultReturned( params object[] args ) {
            bool result = (bool)args[0];
            btnCreateAccount.interactable = !result;
        }

        private void OnNetworkAccountCreatedResult( params object[] args ) {
            txtErrorLog.enabled = true;
            txtErrorLog.color = UnityEngine.Color.green;
            btnCreateAccount.interactable = false;
        }

        private void OnEnable() {
            EventProcessor.AddInstructionParams( Packet.DoesUserExist, OnNetworkQueryUsernameResultReturned );
            EventProcessor.AddInstructionParams( Packet.Account_Create_Success, OnNetworkAccountCreatedResult );

        }
        private void OnDisable() {

            EventProcessor.RemoveInstructionParams( Packet.DoesUserExist );
            EventProcessor.RemoveInstructionParams( Packet.Account_Create_Success );

        }

        protected override void Awake() {
            base.Awake();
        }

        private void OnDestroy() {
            EventProcessor.RemoveInstructionParams( Packet.DoesUserExist );
            EventProcessor.RemoveInstructionParams( Packet.Account_Create_Success );
        }
    }
}