using System.Collections.Generic;
using System.Linq;
using Koople.Sdk.Infrastructure;

namespace Koople.Sdk.Evaluator
{
    public abstract class KStore
    {
        public abstract IEnumerable<KFeatureFlag> GetFeatureFlags();

        public abstract Dictionary<string, KRemoteConfig>.ValueCollection GetRemoteConfigs();

        public abstract KSegment FindSegmentByKey(string key);

        public abstract KFeatureFlag GetFeatureFlag(string feature);

        public abstract KRemoteConfig GetRemoteConfig(string remoteConfig);
    }

    public class KInMemoryStore : KStore
    {
        private readonly Dictionary<string, KSegment> _segments;
        private readonly Dictionary<string, KFeatureFlag> _featureFlags;
        private readonly Dictionary<string, KRemoteConfig> _remoteConfigs;

        public KInMemoryStore(IEnumerable<KFeatureFlag> featureFlags, IEnumerable<KRemoteConfig> remoteConfigs,
            IEnumerable<KSegment> segments)
        {
            _segments = segments.ToDictionary(s => s.Key);
            _featureFlags = featureFlags.ToDictionary(ff => ff.Key);
            _remoteConfigs = remoteConfigs.ToDictionary(rc => rc.Key);
        }

        public override IEnumerable<KFeatureFlag> GetFeatureFlags() => _featureFlags.Values;
        public override Dictionary<string, KRemoteConfig>.ValueCollection GetRemoteConfigs() => _remoteConfigs.Values;

        public override KSegment FindSegmentByKey(string key) => _segments[key];
        public override KFeatureFlag GetFeatureFlag(string feature) => _featureFlags.GetValueOrDefault(feature);
        public override KRemoteConfig GetRemoteConfig(string remoteConfig) => _remoteConfigs.GetValueOrDefault(remoteConfig);

        public static KStore FromServer(KServerInitializeResponseDto dto) =>
            new KInMemoryStore(dto.Features, dto.RemoteConfigs, dto.Segments);

        public static KInMemoryStore Empty() => 
            new KInMemoryStore(new KFeatureFlag[] {}, new KRemoteConfig[] {}, new KSegment[] {});
    }
}