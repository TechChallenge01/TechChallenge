using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.test.Infrastructure;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestBase> { }