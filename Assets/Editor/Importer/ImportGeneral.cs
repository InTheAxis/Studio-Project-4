using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ImportGeneral 
{

    public void ImportFile(string inputFilePath, string targetFileDir, bool isOverwrite, ref List<string> exportAssetNames)
    {

        int pos = inputFilePath.LastIndexOf('/');
        string name = inputFilePath.Substring(pos + 1, inputFilePath.Length - pos - 1);
        string output = targetFileDir + "/";


        /* Copy custom layout directory */
        if (!Directory.Exists(output))
            Directory.CreateDirectory(output);

        output += name;
        /* Overwrite warning */
        if (!isOverwrite && System.IO.File.Exists(output))
        {
            Debug.Log(name + " already exists! Set overwrite to true to replace!");
            return;
        }




        /* Copy/Replace File */
        if (!isOverwrite)
            FileUtil.CopyFileOrDirectory(inputFilePath, output);
        else
            FileUtil.ReplaceFile(inputFilePath, output);

        /* Add to list of assets to export */
        exportAssetNames.Add(getRelativePath(output));

        Debug.Log("[Importer] " + name);
    }

    public void ImportDir(string inputFilePath, string targetFileDir, bool isOverwrite, bool isRecursiveImport, bool isLayoutCopied, ref List<string> exportAssetNames)
    {
        inputFilePath = inputFilePath.Replace('/', '\\');
        DirectoryInfo dir = new DirectoryInfo(inputFilePath);
        FileInfo[] info;
        
        if(isRecursiveImport)
           info = dir.GetFiles("*.*", SearchOption.AllDirectories);
        else
           info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            string layoutCategory = f.DirectoryName;
            layoutCategory = layoutCategory.Replace(inputFilePath, "");
            layoutCategory = layoutCategory.Replace('\\', '/');

            string output = targetFileDir;

            if (isLayoutCopied && layoutCategory != "")
                output += layoutCategory + "/";
            else
                output += "/";


            /* Copy custom layout directory */
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);


            output += f.Name;

            /* Overwrite warning */
            if (!isOverwrite && System.IO.File.Exists(output))
            {
                Debug.Log(f.Name + " already exists! Set overwrite to true to replace!");
                continue;
            }


            /* Copy/Replace File */
            if (!isOverwrite)
                FileUtil.CopyFileOrDirectory(f.ToString(), output);
            else
                FileUtil.ReplaceFile(f.ToString(), output);

            /* Add to list of assets to export */
            exportAssetNames.Add(getRelativePath(output));


            Debug.Log("[Importer] " + f.Name);

        }

        Debug.Log("[Importer] Completed!");
    }

    private string getRelativePath(string line)
    {
        int pos = line.IndexOf("Assets");
        return line.Substring(pos, line.Length - pos);
    }

    
}
