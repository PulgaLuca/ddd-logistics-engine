using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Costruisce un albero binario di ricerca per ID e ne restituisce la visita in-order.</summary>
public sealed class HubTreeDomainService : IHubTreeDomainService
{
    public IReadOnlyList<Hub> TraverseInOrder(IEnumerable<Hub> hubs)
    {
        Node? root = null;
        foreach (Hub hub in hubs)
        {
            root = Insert(root, hub);
        }

        List<Hub> result = [];
        Traverse(root, result);
        return result;
    }

    private static Node Insert(Node? node, Hub hub)
    {
        if (node is null)
        {
            return new Node(hub);
        }

        int comparison = hub.Id.CompareTo(node.Hub.Id);
        if (comparison < 0)
        {
            node.Left = Insert(node.Left, hub);
        }
        else if (comparison > 0)
        {
            node.Right = Insert(node.Right, hub);
        }

        return node;
    }

    private static void Traverse(Node? node, ICollection<Hub> result)
    {
        if (node is null)
        {
            return;
        }

        Traverse(node.Left, result);
        result.Add(node.Hub);
        Traverse(node.Right, result);
    }

    private sealed class Node(Hub hub)
    {
        public Hub Hub { get; } = hub;
        public Node? Left { get; set; }
        public Node? Right { get; set; }
    }
}
