using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

//https://stackoverflow.com/a/10442244
namespace Overby.Collections
{
    public class TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();
        private int depth = 0;

        public TreeNode(T value)
        {
            _value = value;
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode<T> Parent { get; private set; }

        public T Value { get { return _value; } }

        public int Depth => depth;

        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this, depth = this.depth + 1 };
            _children.Add(node);
            return node;
        }

        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<T> Flatten()
        {
            return new[] { Value }.Concat(_children.SelectMany(x => x.Flatten()));
        }

        public IEnumerable<TreeNode<T>> FlattenNode()
        {
            var l = new List<TreeNode<T>> { this };
            
            //.AddRange(this._children.ForEach(x => x.Flatten()));
            foreach (var c in _children)
            {
                l.AddRange(c.FlattenNode());
            }

            return l;
        }

        public int NumChildren => _children.Count;

        public TreeNode<T> Find (T val)
        {
            if (_value.Equals(val))
                return this;

            foreach (var c in Children)
            {
                var f = c.Find(val);
                if (f != null)
                    return f;
            }

            return null;
        }

        public void MaxPath (ref int dist)
        {
            // if no children, then done
            if (_children.Count == 0)
                return;

            int savedDist = dist;

            foreach (var c in _children)
            {
                int childDist = savedDist + 1;
                c.MaxPath(ref childDist);
                if (childDist > dist)
                    dist = childDist;
            }
        }
    }
}

