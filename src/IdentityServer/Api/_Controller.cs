// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Api
{
    public class _Controller : Controller
    {
        public _Controller(
            ILogger logger
        ){
            Logger = logger;
        }

        protected ILogger Logger { get; }

        protected void Audit(int action, params object[] fields)
        {
            string msg = string.Join(' ', fields.Select(o => o.ToString()).ToArray());

            Logger.LogWarning(action, "{0} {1} {2}", DateTime.UtcNow, User.GetSubjectId(), msg);
        }
    }
}
