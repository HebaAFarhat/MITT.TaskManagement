using Microsoft.Extensions.Options;
using MITT.GitLabClient.Models;
using Newtonsoft.Json;
using System.Net;

namespace MITT.GitLabClient;

public class GitLabOption
{
    public string PrivateToken { get; set; } = "glpat-TyQzNrWkBYibZXo2dERw";
}

public interface IGitLabClient
{
    Task<List<Branch>> Branches(string projectId = "49012507");

    Task<List<ProjectTags>> GetProjectTags(string projectId = "49012507");

    Task<List<Issue>> Issues(string assigneeId = "14940892", IssueState state = IssueState.all, string labels = "bug");

    Task<IssuesStatistics> IssuesStatistics(string projectId = "49012507");

    Task<bool> AddIssue(AddIssue addIssue);

    Task<List<Issue>> ProjectIssues(string projectId);

    Task<List<Project>> Projects(string? projectId = null);
}

public class GitLabClient : IGitLabClient
{
    private readonly HttpClient _httpClient;
    private readonly string PrivateToken;

    public GitLabClient(HttpClient httpClient, IOptions<GitLabOption>? options = null)
    {
        PrivateToken = options?.Value.PrivateToken ?? "glpat-TyQzNrWkBYibZXo2dERw";
        _httpClient = httpClient;

        httpClient.DefaultRequestHeaders.Add("PRIVATE-TOKEN", PrivateToken);
    }

    public async Task<List<Project>> Projects(string? projectId = null)
    {
        var url = projectId is null ? "https://gitlab.com/api/v4/projects?owned=true" : $"https://gitlab.com/api/v4/projects/{projectId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadAsStringAsync();

        List<Project> result = new();

        switch (projectId)
        {
            case not null:
                result.Add(JsonConvert.DeserializeObject<Project>(content));
                return result;

            default:
                return JsonConvert.DeserializeObject<List<Project>>(content);
        }
    }

    public async Task<List<Branch>> Branches(string projectId = "49012507")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/projects/{projectId}/repository/branches");
        var response = await _httpClient.SendAsync(request);

        var content = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<Branch>>(content);
    }

    public async Task<List<Issue>> ProjectIssues(string projectId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/projects/{projectId}/issues");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<Issue>>(content);
    }

    public async Task<List<Issue>> Issues(string assigneeId = "14940892", IssueState state = IssueState.all, string labels = "bug")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/issues?assignee_id={assigneeId}&state={state}&labels={labels}");
        var response = await _httpClient.SendAsync(request);

        var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<Issue>>(content);
    }

    public async Task<IssuesStatistics> IssuesStatistics(string projectId = "49012507")
    {
        var url = projectId is not null ? $"https://gitlab.com/api/v4/projects/{projectId}/issues_statistics" : "https://gitlab.com/api/v4/issues_statistics";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IssuesStatistics>(content);
    }

    public async Task<bool> AddIssue(AddIssue addIssue)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://gitlab.com/api/v4/projects/{addIssue.ProjectId}/issues")
        {
            Content = new StringContent(JsonConvert.SerializeObject(addIssue))
        };
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return response.StatusCode > HttpStatusCode.OK && response.StatusCode < HttpStatusCode.IMUsed;
    }

    public async Task<List<ProjectTags>> GetProjectTags(string projectId = "49012507")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/projects/{projectId}/repository/tags");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<ProjectTags>>(content);
    }
}

public enum IssueState
{
    opened, closed, all
}