﻿// The MIT License (MIT)
//
// Copyright (c) 2015 Rasmus Mikkelsen
// https://github.com/rasmus/EventFlow
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using EventFlow.Extensions;
using EventFlow.Logs;
using EventFlow.Test;
using Owin;

namespace EventFlow.Owin.Tests.IntegrationTests.Site
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var resolver = EventFlowOptions.New
                .AddEvents(EventFlowTest.Assembly)
                //.AddOwinMetadataProviders()
                .CreateResolver(false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterApiControllers(typeof (Startup).Assembly);
            containerBuilder.Register(c => resolver.Resolve<ICommandBus>()).As<ICommandBus>();
            containerBuilder.Register(c => resolver.Resolve<ILog>()).As<ILog>();
            var container = containerBuilder.Build();

            var config = new HttpConfiguration
                {
                    DependencyResolver = new AutofacWebApiDependencyResolver(container),
                };
            config.MapHttpAttributeRoutes();

            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseWebApi(config);
        }
    }
}
