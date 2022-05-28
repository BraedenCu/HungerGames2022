using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    abstract public class Turn
    {
        protected MovingObject Owner { get; }

        public Turn (MovingObject owner)
        {
            Owner = owner;
        }

        abstract public bool DoTurn();
    }
}
