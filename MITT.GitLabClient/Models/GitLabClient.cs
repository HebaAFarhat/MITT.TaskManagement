using GitLabApiClient.Models.Branches.Responses;
using MITT.GitLabClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp;

public class GitLabClient
{
    private readonly HttpClient _httpClient;
    private readonly string _privateToken = "glpat-TyQzNrWkBYibZXo2dERw";

    public GitLabClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MITT.GitLabClient.Models.Branch>> Branches(string projectId = "47123376")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/projects/{projectId}/repository/branches");
        request.Headers.Add("PRIVATE-TOKEN", _privateToken);
        var response = await _httpClient.SendAsync(request);

        var content = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<MITT.GitLabClient.Models.Branch>>(content);
    }

    public async Task<List<Issue>> Issues(string assigneeId = "14940892", IssueState state = IssueState.all, string labels = "bug")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/issues?assignee_id={assigneeId}&state={state}&labels={labels}");
        request.Headers.Add("PRIVATE-TOKEN", _privateToken);
        var response = await _httpClient.SendAsync(request);

        var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<Issue>>(content);
    }

    public async Task<IssuesStatistics> IssuesStatistics(string projectId = "47123376")
    {
        var url = projectId is not null ? $"https://gitlab.com/api/v4/projects/{projectId}/issues_statistics" : "https://gitlab.com/api/v4/issues_statistics";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("PRIVATE-TOKEN", _privateToken);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IssuesStatistics>(content);
    }

    public async Task<bool> AddIssue(AddIssue addIssue)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://gitlab.com/api/v4/projects/{addIssue.ProjectId}/issues");
        request.Headers.Add("PRIVATE-TOKEN", _privateToken);

        request.Content = new StringContent(JsonConvert.SerializeObject(addIssue));
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return response.StatusCode > HttpStatusCode.OK && response.StatusCode < HttpStatusCode.IMUsed;
    }

    public async Task<List<ProjectTags>> GetProjectTags(string projectId = "47123376")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://gitlab.com/api/v4/projects/{projectId}/repository/tags");
        request.Headers.Add("PRIVATE-TOKEN", _privateToken);
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