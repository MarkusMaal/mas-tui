dotnet publish MasCpanel -c Release -o out -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugSymbols=false
$mas_root = [environment]::getfolderpath(“userprofile”) + "/.mas"
Copy-Item -Path out/MasCpanel -Destination "$mas_root/Markuse asjad"
