namespace ReldawinServerMaster
{
    public class Vector2Int
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2Int( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int( int[] coordinates )
        {
            this.x = coordinates[0];
            this.y = coordinates[1];
        }

        public int this[int index]
        {
            get
            {
                return index == 0 ? x : y;
            }
            set
            {
                switch ( index )
                {
                    case 0:
                        x = value;
                        break;

                    case 1:
                        y = value;
                        break;
                }
            }
        }

        public static Vector2Int Zero
        {
            get
            {
                return new Vector2Int( 0, 0 );
            }
        }

        public static Vector2Int operator +( Vector2Int a, Vector2Int b )
        {
            int x = a.x + b.x;
            int y = a.y + b.y;

            return new Vector2Int( x, y );
        }

        public static Vector2Int operator -( Vector2Int a, Vector2Int b )
        {
            int x = a.x - b.x;
            int y = a.y - b.y;

            return new Vector2Int( x, y );
        }

        public static bool operator ==( Vector2Int a, Vector2Int b )
        {
            if ( a?.x == b?.x && a?.y == b?.y )
                return true;
            else
                return false;
        }

        public static bool operator !=( Vector2Int a, Vector2Int b )
        {
            if ( a?.x != b?.x || a?.y != b?.y )
                return true;
            else
                return false;
        }

        public override bool Equals( object obj )
        {
            return obj is Vector2Int p 
                        && x == p.x
                        && y == p.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }
}
