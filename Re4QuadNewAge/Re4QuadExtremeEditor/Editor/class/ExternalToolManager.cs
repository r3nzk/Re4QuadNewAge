using Re4QuadExtremeEditor.Editor.Class.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Re4QuadExtremeEditor.Editor.Class
{
    public static class ExternalToolManager
    {
        private static async Task<bool> RunTool(string toolPath, string targetPath){
            if (string.IsNullOrEmpty(toolPath) || !File.Exists(toolPath)){
                Editor.Console.Error($"Tool not found at path: {toolPath}. Please configure the tool path in the options.");
                return false;
            }
            if (!File.Exists(targetPath) && !Directory.Exists(targetPath)){
                Editor.Console.Error($"Target path not found: {targetPath}");
                return false;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = $"\"{targetPath}\"",

                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(toolPath)
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode == 0){
                        Editor.Console.Log($"Tool '{Path.GetFileName(toolPath)}' successfully processed '{Path.GetFileName(targetPath)}'.");
                        return true;
                    }else{
                        //log that the tool ended with some type of unexpected behaviour (not always problematic, but can be)
                        Editor.Console.Warning($"Tool '{Path.GetFileName(toolPath)}' finished with a non-zero exit code ({process.ExitCode}). There may have been an error.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Editor.Console.Error($"An exception occurred while running the external tool: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> RepackFolderAsync(string toolPath, string folderPath){
            Editor.Console.Log($"Attempting to repack folder: {folderPath}");
            return await RunTool(toolPath, folderPath);
        }

        public static async Task<bool> UnpackFileAsync(string toolPath, string filePath){
            Editor.Console.Log($"Attempting to unpack file: {filePath}");
            return await RunTool(toolPath, filePath);
        }


        #region PACK Tool

        public static async Task<bool> UnpackPack(string filePath)
        {
            Editor.Console.Log($"Attempting to unpack PACK file: {Path.GetFileName(filePath)}");
            if (string.IsNullOrEmpty(Globals.ToolPathPACK)){
                Editor.Console.Error("PACK Tool path is not configured. Please set it in the options menu.");
                return false;
            }
            return await RunTool(Globals.ToolPathPACK, filePath);
        }

        public static async Task<bool> RepackPackFolderAsync(string folderPath)
        {
            Editor.Console.Log($"Attempting to repack PACK folder: {Path.GetFileName(folderPath)}");
            if (string.IsNullOrEmpty(Globals.ToolPathPACK)){
                Editor.Console.Error("PACK Tool path is not configured. Please set it in the options menu.");
                return false;
            }
            return await RunTool(Globals.ToolPathPACK, folderPath);
        }

        public static async Task UnpackAllPacks(string targetImagePack)
        {
            if (string.IsNullOrEmpty(Globals.ToolPathPACK)){
                Editor.Console.Error("PACK Tool path is not configured. Please set it in the options menu.");
                return;
            }

            string targetDirectory = Path.Combine(Globals.DirectoryUHDRE4, "BIO4", targetImagePack); 
            if (!Directory.Exists(targetDirectory)){
                Editor.Console.Error($"Target directory not found: '{targetDirectory}'. Mass unpack aborted.");
                return;
            }

            Editor.Console.Log($"Starting mass unpack of all files in '{targetDirectory}'...");

            var allFiles = Directory.GetFiles(targetDirectory, "*.*", SearchOption.TopDirectoryOnly);
            var filesToUnpackWithPackTool = new List<string>();

            //.LFS pack list
            Editor.Console.Log("Scanning for .lfs files to decompress first...");
            foreach (string file in allFiles.Where(f => f.EndsWith(".lfs", StringComparison.OrdinalIgnoreCase))){
                string unpackedLfsPath = await UnpackLfs(file);
                if (unpackedLfsPath != null)
                {
                    filesToUnpackWithPackTool.Add(unpackedLfsPath);
                }
            }

            //raw pack list
            var unpackedLfsBaseNames = new HashSet<string>(filesToUnpackWithPackTool.Select(f => Path.GetFileName(f)));
            foreach (string file in allFiles){
                if (!file.EndsWith(".lfs", StringComparison.OrdinalIgnoreCase) && !unpackedLfsBaseNames.Contains(Path.GetFileName(file)))
                {
                    filesToUnpackWithPackTool.Add(file);
                }
            }

            //unpack
            if (filesToUnpackWithPackTool.Count > 0){
                Editor.Console.Log($"Found {filesToUnpackWithPackTool.Count} file(s) to process with PACK tool. Unpacking...");
                foreach (string filePath in filesToUnpackWithPackTool)
                {
                    await UnpackPack(filePath);
                }
            }else{
                Editor.Console.Log("No files found to unpack in the target directory.");
            }

            Editor.Console.Log("Mass PACK unpack process finished.");
        }

        #endregion


        #region LFS Tool

        public static async Task<string> UnpackLfs(string lfsFilePath, bool deleteAfter = false)
        {
            Editor.Console.Log($"Attempting to uncompress LFS file: {Path.GetFileName(lfsFilePath)}");
            if (string.IsNullOrEmpty(Globals.ToolPathLFS)){
                Editor.Console.Error("LFS Tool path is not configured. Please set it in the options menu.");
                return null;
            }

            bool success = await RunTool(Globals.ToolPathLFS, lfsFilePath);
            if (success)
            {
                string unpackedPath = lfsFilePath.Substring(0, lfsFilePath.Length - ".lfs".Length);
                if (File.Exists(unpackedPath)){
                    if (deleteAfter){
                        try{
                            File.Delete(lfsFilePath);
                            Editor.Console.Log($"Deleted original file: {Path.GetFileName(lfsFilePath)}");
                        }catch (Exception ex){
                            Editor.Console.Warning($"Failed to delete original LFS file: {ex.Message}");
                        }
                    }

                    return unpackedPath;
                }
            }
            return null;
        }

        public static async Task<string> RepackLfs(string filePath)
        {
            Editor.Console.Log($"Attempting to compress file to LFS: {Path.GetFileName(filePath)}");
            if (string.IsNullOrEmpty(Globals.ToolPathLFS)){
                Editor.Console.Error("LFS Tool path is not configured. Please set it in the options menu.");
                return null;
            }

            bool success = await RunTool(Globals.ToolPathLFS, filePath);
            if (success)
            {
                string repackedPath = filePath + ".lfs";
                if (File.Exists(repackedPath))
                {
                    return repackedPath;
                }
            }
            return null;
        }

        #endregion


        #region UDAS TOOL
        public static async Task RepackCurrentRoomUdas(){
            if (DataBase.SelectedRoom == null){
                Editor.Console.Warning("No room is currently loaded. Cannot perform repack.");
                return;
            }

            if (string.IsNullOrEmpty(Globals.ToolPathUDAS)){
                Editor.Console.Error("'UDAS Tool' path is not assigned. Please set it in the 'settings' menu.");
                return;
            }

            string roomDirectory = Utils.GetCurrentRoomDirectory();
            if (roomDirectory == null){
                Editor.Console.Error($"Could not find the path for the current room. Repack aborted.");
                return;
            }


            //get room unpacked folder
            string parentDirectory = Directory.GetParent(roomDirectory).FullName;
            //get room folder name/id
            string roomName = new DirectoryInfo(roomDirectory).Name;

            //build idx path
            string idxJPath = Path.Combine(parentDirectory, roomName + ".idxJ"); //for jader's tool
            string idxPath = Path.Combine(parentDirectory, roomName + ".idx");

            string targetIdxFile = null;

            //try to find idxJ or idx
            if (File.Exists(idxJPath)){
                targetIdxFile = idxJPath;
            }else if (File.Exists(idxPath)){
                targetIdxFile = idxPath;
            }

            if (targetIdxFile != null){
                Editor.Console.Log($"Found file: {Path.GetFileName(targetIdxFile)}. Attempting to repack...");
                await RepackFolderAsync(Globals.ToolPathUDAS, targetIdxFile);
            }else{
                Editor.Console.Error($"Could not find .idxJ or .idx file for repacking in '{parentDirectory}'. Repack cancelled.");
            }
        }

        public static async Task UnpackRoomUdas()
        {
            //TODO
            //add some a dialog window to select desired room here
            string roomDirectory = Utils.GetCurrentRoomDirectory(); //gets "current" room for now but this is kinda useless

            if (string.IsNullOrEmpty(Globals.ToolPathUDAS))
            {
                Editor.Console.Error("UDAS Tool path is not configured. Please set it in the options menu.");
                return;
            }
            if (roomDirectory != null)
            {
                string roomName = new DirectoryInfo(roomDirectory).Name;
                string stageDirectory = Directory.GetParent(roomDirectory).FullName;
                string udasPath = Path.Combine(stageDirectory, roomName + ".udas");

                if (File.Exists(udasPath))
                {
                    await UnpackFileAsync(Globals.ToolPathUDAS, udasPath);
                }
                else
                {
                    Editor.Console.Error($"Could not find .udas file at '{udasPath}'. Unpack aborted.");
                }
            }
             else
             {
                 Editor.Console.Error($"Could not find the directory for the current room. Unpack aborted.");
             }
        }

        public static async Task UnpackAllRoomsUdas(bool deleteLFS = true)
        {
            if (string.IsNullOrEmpty(Globals.ToolPathUDAS)){
                Editor.Console.Error("DAT/UDAS Tool path is not configured. Please set it in the options menu.");
                return;
            }

            EditorRe4Ver gameVersion = Globals.PreferredVersion;

            //get game version and base path
            string rootGamePath = "";
            bool isUhd = false;
            bool isPs4NsAdapted = false;

            switch (gameVersion)
            {
                case EditorRe4Ver.UHD:
                    rootGamePath = Globals.DirectoryUHDRE4;
                    isUhd = true;
                    isPs4NsAdapted = false;
                    break;
                case EditorRe4Ver.SourceNext2007:
                    rootGamePath = Globals.Directory2007RE4;
                    isUhd = false;
                    isPs4NsAdapted = false;
                    break;
                case EditorRe4Ver.PS4NS:
                    rootGamePath = Globals.DirectoryPS4NSRE4;
                    isUhd = true;
                    isPs4NsAdapted = true;
                    break;
                default:
                    Editor.Console.Warning($"The game version '{gameVersion}' is not supported for auto-unpacking files.");
                    return;
            }

            //find the room directory (combine root game path + /bio4 for UHD)
            string basePath = (gameVersion == EditorRe4Ver.UHD || gameVersion == EditorRe4Ver.PS4NS)
            ? Path.Combine(rootGamePath, "BIO4")
            : rootGamePath;

            Editor.Console.Log($"Starting mass unpack of all DAT/UDAS files in '{basePath}'...");

            //loop through stage folders st0-7
            for (int i = 0; i <= 7; i++)
            {
                string stagePath = Path.Combine(basePath, "St" + i);
                if (!Directory.Exists(stagePath)) {
                    // 2007 stage folders are compressed as .dat
                    if (gameVersion == EditorRe4Ver.SourceNext2007)
                    {
                        if (i == 0 || i > 5) continue; //2007 st only go from 1 to 5

                        if (string.IsNullOrEmpty(Globals.ToolPathGCA)){
                            Editor.Console.Error($"GCA Tool path is not configured. Unpacked stage file '{stagePath}' will be skipped...");
                            continue;
                        }

                        //convert stagepath to stagefile (add ".dat" at the end)
                        string stageFile = stagePath + ".dat";
                        Editor.Console.Log($"Unpack DAT stage file '{stageFile}'...");
                        await UnpackFileAsync(Globals.ToolPathGCA, stageFile);
                    }else{
                        continue;
                    }
                }


                string[] lfsFiles = Array.Empty<string>();
                string[] udasFiles = Array.Empty<string>();
                if (gameVersion == EditorRe4Ver.UHD || gameVersion == EditorRe4Ver.PS4NS){
                    //UHD
                    lfsFiles = Directory.GetFiles(stagePath, "*.udas.lfs", SearchOption.TopDirectoryOnly);
                    udasFiles = Directory.GetFiles(stagePath, "*.udas", SearchOption.TopDirectoryOnly);
                }else{
                    //2007
                    udasFiles = Directory.GetFiles(stagePath, "*.dat", SearchOption.TopDirectoryOnly);
                }

                if (lfsFiles.Length == 0 && udasFiles.Length == 0) { continue; }

                Editor.Console.Log($"Found rooms in St{i}. Processing...");

                //.LFS udas
                foreach (string lfsPath in lfsFiles)
                {
                    string unpackedUdasPath = await UnpackLfs(lfsPath, deleteLFS);
                    if (unpackedUdasPath != null)
                    {
                        await UnpackFileAsync(Globals.ToolPathUDAS, unpackedUdasPath);
                    }
                }

                //raw udas
                var lfsBaseNames = new HashSet<string>(lfsFiles.Select(f => Path.GetFileNameWithoutExtension(f)));
                foreach (string udasPath in udasFiles)
                {
                    if (!lfsBaseNames.Contains(Path.GetFileName(udasPath)))
                    {
                        await UnpackFileAsync(Globals.ToolPathUDAS, udasPath);
                    }
                }
            }

            Editor.Console.Log("Mass room unpack process finished.");
        }

        #endregion
    }
}