﻿using System.Net;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Adapters;
using tusdotnet.Extensions;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Validation;

namespace tusdotnet.IntentHandlers
{
    internal abstract class IntentHandler
    {
        internal static IntentHandler NotApplicable { get; } = null;

        internal static Requirement[] NoRequirements = new Requirement[0];

        internal LockType LockType { get; }

        internal IntentType Intent { get; }

        internal abstract Requirement[] Requires { get; }

        protected ContextAdapter Context { get; }

        protected RequestAdapter Request { get; }

        protected ResponseAdapter Response { get; }

        protected CancellationToken CancellationToken { get; }

        protected ITusStore Store { get; }

        internal abstract Task Invoke();

        protected IntentHandler(ContextAdapter context, IntentType intent, LockType requiresLock)
        {
            Context = context;
            Request = context.Request;
            Response = context.Response;
            CancellationToken = context.CancellationToken;
            Store = context.Configuration.Store;

            Intent = intent;
            LockType = requiresLock;
        }

        internal async Task<bool> Validate(ContextAdapter context)
        {
            var validator = new Validator(Requires);

            validator.Validate(context);

            if (validator.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            if (validator.StatusCode == HttpStatusCode.NotFound)
            {
                context.Response.NotFound();
                return false;
            }

            await context.Response.Error(validator.StatusCode, validator.ErrorMessage);
            return false;
        }
    }
}
