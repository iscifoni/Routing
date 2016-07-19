// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Routing
{
    public class InvalidRouteException : Exception
    {
        public InvalidRouteException(string message) :
            base(message)
        {

        }

        public InvalidRouteException(string message, Exception innerException) :
            base(message, innerException)
        {

        }
    }
}
