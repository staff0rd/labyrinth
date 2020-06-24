namespace Events.TeamsWeb
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Overview
    {
        [JsonProperty("teams")]
        public Team[] Teams { get; set; }

        [JsonProperty("chats")]
        public Chat[] Chats { get; set; }

        [JsonProperty("users")]
        public object[] Users { get; set; }

        [JsonProperty("privateFeeds")]
        public PrivateFeed[] PrivateFeeds { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public partial class Chat
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("consumptionHorizon")]
        public ConsumptionHorizon ConsumptionHorizon { get; set; }

        [JsonProperty("retentionHorizon")]
        public object RetentionHorizon { get; set; }

        [JsonProperty("retentionHorizonV2")]
        public object RetentionHorizonV2 { get; set; }

        [JsonProperty("members")]
        public Member[] Members { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("threadVersion")]
        public long ThreadVersion { get; set; }

        [JsonProperty("isRead")]
        public bool IsRead { get; set; }

        [JsonProperty("isHighImportance")]
        public bool IsHighImportance { get; set; }

        [JsonProperty("isOneOnOne")]
        public bool IsOneOnOne { get; set; }

        [JsonProperty("lastMessage")]
        public LastMessage LastMessage { get; set; }

        [JsonProperty("isLastMessageFromMe")]
        public bool IsLastMessageFromMe { get; set; }

        [JsonProperty("chatSubType")]
        public long ChatSubType { get; set; }

        [JsonProperty("lastJoinAt")]
        public DateTimeOffset LastJoinAt { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("isGapDetectionEnabled")]
        public bool IsGapDetectionEnabled { get; set; }

        [JsonProperty("interopType")]
        public long InteropType { get; set; }

        [JsonProperty("isMessagingDisabled")]
        public bool IsMessagingDisabled { get; set; }

        [JsonProperty("isMuted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsMuted { get; set; }

        [JsonProperty("extensionDefinition", NullValueHandling = NullValueHandling.Ignore)]
        public ExtensionDefinition ExtensionDefinition { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("chatType")]
        public ChatType ChatType { get; set; }

        [JsonProperty("interopConversationStatus")]
        public InteropConversationStatus InteropConversationStatus { get; set; }

        [JsonProperty("conversationBlockedAt")]
        public long ConversationBlockedAt { get; set; }

        [JsonProperty("hasTranscript")]
        public bool HasTranscript { get; set; }

        [JsonProperty("meetingInformation", NullValueHandling = NullValueHandling.Ignore)]
        public MeetingInformation MeetingInformation { get; set; }

        [JsonProperty("shareHistoryFromTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? ShareHistoryFromTime { get; set; }
    }

    public partial class ConsumptionHorizon
    {
        [JsonProperty("originalArrivalTime")]
        public long OriginalArrivalTime { get; set; }

        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("clientMessageId")]
        public string ClientMessageId { get; set; }
    }

    public partial class ExtensionDefinition
    {
        [JsonProperty("updatedTime")]
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public partial class LastMessage
    {
        [JsonProperty("messageType")]
        public MessageType? MessageType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("clientMessageId")]
        public string ClientMessageId { get; set; }

        [JsonProperty("imDisplayName")]
        public string ImDisplayName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public TypeEnum? Type { get; set; }

        [JsonProperty("composeTime")]
        public DateTimeOffset? ComposeTime { get; set; }

        [JsonProperty("originalArrivalTime")]
        public DateTimeOffset? OriginalArrivalTime { get; set; }

        [JsonProperty("containerId")]
        public string ContainerId { get; set; }

        [JsonProperty("parentMessageId")]
        public string ParentMessageId { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("sequenceId")]
        public long SequenceId { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("threadType")]
        public object ThreadType { get; set; }

        [JsonProperty("isEscalationToNewPerson")]
        public bool IsEscalationToNewPerson { get; set; }
    }

    public partial class MeetingInformation
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTimeOffset EndTime { get; set; }

        [JsonProperty("exchangeId")]
        public string ExchangeId { get; set; }

        [JsonProperty("iCalUid")]
        public string ICalUid { get; set; }

        [JsonProperty("isCancelled")]
        public bool IsCancelled { get; set; }

        [JsonProperty("appointmentType")]
        public long AppointmentType { get; set; }

        [JsonProperty("meetingType")]
        public long MeetingType { get; set; }

        [JsonProperty("eventRecurrenceRange", NullValueHandling = NullValueHandling.Ignore)]
        public EventRecurrenceRange EventRecurrenceRange { get; set; }

        [JsonProperty("eventRecurrencePattern", NullValueHandling = NullValueHandling.Ignore)]
        public EventRecurrencePattern EventRecurrencePattern { get; set; }
    }

    public partial class EventRecurrencePattern
    {
        [JsonProperty("patternType")]
        public long PatternType { get; set; }

        [JsonProperty("weekly")]
        public Weekly Weekly { get; set; }
    }

    public partial class Weekly
    {
        [JsonProperty("interval")]
        public long Interval { get; set; }

        [JsonProperty("daysOfTheWeek")]
        public long[] DaysOfTheWeek { get; set; }
    }

    public partial class EventRecurrenceRange
    {
        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }
    }

    public partial class Member
    {
        [JsonProperty("isMuted")]
        public bool IsMuted { get; set; }

        [JsonProperty("mri")]
        public string Mri { get; set; }

        [JsonProperty("role")]
        public Role Role { get; set; }

        [JsonProperty("tenantId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? TenantId { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("syncToken")]
        public string SyncToken { get; set; }
    }

    public partial class PrivateFeed
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("lastMessage")]
        public LastMessage LastMessage { get; set; }

        [JsonProperty("messages")]
        public Uri Messages { get; set; }

        [JsonProperty("targetLink")]
        public Uri TargetLink { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("isemptyconversation")]
        public string Isemptyconversation { get; set; }

        [JsonProperty("consumptionhorizon")]
        public string Consumptionhorizon { get; set; }
    }

    public partial class Team
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }

        [JsonProperty("pictureETag")]
        public string PictureETag { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isFavorite")]
        public bool IsFavorite { get; set; }

        [JsonProperty("isCollapsed")]
        public bool IsCollapsed { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("isTenantWide")]
        public bool IsTenantWide { get; set; }

        [JsonProperty("smtpAddress")]
        public string SmtpAddress { get; set; }

        [JsonProperty("threadVersion")]
        public ThreadVersion ThreadVersion { get; set; }

        [JsonProperty("threadSchemaVersion")]
        public ThreadVersion ThreadSchemaVersion { get; set; }

        [JsonProperty("conversationVersion")]
        public object ConversationVersion { get; set; }

        [JsonProperty("classification")]
        public object Classification { get; set; }

        [JsonProperty("accessType")]
        public long AccessType { get; set; }

        [JsonProperty("guestUsersCategory")]
        public GuestUsersCategory? GuestUsersCategory { get; set; }

        [JsonProperty("dynamicMembership")]
        public bool DynamicMembership { get; set; }

        [JsonProperty("maximumMemberLimitExceeded")]
        public bool MaximumMemberLimitExceeded { get; set; }

        [JsonProperty("teamSettings")]
        public TeamSettings TeamSettings { get; set; }

        [JsonProperty("teamGuestSettings")]
        public TeamSettings TeamGuestSettings { get; set; }

        [JsonProperty("teamStatus")]
        public TeamStatus TeamStatus { get; set; }

        [JsonProperty("teamSiteInformation")]
        public TeamSiteInformation TeamSiteInformation { get; set; }

        [JsonProperty("isCreator")]
        public bool IsCreator { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("membershipVersion")]
        public long MembershipVersion { get; set; }

        [JsonProperty("membershipSummary")]
        public MembershipSummary MembershipSummary { get; set; }

        [JsonProperty("isUserMuted")]
        public bool IsUserMuted { get; set; }

        [JsonProperty("lastJoinAt")]
        public DateTimeOffset LastJoinAt { get; set; }

        [JsonProperty("membershipExpiry")]
        public long MembershipExpiry { get; set; }

        [JsonProperty("memberRole")]
        public long MemberRole { get; set; }

        [JsonProperty("isFollowed")]
        public bool IsFollowed { get; set; }

        [JsonProperty("tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty("teamType")]
        public long TeamType { get; set; }

        [JsonProperty("extensionDefinition", NullValueHandling = NullValueHandling.Ignore)]
        public ExtensionDefinition ExtensionDefinition { get; set; }

        [JsonProperty("isArchived")]
        public bool IsArchived { get; set; }

        [JsonProperty("isTeamLocked")]
        public bool IsTeamLocked { get; set; }

        [JsonProperty("lastLeaveAt", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastLeaveAt { get; set; }
    }

    public partial class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("consumptionHorizon", NullValueHandling = NullValueHandling.Ignore)]
        public ConsumptionHorizon ConsumptionHorizon { get; set; }

        [JsonProperty("retentionHorizon")]
        public object RetentionHorizon { get; set; }

        [JsonProperty("retentionHorizonV2")]
        public object RetentionHorizonV2 { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("threadVersion")]
        public long ThreadVersion { get; set; }

        [JsonProperty("threadSchemaVersion", NullValueHandling = NullValueHandling.Ignore)]
        public ThreadSchemaVersion? ThreadSchemaVersion { get; set; }

        [JsonProperty("parentTeamId")]
        public string ParentTeamId { get; set; }

        [JsonProperty("isGeneral")]
        public bool IsGeneral { get; set; }

        [JsonProperty("isFavorite")]
        public bool IsFavorite { get; set; }

        [JsonProperty("isFollowed")]
        public bool IsFollowed { get; set; }

        [JsonProperty("isMember")]
        public bool IsMember { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("isMessageRead")]
        public bool IsMessageRead { get; set; }

        [JsonProperty("isImportantMessageRead")]
        public bool IsImportantMessageRead { get; set; }

        [JsonProperty("isGapDetectionEnabled")]
        public bool IsGapDetectionEnabled { get; set; }

        [JsonProperty("defaultFileSettings")]
        public DefaultFileSettings DefaultFileSettings { get; set; }

        [JsonProperty("tabs", NullValueHandling = NullValueHandling.Ignore)]
        public Tab[] Tabs { get; set; }

        [JsonProperty("connectorProfiles", NullValueHandling = NullValueHandling.Ignore)]
        public ConnectorProfile[] ConnectorProfiles { get; set; }

        [JsonProperty("lastMessage", NullValueHandling = NullValueHandling.Ignore)]
        public LastMessage LastMessage { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("isPinned")]
        public bool IsPinned { get; set; }

        [JsonProperty("lastJoinAt", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastJoinAt { get; set; }

        [JsonProperty("memberRole")]
        public long MemberRole { get; set; }

        [JsonProperty("isMuted")]
        public bool IsMuted { get; set; }

        [JsonProperty("membershipExpiry")]
        public long MembershipExpiry { get; set; }

        [JsonProperty("isFavoriteByDefault")]
        public bool IsFavoriteByDefault { get; set; }

        [JsonProperty("creationTime")]
        public DateTimeOffset CreationTime { get; set; }

        [JsonProperty("isArchived")]
        public bool IsArchived { get; set; }

        [JsonProperty("channelType")]
        public long ChannelType { get; set; }

        [JsonProperty("membershipVersion")]
        public long MembershipVersion { get; set; }

        [JsonProperty("membershipSummary")]
        public MembershipSummary MembershipSummary { get; set; }

        [JsonProperty("isModerator")]
        public bool IsModerator { get; set; }

        [JsonProperty("groupId")]
        public Guid GroupId { get; set; }

        [JsonProperty("lastImportantMessageTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastImportantMessageTime { get; set; }

        [JsonProperty("lastLeaveAt", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastLeaveAt { get; set; }

        [JsonProperty("tenantId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? TenantId { get; set; }

        [JsonProperty("memberSettings", NullValueHandling = NullValueHandling.Ignore)]
        public Settings MemberSettings { get; set; }

        [JsonProperty("guestSettings", NullValueHandling = NullValueHandling.Ignore)]
        public Settings GuestSettings { get; set; }

        [JsonProperty("channelSettings", NullValueHandling = NullValueHandling.Ignore)]
        public ChannelSettings ChannelSettings { get; set; }

        [JsonProperty("sharepointSiteUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri SharepointSiteUrl { get; set; }
    }

    public partial class ChannelSettings
    {
        [JsonProperty("channelPostPermissions")]
        public long ChannelPostPermissions { get; set; }

        [JsonProperty("channelReplyPermissions")]
        public long ChannelReplyPermissions { get; set; }

        [JsonProperty("channelPinPostPermissions")]
        public long ChannelPinPostPermissions { get; set; }

        [JsonProperty("channelBotsPostPermissions")]
        public long ChannelBotsPostPermissions { get; set; }

        [JsonProperty("channelConnectorsPostPermissions")]
        public long ChannelConnectorsPostPermissions { get; set; }

        [JsonProperty("memberSettings")]
        public Settings MemberSettings { get; set; }

        [JsonProperty("guestSettings")]
        public Settings GuestSettings { get; set; }
    }

    public partial class Settings
    {
        [JsonProperty("createTab")]
        public bool CreateTab { get; set; }

        [JsonProperty("deleteTab")]
        public bool DeleteTab { get; set; }

        [JsonProperty("createIntegration")]
        public bool CreateIntegration { get; set; }

        [JsonProperty("updateIntegration")]
        public bool UpdateIntegration { get; set; }

        [JsonProperty("deleteIntegration")]
        public bool DeleteIntegration { get; set; }

        [JsonProperty("giphyEnabled")]
        public bool GiphyEnabled { get; set; }

        [JsonProperty("stickersEnabled")]
        public bool StickersEnabled { get; set; }

        [JsonProperty("giphyRating")]
        public long GiphyRating { get; set; }

        [JsonProperty("customMemesEnabled")]
        public bool CustomMemesEnabled { get; set; }

        [JsonProperty("teamMemesEnabled")]
        public bool TeamMemesEnabled { get; set; }

        [JsonProperty("teamMention")]
        public bool TeamMention { get; set; }

        [JsonProperty("adminDeleteEnabled")]
        public bool AdminDeleteEnabled { get; set; }

        [JsonProperty("deleteEnabled")]
        public bool DeleteEnabled { get; set; }

        [JsonProperty("editEnabled")]
        public bool EditEnabled { get; set; }

        [JsonProperty("installApp")]
        public bool InstallApp { get; set; }

        [JsonProperty("uninstallApp")]
        public bool UninstallApp { get; set; }
    }

    public partial class ConnectorProfile
    {
        [JsonProperty("avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("incomingUrl")]
        public object IncomingUrl { get; set; }

        [JsonProperty("connectorType")]
        public string ConnectorType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public partial class DefaultFileSettings
    {
        [JsonProperty("filesRelativePath")]
        public string FilesRelativePath { get; set; }

        [JsonProperty("documentLibraryId", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentLibraryId? DocumentLibraryId { get; set; }
    }

    public partial class MembershipSummary
    {
        [JsonProperty("botCount")]
        public long BotCount { get; set; }

        [JsonProperty("mutedMembersCount")]
        public long MutedMembersCount { get; set; }

        [JsonProperty("totalMemberCount")]
        public long TotalMemberCount { get; set; }

        [JsonProperty("adminRoleCount")]
        public long AdminRoleCount { get; set; }

        [JsonProperty("userRoleCount")]
        public long UserRoleCount { get; set; }

        [JsonProperty("guestRoleCount", NullValueHandling = NullValueHandling.Ignore)]
        public long? GuestRoleCount { get; set; }
    }

    public partial class Tab
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("definitionId")]
        public DefinitionIdUnion DefinitionId { get; set; }

        [JsonProperty("directive")]
        public Directive Directive { get; set; }

        [JsonProperty("tabType")]
        public TabType TabType { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }

        [JsonProperty("replyChainId")]
        public long ReplyChainId { get; set; }

        [JsonProperty("settings")]
        public SettingsClass Settings { get; set; }
    }

    public partial class SettingsClass
    {
        [JsonProperty("wikiTabId", NullValueHandling = NullValueHandling.Ignore)]
        public long? WikiTabId { get; set; }

        [JsonProperty("wikiDefaultTab", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WikiDefaultTab { get; set; }

        [JsonProperty("hasContent", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasContent { get; set; }

        [JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
        public Subtype? Subtype { get; set; }

        [JsonProperty("isPrivateMeetingWiki", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPrivateMeetingWiki { get; set; }

        [JsonProperty("meetingNotes", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MeetingNotes { get; set; }

        [JsonProperty("scenarioName", NullValueHandling = NullValueHandling.Ignore)]
        public string ScenarioName { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("websiteUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri WebsiteUrl { get; set; }

        [JsonProperty("removeUrl")]
        public string RemoveUrl { get; set; }

        [JsonProperty("entityId", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityId { get; set; }

        [JsonProperty("dateAdded", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? DateAdded { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("file", NullValueHandling = NullValueHandling.Ignore)]
        public string File { get; set; }

        [JsonProperty("siteUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri SiteUrl { get; set; }

        [JsonProperty("libraryServerRelativeUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string LibraryServerRelativeUrl { get; set; }

        [JsonProperty("libraryId", NullValueHandling = NullValueHandling.Ignore)]
        public string LibraryId { get; set; }

        [JsonProperty("selectedDocumentLibraryTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedDocumentLibraryTitle { get; set; }

        [JsonProperty("selectedSiteImageUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri SelectedSiteImageUrl { get; set; }

        [JsonProperty("selectedSiteTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedSiteTitle { get; set; }
    }

    public partial class TeamSettings
    {
        [JsonProperty("createTopic")]
        public bool CreateTopic { get; set; }

        [JsonProperty("updateTopic")]
        public bool UpdateTopic { get; set; }

        [JsonProperty("deleteTopic")]
        public bool DeleteTopic { get; set; }

        [JsonProperty("createTab")]
        public bool CreateTab { get; set; }

        [JsonProperty("deleteTab")]
        public bool DeleteTab { get; set; }

        [JsonProperty("createIntegration")]
        public bool CreateIntegration { get; set; }

        [JsonProperty("updateIntegration")]
        public bool UpdateIntegration { get; set; }

        [JsonProperty("deleteIntegration")]
        public bool DeleteIntegration { get; set; }

        [JsonProperty("teamMention")]
        public bool TeamMention { get; set; }

        [JsonProperty("channelMention")]
        public bool ChannelMention { get; set; }

        [JsonProperty("giphyEnabled")]
        public bool GiphyEnabled { get; set; }

        [JsonProperty("stickersEnabled")]
        public bool StickersEnabled { get; set; }

        [JsonProperty("giphyRating")]
        public long GiphyRating { get; set; }

        [JsonProperty("customMemesEnabled")]
        public bool CustomMemesEnabled { get; set; }

        [JsonProperty("teamMemesEnabled")]
        public bool TeamMemesEnabled { get; set; }

        [JsonProperty("addDisplayContent")]
        public bool AddDisplayContent { get; set; }

        [JsonProperty("removeDisplayContent")]
        public bool RemoveDisplayContent { get; set; }

        [JsonProperty("adminDeleteEnabled")]
        public bool AdminDeleteEnabled { get; set; }

        [JsonProperty("deleteEnabled")]
        public bool DeleteEnabled { get; set; }

        [JsonProperty("editEnabled")]
        public bool EditEnabled { get; set; }

        [JsonProperty("messageThreadingEnabled")]
        public bool MessageThreadingEnabled { get; set; }

        [JsonProperty("generalChannelPosting")]
        public long GeneralChannelPosting { get; set; }

        [JsonProperty("installApp")]
        public bool InstallApp { get; set; }

        [JsonProperty("uninstallApp")]
        public bool UninstallApp { get; set; }

        [JsonProperty("isPrivateChannelCreationEnabled")]
        public bool IsPrivateChannelCreationEnabled { get; set; }

        [JsonProperty("uploadCustomApp")]
        public bool UploadCustomApp { get; set; }
    }

    public partial class TeamSiteInformation
    {
        [JsonProperty("groupId")]
        public Guid GroupId { get; set; }

        [JsonProperty("sharepointSiteUrl")]
        public Uri SharepointSiteUrl { get; set; }

        [JsonProperty("notebookId", NullValueHandling = NullValueHandling.Ignore)]
        public string NotebookId { get; set; }

        [JsonProperty("isOneNoteProvisioned")]
        public bool IsOneNoteProvisioned { get; set; }
    }

    public partial class TeamStatus
    {
        [JsonProperty("exchangeTeamCreationStatus")]
        public long ExchangeTeamCreationStatus { get; set; }

        [JsonProperty("sharePointSiteCreationStatus")]
        public long SharePointSiteCreationStatus { get; set; }

        [JsonProperty("teamNotebookCreationStatus", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamNotebookCreationStatus { get; set; }
    }

    public enum ChatType { Chat, Meeting };

    public enum InteropConversationStatus { None };

    public enum MessageType { RichTextHtml, Text };

    public enum TypeEnum { Message };

    public enum Role { Admin, Guest, User };

    public enum DocumentLibraryId { Default };

    public enum DefinitionIdEnum { ComMicrosoftTeamspaceTabFileStaticviewerExcel, ComMicrosoftTeamspaceTabFileStaticviewerPdf, ComMicrosoftTeamspaceTabFileStaticviewerWord, ComMicrosoftTeamspaceTabFilesSharepoint, ComMicrosoftTeamspaceTabPlanner, ComMicrosoftTeamspaceTabVsts, ComMicrosoftTeamspaceTabWeb, ComMicrosoftTeamspaceTabWiki, ComMicrosoftstreamEmbedSkypeteamstab };

    public enum Directive { ExtensionTab, FilesCustomSpoTab, FilesPinTab, WebpageTab };

    public enum Subtype { Excelpin, Extension, Pdfpin, Sharepointfiles, Webpage, WikiTab, Wordpin };

    public enum TabType { Tab };

    public enum ThreadSchemaVersion { V5 };

    public enum GuestUsersCategory { Members };

    public enum ThreadVersion { V9 };

    public partial struct DefinitionIdUnion
    {
        public DefinitionIdEnum? Enum;
        public Guid? Uuid;

        public static implicit operator DefinitionIdUnion(DefinitionIdEnum Enum) => new DefinitionIdUnion { Enum = Enum };
        public static implicit operator DefinitionIdUnion(Guid Uuid) => new DefinitionIdUnion { Uuid = Uuid };
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ChatTypeConverter.Singleton,
                InteropConversationStatusConverter.Singleton,
                MessageTypeConverter.Singleton,
                TypeEnumConverter.Singleton,
                RoleConverter.Singleton,
                DocumentLibraryIdConverter.Singleton,
                DefinitionIdUnionConverter.Singleton,
                DefinitionIdEnumConverter.Singleton,
                DirectiveConverter.Singleton,
                SubtypeConverter.Singleton,
                TabTypeConverter.Singleton,
                ThreadSchemaVersionConverter.Singleton,
                GuestUsersCategoryConverter.Singleton,
                ThreadVersionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ChatTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ChatType) || t == typeof(ChatType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "chat":
                    return ChatType.Chat;
                case "meeting":
                    return ChatType.Meeting;
            }
            throw new Exception("Cannot unmarshal type ChatType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ChatType)untypedValue;
            switch (value)
            {
                case ChatType.Chat:
                    serializer.Serialize(writer, "chat");
                    return;
                case ChatType.Meeting:
                    serializer.Serialize(writer, "meeting");
                    return;
            }
            throw new Exception("Cannot marshal type ChatType");
        }

        public static readonly ChatTypeConverter Singleton = new ChatTypeConverter();
    }

    internal class InteropConversationStatusConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(InteropConversationStatus) || t == typeof(InteropConversationStatus?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "None")
            {
                return InteropConversationStatus.None;
            }
            throw new Exception("Cannot unmarshal type InteropConversationStatus");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (InteropConversationStatus)untypedValue;
            if (value == InteropConversationStatus.None)
            {
                serializer.Serialize(writer, "None");
                return;
            }
            throw new Exception("Cannot marshal type InteropConversationStatus");
        }

        public static readonly InteropConversationStatusConverter Singleton = new InteropConversationStatusConverter();
    }

    internal class MessageTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(MessageType) || t == typeof(MessageType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "RichText/Html":
                    return MessageType.RichTextHtml;
                case "Text":
                    return MessageType.Text;
            }
            throw new Exception("Cannot unmarshal type MessageType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (MessageType)untypedValue;
            switch (value)
            {
                case MessageType.RichTextHtml:
                    serializer.Serialize(writer, "RichText/Html");
                    return;
                case MessageType.Text:
                    serializer.Serialize(writer, "Text");
                    return;
            }
            throw new Exception("Cannot marshal type MessageType");
        }

        public static readonly MessageTypeConverter Singleton = new MessageTypeConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Message")
            {
                return TypeEnum.Message;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            if (value == TypeEnum.Message)
            {
                serializer.Serialize(writer, "Message");
                return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class RoleConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Role) || t == typeof(Role?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Admin":
                    return Role.Admin;
                case "Guest":
                    return Role.Guest;
                case "User":
                    return Role.User;
            }
            throw new Exception("Cannot unmarshal type Role");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Role)untypedValue;
            switch (value)
            {
                case Role.Admin:
                    serializer.Serialize(writer, "Admin");
                    return;
                case Role.Guest:
                    serializer.Serialize(writer, "Guest");
                    return;
                case Role.User:
                    serializer.Serialize(writer, "User");
                    return;
            }
            throw new Exception("Cannot marshal type Role");
        }

        public static readonly RoleConverter Singleton = new RoleConverter();
    }

    internal class DocumentLibraryIdConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DocumentLibraryId) || t == typeof(DocumentLibraryId?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "default")
            {
                return DocumentLibraryId.Default;
            }
            throw new Exception("Cannot unmarshal type DocumentLibraryId");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DocumentLibraryId)untypedValue;
            if (value == DocumentLibraryId.Default)
            {
                serializer.Serialize(writer, "default");
                return;
            }
            throw new Exception("Cannot marshal type DocumentLibraryId");
        }

        public static readonly DocumentLibraryIdConverter Singleton = new DocumentLibraryIdConverter();
    }

    internal class DefinitionIdUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DefinitionIdUnion) || t == typeof(DefinitionIdUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    switch (stringValue)
                    {
                        case "com.microsoft.teamspace.tab.file.staticviewer.excel":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerExcel };
                        case "com.microsoft.teamspace.tab.file.staticviewer.pdf":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerPdf };
                        case "com.microsoft.teamspace.tab.file.staticviewer.word":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerWord };
                        case "com.microsoft.teamspace.tab.files.sharepoint":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabFilesSharepoint };
                        case "com.microsoft.teamspace.tab.planner":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabPlanner };
                        case "com.microsoft.teamspace.tab.vsts":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabVsts };
                        case "com.microsoft.teamspace.tab.web":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabWeb };
                        case "com.microsoft.teamspace.tab.wiki":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftTeamspaceTabWiki };
                        case "com.microsoftstream.embed.skypeteamstab":
                            return new DefinitionIdUnion { Enum = DefinitionIdEnum.ComMicrosoftstreamEmbedSkypeteamstab };
                    }
                    Guid guid;
                    if (Guid.TryParse(stringValue, out guid))
                    {
                        return new DefinitionIdUnion { Uuid = guid };
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type DefinitionIdUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (DefinitionIdUnion)untypedValue;
            if (value.Enum != null)
            {
                switch (value.Enum)
                {
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerExcel:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.file.staticviewer.excel");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerPdf:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.file.staticviewer.pdf");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerWord:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.file.staticviewer.word");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabFilesSharepoint:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.files.sharepoint");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabPlanner:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.planner");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabVsts:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.vsts");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabWeb:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.web");
                        return;
                    case DefinitionIdEnum.ComMicrosoftTeamspaceTabWiki:
                        serializer.Serialize(writer, "com.microsoft.teamspace.tab.wiki");
                        return;
                    case DefinitionIdEnum.ComMicrosoftstreamEmbedSkypeteamstab:
                        serializer.Serialize(writer, "com.microsoftstream.embed.skypeteamstab");
                        return;
                }
            }
            if (value.Uuid != null)
            {
                serializer.Serialize(writer, value.Uuid.Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture));
                return;
            }
            throw new Exception("Cannot marshal type DefinitionIdUnion");
        }

        public static readonly DefinitionIdUnionConverter Singleton = new DefinitionIdUnionConverter();
    }

    internal class DefinitionIdEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DefinitionIdEnum) || t == typeof(DefinitionIdEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "com.microsoft.teamspace.tab.file.staticviewer.excel":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerExcel;
                case "com.microsoft.teamspace.tab.file.staticviewer.pdf":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerPdf;
                case "com.microsoft.teamspace.tab.file.staticviewer.word":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerWord;
                case "com.microsoft.teamspace.tab.files.sharepoint":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabFilesSharepoint;
                case "com.microsoft.teamspace.tab.planner":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabPlanner;
                case "com.microsoft.teamspace.tab.vsts":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabVsts;
                case "com.microsoft.teamspace.tab.web":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabWeb;
                case "com.microsoft.teamspace.tab.wiki":
                    return DefinitionIdEnum.ComMicrosoftTeamspaceTabWiki;
                case "com.microsoftstream.embed.skypeteamstab":
                    return DefinitionIdEnum.ComMicrosoftstreamEmbedSkypeteamstab;
            }
            throw new Exception("Cannot unmarshal type DefinitionIdEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DefinitionIdEnum)untypedValue;
            switch (value)
            {
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerExcel:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.file.staticviewer.excel");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerPdf:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.file.staticviewer.pdf");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabFileStaticviewerWord:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.file.staticviewer.word");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabFilesSharepoint:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.files.sharepoint");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabPlanner:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.planner");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabVsts:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.vsts");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabWeb:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.web");
                    return;
                case DefinitionIdEnum.ComMicrosoftTeamspaceTabWiki:
                    serializer.Serialize(writer, "com.microsoft.teamspace.tab.wiki");
                    return;
                case DefinitionIdEnum.ComMicrosoftstreamEmbedSkypeteamstab:
                    serializer.Serialize(writer, "com.microsoftstream.embed.skypeteamstab");
                    return;
            }
            throw new Exception("Cannot marshal type DefinitionIdEnum");
        }

        public static readonly DefinitionIdEnumConverter Singleton = new DefinitionIdEnumConverter();
    }

    internal class DirectiveConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Directive) || t == typeof(Directive?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "extension-tab":
                    return Directive.ExtensionTab;
                case "files-custom-spo-tab":
                    return Directive.FilesCustomSpoTab;
                case "files-pin-tab":
                    return Directive.FilesPinTab;
                case "webpage-tab":
                    return Directive.WebpageTab;
            }
            throw new Exception("Cannot unmarshal type Directive");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Directive)untypedValue;
            switch (value)
            {
                case Directive.ExtensionTab:
                    serializer.Serialize(writer, "extension-tab");
                    return;
                case Directive.FilesCustomSpoTab:
                    serializer.Serialize(writer, "files-custom-spo-tab");
                    return;
                case Directive.FilesPinTab:
                    serializer.Serialize(writer, "files-pin-tab");
                    return;
                case Directive.WebpageTab:
                    serializer.Serialize(writer, "webpage-tab");
                    return;
            }
            throw new Exception("Cannot marshal type Directive");
        }

        public static readonly DirectiveConverter Singleton = new DirectiveConverter();
    }

    internal class SubtypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Subtype) || t == typeof(Subtype?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "excelpin":
                    return Subtype.Excelpin;
                case "extension":
                    return Subtype.Extension;
                case "pdfpin":
                    return Subtype.Pdfpin;
                case "sharepointfiles":
                    return Subtype.Sharepointfiles;
                case "webpage":
                    return Subtype.Webpage;
                case "wiki-tab":
                    return Subtype.WikiTab;
                case "wordpin":
                    return Subtype.Wordpin;
            }
            throw new Exception("Cannot unmarshal type Subtype");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Subtype)untypedValue;
            switch (value)
            {
                case Subtype.Excelpin:
                    serializer.Serialize(writer, "excelpin");
                    return;
                case Subtype.Extension:
                    serializer.Serialize(writer, "extension");
                    return;
                case Subtype.Pdfpin:
                    serializer.Serialize(writer, "pdfpin");
                    return;
                case Subtype.Sharepointfiles:
                    serializer.Serialize(writer, "sharepointfiles");
                    return;
                case Subtype.Webpage:
                    serializer.Serialize(writer, "webpage");
                    return;
                case Subtype.WikiTab:
                    serializer.Serialize(writer, "wiki-tab");
                    return;
                case Subtype.Wordpin:
                    serializer.Serialize(writer, "wordpin");
                    return;
            }
            throw new Exception("Cannot marshal type Subtype");
        }

        public static readonly SubtypeConverter Singleton = new SubtypeConverter();
    }

    internal class TabTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TabType) || t == typeof(TabType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "tab:")
            {
                return TabType.Tab;
            }
            throw new Exception("Cannot unmarshal type TabType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TabType)untypedValue;
            if (value == TabType.Tab)
            {
                serializer.Serialize(writer, "tab:");
                return;
            }
            throw new Exception("Cannot marshal type TabType");
        }

        public static readonly TabTypeConverter Singleton = new TabTypeConverter();
    }

    internal class ThreadSchemaVersionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ThreadSchemaVersion) || t == typeof(ThreadSchemaVersion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "v5")
            {
                return ThreadSchemaVersion.V5;
            }
            throw new Exception("Cannot unmarshal type ThreadSchemaVersion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ThreadSchemaVersion)untypedValue;
            if (value == ThreadSchemaVersion.V5)
            {
                serializer.Serialize(writer, "v5");
                return;
            }
            throw new Exception("Cannot marshal type ThreadSchemaVersion");
        }

        public static readonly ThreadSchemaVersionConverter Singleton = new ThreadSchemaVersionConverter();
    }

    internal class GuestUsersCategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(GuestUsersCategory) || t == typeof(GuestUsersCategory?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "members")
            {
                return GuestUsersCategory.Members;
            }
            throw new Exception("Cannot unmarshal type GuestUsersCategory");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (GuestUsersCategory)untypedValue;
            if (value == GuestUsersCategory.Members)
            {
                serializer.Serialize(writer, "members");
                return;
            }
            throw new Exception("Cannot marshal type GuestUsersCategory");
        }

        public static readonly GuestUsersCategoryConverter Singleton = new GuestUsersCategoryConverter();
    }

    internal class ThreadVersionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ThreadVersion) || t == typeof(ThreadVersion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "v9")
            {
                return ThreadVersion.V9;
            }
            throw new Exception("Cannot unmarshal type ThreadVersion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ThreadVersion)untypedValue;
            if (value == ThreadVersion.V9)
            {
                serializer.Serialize(writer, "v9");
                return;
            }
            throw new Exception("Cannot marshal type ThreadVersion");
        }

        public static readonly ThreadVersionConverter Singleton = new ThreadVersionConverter();
    }
}
