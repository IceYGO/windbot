#!/usr/bin/env bash

set -euxo pipefail

# These registry entries are normally set through the GUI: Tools\Options\Xamarin\Android Settings
reg add 'HKCU\SOFTWARE\Xamarin\VisualStudio\15.0_e43b966e\Android' -v AndroidSdkDirectory -t REG_SZ -d "$ANDROID_SDK_ROOT" -f
# Sometimes installed by Microsoft in C:\ProgramData\Microsoft\AndroidNDK64 but not present on CI
reg add 'HKCU\SOFTWARE\Xamarin\VisualStudio\15.0_e43b966e\Android' -v AndroidNdkDirectory -t REG_SZ -d "C:\android-ndk-r15c" -f
# Visual Studio Installer provides this JDK for Android development
reg add 'HKCU\SOFTWARE\Xamarin\VisualStudio\15.0_e43b966e\Android' -v JavaSdkDirectory -t REG_SZ -d "$JAVA_HOME" -f

# Manually install Android SDK Platform 24, the most recent version that still works with Embeddinator 0.4.0
cd "$ANDROID_SDK_ROOT"
(yes || true) | tools/bin/sdkmanager.bat --sdk_root=. "platforms;android-24"
cd -

# Manually install Android NDK r15c, the most recent version that still works with Embeddinator 0.4.0
curl --retry 5 --connect-timeout 30 --location --remote-header-name --remote-name https://dl.google.com/android/repository/android-ndk-r15c-windows-x86_64.zip
echo "970bb2496de0eada74674bb1b06d79165f725696 *android-ndk-r15c-windows-x86_64.zip" | sha1sum -c
7z x android-ndk-r15c-windows-x86_64.zip -o'C:'
