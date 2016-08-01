// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Routing
{
    public class RouteProcessingException : Exception
    {
        public RouteProcessingException(string message) :
            base(message)
        {
        }

        public RouteProcessingException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
