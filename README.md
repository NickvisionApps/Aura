[![Nuget](https://img.shields.io/nuget/v/Nickvision.Aura)](https://www.nuget.org/packages/Nickvision.Aura/)

# Nickvision.Aura

<img width='128' height='128' alt='Logo' src='Nickvision.Aura/Resources/logo-r.png'/>

 **A cross-platform base for Nickvision applications**

 Aura provides the following functionality:
 1. Stores application information: name, version, changelog etc
 2. Allows to load and save configuration files in JSON format
 3. Can start an IPC server using named pipe. In this case, an application becomes single-instance. New instances will send command-line arguments to existing one and quit.
