// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Mvc.Features;

namespace Mvc.Features
{
    public interface IFeatureService
    {
    }

    public class FeatureConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            controller.Properties.Add("feature", GetFeatureName(controller.ControllerType));
        }

        private string GetFeatureName(TypeInfo controllerType)
        {
            string[] tokens = controllerType.FullName.Split('.');
            if (!tokens.Any(t => t == "Features")) return "";
            string featureName = tokens
                .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
                .Skip(1)
                .Take(1)
                .FirstOrDefault();
            return featureName;
        }
    }

    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations
        )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (viewLocations == null)
            {
                throw new ArgumentNullException(nameof(viewLocations));
            }

            var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                throw new NullReferenceException("ControllerActionDescriptor cannot be null.");
            }

            string featureName = controllerActionDescriptor.Properties["feature"] as string;
            foreach (var location in viewLocations)
            {
                yield return location.Replace("{3}", featureName);
            }
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["action_displayname"] = context.ActionContext.ActionDescriptor.DisplayName;
        }
    }
}
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IMvcBuilder AddFeatureFolder(this IMvcBuilder services)
        {
            services.AddMvcOptions(o => o.Conventions.Add(new FeatureConvention()))
                .AddRazorOptions(opt =>
                {
                    opt.ViewLocationFormats.Clear();
                    opt.ViewLocationFormats.Add("/Features/{3}/{1}/{0}.cshtml");
                    opt.ViewLocationFormats.Add("/Features/{3}/{0}.cshtml");
                    opt.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");
                    opt.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
                });

            foreach (var type in Assembly.GetEntryAssembly().GetTypes().Where(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IFeatureService))))
            {
                services.Services.AddScoped(type);
            }
            return services;
        }
    }
}
