// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class ActivatorTests
    {
        [InMemoryFact("Verifies Exception Behavior")]
        public async Task ControllerThatCannotBeActivated_ThrowsWhenAttemptedToBeInvoked()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expectedMessage = "No service for type 'ActivatorWebSite.CannotBeActivatedController+FakeType' " +
                                   "has been registered.";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetAsync("http://localhost/CannotBeActivated/Index"));
            Assert.Equal(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task PropertiesForPocoControllersAreInitialized()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = "4|some-text";

            // Act
            var response = await client.GetAsync("http://localhost/Plain?foo=some-text");

            // Assert
            var headerValue = Assert.Single(response.Headers.GetValues("X-Fake-Header"));
            Assert.Equal("Fake-Value", headerValue);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(expected, body);
        }

        [Fact]
        public async Task PropertiesForTypesDerivingFromControllerAreInitialized()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = "Hello world";

            // Act
            var body = await client.GetStringAsync("http://localhost/Regular");

            // Assert
            Assert.Equal(expected, body);
        }

        [Fact]
        public async Task ViewActivator_ActivatesDefaultInjectedProperties()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = @"<label for=""Hello"">Hello</label> world! /View/ConsumeServicesFromBaseType";

            // Act
            var body = await client.GetStringAsync("http://localhost/View/ConsumeDefaultProperties");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task ViewActivator_ActivatesAndContextualizesInjectedServices()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = "4 test-value";

            // Act
            var body = await client.GetStringAsync("http://localhost/View/ConsumeInjectedService?test=test-value");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task ViewActivator_ActivatesServicesFromBaseType()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = @"/content/scripts/test.js";

            // Act
            var body = await client.GetStringAsync("http://localhost/View/ConsumeServicesFromBaseType");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task ViewComponentActivator_ActivatesProperties()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = @"Random Number:4";

            // Act
            var body = await client.GetStringAsync("http://localhost/View/ConsumeViewComponent");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task ViewComponentActivator_ActivatesPropertiesAndContextualizesThem()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = "test-value";

            // Act
            var body = await client.GetStringAsync("http://localhost/View/ConsumeValueComponent?test=test-value");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task ViewComponentActivator_ActivatesPropertiesAndContextualizesThem_WhenMultiplePropertiesArePresent()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expected = "Random Number:4 test-value";

            // Act
            var body = await client.GetStringAsync("http://localhost/View/ConsumeViewAndValueComponent?test=test-value");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [InMemoryFact("Verifies Exception Behavior")]
        public async Task ViewComponentThatCannotBeActivated_ThrowsWhenAttemptedToBeInvoked()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ActivatorWebSite));
            var client = site.CreateClient();
            var expectedMessage = "No service for type 'ActivatorWebSite.CannotBeActivatedComponent+FakeType' " +
                                   "has been registered.";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => client.GetAsync("http://localhost/View/ConsumeCannotBeActivatedComponent"));
            Assert.Equal(expectedMessage, ex.Message);
        }
    }
}