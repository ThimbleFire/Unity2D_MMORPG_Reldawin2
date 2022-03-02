using UnityEngine;

namespace LowCloud.Reldawin
{
    public class ClientParams
    {
        public Vector2Int GetCellPos
        {
            get 
            { 
                return new Vector2Int( cellPosX, cellPosY ); 
            } 
        }

        public ClientParams()
        { }

        public ClientParams( string username, int cellPosX, int cellPosY, int ID, bool Running )
        {
            this.username = username;
            this.cellPosX = cellPosX;
            this.cellPosY = cellPosY;
            this.ID = ID;
            this.Running = Running;
        }

        public string username;
        public int cellPosX, cellPosY;
        public int ID;
        public bool Running { get; set; }
    }
}