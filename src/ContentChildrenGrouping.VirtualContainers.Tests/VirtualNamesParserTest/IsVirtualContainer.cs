using EPiServer.Core;
using Xunit;

namespace ContentChildrenGrouping.VirtualContainers.Tests.VirtualNamesParserTest
{
    [Trait(nameof(VirtualNamesParser), nameof(VirtualNamesParser.IsVirtualContainer))]
    public class IsVirtualContainer : VirtualNamesParserTestBase
    {
        public class When_provider_name_contains_VirtualContainers : IsVirtualContainer
        {
            [Fact]
            void It_should_return_true()
            {
                Assert.True(new ContentReference(100, 100, "VirtualContainers_a_b").IsVirtualContainer());
            }
        }

        public class When_provider_name_DOES_NOT_contain_VirtualContainers : IsVirtualContainer
        {
            [Fact]
            void It_should_return_false()
            {
                Assert.False(new ContentReference(100, 100).IsVirtualContainer());
            }
        }
    }
}