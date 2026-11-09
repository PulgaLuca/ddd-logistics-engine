using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Indicizza i nomi in un trie per effettuare ricerche per prefisso.</summary>
public sealed class HubTrieDomainService : IHubTrieDomainService
{
    public IReadOnlyList<Hub> SearchByNamePrefix(IEnumerable<Hub> hubs, string? prefix)
    {
        TrieNode root = new();
        foreach (Hub hub in hubs)
        {
            Insert(root, hub.Name.Value, hub);
        }

        TrieNode? node = FindNode(root, prefix?.Trim().ToUpperInvariant() ?? string.Empty);
        return node is null ? [] : Collect(node).OrderBy(hub => hub.Name.Value, StringComparer.Ordinal).ToArray();
    }

    private static void Insert(TrieNode root, string value, Hub hub)
    {
        TrieNode node = root;
        foreach (char character in value.ToUpperInvariant())
        {
            if (!node.Children.TryGetValue(character, out TrieNode? child))
            {
                child = new TrieNode();
                node.Children.Add(character, child);
            }

            node = child;
        }

        node.Hubs.Add(hub);
    }

    private static TrieNode? FindNode(TrieNode root, string prefix)
    {
        TrieNode node = root;
        foreach (char character in prefix)
        {
            if (!node.Children.TryGetValue(character, out TrieNode? child))
            {
                return null;
            }

            node = child;
        }

        return node;
    }

    private static IEnumerable<Hub> Collect(TrieNode node)
    {
        foreach (Hub hub in node.Hubs)
        {
            yield return hub;
        }

        foreach (TrieNode child in node.Children.Values)
        {
            foreach (Hub hub in Collect(child))
            {
                yield return hub;
            }
        }
    }

    private sealed class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new();
        public List<Hub> Hubs { get; } = [];
    }
}
