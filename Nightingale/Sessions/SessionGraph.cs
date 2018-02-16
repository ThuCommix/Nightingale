using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Entities;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents the session graph which contains the order of the entity hierarchy.
    /// </summary>
    public class SessionGraph : IEnumerable<SessionGraphNode>
    {
        /// <summary>
        /// Gets the session graph nodes.
        /// </summary>
        public IEnumerable<SessionGraphNode> Nodes => _nodes;

        private readonly List<SessionGraphNode> _nodes;
        private readonly HashSet<Entity> _hashSet;

        /// <summary>
        /// Initializes a new SessionGraph class.
        /// </summary>
        public SessionGraph()
        {
            _nodes = new List<SessionGraphNode>();
            _hashSet = new HashSet<Entity>();
        }

        /// <summary>
        /// Adds a new session graph node to this node.
        /// </summary>
        /// <param name="sessionGraphNode">The session graph node.</param>
        public void AddNode(SessionGraphNode sessionGraphNode)
        {
            if (!Contains(sessionGraphNode.Entity))
            {
                AddEntity(sessionGraphNode.Entity);
                _nodes.Add(sessionGraphNode);
            }
        }

        /// <summary>
        /// Adds a new session graph node to this node.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="isDelete">A value indicating whether the node represents a deleted entity.</param>
        /// <returns>Returns the created session graph node.</returns>
        public SessionGraphNode AddNode(Entity entity, bool isDelete)
        {
            var sessionGraphNode = new SessionGraphNode(this, entity, isDelete);
            AddNode(sessionGraphNode);

            return sessionGraphNode;
        }

        /// <summary>
        /// Removes a session graph node from this node.
        /// </summary>
        /// <param name="sessionGraphNode">The session graph node.</param>
        /// <returns>Returns true when the session graph node was removed.</returns>
        public bool RemoveNode(SessionGraphNode sessionGraphNode)
        {
            if (_nodes.Remove(sessionGraphNode))
            {
                RemoveEntity(sessionGraphNode.Entity);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a session graph node from this node.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true when the session graph node was removed.</returns>
        public bool RemoveNode(Entity entity)
        {
            var sessionGraphNode = _nodes.FirstOrDefault(x => x.Entity == entity);
            if (sessionGraphNode == null)
                return false;

            return RemoveNode(sessionGraphNode);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SessionGraph"/> contains a specific entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true when the <see cref="SessionGraph"/> contains the entity.</returns>
        public bool Contains(Entity entity)
        {
            return _hashSet.Contains(entity);
        }

        /// <summary>
        /// Adds an entity to the internal hashset.
        /// </summary>
        /// <param name="entity">The entity.</param>
        internal void AddEntity(Entity entity)
        {
            _hashSet.Add(entity);
        }

        /// <summary>
        /// Removes the entity from the internal hashset.
        /// </summary>
        /// <param name="entity">The entity.</param>
        internal void RemoveEntity(Entity entity)
        {
            _hashSet.Remove(entity);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<SessionGraphNode> GetEnumerator()
        {
            var nodes = new List<SessionGraphNode>();
            TraverseSessionGraphNodes(Nodes, nodes);

            return nodes.GetEnumerator();
        }

        /// <summary>
        /// Clears the session graph.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
            _hashSet.Clear();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Traverses all session graph nodes recursive and collects the entities.
        /// </summary>
        /// <param name="nodes">The session graph nodes.</param>
        /// <param name="resultNodes">The result nodes.</param>
        private void TraverseSessionGraphNodes(IEnumerable<SessionGraphNode> nodes, List<SessionGraphNode> resultNodes)
        {
            foreach (var sessionGraphNode in nodes)
            {
                TraverseSessionGraphNodes(sessionGraphNode.Nodes, resultNodes);
                resultNodes.Add(sessionGraphNode);
            }
        }
    }
}
