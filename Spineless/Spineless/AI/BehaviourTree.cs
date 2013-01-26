using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.AI
{
    public abstract class Behaviour<T>
    {
        public abstract bool Evaluate(T actor);
    }

    public class IdentityBehaviour<T> : Behaviour<T>
    {
        public override bool Evaluate(T actor)
        {
            return true;
        }
    }

    public abstract class Composite<T> : Behaviour<T>
    {
        protected List<Behaviour<T>> m_nodes;

        public Composite()
        {
            m_nodes = new List<Behaviour<T>>();
        }

        public List<Behaviour<T>> Children
        {
            get { return m_nodes; }
        }
    }

    public class Selector<T> : Composite<T>
    {
        public override bool Evaluate(T actor)
        {
            foreach(Behaviour<T> beh in m_nodes)
            {
                if (beh.Evaluate(actor))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Sequence<T> : Composite<T>
    {
        public override bool Evaluate(T actor)
        {
            foreach(Behaviour<T> beh in m_nodes)
            {
                if (!beh.Evaluate(actor))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
