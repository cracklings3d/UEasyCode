namespace LibUEasyCode

open System
open System.Text.RegularExpressions
open Newtonsoft.Json.Linq


module UProject =
  type EngineVersion =
    | LauncherInstall of version : Version
    | SourceBuild of uuid : Guid

  type Platform =
    | Win64 = 0x01
    | Win32 = 0x02
    | Android = 0x04
    | Linux = 0x08
    | MacOS = 0x10

  type ModuleType =
    | Runtime
    | Editor

  type LoadingPhase = | Default

  type Module =
    {
      name : string
      type_ : ModuleType
      loading_phase : LoadingPhase
      dependencies : string seq
    }

  type Plugin =
    {
      name : string
      enabled : bool
      supported_platforms : Platform
      marketplace_url : string option
    }

  type UProjectDescriptor =
    {
      supported_platforms : Platform
      engine_version : EngineVersion
      modules : Module seq
    }

  let parse_module_type j_type =
    match j_type with
    | "Runtime" -> Runtime
    | "Editor" -> Editor
    | _ -> failwith "Invalid module type"

  let parse_module_loading_phase j_loading_phase =
    match j_loading_phase with
    | "Default" -> Default
    | _ -> failwith "Invalid loading phase"
    
  let parse_platforms (str_list: string seq): Platform =
    let ccccc = if Seq.contains "Win64" then Platform.Win64 else Platform.Win32
    Platform.Win64

  let parse_module (j_module : JToken) =
    {
      name = j_module.Value<string> "Name"
      type_ = parse_module_type (j_module.Value<string> "Type")
      loading_phase = parse_module_loading_phase (j_module.Value<string> "LoadingPhase")
      dependencies = (j_module.Value<JToken> "AdditionalDependencies").Values<string> ()
    }

  let parse_plugin (j_plugin : JToken) =
    {
      name = j_plugin.Value<string> "Name"
      enabled = j_plugin.Value<bool> "Enabled"
      supported_platforms = parse_platforms (j_module.Value<string> "LoadingPhase")
      marketplace_url = None
    }

  let parse (descriptor : string) : UProjectDescriptor =
    let j = JObject.Parse descriptor

    let _ver = j.Value<string> "EngineAssociation"

    let regex_launcher_install =
      Regex ("\d\.\d+", RegexOptions.Compiled)

    let regex_source_build = Regex ("{.*}", RegexOptions.Compiled)

    let ver : EngineVersion =
      if regex_launcher_install.IsMatch _ver then
        EngineVersion.LauncherInstall (Version.Parse _ver)
      else if regex_source_build.IsMatch _ver then
        let uuid : string = _ver.Trim ('{', '}')
        EngineVersion.SourceBuild (Guid.Parse uuid)
      else
        failwith "Invalid version string."

    {
      supported_platforms = Platform.Win64 ||| Platform.Linux
      engine_version = ver
      modules = Seq.map parse_module (j.Value<JToken> "Modules")
    }
