using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PowerWeatherApp.Application;
using PowerWeatherApp.Domain.Interfaces;
using PowerWeatherApp.Infrastructure.Clients;
using PowerWeatherApp.Infrastructure.Persistence.Interface;
using PowerWeatherApp.Infrastructure.Repository;
using Refit;
using Serilog;
using System.Configuration;
using System.Reflection;
using System.Web.Http;

namespace PowerWeatherApp.Api.App_Start
{
    public sealed class AutofacConfig
    {
        public static void RegisterDependencies()
        {
            var apiKey = ConfigurationManager.AppSettings["WeatherAPI:Key"]
                ?? "fa8b3df74d4042b9aa7135114252304";
            var collectionName = ConfigurationManager.AppSettings["MongoDb:CollectionName"]
                ?? "weather";

            var builder = new ContainerBuilder();

            builder.Register(c =>
            {
                var factory = new LoggerFactory();
                factory.AddSerilog(Log.Logger); 
                return factory;
            })
            .As<ILoggerFactory>()
            .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .InstancePerDependency();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.Register(c =>
            {
                var connectionString = ConfigurationManager.AppSettings["MongoDb:ConnectionString"]
                    ?? "mongodb://192.168.88.32:27017";
                var databaseName = ConfigurationManager.AppSettings["MongoDb:DatabaseName"]
                    ?? "power";

                var client = new MongoClient(connectionString);

                return client.GetDatabase(databaseName);
            })
                .As<IMongoDatabase>()
                .SingleInstance();

            builder.Register(c =>
            {
                var baseUri = ConfigurationManager.AppSettings["BaseUri:Uri"]
                ?? "http://api.weatherapi.com/v1";

                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                };

                var refitSettings = new RefitSettings
                {
                    ContentSerializer = new NewtonsoftJsonContentSerializer(jsonSettings)
                };

                return RestService.For<IWeatherApi>(baseUri, refitSettings);
            })
            .As<IWeatherApi>()
            .SingleInstance();

            builder.RegisterType<MongoDbContext>().As<IMongoDbContext>()
                .WithParameter("collectionName", collectionName)
                .InstancePerRequest();

            builder.RegisterType<WeatherRepository>().As<IWeatherRepository>()
                .InstancePerRequest();

            builder.RegisterType<WeatherGateway>().As<IWeatherGateway>()
                .WithParameter("apiKey", apiKey)
                .InstancePerRequest();

            builder.RegisterType<WeatherService>().As<IWeatherService>()
                .InstancePerRequest();
            
            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

    }
    
}