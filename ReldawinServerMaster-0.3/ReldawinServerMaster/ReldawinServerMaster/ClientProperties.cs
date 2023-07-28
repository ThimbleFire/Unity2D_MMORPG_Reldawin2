using System;
using System.Collections.Generic;
using ReldawinServerMaster;

namespace Bindings
{
    internal class ClientProperties
    {
        public ClientProperties()
        {
            Clear();
        }

        public string Username { get; set; }
        public int ID { get; set; }
        public List<int> items = new List<int>();
        public Vector2Int Position { get; set; }
        public bool Running { get; set; }
        public bool Swimming { get; set; }
       
        // when we interact with a doodad we need to store the type of object
        public int type;

        public void Clear()
        {
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
