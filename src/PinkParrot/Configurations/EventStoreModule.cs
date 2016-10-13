﻿// ==========================================================================
//  EventStoreModule.cs
//  PinkParrot Headless CMS
// ==========================================================================
//  Copyright (c) PinkParrot Group
//  All rights reserved.
// ==========================================================================

using System.Net;
using Autofac;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Options;
using PinkParrot.Infrastructure.CQRS.EventStore;

namespace PinkParrot.Configurations
{
    public class EventStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var options = context.Resolve<IOptions<EventStoreOptions>>().Value;

                var eventStore =
                    EventStoreConnection.Create(
                       ConnectionSettings.Create()
                           .UseConsoleLogger()
                           .UseDebugLogger()
                           .KeepReconnecting()
                           .KeepRetrying(),
                       new IPEndPoint(IPAddress.Parse(options.IPAddress), options.Port));

                eventStore.ConnectAsync().Wait();

                return eventStore;
            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<IOptions<EventStoreOptions>>().Value;

                return new UserCredentials(options.Username, options.Password);
            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<IOptions<EventStoreOptions>>().Value;

                return new DefaultNameResolver(options.Prefix);
            }).SingleInstance();
        }
    }
}
