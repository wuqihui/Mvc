using System;
using System.Collections.Generic;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Mvc.Core.Logging
{
    public class ActionExecutionScopeValues : ILogValues
    {
        private readonly ActionDescriptor _actionDescriptor;

        public ActionExecutionScopeValues(ActionDescriptor actionDescriptor)
        {
            _actionDescriptor = actionDescriptor;
            ActionName = actionDescriptor.DisplayName;
        }

        public string ActionName { get; }

        public string Format()
        {
            return LogFormatter.FormatLogValues(this);
        }

        public IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            return new[]
            {
                new KeyValuePair<string, object>(nameof(ActionName), ActionName)
            };
        }
    }
}