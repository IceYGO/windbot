#!/usr/bin/env bash

set -euxo pipefail

# Embeddinator uses Xamarin Android Tools to find Android SDKs, NDKs, and Java SDKs
# The main registry key is decided by the envvar read here under Visual Studio and MSBuild
# https://github.com/xamarin/xamarin-android-tools/blob/294f4471a76da6df798c8520a8f8da0e6e83d3a5/src/Xamarin.Android.Tools.AndroidSdk/Sdks/AndroidSdkWindows.cs#L65
# that is set by Xamarin Android here in the build tasks
# https://github.com/xamarin/xamarin-android/blob/d4c8f077faefdfc6174355848a9d8c74ecaa8f56/src/Xamarin.Android.Build.Tasks/Tasks/SetVsMonoAndroidRegistryKey.cs
# and ultimately obtained by reading out the Visual Studio installation id, which seems to be unique
# https://github.com/xamarin/xamarin-android/blob/a677c1794db64d5559f53a960927447bac3063a2/src/Xamarin.Android.Build.Tasks/MSBuild/Xamarin/Xamarin.Android.Sdk.props#L16

# devenv.isolation.ini does not exist on Travis CI since there is no IDE though, only build tools
# so it's unclear where this installation id is obtained from.
# VSROOT="/c/Program Files (x86)/Microsoft Visual Studio/2017"
# INSTALLATION_ID=$(grep InstallationID= "$VSROOT/Common7/IDE/devenv.isolation.ini" | cut -f2 -d=)
# Thus for now, the installation ID after 15.0_ must be manually updated if Travis CI updates the Windows runner
# Visual Studio id for github actions 2019 16.0_42bb5ccc
# Visual Studio id for github actions 2016 15.0_87d23d74
# Visual Studio id for travis ci 15.0_09147932

# As workaround to find the installation id of visual studio,
# use the registry key used to associate .vssettings file extensions
# and extract the installation id from there,
# the runner has to set the "VS_PREFIX" environment variable (like "16.0_")
# to be able to generate the full installation id

KEY=$(echo $(reg query 'HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.vssettings\OpenWithProgids') | cut -d ' ' -f 3)
INSTALLATION_ID="$VS_PREFIX${KEY##*.}"

# These registry entries are normally set through the GUI: Tools\Options\Xamarin\Android Settings
reg add 'HKCU\SOFTWARE\Xamarin\VisualStudio\'$INSTALLATION_ID'\Android' -v AndroidSdkDirectory -t REG_SZ -d "$ANDROID_SDK_ROOT" -f
# Sometimes installed by Microsoft in C:\ProgramData\Microsoft\AndroidNDK64 but not present on CI
reg add 'HKCU\SOFTWARE\Xamarin\VisualStudio\'$INSTALLATION_ID'\Android' -v AndroidNdkDirectory -t REG_SZ -d "C:\android-ndk-r15c" -f
# Visual Studio Installer provides this JDK for Android development
reg add 'HKCU\SOFTWARE\Xamarin\VisualStudio\'$INSTALLATION_ID'\Android' -v JavaSdkDirectory -t REG_SZ -d "$JAVA_HOME" -f

# Manually install Android SDK Platform 24, the most recent version that still works with Embeddinator 0.4.0
# cd "$ANDROID_SDK_ROOT"
# (yes || true) | tools/bin/sdkmanager.bat --sdk_root=. "platforms;android-24"
# cd -

# Manually install Android NDK r15c, the most recent version that still works with Embeddinator 0.4.0
curl --retry 5 --connect-timeout 30 --location --remote-header-name --remote-name https://dl.google.com/android/repository/android-ndk-r15c-windows-x86_64.zip
echo "970bb2496de0eada74674bb1b06d79165f725696 *android-ndk-r15c-windows-x86_64.zip" | sha1sum -c
7z x android-ndk-r15c-windows-x86_64.zip -o'C:'
