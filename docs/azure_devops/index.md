# Task Mappings

Click [here](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks) to view the full listing of built-in Azure DevOps tasks.

| Azure DevOps                                                    | GitHub                                                                                          |
| :-------------------------------------------------------------- | :---------------------------------------------------------------------------------------------- |
| [Ant](Ant.md)                                                   | run                                                                                             |
| [ArchiveFiles](ArchiveFiles.md)                                 | run                                                                                             |
| [AzureAppServiceManage](AzureAppServiceManage.md)               | azure/login@v2, azure/cli@v2                                                                   |
| [AzureCLI](AzureCLI.md)                                         | run                                                                                             |
| [AzureFileCopy](AzureFileCopy.md)                               | run                                                                                             |
| [AzureFunction](AzureFunction.md)                               | run                                                                                             |
| [AzureFunctionApp](AzureFunctionApp.md)                         | azure/login@v2, azure/functions-action@v1, azure/appservice-settings@v1                         |
| [AzureKeyVault](AzureKeyVault.md)                               | azure/login@v2, run                                                                             |
| [AzurePowershell](AzurePowershell.md)                           | azure/login@v2, azure/powershell@v2                                                             |
| [AzureResourceGroupDeployment](AzureResourceGroupDeployment.md) | run                                                                                             |
| [AzureRmWebAppDeployment](AzureRmWebAppDeployment.md)           | azure/login@v2, azure/functions-action@v1, azure/webapps-deploy@v3,azure/appservice-settings@v1 |
| [AzureServicesSecurityStatus](AzureServicesSecurityStatus.md)   | azure/login@v2, run                                                                             |
| [AzureWebApp](AzureWebApp.md)                                   | azure/login@v2, azure/webapps-deploy@v3, azure/appservice-settings@v1                           |
| [AzureWebAppContainer](AzureWebAppContainer.md)                 | azure/login@v2, azure/webapps-deploy@v3, azure/appservice-settings@v1                           |
| [Bash](Bash.md)                                                 | run                                                                                             |
| [BatchScript](BatchScript.md)                                   | run                                                                                             |
| [Checkout](Checkout.md)                                         | actions/checkout@v6                                                                             |
| [CmdLine](CmdLine.md)                                           | run                                                                                             |
| [colinsalmcorner.colinsalmcorner-buildtasks.replace-tokens-task.ReplaceTokens](colinsalmcorner.colinsalmcorner-buildtasks.replace-tokens-task.ReplaceTokens.md)   | cschleiden/replace-tokens@v1  |
| [ContinuousIntegration](ContinuousIntegration.md)               | on.push                                                                                         |
| [CopyFiles](CopyFiles.md)                                       | actions/github-script@v8                                                                        |
| [CopyPublishBuildArtifacts](CopyPublishBuildArtifacts.md)       |actions/github-script@v8, actions/upload-artifact@v6                                     |
| [databricksDeployScripts](DatabricksDeployScripts.md)           | microsoft/install-databricks-cli@v1, microsoft/databricks-import-notebook@v1, run       |
| [DeleteFiles](DeleteFiles.md)                                   | actions/github-script@v8                                                                        |
| [DeployADFJSON](DeployAdfJson.md)                               | azure/login@v2, run                                                                             |
| [Docker](Docker.md)                                             | docker/login-action@v3                                                                          |
| [DotNetCoreCLI](DotNetCoreCLI.md)                               | run                                                                                             |
| [DownloadBuildArtifacts](DownloadBuildArtifacts.md)             | actions/download-artifact@v7                                                                    |
| [DownloadPipelineArtifact](DownloadPipelineArtifact.md)         | actions/download-artifact@v7                                                                    |
| [ExtractFiles](ExtractFiles.md)                                 | actions/github-script@v8                                                                        |
| [Gradle](Gradle.md)                                             | run                                                                                             |
| [HelmDeploy](HelmDeploy.md)                                     | run, azure/aks-set-context@v4, azure/k8s-set-context@v4                                         |
| [HelmInstaller](HelmInstaller.md)                               | azure/setup-kubectl@v4, azure/setup-helm@v4                                                    |
| [IISWebAppDeploymentOnMachineGroup](IisWebAppDeploymentOnMachineGroup.md)                               | cschleiden/webdeploy-action@v1                                                    |
| [InlineAzurePowershell](InlineAzurePowershell.md)               | azure/powershell@v2,  azure/login@v2                                                            |
| [InlinePowershell](InlinePowershell.md)                         | run                                                                                             |
| [InstallAppleCertificate](InstallAppleCertificate.md)           | run                                                                                             |
| [InstallAppleProvisioningProfile](InstallAppleProvisioningProfile.md) | run                                                                                       |
| [InvokeRestApi](InvokeRestApi.md)                               | run, azure/login@v2                                                                             |
| [Kubernetes](Kubernetes.md)                                     | run, azure/setup-kubectl@v4, azure/aks-set-context@v4, azure/k8s-set-context@v4, azure/k8s-create-secret@v5 |
| [Maven](Maven.md)                                               | actions/setup-java@v5, run                                                                      |
| [ManualIntervention](ManualIntervention.md)                     |                                                                                                 |
| [MsBuild](MsBuild.md)                                           | microsoft/setup-msbuild@v2, run                                                             |
| [Npm](Npm.md)                                                   | run                                                                                             |
| [NodeTool](NodeTool.md)                                         | actions/setup-node@v6                                                                           |
| [NuGetAuthenticate](NuGetAuthenticate.md)                       | actions/setup-dotnet@v5                                                                         |
| [NuGetCommand](NuGetCommand.md)                                 | run                                                                                             |
| [NuGetInstaller](NuGetCommand.md#NuGetInstaller)                | run                                                                                             |
| [NuGetToolInstaller](NuGetToolInstaller.md)                     | nuget/setup-nuget@v2                                                                        |
| [PowerShell](PowerShell.md)                                     | run                                                                                             |
| [PowerShellOnTargetMachines](PowerShellOnTargetMachines.md)     | azure/login@v2, azure/powershell@v2                                                             |
| [PublishBuildArtifacts](PublishBuildArtifacts.md)               | actions/upload-artifact@v6                                                                      |
| [PublishCodeCoverageResults](PublishCodeCoverageResults.md)     | actions/upload-artifact@v6, danielpalme/ReportGenerator-GitHub-Action@5                    |
| [PublishPipelineArtifact](PublishPipelineArtifact.md)           | actions/upload-artifact@v6                                                                      |
| [PublishSymbols](PublishSymbols.md)                             | microsoft/action-publish-symbols@v2                                                             |
| [PublishTestResults](PublishTestResults.md)                     | EnricoMi/publish-unit-test-result-action@v2 or dorny/test-reporter@v2                     |
| [PullRequest](PullRequest.md)                                   | on.pull_request                                                                                 |
| [PythonScript](PythonScript.md)                                 | run                                                                                             |
| [ReplaceTokens](ReplaceTokens.md)                               | cschleiden/replace-tokens@v1                                                                    |
| [Shell++](ShellPlusPlus.md)                                     | run                                                                                             |
| [SSISBuild](SSISBuild.md)                                       | run                                                                                             |
| [SqlAzureDacpacDeployment](SqlAzureDacpacDeployment.md)         | run, azure/login@v2, azure/sql-action@v2                                                              |
| [SqlDacpacDeploy](SqlDacpacDeploy.md)                           | run, azure/login@v2, azure/sql-action@v2                                                              |
| [SqlDacpacDeploymentOnMachineGroup](SqlDacpacDeploymentOnMachineGroup.md) | run                                                                                   |
| [TerraformInstaller](TerraformInstaller.md)                     | hashicorp/setup-terraform@v3                                                                    |
| [Terraform](Terraform.md)                                       | run                                                                                             |
| [Tokenization](Tokenization.md)                                 | cschleiden/replace-tokens@v1                                                                    |
| [Tokenizer](Tokenizer.md)                                       | run                                                                                             |
| [Toggle-ADF-Trigger](ToggleAdfTrigger.md)                       | azure/login@v2, run                                                                             |
| [UseDotNet](UseDotNet.md)                                       | actions/setup-dotnet@v5                                                                         |
| [UseRubyVersion](UseRubyVersion.md)                             | ruby/setup-ruby@v1                                                                              |
| [UsePythonVersion](UsePythonVersion.md)                         | actions/setup-python@v6                                                                         |
| [VsBuild](MsBuild.md)                                           | microsoft/setup-msbuild@v2, run                                                             |
| [VSTest](VsTest.md)                                             | microsoft/vstest-action@v1         |
| [WindowsMachineFileCopy](WindowsMachineFileCopy.md)             | run                                                                                             |
| [XamarinAndroid](XamarinAndroid.md)                             | actions/setup-java@v5, microsoft/setup-msbuild@v2, run                                      |
| [Xamarin iOS](XamarinIos.md)                                    | run                                                                                             |
| [Xcode](Xcode.md)                                               | run, maxim-lobanov/setup-xcode@v1                                                               |
| [Yarn](Yarn.md)                                                 | run                                                                                             |
| [YarnInstaller](YarnInstaller.md)                               | run                                                                                             |

## Unsupported

The following tasks do not have any equivalent in GitHub Actions:

- BuildQualityChecks
- Package
- ArtifactSource
- PublishSecurityAnalysisLogs

Any task not listed above will not be mapped to an action and will be left as a comment in the converted workflow.
