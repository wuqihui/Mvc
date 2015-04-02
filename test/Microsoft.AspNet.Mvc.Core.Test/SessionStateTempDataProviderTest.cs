﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class SessionStateTempDataProviderTest
    {
        [Fact]
        public void Load_NullSession_ReturnsEmptyDictionary()
        {
            // Arrange
            var testProvider = new SessionStateTempDataProvider();

            // Act
            var tempDataDictionary = testProvider.LoadTempData(
                GetHttpContext(session: null, sessionEnabled: true));

            // Assert
            Assert.Empty(tempDataDictionary);
        }

        [Fact]
        public void Load_NonNullSession_NoSessionData_ReturnsEmptyDictionary()
        {
            // Arrange
            var testProvider = new SessionStateTempDataProvider();

            // Act
            var tempDataDictionary = testProvider.LoadTempData(
                GetHttpContext(Mock.Of<ISessionCollection>()));

            // Assert
            Assert.Empty(tempDataDictionary);
        }

        [Fact]
        public void Save_NullSession_NullDictionary_DoesNotThrow()
        {
            // Arrange
            var testProvider = new SessionStateTempDataProvider();

            // Act & Assert (does not throw)
            testProvider.SaveTempData(GetHttpContext(session: null, sessionEnabled: false), null);
        }

        [Fact]
        public void Save_NullSession_EmptyDictionary_DoesNotThrow()
        {
            // Arrange
            var testProvider = new SessionStateTempDataProvider();

            // Act & Assert (does not throw)
            testProvider.SaveTempData(
                GetHttpContext(session: null, sessionEnabled: false), new Dictionary<string, object>());
        }

        [Fact]
        public void Save_NullSession_NonEmptyDictionary_Throws()
        {
            // Arrange
            var testProvider = new SessionStateTempDataProvider();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                testProvider.SaveTempData(
                    GetHttpContext(session: null, sessionEnabled: false),
                    new Dictionary<string, object> { { "foo", "bar" } }
                );
            });
        }

        [Theory]
        [MemberData(nameof(InvalidTypes))]
        public void Save_InvalidType_Throws(string key, object value, Type type)
        {
            // Arrange
            var testProvider = new SessionStateTempDataProvider();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                testProvider.SaveTempData(
                    GetHttpContext(session: null, sessionEnabled: false),
                    new Dictionary<string, object> { { key, value } }
                );
            });
            Assert.Equal(string.Format("The type {0} cannot be serialized to Session.", type), exception.Message);
        }

        public static TheoryData<string, object, Type> InvalidTypes
        {
            get
            {
                return new TheoryData<string, object, Type>
                {
                    { "Object", new object(), typeof(object) },
                    { "ObjectArray", new object[3], typeof(object) },
                    { "TestItem", new TestItem(), typeof(TestItem) },
                    { "ListTestItem", new List<TestItem>(), typeof(TestItem) },
                    { "DictTestItem", new Dictionary<string, TestItem>(), typeof(TestItem) }
                };
            }
        }

        private class TestItem
        {
            public int DummyInt { get; set; }
        }

        private HttpContext GetHttpContext(ISessionCollection session, bool sessionEnabled=true)
        {
            var httpContext = new Mock<HttpContext>();
            if (session != null)
            {
                httpContext.Setup(h => h.Session).Returns(session);
            }
            else if (!sessionEnabled)
            {
                httpContext.Setup(h => h.Session).Throws<InvalidOperationException>();
            }
            if (sessionEnabled)
            {
                httpContext.Setup(h => h.GetFeature<ISessionFeature>()).Returns(Mock.Of<ISessionFeature>());
                httpContext.Setup(h => h.Session[It.IsAny<string>()]);
            }
            return httpContext.Object;
        }
    }
}