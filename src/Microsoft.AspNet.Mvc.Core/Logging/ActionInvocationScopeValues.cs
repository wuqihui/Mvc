// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Internal;
namespace Microsoft.AspNet.Mvc.Logging
{
    public class ActionInvocationScopeValues : ReflectionBasedLogValues
    {
        public ActionInvocationScopeValues([NotNull] ControllerActionDescriptor controllerActionDescriptor)
        {
            ActionName = controllerActionDescriptor.DisplayName;
        }

        public string ActionName { get; }

        public override string Format()
        {
            return LogFormatter.FormatLogValues(this);
        }
    }
}