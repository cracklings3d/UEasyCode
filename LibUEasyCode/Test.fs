module LibUEasyCode.Test

open System
open System.IO


let path = "D:/SVN_Mirror_SH/PICOVR/PICOVR.uproject"

let project_descriptor = File.ReadAllText path

let d = UProject.parse project_descriptor

Console.WriteLine d.engine_version
