using UnityEngine;
using UnityEngine.SceneManagement;
namespace AlwaysEast
{
    public class SceneBehaviour : MonoBehaviour
    {
        public static EventProcessor EventProcessor {
            get;
            set;
        }
        /// <summary>
        /// YOU MUST CALL Base.Awake() before setting up events!
        /// </summary>
        protected virtual void Awake() {
            try {
                EventProcessor = Component.FindObjectOfType<EventProcessor>();
            } catch( System.Exception ) {
                if( SceneManager.GetActiveScene().name != "MainMenu" ) {
                    LoadScene( "MainMenu" );
                }
            }
        }
        protected void LoadScene( string name ) {
            SceneManager.LoadScene( name );
        }
        protected void LoadScene( int index ) {
            SceneManager.LoadScene( index );
        }
    }
}