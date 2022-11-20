module LibUEasyCode.Test

open System
open System.IO


let path = "F:/UE/Project/PICOVR/PICOVR.uproject"

let project_descriptor = File.ReadAllText path

let d = UProject.analyse project_descriptor

Console.WriteLine d.name
