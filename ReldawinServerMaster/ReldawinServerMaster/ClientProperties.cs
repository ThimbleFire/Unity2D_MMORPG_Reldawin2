using ReldawinServerMaster;

namespace Bindings
{
    internal class ClientProperties
    {
        public List<int> items = new List<int>();

        // when we interact with a doodad we need to store the type of object
        public int type;

        public ClientProperties() {
            Clear();
        }

        public int ID { get; set; }
        public Vector2Int Position { get; set; }
        public bool Running { get; set; }
        public bool Swimming { get; set; }
        public string Username { get; set; }
        public void Clear() {
            Position = Vector2Int.Zero;
            Username = "Unknown";
            items.Clear();
            Running = false;
            Swimming = false;
            ID = int.MaxValue;
            type = int.MaxValue;
        }
    }
}