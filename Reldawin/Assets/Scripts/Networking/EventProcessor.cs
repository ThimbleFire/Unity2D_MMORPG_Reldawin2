using System;
using System.Collections.Generic;
using UnityEngine;
namespace AlwaysEast
{
    public class EventProcessor : MonoBehaviour
    {
        // When the client needs to interact with the main thread, if a specific client packet is recieved, a delegate is fired
        public Dictionary<Packet, Action<object[]>> instructionParams = new Dictionary<Packet, Action<object[]>>();
        private readonly List<Packet> m_executingEvents = new List<Packet>();
        private readonly List<object[]> m_executingParams = new List<object[]>();
        private readonly List<Packet> m_queuedEvents = new List<Packet>();
        private readonly List<object[]> m_queuedParams = new List<object[]>();
        public void AddInstructionParams( Packet packet, Action<object[]> d ) {
            instructionParams.Add( packet, d );
        }
        public void ClearInstructions() {
            instructionParams.Clear();
        }
        public void QueueEvent( Packet action, params object[] m ) {
            m_queuedEvents.Add( action );
            m_queuedParams.Add( m );
        }
        public void RemoveInstructionParams( Packet packet ) {
            instructionParams.Remove( packet );
        }
        private void Awake() {
            DontDestroyOnLoad( this );
        }
        private void MoveQueuedEventsToExecuting() {
            if( m_queuedEvents.Count > 0 ) {
                Packet e = m_queuedEvents[0];
                object[] p = m_queuedParams[0];
                m_executingEvents.Add( e );
                m_executingParams.Add( p );
                m_queuedEvents.RemoveAt( 0 );
                m_queuedParams.RemoveAt( 0 );
            }
        }
        private void Update() {
            MoveQueuedEventsToExecuting();
            if( m_executingEvents.Count > 0 ) {
                Packet action = m_executingEvents[0];
                object[] p = m_executingParams[0];
                m_executingEvents.RemoveAt( 0 );
                m_executingParams.RemoveAt( 0 );
                if( instructionParams.ContainsKey( action ) ) {
                    instructionParams[action].Invoke( p );
                }
                else {
                    Debug.LogError( string.Format( "No instruction for {0}.", action ) );
                }
            }
        }
    }
}