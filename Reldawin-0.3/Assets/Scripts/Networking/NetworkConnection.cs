using UnityEngine;
using UnityEngine.UI;

namespace AlwaysEast
{
    [RequireComponent( typeof( GameObject ) )]
    public class NetworkConnection : SceneBehaviour
    {
        public enum ConnectionStrength
        {
            Offline = 0,
            Connecting = 1,
            Connected = 2
        };

        public TMPro.TMP_Text connectionAndFramerateText;
        public Sprite[] connectionStrengthIco;
        public Image connectionStrengthImg;
        private const float framerate_update_interval = 1.0f;
        private const float ping_timer_iterator = 8.0f;
        private float framerate_deltaTime = 0.0f;
        private float framerate_update_timer = 1.0f;
        private float ping_active_test_duration = 0.0f;
        private float ping_last_ms = 0.31f;
        private bool ping_pinging = false;
        private float ping_timer = 8.0f;
        public ConnectionStrength Strength { get; set; }

        public delegate void OnConnectedEventHandler();

        public static event OnConnectedEventHandler OnConnectedEvent;

        public void SetConnectionStrength( ConnectionStrength strength )
        {
            connectionStrengthImg.sprite = connectionStrengthIco[(byte)strength];
        }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        private void Awake()
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
        {
            Strength = ConnectionStrength.Offline;

            base.Awake();

            DontDestroyOnLoad( gameObject );

            EventProcessor.AddInstructionParams( Packet.ConnectionOK, OnConnected );
            EventProcessor.AddInstructionParams( Packet.PingTest, PingResponse );
            EventProcessor.AddInstructionParams( Packet.Crash, CrashRecovery );
        }

        private void OnConnected( params object[] args )
        {
            SetConnectionStrength( ConnectionStrength.Connected );
            Strength = ConnectionStrength.Connected;

            OnConnectedEvent?.Invoke();
        }

        private void CrashRecovery( object[] obj )
        {
            SetConnectionStrength( ConnectionStrength.Connecting );
            Strength = ConnectionStrength.Connecting;
        }

        private void PingResponse( params object[] args )
        {
            ping_pinging = false;
            ping_last_ms = ping_active_test_duration * 100;
            ping_active_test_duration = 0.0f;
        }

        private void Update()
        {
            if ( Strength == ConnectionStrength.Connected )
            {
                if ( ping_pinging == true )
                {
                    ping_active_test_duration += Time.deltaTime;
                }
                else
                {
                    ping_timer -= Time.deltaTime;

                    if ( ping_timer <= 0.0f )
                    {
                        ping_timer = ping_timer_iterator + ping_timer;

                        ping_pinging = true;
                        ClientTCP.SendPing();
                        Resources.UnloadUnusedAssets();
                    }
                }
            }

            //fps tracker
            framerate_deltaTime += ( Time.unscaledDeltaTime - framerate_deltaTime ) * 0.1f;
            float fps = 1.0f / framerate_deltaTime;

            framerate_update_timer -= Time.deltaTime;
            if ( framerate_update_timer <= 0.0f )
            {
                //reset timer
                framerate_update_timer = framerate_update_interval - framerate_update_timer;

                //update text
                connectionAndFramerateText.text = string.Format( "FPS: {0}   " + ( Strength == ConnectionStrength.Connected ? "Ping: {1}ms" : "Offline" ), (int)fps, ping_last_ms.ToString( "0" ) );
            }
        }
    }
}