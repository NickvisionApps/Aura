[![Nuget](https://img.shields.io/nuget/v/Nickvision.Aura)](https://www.nuget.org/packages/Nickvision.Aura/)

# Nickvision.Aura

<img width='128' height='128' alt='Logo' src='Nickvision.Aura/Resources/logo-r.png'/>

 **A cross-platform base for Nickvision applications**

 Aura provides the following functionality across platforms (Linux and Windows):
 - Stores application information: name, version, changelog etc
 - Allows to load and save configuration files in JSON format
 - Can start an IPC server using named pipe. In this case, an application becomes single-instance. New instances will send command-line arguments to existing one and quit.
 - Access system's network status and listen for changes
 - Store credentials in a secure fashion
 - Check for updates on the application's source repo (and download and install them on Windows-only)
 - Display progress, urgency, and count information on an application's taskbar item
