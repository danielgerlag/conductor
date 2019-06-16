using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Conductor.IntegrationTests
{
    [CollectionDefinition("Conductor")]
    public class IntegrationCollection : ICollectionFixture<Setup>
    {
    }
}
