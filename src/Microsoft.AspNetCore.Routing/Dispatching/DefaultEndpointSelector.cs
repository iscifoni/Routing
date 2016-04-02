using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class DefaultEndpointSelector : IEndpointSelector
    {
        private readonly IReadOnlyList<EndpointDescriptor> _endpoints;

        public DefaultEndpointSelector(IReadOnlyList<EndpointDescriptor> endpoints)
        {
            _endpoints = endpoints;
        }

        /// <inheritdoc />
        public EndpointDescriptor Select(HttpContext httpContext, RouteData routeData)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (routeData == null)
            {
                throw new ArgumentNullException(nameof(routeData));
            }

            var matches = GetMatches(httpContext, routeData, _endpoints, startingOrder: null);
            if (matches == null || matches.Count == 0)
            {
                return null;
            }
            else if (matches.Count == 1)
            {
                var selectedAction = matches[0];

                return selectedAction;
            }
            else
            {
                var actionNames = string.Join(
                    Environment.NewLine,
                    matches.Select(a => a.ToString()));

                throw new InvalidOperationException($"Ambiguious bro! {actionNames}");
            }
        }

        private IReadOnlyList<EndpointDescriptor> GetMatches(
            HttpContext httpContext,
            RouteData routeData,
            IReadOnlyList<EndpointDescriptor> candidates,
            int? startingOrder)
        {
            // Find the next group of constraints to process. This will be the lowest value of
            // order that is higher than startingOrder.
            int? order = null;

            // Perf: Avoid allocations
            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                if (candidate.Constraints != null)
                {
                    for (var j = 0; j < candidate.Constraints.Count; j++)
                    {
                        var constraint = candidate.Constraints[j];
                        if ((startingOrder == null || constraint.Order > startingOrder) &&
                            (order == null || constraint.Order < order))
                        {
                            order = constraint.Order;
                        }
                    }
                }
            }

            // If we don't find a 'next' then there's nothing left to do.
            if (order == null)
            {
                return candidates;
            }

            // Since we have a constraint to process, bisect the set of actions into those with and without a
            // constraint for the 'current order'.
            var actionsWithConstraint = new List<EndpointDescriptor>();
            var actionsWithoutConstraint = new List<EndpointDescriptor>();

            var constraintContext = new EndpointSelectorContext();
            constraintContext.Candidates = candidates;
            constraintContext.HttpContext = httpContext;
            constraintContext.RouteData = routeData;

            // Perf: Avoid allocations
            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                var isMatch = true;
                var foundMatchingConstraint = false;

                if (candidate.Constraints != null)
                {
                    constraintContext.CurrentCandidate = candidate;
                    for (var j = 0; j < candidate.Constraints.Count; j++)
                    {
                        var constraint = candidate.Constraints[j];
                        if (constraint.Order == order)
                        {
                            foundMatchingConstraint = true;

                            if (!constraint.Accept(constraintContext))
                            {
                                isMatch = false;
                                break;
                            }
                        }
                    }
                }

                if (isMatch && foundMatchingConstraint)
                {
                    actionsWithConstraint.Add(candidate);
                }
                else if (isMatch)
                {
                    actionsWithoutConstraint.Add(candidate);
                }
            }

            // If we have matches with constraints, those are 'better' so try to keep processing those
            if (actionsWithConstraint.Count > 0)
            {
                var matches = GetMatches(httpContext, routeData, actionsWithConstraint, order);
                if (matches?.Count > 0)
                {
                    return matches;
                }
            }

            // If the set of matches with constraints can't work, then process the set without constraints.
            if (actionsWithoutConstraint.Count == 0)
            {
                return null;
            }
            else
            {
                return GetMatches(httpContext, routeData, actionsWithoutConstraint, order);
            }
        }

    }
}
