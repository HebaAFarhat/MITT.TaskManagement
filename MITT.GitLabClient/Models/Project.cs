using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITT.GitLabClient.Models;

public partial class Project
{
    public long Id { get; set; }
    public object Description { get; set; }
    public string Name { get; set; }
    public string NameWithNamespace { get; set; }
    public string Path { get; set; }
    public string PathWithNamespace { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string DefaultBranch { get; set; }
    public List<object> TagList { get; set; }
    public List<object> Topics { get; set; }
    public string SshUrlToRepo { get; set; }
    public Uri HttpUrlToRepo { get; set; }
    public Uri WebUrl { get; set; }
    public Uri ReadmeUrl { get; set; }
    public long ForksCount { get; set; }
    public object AvatarUrl { get; set; }
    public long StarCount { get; set; }
    public DateTimeOffset LastActivityAt { get; set; }
    public Namespace Namespace { get; set; }
    public string ContainerRegistryImagePrefix { get; set; }
    public Links Links { get; set; }
    public bool PackagesEnabled { get; set; }
    public bool EmptyRepo { get; set; }
    public bool Archived { get; set; }
    public string Visibility { get; set; }
    public Owner Owner { get; set; }
    public bool ResolveOutdatedDiffDiscussions { get; set; }
    public ContainerExpirationPolicy ContainerExpirationPolicy { get; set; }
    public bool IssuesEnabled { get; set; }
    public bool MergeRequestsEnabled { get; set; }
    public bool WikiEnabled { get; set; }
    public bool JobsEnabled { get; set; }
    public bool SnippetsEnabled { get; set; }
    public bool ContainerRegistryEnabled { get; set; }
    public bool ServiceDeskEnabled { get; set; }
    public string ServiceDeskAddress { get; set; }
    public bool CanCreateMergeRequestIn { get; set; }
    public string IssuesAccessLevel { get; set; }
    public string RepositoryAccessLevel { get; set; }
    public string MergeRequestsAccessLevel { get; set; }
    public string ForkingAccessLevel { get; set; }
    public string WikiAccessLevel { get; set; }
    public string BuildsAccessLevel { get; set; }
    public string SnippetsAccessLevel { get; set; }
    public string PagesAccessLevel { get; set; }
    public string AnalyticsAccessLevel { get; set; }
    public string ContainerRegistryAccessLevel { get; set; }
    public string SecurityAndComplianceAccessLevel { get; set; }
    public string ReleasesAccessLevel { get; set; }
    public string EnvironmentsAccessLevel { get; set; }
    public string FeatureFlagsAccessLevel { get; set; }
    public string InfrastructureAccessLevel { get; set; }
    public string MonitorAccessLevel { get; set; }
    public bool EmailsDisabled { get; set; }
    public bool EmailsEnabled { get; set; }
    public bool SharedRunnersEnabled { get; set; }
    public bool LfsEnabled { get; set; }
    public long CreatorId { get; set; }
    public Uri ImportUrl { get; set; }
    public string ImportType { get; set; }
    public string ImportStatus { get; set; }
    public long OpenIssuesCount { get; set; }
    public string DescriptionHtml { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long CiDefaultGitDepth { get; set; }
    public bool CiForwardDeploymentEnabled { get; set; }
    public bool CiForwardDeploymentRollbackAllowed { get; set; }
    public bool CiJobTokenScopeEnabled { get; set; }
    public bool CiSeparatedCaches { get; set; }
    public bool CiAllowForkPipelinesToRunInParentProject { get; set; }
    public string BuildGitStrategy { get; set; }
    public bool KeepLatestArtifact { get; set; }
    public bool RestrictUserDefinedVariables { get; set; }
    public string RunnersToken { get; set; }
    public object RunnerTokenExpirationInterval { get; set; }
    public bool GroupRunnersEnabled { get; set; }
    public string AutoCancelPendingPipelines { get; set; }
    public long BuildTimeout { get; set; }
    public bool AutoDevopsEnabled { get; set; }
    public string AutoDevopsDeployStrategy { get; set; }
    public string CiConfigPath { get; set; }
    public bool PublicJobs { get; set; }
    public List<object> SharedWithGroups { get; set; }
    public bool OnlyAllowMergeIfPipelineSucceeds { get; set; }
    public object AllowMergeOnSkippedPipeline { get; set; }
    public bool RequestAccessEnabled { get; set; }
    public bool OnlyAllowMergeIfAllDiscussionsAreResolved { get; set; }
    public bool RemoveSourceBranchAfterMerge { get; set; }
    public bool PrintingMergeRequestLinkEnabled { get; set; }
    public string MergeMethod { get; set; }
    public string SquashOption { get; set; }
    public bool EnforceAuthChecksOnUploads { get; set; }
    public object SuggestionCommitMessage { get; set; }
    public object MergeCommitTemplate { get; set; }
    public object SquashCommitTemplate { get; set; }
    public object IssueBranchTemplate { get; set; }
    public bool AutocloseReferencedIssues { get; set; }
    public string ExternalAuthorizationClassificationLabel { get; set; }
    public bool RequirementsEnabled { get; set; }
    public string RequirementsAccessLevel { get; set; }
    public bool SecurityAndComplianceEnabled { get; set; }
    public List<object> ComplianceFrameworks { get; set; }
    public Permissions Permissions { get; set; }
}

public partial class ContainerExpirationPolicy
{
    public string Cadence { get; set; }
    public bool Enabled { get; set; }
    public long KeepN { get; set; }
    public string OlderThan { get; set; }
    public string NameRegex { get; set; }
    public object NameRegexKeep { get; set; }
    public DateTimeOffset NextRunAt { get; set; }
}

public partial class Links
{
    public Uri Issues { get; set; }
    public Uri MergeRequests { get; set; }
    public Uri RepoBranches { get; set; }
    public Uri Labels { get; set; }
    public Uri Events { get; set; }
    public Uri Members { get; set; }
    public Uri ClusterAgents { get; set; }
}

public partial class Namespace
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Kind { get; set; }
    public string FullPath { get; set; }
    public object ParentId { get; set; }
    public Uri AvatarUrl { get; set; }
    public Uri WebUrl { get; set; }
}

public partial class Owner
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public Uri AvatarUrl { get; set; }
    public Uri WebUrl { get; set; }
}

public partial class Permissions
{
    public Access ProjectAccess { get; set; }
    public Access GroupAccess { get; set; }
}

public partial class Access
{
    public long AccessLevel { get; set; }
    public long NotificationLevel { get; set; }
}