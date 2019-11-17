param(
    [Parameter()]
        [string] $ProjectPath = 'C:\Repos\Github\backblaze\src\Cmdlet\Backblaze.Cmdlet.csproj',
	[Parameter()]
        [string] $OutDir = 'bin\Debug\netstandard2.0\'
)

$project_path = (Get-Item $ProjectPath ).DirectoryName
$project_xml = [xml](Get-Content $ProjectPath)
$root_module = -Join((Get-Item $ProjectPath).Basename, '.psd1')
$module_manifest = -Join($project_path, '\', $($OutDir), $root_module)
$project_files = Get-ChildItem $project_path -Include * -Recurse;



#$assemblies = Get-ChildItem $($OutDir) -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse | Select-Object -ExpandProperty Name
#$formats = Get-ChildItem $project_path -Include *.ps1xml -Recurse | Select-Object -ExpandProperty Name

 #$module = -Join($project_path, '\', $($OutDir), 'Backblaze.Cmdlet.dll')
 #Import-Module $module

#$CmdletsToExport = (Get-Command -Module 'Backblaze.Cmdlet' -CommandType Cmdlet).Name
#$AliasesToExport = (Get-Alias | Where-Object {$_.source -eq $moduleName}).Name
#Write-Output "Exporting $($CmdletsToExport.Count ) cmdlet(s), $($functions.count) function(s) and $($AliasesToExport.Count) Aliase(s)."
#Write-Output "Importing $($NestedModules.count) module(s), $($FormatsToProcess.count) format file(s), and $($TypesToProcess.count) type file(s)."

$manifest = @{
	Path                   = $module_manifest
	Guid                   = 'DB8CEDD3-6055-4358-A59E-FCA8541E1F9D'
	Description            = [string]$project_xml.Project.PropertyGroup.Description
	Author                 = [string]$project_xml.Project.PropertyGroup.Authors
	CompanyName            = [string]$project_xml.Project.PropertyGroup.Company
	Copyright              = [string]$project_xml.Project.PropertyGroup.Copyright
	ModuleVersion          = [string]$project_xml.Project.PropertyGroup.Version
	PowerShellVersion      = '5.1'
	DotNetFrameworkVersion = '4.7'
	RootModule             = $root_module
	DefaultCommandPrefix   = "Sonarr"
	RequiredAssemblies     = $assemblies
	CompatiblePSEditions   = "Core", "Desktop"
	FormatsToProcess       = if ($formats.Length -gt 0) { $formats } else { @() };
	ProjectUri             = [string]$project_xml.Project.PropertyGroup.RepositoryUrl
	HelpInfoUri			   = [string]$project_xml.Project.PropertyGroup.PackageProjectUrl
	Tags                   = [string]$project_xml.Project.PropertyGroup.PackageTags
};

Write-Output "Backblaze.Cmdlet ->  $module_manifest"
New-ModuleManifest @manifest

#Test-ModuleManifest -Path $module_manifest 