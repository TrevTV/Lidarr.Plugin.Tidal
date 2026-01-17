// class for parsing MPEG-DASH MPD XMLs used by Tidal
// based on https://github.com/sangwonl/python-mpegdash/

using System.Xml;

namespace TidalSharp.Downloading;

internal interface IMPDNode
{
    void Parse(XmlNode node);
}

internal class MPD : IMPDNode
{
    public static MPD Parse(string xml)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        return xmlDoc.DocumentElement!.NodeToMPDType<MPD>();
    }

    public string? XMLNS { get; set; }
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Profiles { get; set; }
    public string? CENC { get; set; }
    public DateTime? AvailabilityStartTime { get; set; }
    public DateTime? AvailabilityEndTime { get; set; }
    public DateTime? PublishTime { get; set; }
    public TimeSpan? MediaPresentationDuration { get; set; }
    public TimeSpan? MinimumUpdatePeriod { get; set; }
    public TimeSpan? MinimumBufferTime { get; set; }
    public TimeSpan? TimeShiftBufferDepth { get; set; }
    public TimeSpan? SuggestedPresentationDelay { get; set; }
    public TimeSpan? MaximumSegmentDuration { get; set; }
    public TimeSpan? MaximumSubsegmentDuration { get; set; }

    public ProgramInformation[]? ProgramInformations { get; set; }
    public BaseUrl[]? BaseUrls { get; set; }
    public string[]? Locations { get; set; }
    public Period[]? Periods { get; set; }
    public Metrics[]? Metrics { get; set; }
    public Descriptor[]? UTCTimings { get; set; }

    public void Parse(XmlNode node)
    {
        XMLNS = node.GetAttributeValue<string>("xmlns");
        Id = node.GetAttributeValue<string>("id");
        Type = node.GetAttributeValue<string>("type");
        Profiles = node.GetAttributeValue<string>("profiles");
        CENC = node.GetAttributeValue<string>("xmlns:cenc");

        AvailabilityStartTime = node.GetAttributeValue<DateTime>("availabilityStartTime");
        AvailabilityEndTime = node.GetAttributeValue<DateTime>("availabilityEndTime");
        PublishTime = node.GetAttributeValue<DateTime>("publishTime");

        MediaPresentationDuration = node.GetAttributeValue<TimeSpan>("mediaPresentationDuration");
        MinimumUpdatePeriod = node.GetAttributeValue<TimeSpan>("minimumUpdatePeriod");
        MinimumBufferTime = node.GetAttributeValue<TimeSpan>("minBufferTime");
        TimeShiftBufferDepth = node.GetAttributeValue<TimeSpan>("timeShiftBufferDepth");
        SuggestedPresentationDelay = node.GetAttributeValue<TimeSpan>("suggestedPresentationDelay");
        MaximumSegmentDuration = node.GetAttributeValue<TimeSpan>("maxSegmentDuration");
        MaximumSubsegmentDuration = node.GetAttributeValue<TimeSpan>("maxSubsegmentDuration");

        ProgramInformations = node.GetChildrenOfType<ProgramInformation>("ProgramInformation");
        BaseUrls = node.GetChildrenOfType<BaseUrl>("BaseURL");
        Locations = node.ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Location").Select(n => n.InnerText).ToArray();
        Periods = node.GetChildrenOfType<Period>("Period");
        Metrics = node.GetChildrenOfType<Metrics>("Metrics");
        UTCTimings = node.GetChildrenOfType<Descriptor>("UTCTiming");
    }
}

internal class ProgramInformation : IMPDNode
{
    public string? Language { get; set; }
    public string? MoreInformationURL { get; set; }

    public string[]? Titles { get; set; }
    public string[]? Sources { get; set; }
    public string[]? Copyrights { get; set; }

    public void Parse(XmlNode node)
    {
        Language = node.GetAttributeValue<string>("lang");
        MoreInformationURL = node.GetAttributeValue<string>("moreInformationURL");

        Titles = node.ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Title").Select(n => n.InnerText).ToArray();
        Sources = node.ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Source").Select(n => n.InnerText).ToArray();
        Copyrights = node.ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Copyright").Select(n => n.InnerText).ToArray();
    }
}

internal class BaseUrl : IMPDNode
{
    public string? BaseUrlValue { get; set; }

    public string? ServiceLocation { get; set; }
    public string? ByteRange { get; set; }
    public double? AvailabilityTimeOffset { get; set; }
    public bool? AvailabilityTimeComplete { get; set; }

    public void Parse(XmlNode node)
    {
        BaseUrlValue = node.Value;

        ServiceLocation = node.GetAttributeValue<string>("serviceLocation");
        ByteRange = node.GetAttributeValue<string>("byteRange");
        AvailabilityTimeOffset = node.GetAttributeValue<double>("availabilityTimeOffset");
        AvailabilityTimeComplete = node.GetAttributeValue<bool>("availabilityTimeComplete");
    }
}

internal class Period : IMPDNode
{
    public string? Id { get; set; }
    public TimeSpan? Start { get; set; }
    public TimeSpan? Duration { get; set; }
    public bool? BitstreamSwitching { get; set; }

    public BaseUrl[]? BaseUrls { get; set; }
    public SegmentBase[]? SegmentBases { get; set; }
    public SegmentList[]? SegmentLists { get; set; }
    public SegmentTemplate[]? SegmentTemplates { get; set; }
    public Descriptor[]? AssetIdentifiers { get; set; }
    public EventStream[]? EventStreams { get; set; }
    public AdaptationSet[]? AdaptationSets { get; set; }
    public Subset[]? Subsets { get; set; }

    public void Parse(XmlNode node)
    {
        Id = node.GetAttributeValue<string>("id");

        Start = node.GetAttributeValue<TimeSpan>("start");
        Duration = node.GetAttributeValue<TimeSpan>("duration");
        BitstreamSwitching = node.GetAttributeValue<bool>("bitstreamSwitching");

        BaseUrls = node.GetChildrenOfType<BaseUrl>("BaseURL");
        SegmentBases = node.GetChildrenOfType<SegmentBase>("SegmentBase");
        SegmentLists = node.GetChildrenOfType<SegmentList>("SegmentList");
        SegmentTemplates = node.GetChildrenOfType<SegmentTemplate>("SegmentTemplate");
        AssetIdentifiers = node.GetChildrenOfType<Descriptor>("Descriptor");
        EventStreams = node.GetChildrenOfType<EventStream>("EventStream");
        AdaptationSets = node.GetChildrenOfType<AdaptationSet>("AdaptationSet");
        Subsets = node.GetChildrenOfType<Subset>("Subset");
    }
}

internal class Metrics : IMPDNode
{
    // 'metrics' is required
    public string MetricsData { get; set; } = "";

    public Descriptor[]? Reportings { get; set; }
    public Range[]? Ranges { get; set; }

    public void Parse(XmlNode node)
    {
        MetricsData = node.GetAttributeValue<string>("metrics")!;

        Reportings = node.GetChildrenOfType<Descriptor>("Reporting");
        Ranges = node.GetChildrenOfType<Range>("Range");
    }
}

internal class Range : IMPDNode
{
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? Duration { get; set; }

    public void Parse(XmlNode node)
    {
        StartTime = node.GetAttributeValue<TimeSpan>("starttime");
        Duration = node.GetAttributeValue<TimeSpan>("duration");
    }
}

internal class Descriptor : IMPDNode
{
    // 'schemeIdUri' is required
    public string SchemeIdUri { get; set; } = "";
    public string? Value { get; set; }
    public string? Id { get; set; }

    public void Parse(XmlNode node)
    {
        SchemeIdUri = node.GetAttributeValue<string>("schemeIdUri")!;
        Value = node.GetAttributeValue<string>("value");
        Id = node.GetAttributeValue<string>("id");
    }
}

internal class SegmentBase : IMPDNode
{
    public uint? TimeScale { get; set; }
    public string? IndexRange { get; set; }
    public bool? IndexRangeExact { get; set; }
    public ulong? PresentationTimeOffset { get; set; }
    public double? AvailabilityTimeOffset { get; set; }
    public bool? AvailableTimeComplete { get; set; }

    public Url[]? Initializations { get; set; }
    public Url[]? RepresentationIndexes { get; set; }

    public virtual void Parse(XmlNode node)
    {
        TimeScale = node.GetAttributeValue<uint>("timescale");
        IndexRange = node.GetAttributeValue<string>("indexRange");
        IndexRangeExact = node.GetAttributeValue<bool>("indexRangeExact");
        PresentationTimeOffset = node.GetAttributeValue<ulong>("presentationTimeOffset");
        AvailabilityTimeOffset = node.GetAttributeValue<double>("availabilityTimeOffset");
        AvailableTimeComplete = node.GetAttributeValue<bool>("availabilityTimeComplete");

        Initializations = node.GetChildrenOfType<Url>("Initialization");
        RepresentationIndexes = node.GetChildrenOfType<Url>("RepresentationIndex");
    }
}

internal class MultipleSegmentBase : SegmentBase
{
    public uint? Duration { get; set; }
    public uint? StartNumber { get; set; }
    public uint? EndNumber { get; set; }

    public SegmentTimeline[]? SegmentTimelines { get; set; }
    public Url[]? BitstreamSwitchings { get; set; }

    public override void Parse(XmlNode node)
    {
        base.Parse(node);

        Duration = node.GetAttributeValue<uint>("duration");
        StartNumber = node.GetAttributeValue<uint>("startNumber");
        EndNumber = node.GetAttributeValue<uint>("endNumber");

        SegmentTimelines = node.GetChildrenOfType<SegmentTimeline>("SegmentTimeline");
        RepresentationIndexes = node.GetChildrenOfType<Url>("BitstreamSwitching");
    }
}

internal class SegmentList : MultipleSegmentBase
{
    public SegmentUrl[]? SegmentUrls { get; set; }

    public override void Parse(XmlNode node)
    {
        base.Parse(node);

        SegmentUrls = node.GetChildrenOfType<SegmentUrl>("SegmentURL");
    }
}

internal class SegmentTemplate : MultipleSegmentBase
{
    public string? Media { get; set; }
    public string? Index { get; set; }
    public string? Initialization { get; set; }
    public string? BitstreamSwitching { get; set; }

    public override void Parse(XmlNode node)
    {
        base.Parse(node);

        Media = node.GetAttributeValue<string>("media");
        Index = node.GetAttributeValue<string>("index");
        Initialization = node.GetAttributeValue<string>("initialization");
        BitstreamSwitching = node.GetAttributeValue<string>("bitstreamSwitching");
    }
}

internal class Url : IMPDNode
{
    public string? SourceUrl { get; set; }
    public string? Range { get; set; }

    public void Parse(XmlNode node)
    {
        SourceUrl = node.GetAttributeValue<string>("sourceURL");
        Range = node.GetAttributeValue<string>("range");
    }
}

internal class SegmentUrl : IMPDNode
{
    public string? Media { get; set; }
    public string? MediaRange { get; set; }
    public string? Index { get; set; }
    public string? IndexRange { get; set; }

    public void Parse(XmlNode node)
    {
        Media = node.GetAttributeValue<string>("media");
        MediaRange = node.GetAttributeValue<string>("mediaRange");
        Index = node.GetAttributeValue<string>("index");
        IndexRange = node.GetAttributeValue<string>("indexRange");
    }
}

internal class SegmentTimeline : IMPDNode
{
    public S[]? Ss { get; set; }

    public void Parse(XmlNode node)
    {
        Ss = node.GetChildrenOfType<S>("S");
    }
}

internal class S : IMPDNode
{
    public ulong? T { get; set; }
    public ulong D { get; set; } = 0; // 'd' is required
    public int? R { get; set; }


    public void Parse(XmlNode node)
    {
        T = node.GetAttributeValue<ulong>("t");
        D = node.GetAttributeValue<ulong>("d")!.Value;
        R = node.GetAttributeValue<int>("r");
    }
}

internal class EventStream : IMPDNode
{
    public string SchemeIdUri { get; set; } = "";
    public string? Value { get; set; }
    public uint? Timescale { get; set; }

    public Event[]? Events { get; set; }

    public void Parse(XmlNode node)
    {
        SchemeIdUri = node.GetAttributeValue<string>("schemeIdUri")!;
        Value = node.GetAttributeValue<string>("value");
        Timescale = node.GetAttributeValue<uint>("timescale");

        Events = node.GetChildrenOfType<Event>("Event");
    }
}

internal class Event : IMPDNode
{
    public string? EventValue { get; set; }
    public string? MessageData { get; set; }
    public ulong? PresentationTime { get; set; }
    public ulong? Duration { get; set; }
    public uint? Id { get; set; }

    public void Parse(XmlNode node)
    {
        EventValue = node.Value;
        MessageData = node.GetAttributeValue<string>("messageData");
        PresentationTime = node.GetAttributeValue<ulong>("presentationTime");
        Duration = node.GetAttributeValue<ulong>("duration");
        Id = node.GetAttributeValue<uint>("id");
    }
}

internal class RepresentationBase : IMPDNode
{
    public string? Profile { get; set; }
    public string? Profiles { get; set; }
    public uint? Width { get; set; }
    public uint? Height { get; set; }
    public string? Sar { get; set; }
    public string? FrameRate { get; set; }
    public string? AudioSamplingRate { get; set; }
    public string? MimeType { get; set; }
    public string? SegmentProfiles { get; set; }
    public string? Codecs { get; set; }
    public double? MaximumSapPeriod { get; set; }
    public int? StartWithSap { get; set; }
    public double? MaxPlayoutRate { get; set; }
    public bool? CodingDependency { get; set; }
    public string? ScanType { get; set; }

    public Descriptor[]? FramePackings { get; set; }
    public Descriptor[]? AudioChannelConfigurations { get; set; }
    public ContentProtection[]? ContentProtections { get; set; }
    public Descriptor[]? EssentialProperties { get; set; }
    public Descriptor[]? SupplementalProperties { get; set; }
    public Descriptor[]? InbandEventStreams { get; set; }

    public virtual void Parse(XmlNode node)
    {
        Profile = node.GetAttributeValue<string>("profile");
        Profiles = node.GetAttributeValue<string>("profiles");
        Width = node.GetAttributeValue<uint>("width");
        Height = node.GetAttributeValue<uint>("height");
        Sar = node.GetAttributeValue<string>("sar");
        FrameRate = node.GetAttributeValue<string>("frameRate");
        AudioSamplingRate = node.GetAttributeValue<string>("audioSamplingRate");
        MimeType = node.GetAttributeValue<string>("mimeType");
        SegmentProfiles = node.GetAttributeValue<string>("segmentProfiles");
        Codecs = node.GetAttributeValue<string>("codecs");
        MaximumSapPeriod = node.GetAttributeValue<double>("maximumSAPPeriod");
        StartWithSap = node.GetAttributeValue<int>("startWithSAP");
        MaxPlayoutRate = node.GetAttributeValue<double>("maxPlayoutRate");
        CodingDependency = node.GetAttributeValue<bool>("codingDependency");
        ScanType = node.GetAttributeValue<string>("scanType");

        FramePackings = node.GetChildrenOfType<Descriptor>("FramePacking");
        AudioChannelConfigurations = node.GetChildrenOfType<Descriptor>("AudioChannelConfiguration");
        ContentProtections = node.GetChildrenOfType<ContentProtection>("ContentProtection");
        EssentialProperties = node.GetChildrenOfType<Descriptor>("EssentialProperty");
        SupplementalProperties = node.GetChildrenOfType<Descriptor>("SupplementalProperty");
        InbandEventStreams = node.GetChildrenOfType<Descriptor>("InbandEventStream");
    }
}

internal class SubRepresentation : RepresentationBase
{
    public uint? Level { get; set; }
    public uint? Bandwidth { get; set; }
    public uint[]? DependencyLevel { get; set; }
    public string[]? ContentComponent { get; set; }

    public override void Parse(XmlNode node)
    {
        base.Parse(node);

        Level = node.GetAttributeValue<uint>("level");
        Bandwidth = node.GetAttributeValue<uint>("bandwidth");
        /*DependencyLevel = node.GetAttributeValue<uint[]>("dependencyLevel");*/  // i haven't seen any examples of this so im not too sure how to read it into an array
        ContentComponent = node.GetAttributeValue<string[]>("contentComponent");
    }
}

internal class Representation : RepresentationBase
{
    public string? Id { get; set; }
    public uint Bandwidth { get; set; }
    public uint? QualityRanking { get; set; }
    public string[]? DependencyId { get; set; }
    public uint? NumChannels { get; set; }
    public ulong? SampleRate { get; set; }

    public BaseUrl[]? BaseUrls { get; set; }
    public SegmentBase[]? SegmentBases { get; set; }
    public SegmentList[]? SegmentLists { get; set; }
    public SegmentTemplate[]? SegmentTemplates { get; set; }
    public SubRepresentation[]? SubRepresentations { get; set; }

    public override void Parse(XmlNode node)
    {
        base.Parse(node);

        Id = node.GetAttributeValue<string>("id");
        Bandwidth = node.GetAttributeValue<uint>("bandwidth")!.Value;
        QualityRanking = node.GetAttributeValue<uint>("qualityRanking");
        DependencyId = node.GetAttributeValue<string[]>("dependencyId");
        NumChannels = node.GetAttributeValue<uint>("numChannels");
        SampleRate = node.GetAttributeValue<ulong>("sampleRate");

        BaseUrls = node.GetChildrenOfType<BaseUrl>("BaseURL");
        SegmentBases = node.GetChildrenOfType<SegmentBase>("SegmentBase");
        SegmentLists = node.GetChildrenOfType<SegmentList>("SegmentList");
        SegmentTemplates = node.GetChildrenOfType<SegmentTemplate>("SegmentTemplate");
        SubRepresentations = node.GetChildrenOfType<SubRepresentation>("SubRepresentation");
    }
}

internal class AdaptationSet : RepresentationBase
{
    public uint? Id { get; set; }
    public string? Group { get; set; }
    public string? Lang { get; set; }
    public string? Label { get; set; }
    public string? ContentType { get; set; }
    public string? Par { get; set; }
    public uint? MinBandwidth { get; set; }
    public uint? MaxBandwidth { get; set; }
    public uint? MinWidth { get; set; }
    public uint? MaxWidth { get; set; }
    public uint? MinHeight { get; set; }
    public uint? MaxHeight { get; set; }
    public string? MinFrameRate { get; set; }
    public string? MaxFrameRate { get; set; }
    public bool? SegmentAlignment { get; set; }
    public uint? SelectionPriority { get; set; }
    public bool? SubsegmentAlignment { get; set; }
    public int? SubsegmentStartsWithSap { get; set; }
    public bool? BitstreamSwitching { get; set; }

    public Descriptor[]? Accessibilities { get; set; }
    public Descriptor[]? Roles { get; set; }
    public Descriptor[]? Ratings { get; set; }
    public Descriptor[]? Viewpoints { get; set; }
    public ContentComponent[]? ContentComponents { get; set; }
    public BaseUrl[]? BaseUrls { get; set; }
    public SegmentBase[]? SegmentBases { get; set; }
    public SegmentList[]? SegmentLists { get; set; }
    public SegmentTemplate[]? SegmentTemplates { get; set; }
    public Representation[]? Representations { get; set; }

    public override void Parse(XmlNode node)
    {
        base.Parse(node);

        Id = node.GetAttributeValue<uint>("id");
        Group = node.GetAttributeValue<string>("group");
        Lang = node.GetAttributeValue<string>("lang");
        Label = node.GetAttributeValue<string>("label");
        ContentType = node.GetAttributeValue<string>("contentType");
        Par = node.GetAttributeValue<string>("par");
        MinBandwidth = node.GetAttributeValue<uint>("minBandwidth");
        MaxBandwidth = node.GetAttributeValue<uint>("maxBandwidth");
        MinWidth = node.GetAttributeValue<uint>("minWidth");
        MaxWidth = node.GetAttributeValue<uint>("maxWidth");
        MinHeight = node.GetAttributeValue<uint>("minHeight");
        MaxHeight = node.GetAttributeValue<uint>("maxHeight");
        MinFrameRate = node.GetAttributeValue<string>("minFrameRate");
        MaxFrameRate = node.GetAttributeValue<string>("maxFrameRate");
        SegmentAlignment = node.GetAttributeValue<bool>("segmentAlignment");
        SelectionPriority = node.GetAttributeValue<uint>("selectionPriority");
        SubsegmentAlignment = node.GetAttributeValue<bool>("subsegmentAlignment");
        SubsegmentStartsWithSap = node.GetAttributeValue<int>("subsegmentStartsWithSAP");
        BitstreamSwitching = node.GetAttributeValue<bool>("bitstreamSwitching");

        Accessibilities = node.GetChildrenOfType<Descriptor>("Accessibility");
        Roles = node.GetChildrenOfType<Descriptor>("Role");
        Ratings = node.GetChildrenOfType<Descriptor>("Rating");
        Viewpoints = node.GetChildrenOfType<Descriptor>("Viewpoint");
        ContentComponents = node.GetChildrenOfType<ContentComponent>("ContentComponent");
        BaseUrls = node.GetChildrenOfType<BaseUrl>("BaseURL");
        SegmentBases = node.GetChildrenOfType<SegmentBase>("SegmentBase");
        SegmentLists = node.GetChildrenOfType<SegmentList>("SegmentList");
        SegmentTemplates = node.GetChildrenOfType<SegmentTemplate>("SegmentTemplate");
        Representations = node.GetChildrenOfType<Representation>("Representation");
    }
}

internal class ContentProtection : IMPDNode
{
    public string SchemeIdUri { get; set; } = "";
    public string? Value { get; set; }
    public string? Id { get; set; }
    public string? DefaultKeyId { get; set; }
    public string? Ns2KeyId { get; set; }
    public string? CENCDefaultKeyId { get; set; }

    public PSSH[]? PSSHs { get; set; }

    public void Parse(XmlNode node)
    {
        SchemeIdUri = node.GetAttributeValue<string>("schemeIdUri")!;
        Value = node.GetAttributeValue<string>("value");
        Id = node.GetAttributeValue<string>("id");
        DefaultKeyId = node.GetAttributeValue<string>("default_KID");
        Ns2KeyId = node.GetAttributeValue<string>("ns2:default_KID");
        CENCDefaultKeyId = node.GetAttributeValue<string>("cenc:default_KID");

        PSSHs = node.GetChildrenOfType<PSSH>("cenc:pssh");
    }
}

internal class Subset : IMPDNode
{
    public string? Id { get; set; }
    public uint[]? Contains { get; set; }

    public void Parse(XmlNode node)
    {
        Id = node.GetAttributeValue<string>("id");
        /*Contains = node.GetAttributeValue<uint[]>("contains")!;*/ // i haven't seen any examples of this so im not too sure how to read it into an array
    }
}

internal class ContentComponent : IMPDNode
{
    public uint? Id { get; set; }
    public string? Lang { get; set; }
    public string? ContentType { get; set; }
    public string? Par { get; set; }

    public Descriptor[]? Accessibilities { get; set; }
    public Descriptor[]? Roles { get; set; }
    public Descriptor[]? Ratings { get; set; }
    public Descriptor[]? Viewpoints { get; set; }

    public void Parse(XmlNode node)
    {
        Id = node.GetAttributeValue<uint>("id");
        Lang = node.GetAttributeValue<string>("lang");
        ContentType = node.GetAttributeValue<string>("contentType");
        Par = node.GetAttributeValue<string>("par");

        Accessibilities = node.GetChildrenOfType<Descriptor>("Accessibility");
        Roles = node.GetChildrenOfType<Descriptor>("Role");
        Ratings = node.GetChildrenOfType<Descriptor>("Rating");
        Viewpoints = node.GetChildrenOfType<Descriptor>("Viewpoint");
    }
}

internal class PSSH : IMPDNode
{
    public string? Pssh { get; set; }

    public void Parse(XmlNode node)
    {
        Pssh = node.Value;
    }
}
