using Nancy;
using Nancy.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    /// <summary>
    /// add static file for views
    /// </summary>
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/views"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/css","/views/css"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/js", "/views/js"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}
