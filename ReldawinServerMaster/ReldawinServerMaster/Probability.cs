using System;
using System.Collections.Generic;

namespace ReldawinServerMaster
{
    class Probability : IDisposable
    {
        class element
        {
            public int min, max;
        }

        private List<object> obj = new List<object>();
        private List<element> probability = new List<element>();

        double maxProb = 0;
        
        public Probability()
        {
            //The probability of returning nothing by default is 100% untill we add other stuff
            obj.Add( null );
            probability.Add( new element() { min = 0, max = 100 } );
        }
        
        public void Add( object d, int prob )
        {
            if ( prob + maxProb > 100 )
            {
                Console.WriteLine( "Cannot exceed maximum probability" );
                return;
            }

            // increase the maximum occupied probability
            maxProb += prob;

            // decrease the probability of returning null
            probability[0].max -= prob;

            obj.Add( d );
            probability.Add( new element() { min = probability[0].max, max = probability[0].max + prob } );
        }

        public object Roll()
        {
            if ( probability.Count == 1 )
                return obj[0];

            int roll = World.random.Next(0, 100);

            for(int i = 0; i < probability.Count; i++)
            {
                if ( probability[i].min < roll && probability[i].max >= roll )
                    return obj[i];
            }

            return null;
        }

        public void Dispose()
        {
            obj.Clear();
            obj = null;

            probability.Clear();
            probability = null;
        }
    }
}
