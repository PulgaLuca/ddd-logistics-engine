using LogisticsEngine.Domain.Shared.Sorting;

namespace LogisticsEngine.UnitTests.Domain.Sorting;

public sealed class SorterTests
{
    [Fact]
    public void Sort_uses_caller_selected_key()
    {
        ISorter<string> sorter = new Sorter<string>();
        sorter.Sort(new[] { "bbb", "a", "cc" }, value => value.Length).ShouldBe(new[] { "a", "cc", "bbb" });
    }
}
