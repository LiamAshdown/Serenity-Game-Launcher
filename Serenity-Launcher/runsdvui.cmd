cd /d "F:\liam-work\Serenity-Launcher\Serenity-Launcher" &msbuild "Serenity-Launcher.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=Any CPU
exit %errorlevel% 