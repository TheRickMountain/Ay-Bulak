using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace palmesneo_village
{
    public class Entity
    {
        public Guid Id { get; set; }
        public Entity Parent { get; private set; }

        public string Name { get; set; } = "";

        public bool IsVisible { get; set; } = true;

        public virtual Vector2 LocalPosition { get; set; } = Vector2.Zero;
        public virtual float LocalRotation { get; set; } = 0f;
        public virtual Vector2 LocalScale { get; set; } = Vector2.One;

        public virtual Vector2 GlobalPosition { get => (LocalPosition * (Parent != null ? Parent.GlobalScale : Vector2.One)) + (Parent != null ? Parent.GlobalPosition : Vector2.Zero); }

        public float GlobalRotation { get => LocalRotation + (Parent != null ? Parent.GlobalRotation : 0); }

        public Vector2 GlobalScale { get => LocalScale * (Parent != null ? Parent.GlobalScale : Vector2.One); }

        public Color SelfColor { get; set; } = Color.White;

        private bool isDepthSortEnabled = false;
        public bool IsDepthSortEnabled 
        {
            get => isDepthSortEnabled;
            set
            {
                if (isDepthSortEnabled == value) return;

                isDepthSortEnabled = value;

                if (Parent != null)
                {
                    Parent.unsorted = true;
                }
            }
        }

        private float depth = 0;
        public float Depth
        {
            get => depth;
            set
            {
                if (depth == value) return;

                depth = value;

                if (Parent != null)
                {
                    Parent.unsorted = true;
                }
            }
        }

        private List<Entity> children;
        private List<Entity> childrenToAdd;
        private List<Entity> childrenToRemove;

        private Dictionary<string, object> metadata;

        private bool unsorted = false;

        public Entity()
        {
            Id = Guid.NewGuid();

            Name = GetType().Name;

            children = new List<Entity>();
            childrenToAdd = new List<Entity>();
            childrenToRemove = new List<Entity>();

            metadata = new Dictionary<string, object>();
        }

        public virtual void Begin()
        {
            foreach (var child in children)
            {
                child.Begin();
            }
        }

        public virtual void Update()
        {
            if(childrenToAdd.Count > 0)
            {
                for (int i = 0; i < childrenToAdd.Count; i++)
                {
                    Entity child = childrenToAdd[i];
                    children.Add(child);
                }

                childrenToAdd.Clear();

                if(IsDepthSortEnabled)
                {
                    unsorted = true;
                }
            }

            for(int i = 0; i < children.Count; i++)
            {
                Entity child = children[i];

                if (child.Parent != this) continue;

                child.Update();
            }

            if (childrenToRemove.Count > 0)
            {
                for (int i = 0; i < childrenToRemove.Count; i++)
                {
                    Entity child = childrenToRemove[i];
                    children.Remove(child);
                }

                childrenToRemove.Clear();
            }

            if (unsorted)
            {
                unsorted = false;
                children.Sort(CompareDepth);

                Debug.WriteLine("Sorted in: " + this);
            }
        }

        public virtual void Render()
        {
            foreach (var child in children)
            {
                if (child.IsVisible)
                {
                    child.Render();
                }
            }
        }

        public virtual void DebugRender()
        {
            foreach (var child in children)
            {
                if (child.IsVisible)
                {
                    child.DebugRender();
                }
            }
        }

        public virtual void AddChild(Entity child)
        {
            if(child.Parent != null)
            {
                throw new Exception($"Entity:{child} already has parent Entity:{child.Parent}");
            }

            child.Parent = this;

            // Если ребёнок уже добавлен в список на удаление, значит он есть в списке children
            if (childrenToRemove.Contains(child))
            {
                childrenToRemove.Remove(child);
            }
            else
            {
                childrenToAdd.Add(child);
            }
        }

        public T GetChildByName<T>(string name) where T : Entity
        {
            for (int i = 0; i < childrenToAdd.Count; i++)
            {
                Entity child = childrenToAdd[i];

                if (child.Parent != this) continue;

                if (child.Name == name) return (T)child;
            }

            for (int i = 0; i < children.Count; i++)
            {
                Entity child = children[i];

                if (child.Parent != this) continue;

                if (child.Name == name) return (T)child;
            }

            return null;
        }

        public Entity this[string name]
        {
            get
            {
                return GetChildByName<Entity>(name);
            }
        }

        public virtual bool ContainsChild(Entity child)
        {
            if (child == null) return false;

            return child.Parent == this;
        }

        public virtual void RemoveChild(Entity child)
        {
            if(child == null) return;

            if(child.Parent != this)
            {
                throw new Exception($"Entity:{child} has another parent Entity:{child.Parent}");
            }

            child.Parent = null;
            childrenToAdd.Remove(child);
            childrenToRemove.Add(child);
        }

        public IEnumerable<Entity> GetChildren()
        {
            foreach(var child in children)
            {
                if (child.Parent != this) continue;

                yield return child;
            }

            foreach(var child in childrenToAdd)
            {
                if (child.Parent != this) continue;

                yield return child;
            }
        }

        public IEnumerable<T> GetChildren<T>() where T : Entity
        {
            foreach (var child in children)
            {
                if (child.Parent != this) continue;

                if (child is T)
                {
                    yield return (T)child;
                }
            }

            foreach (var child in childrenToAdd)
            {
                if (child.Parent != this) continue;

                if (child is T)
                {
                    yield return (T)child;
                }
            }
        }

        public void SetMetadata<T>(string key, T value)
        {
            if (metadata.ContainsKey(key))
            {
                metadata[key] = value;
            }
            else
            {
                metadata.Add(key, value);
            }
        }

        public T GetMetadata<T>(string key)
        {
            return (T)metadata[key];
        }

        public void RemoveMetadata(string key)
        {
            if (metadata.ContainsKey(key))
            {
                metadata.Remove(key);
            }
        }

        public static Comparison<Entity> CompareDepth = (a, b) => { return Math.Sign(a.Depth - b.Depth); };

    }
}
