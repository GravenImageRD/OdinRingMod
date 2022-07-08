param(
    [Parameter(Mandatory)]
    [ValidateSet('Debug','Release')]
    [System.String]$Target,
    
    [Parameter(Mandatory)]
    [System.String]$TargetPath,
    
    [Parameter(Mandatory)]
    [System.String]$TargetAssembly,

    [Parameter(Mandatory)]
    [System.String]$ValheimPath,

    [Parameter(Mandatory)]
    [System.String]$ProjectPath,
    
    [System.String]$DeployPath
)

# Make sure Get-Location is the script path
Push-Location -Path (Split-Path -Parent $MyInvocation.MyCommand.Path)

# Test some preliminaries
("$TargetPath",
 "$ValheimPath",
 "$(Get-Location)\libraries"
) | % {
    if (!(Test-Path "$_")) {Write-Error -ErrorAction Stop -Message "$_ folder is missing"}
}

# Plugin name without ".dll"
$name = "$TargetAssembly" -Replace('.dll')

# Create the mdb file
$pdb = "$TargetPath\$name.pdb"
if (Test-Path -Path "$pdb") {
    Write-Host "Create mdb file for plugin $name"
    Invoke-Expression "& `"$(Get-Location)\libraries\Debug\pdb2mdb.exe`" `"$TargetPath\$TargetAssembly`""
}

# Main Script
Write-Host "Publishing for $Target from $TargetPath"

if ($Target.Equals("Debug")) {
    if ($DeployPath.Equals("")){
      $DeployPath = "$ValheimPath\BepInEx\plugins"
    }
    
    $plug = New-Item -Type Directory -Path "$DeployPath\$name" -Force
    Write-Host "Copy $TargetAssembly to $plug"
    Copy-Item -Path "$TargetPath\$name.dll" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.pdb" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.dll.mdb" -Destination "$plug" -Force
    $translation = New-Item -Type Directory -Path "$plug\Assets\Translations\English" -Force
    Copy-Item -Path "$TargetPath\Assets\Translations\English\odinring.json" -Destination "$translation" -Force
}

if($Target.Equals("Release")) {
    Write-Host "Packaging for ThunderStore..."
    $Package="Package"
    $PackagePath="$ProjectPath\$Package"

    Write-Host "$PackagePath\$TargetAssembly"
    $odinRing = New-Item -Type Directory -Path "$PackagePath\OdinRing" -Force
    Copy-Item -Path "$TargetPath\$TargetAssembly" -Destination "$odinRing\$TargetAssembly" -Force
    Copy-Item -Path "$ProjectPath\README.md" -Destination "$PackagePath\README.md" -Force
    $translation = New-Item -Type Directory -Path "$odinRing\Assets\Translations\English" -Force
    Copy-Item -Path "$TargetPath\Assets\Translations\English\odinring.json" -Destination "$translation" -Force
    Compress-Archive -Path "$PackagePath\*" -DestinationPath "GravenImageRD-OdinRing-0.0.1.zip" -Force
}

# Pop Location
Pop-Location