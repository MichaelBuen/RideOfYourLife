using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using ServiceStack.Configuration;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface.Cors;
using Dto;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Marshaller.App_Start.AppHost), "Start")]


/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace Marshaller.App_Start
{
	public class AppHost
		: AppHostBase
	{		
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("StarterTemplate ASP.NET Host", typeof(HelloService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
		
			//Configure User Defined REST Paths
			Routes
			  .Add<Hello>("/hello")
			  .Add<Hello>("/hello/{Name*}");

            Routes
                .Add<LanguageDto>("/language")
                .Add<LanguageDto>("/language", ApplyTo.Post);

		    Routes
		        .Add<TitleDtos>("/titles")
		        .Add<TitleDtos>("/titles/{Ids}/{LanguageCultureCode}");

            
		    Routes
		        .Add<NationalityDtos>("/nationalities")
		        .Add<NationalityDtos>("/nationalities/{Ids}/{LanguageCultureCode}");

		    Routes
		        .Add<AgeBracketDtos>("/age-brackets")
		        .Add<AgeBracketDtos>("/age-brackets/{Ids}/{LanguageCultureCode}");


		    Routes
		        .Add<ReservationResourceDto>("/reservation-resource");

		    Routes		        
		        .Add<ReservationViewDto>("/reservation/{ReservationId}/{LanguageCultureCode}")
                .Add<ReservationPersistDto>("/reservation")
		        .Add<ReservationPersistDto>("/reservation", ApplyTo.Post);




		    container.Register<NHibernate.ISession>(x => DomainMapping.SessionMapper.Mapper.SessionFactory.OpenSession()).ReusedWithin(Funq.ReuseScope.None);

            //// http://stackoverflow.com/questions/14903101/servicestack-returns-405-on-options-request
            //this.RequestFilters.Add((httpReq, httpRes, requestDto) =>
            //{
            //    httpRes.AddHeader("Access-Control-Allow-Origin", "http://localhost:52013");


            //    httpRes.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
            //    httpRes.AddHeader("Access-Control-Allow-Headers", "X-Requested-With, Content-Type: application/json; charset=utf-8");
            //    httpRes.AddHeader("Access-Control-Allow-Credentials", "true");



            //});


            //var originWhitelist = new[] { "http://localhost:52013" };

            //this.PreRequestFilters.Add((httpReq, httpRes) =>
            //{
            //    var origin = httpReq.Headers.Get("Origin");
            //    if (originWhitelist.Contains(origin))
            //    {                    
            //        httpRes.AddHeader(ServiceStack.Common.Web.HttpHeaders.AllowOrigin, origin);
            //    }
            //    httpRes.AddHeader(ServiceStack.Common.Web.HttpHeaders.AllowMethods, "GET, POST, PUT, OPTIONS");
            //    httpRes.AddHeader(ServiceStack.Common.Web.HttpHeaders.AllowHeaders, "X-Requested-With, Content-Type");
            //    httpRes.AddHeader(ServiceStack.Common.Web.HttpHeaders.AllowCredentials, "true");
            //});

            // http://blog.alistairrobinson.com/cross-domain-xhr-cors-and-internet-explorer/
            //ALLOWED_ORIGINS = 'http://mybooksandmovies.com'
            //XS_SHARING_ALLOWED_METHODS = ['POST', 'GET', 'OPTIONS', 'PUT', 'DELETE']
            //XS_SHARING_ALLOWED_HEADERS = ['Authorization', 'Content-Type', 'X-Requested-With', '*']
            //XS_SHARING_ALLOWED_CREDENTIALS = 'true'

            SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders = new Dictionary<string, string>()
                    {
                            { "Access-Control-Allow-Origin", "http://localhost:52013" },
                            { "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" } ,
                             { "Access-Control-Allow-Headers", "Cache-Control, Pragma, Origin, Authorization, Content-Type: application/json; charset=utf-8, X-Requested-With" }, // http://stackoverflow.com/questions/19343316/angularjs-and-cross-domain-post
                          //   { "Access-Control-Expose-Headers", "X-My-Custom-Header, X-Another-Custom-Header" }, // delete
                            { "Access-Control-Allow-Credentials", "true" }
                    }
            });


            RequestFilters.Add((httpReq, httpRes, requestDto) =>
            {
                var originWhitelist = new[] { "http://localhost:52013" };
                var origin = httpReq.Headers.Get("Origin");
          

                if (httpReq.HttpMethod == "OPTIONS")
                {
                    if (true || !originWhitelist.Contains(origin))
                    {
                        httpRes.AddHeader(ServiceStack.Common.Web.HttpHeaders.AllowOrigin, "http://localhost:52013");
                    }

                    httpRes.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    //httpRes.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                    httpRes.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept"); // http://stackoverflow.com/questions/12409600/error-request-header-field-content-type-is-not-allowed-by-access-control-allow/18192705#18192705
                    httpRes.AddHeader("Access-Control-Allow-Credentials", "true");
                    httpRes.End();
                }
                else
                {
                    // httpRes.AddHeader("Access-Control-Allow-Origin", "http://localhost:52013");
                }
            });

            //this.Plugins.RemoveAll(x => true);



            //this.Plugins.Add(
            //        new CorsFeature(allowedOrigins: "http://localhost:52013",
            //        //new CorsFeature(allowedOrigins: "http://localhost:52013/",
            //        allowedMethods: "OPTIONS, TRACE, GET, HEAD, POST, PUT, DELETE",
            //        allowedHeaders: "Content-Type, Authorization, Accept, X-Requested-With",
            //        allowCredentials: true));
            

			//Uncomment to change the default ServiceStack configuration
			//SetConfig(new EndpointHostConfig {
			//});

			//Enable Authentication
			//ConfigureAuth(container);

			//Register all your dependencies
			container.Register(new TodoRepository());			
		}

		/* Uncomment to enable ServiceStack Authentication and CustomUserSession
		private void ConfigureAuth(Funq.Container container)
		{
			var appSettings = new AppSettings();

			//Default route: /auth/{provider}
			Plugins.Add(new AuthFeature(() => new CustomUserSession(),
				new IAuthProvider[] {
					new CredentialsAuthProvider(appSettings), 
					new FacebookAuthProvider(appSettings), 
					new TwitterAuthProvider(appSettings), 
					new BasicAuthProvider(appSettings), 
				})); 

			//Default route: /register
			Plugins.Add(new RegistrationFeature()); 

			//Requires ConnectionString configured in Web.Config
			var connectionString = ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString;
			container.Register<IDbConnectionFactory>(c =>
				new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider));

			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.CreateMissingTables();
		}
		*/

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}
