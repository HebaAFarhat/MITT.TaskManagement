// See https://aka.ms/new-console-template for more information

using MITT.GitLabClient;

IGitLabClient gitLabClient = new GitLabClient(new HttpClient());

var projects = await gitLabClient.Projects();

foreach (var project in projects)
{
    var branches = await gitLabClient.Branches(projectId: project.Id.ToString());
    var issuesStatistics = await gitLabClient.IssuesStatistics(projectId: project.Id.ToString());
    var projectIssues = await gitLabClient.ProjectIssues(projectId: project.Id.ToString());
    var tags = await gitLabClient.GetProjectTags(projectId: project.Id.ToString());
}

var issues = await gitLabClient.Issues();

Console.ReadLine();