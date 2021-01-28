using System;
using System.Collections;
using System.Collections.Generic;
using JsonSubTypes;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IReadOnlyCollection<Type> KnownActivityTypes = new[]
        {
            typeof(LearnWord), typeof(UnlearnWord)
        };

        public static void AddErabikataJsonSettings(this IMvcBuilder addControllers)
        {
            addControllers.AddNewtonsoftJson(
                options =>
                {
                    var builder = JsonSubtypesConverterBuilder
                        // Not using `nameOf()` here because camelCase
                        .Of<Activity>("activityType")
                        .SerializeDiscriminatorProperty(true);
                    foreach (var activityType in KnownActivityTypes)
                    {
                        builder.RegisterSubtype(activityType, activityType.Name);
                    }

                    options.SerializerSettings.Converters.Add(builder.Build());
                }
            );

            BsonClassMap.RegisterClassMap<Activity>(
                map =>
                {
                    map.AutoMap();
                    map.SetDiscriminator(nameof(Activity.ActivityType));
                    foreach (var activityType in KnownActivityTypes)
                    {
                        map.AddKnownType(activityType);
                    }
                }
            );
        }
    }
}