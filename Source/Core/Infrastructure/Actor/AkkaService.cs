using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Routing;
using NLog;

namespace PlayGround.Infrastructure.Actor
{
    /// <summary>
    /// Akka 설정 (appsettings.json)
    /// </summary>
    public class AkkaConfig
    {
        public static readonly string Section = "AkkaConfig";

        public string SystemName { get; set; } = "PlayGroundActorSystem";
        public string? ConfFileName { get; set; }
    }

    /// <summary>
    /// Akka ActorSystem 생명주기 관리 및 액터 생성
    /// </summary>
    public class AkkaService : IHostedService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IServiceProvider ServiceProvider;
        private readonly IConfiguration Configuration;
        private readonly IHostApplicationLifetime ApplicationLifetime;

        public ActorSystem? ActorSystem { get; private set; }
        public ConcurrentDictionary<string, ActorRef> Actors { get; } = new();

        public AkkaService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IHostApplicationLifetime applicationLifetime)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            ApplicationLifetime = applicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var akkaConfig = Configuration.GetSection(AkkaConfig.Section).Get<AkkaConfig>() ?? new AkkaConfig();

            // HOCON 설정 로드
            var config = ConfigurationFactory.Default();
            if (!string.IsNullOrWhiteSpace(akkaConfig.ConfFileName) && File.Exists(akkaConfig.ConfFileName))
            {
                var hocon = await File.ReadAllTextAsync(akkaConfig.ConfFileName, cancellationToken);
                config = ConfigurationFactory.ParseString(hocon);
            }

            // DI 설정
            var bootstrap = BootstrapSetup.Create().WithConfig(config);
            var diSetup = DependencyResolverSetup.Create(ServiceProvider);
            var actorSystemSetup = bootstrap.And(diSetup);

            ActorSystem = ActorSystem.Create(akkaConfig.SystemName, actorSystemSetup);

            ActorSystem.WhenTerminated?.ContinueWith(_ =>
            {
                ApplicationLifetime.StopApplication();
            }, cancellationToken);

            Logger.Info("ActorSystem '{SystemName}' started", akkaConfig.SystemName);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (ActorSystem != null)
            {
                await CoordinatedShutdown.Get(ActorSystem)
                    .Run(CoordinatedShutdown.ClrExitReason.Instance);
                Logger.Info("ActorSystem stopped");
            }
        }

        #region Actor Creation

        /// <summary>
        /// 단일 액터 생성
        /// </summary>
        public ActorRef? CreateActor<TActor>(string actorName, params object[] args)
            where TActor : ActorBase
        {
            Debug.Assert(ActorSystem != null, "ActorSystem is not initialized");

            var props = Props.Create<TActor>(args);
            var actorRef = ActorSystem?.ActorOf(props, actorName);
            if (actorRef == null)
            {
                return null;
            }

            var actor = new ActorRef(actorRef, actorName);
            if (!Actors.TryAdd(actorName, actor))
            {
                Logger.Warn("Actor '{ActorName}' already exists", actorName);
                return null;
            }

            Logger.Debug("Actor '{ActorName}' created", actorName);
            return actor;
        }

        /// <summary>
        /// RoundRobin 라우터 액터 생성
        /// </summary>
        public ActorRef? CreateRouter<TActor>(string routerName, int poolSize, params object[] args)
            where TActor : ActorBase
        {
            Debug.Assert(ActorSystem != null, "ActorSystem is not initialized");

            var props = Props.Create(typeof(TActor), args)
                .WithRouter(new RoundRobinPool(poolSize));

            var actorRef = ActorSystem?.ActorOf(props, routerName);
            if (actorRef == null)
            {
                return null;
            }

            var actor = new ActorRef(actorRef, routerName);
            if (!Actors.TryAdd(routerName, actor))
            {
                Logger.Warn("Router '{RouterName}' already exists", routerName);
                return null;
            }

            Logger.Debug("Router '{RouterName}' created with pool size {PoolSize}", routerName, poolSize);
            return actor;
        }

        /// <summary>
        /// ConsistentHash 라우터 액터 생성
        /// </summary>
        public ActorRef? CreateHashRouter<TActor>(string routerName, int poolSize, params object[] args)
            where TActor : ActorBase
        {
            Debug.Assert(ActorSystem != null, "ActorSystem is not initialized");

            var props = Props.Create(typeof(TActor), args)
                .WithRouter(new ConsistentHashingPool(poolSize));

            var actorRef = ActorSystem?.ActorOf(props, routerName);
            if (actorRef == null)
            {
                return null;
            }

            var actor = new ActorRef(actorRef, routerName);
            if (!Actors.TryAdd(routerName, actor))
            {
                Logger.Warn("HashRouter '{RouterName}' already exists", routerName);
                return null;
            }

            Logger.Debug("HashRouter '{RouterName}' created with pool size {PoolSize}", routerName, poolSize);
            return actor;
        }

        /// <summary>
        /// 이름으로 액터 조회
        /// </summary>
        public ActorRef? GetActor(string actorName)
        {
            if (Actors.TryGetValue(actorName, out var actor))
            {
                return actor;
            }

            return null;
        }

        #endregion
    }
}
