using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibH
{
    class Program
    {
        public class FibonaciHeap<Type> where Type:IComparable, new()
        {
            private class Node
            {
                int degree;
                bool mark;
                Type key;
                public Type Key { get { return key; } }
                public int Degree { get { return degree; } }
                public Node(Type t)
                {
                    Child = Left = Right = null;
                    key = t;
                    mark = false;
                    degree = 0;
                }
                public void Merge(Node M)
                {
                    M.Left = this;
                    M.Right = Right;
                    Right.Left = M;
                    Right = M;
                }
                public void Hang(Node p)
                {
                    if (degree == 0)
                    {
                        Child = p;
                        p.Left = p.Right = p;
                    }
                    else
                        Child.Merge(p);
                    degree += p.degree + 1;
                }
                public Node Extract()
                {
                    if (!object.ReferenceEquals(Left,this))
                    {
                        Left.Right = Right;
                        Right.Left = Left;
                    }
                    return this;
                }
                public void GrowChildren()
                {
                    if (degree == 0)
                        return;
                    degree = 0;
                    if (object.ReferenceEquals(this, Right))
                    {
                        Left = Child.Left;
                        Right = Child;
                        Left.Right = this;
                        Child.Left = this;
                    }
                    else
                        if (object.ReferenceEquals(Child, Child.Left))
                            Merge(Child);
                        else
                        {
                            Child.Left.Right = Right;
                            Right.Left = Child.Left;
                            Child.Left = this;
                            Right = Child;
                        }
                    
                    Child = null;
                }
                public Node Child { get; set; }
                public Node Left { get; set; }
                public Node Right { get; set; }
            }
            public enum func { minheap, maxheap};
            func comp;
            int Log2(int a)
            {
                int res = 0;
                while ((a >>= 1) > 0)
                    res++;
                return res;

            }
            bool compare(Type A, Type B)
            {
                if (comp == func.minheap)
                    if (A.CompareTo(B) <= 0)
                        return true;
                    else
                        return false;
                else
                    if (A.CompareTo(B) >= 0)
                        return true;
                    else
                        return false;

            }
            public static FibonaciHeap<Type> MakeHeap(func minmax)
            {
                FibonaciHeap<Type> res = new FibonaciHeap<Type>();
                res.comp = minmax;
                return res;
            }
            Node root;
            public void Push(Type data)
            {
                Node node = new Node(data);
                if (object.ReferenceEquals(root, null))
                {
                    root = node;
                    root.Left = root;
                    root.Right = root;
                }
                else
                {
                    root.Merge(node);
                    if (compare(node.Key, root.Key))
                        root = node;
                }
            }
            public Type Pop()
            {
                Type res = new Type();
                if (object.ReferenceEquals(root, null))
                    return res;
                root.GrowChildren();
                res = root.Key;
                if (object.ReferenceEquals(root, root.Left))
                {
                    root = null;
                    return res;
                }
                root.Extract();
                root = root.Right;
                consolidate();
                return res;
            }



            List<Node> deg;
            void compress(ref Node temp)
            {
                int tdeg = Log2(temp.Degree + 1); 
                while (!object.ReferenceEquals(deg[tdeg], null))
                {
                    
                    if (compare(deg[tdeg].Key, temp.Key))
                    {
                        temp.Extract();
                        deg[tdeg].Hang(temp);
                        temp = deg[tdeg];
                    }
                    else
                    {
                        deg[tdeg].Extract();
                        temp.Hang(deg[tdeg]);
                    }
                    deg[tdeg] = null;
                    tdeg = Log2(temp.Degree + 1);
                }
                deg[tdeg] = temp;
            }



            void consolidate()
            {
                if (object.ReferenceEquals(deg, null))
                    deg = new List<Node>(64);
                deg.Clear();
                for (int i = 0; i < 64; i++)
                    deg.Add(null);
                Node end = root.Left;
                Node temp = root;
                while(!object.ReferenceEquals(end,temp))
                {
                    Node next = temp.Right;
                    compress(ref temp);
                    temp = next;
                }
                compress(ref end);
                root = end.Left;
                temp = end.Left;
                do
                {
                    temp = temp.Left;
                    if (compare(temp.Key, root.Key))
                        root = temp;
                } while (!object.ReferenceEquals(end, temp));
            }
        }
        static void Main(string[] args)
        {
            FibonaciHeap<int> heap = FibonaciHeap<int>.MakeHeap(FibonaciHeap<int>.func.maxheap);
            Random rng = new Random();
            for (int i = 0; i < 10;i++)
            {
                int k = rng.Next(i + 1, 10 + i); 
                heap.Push(k);
                Console.Write("{0} ", k);
            }
            Console.WriteLine();
            for (int i = 0; i < 10; i++)
                Console.WriteLine("FUCKING MAX ELEMENT {0}", heap.Pop());
            int j;
            j = 10;
        }
    }
}
