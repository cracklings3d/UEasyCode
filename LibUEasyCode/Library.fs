namespace LibUEasyCode

open System
open Newtonsoft.Json
open Newtonsoft.Json.Linq


module Say =
  let hello name = printfn $"Hello %s{name}"

module UProject =
  type EngineVersion =
    | LauncherVersion of version : Version
    | SourceBuild of uuid : Guid

  type Platform =
    | Win64 = 0x01
    | Win32 = 0x02
    | Android = 0x04
    | Linux = 0x08
    | MacOS = 0x10

  type UProjectDescriptor =
    {
      supported_platforms : Platform
      engine_version : EngineVersion
    }
    
  let analyse descriptor =
    let c = JObject.Parse descriptor

    let _ver = c.Value<string> "EngineAssociation"
    
    let ver: EngineVersion =
      match Version.Parse _ver with
        | Version v -> v
        | _ -> Guid.Parse (_ver.Trim ['{', '}'])

    let va = { supported_platforms = Platform.Win64 }

    va
