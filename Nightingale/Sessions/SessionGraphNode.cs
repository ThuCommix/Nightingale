using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Entities;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a graph node within the <see cref="SessionGraph"/>.
    /// </summary>
    public class SessionGraphNode : IEnumerable<SessionGraphNode>
    {
        /// <summary>
        /// Gets the entity for this session graph node.
        /// </summary>
        public Entity Entity { get; }

        /// <summary>
        /// Gets the root session graph for this node.
        /// </summary>
        public SessionGraph Graph { get; }

        /// <summary>
        /// A value indicating whether the node represents a deleted entity.
        /// </summary>
        public bool IsDelete { get; }

        /// <summary>
        /// Gets the sub nodes.
        /// </summary>
        public IEnumerable<SessionGraphNode> Nodes => _nodes;

        private readonly List<SessionGraphNode> _nodes;

        /// <summary>
        /// Initializes a new SessionGraphNode class.
        /// </summary>
        /// <param name="sessionGraph">The session graph.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="isDelete">A value indicating whether the node represents a deleted entity.</param>
        public SessionGraphNode(SessionGraph sessionGraph, Entity entity, bool isDelete)
        {
            Graph = sessionGraph;
            Entity = entity;
            IsDelete = isDelete;

            _nodes = new List<SessionGraphNode>();
        }

        /// <summary>
        /// Adds a new session graph node to this node.
        /// </summary>
        /// <param name="sessionGraphNode">The session graph node.</param>
        public void AddNode(SessionGraphNode sessionGraphNode)
        {
            if (!Graph.Contains(sessionGraphNode.Entity))
            {
                Graph.AddEntity(sessionGraphNode.Entity);
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
            var sessionGraphNode = new SessionGraphNode(Graph, entity, isDelete);
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
                Graph.RemoveEntity(sessionGraphNode.Entity);
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
        /// Traverses all session graph nodes recursive and collects the entities.
        /// </summary>
        /// <param name="nodes">The session graph nodes.</param>
        /// <param name="resultNodes">The result nodes.</param>
        private void TraverseSessionGraphNodes(IEnumerable<SessionGraphNode> nodes, List<SessionGraphNode> resultNodes)
        {
            foreach (var sessionGraphNode in nodes)
            {
                TraverseSessionGraphNodes(sessionGraphNode.Nodes, resultNodes);
            }

            resultNodes.Add(this);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<SessionGraphNode> GetEnumerator()
        {
            var nodes = new List<SessionGraphNode>();
            TraverseSessionGraphNodes(Nodes, nodes);

            return nodes.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
