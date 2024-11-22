using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

class LinesOfCode
{
    [MenuItem("CalmWaves/Compute lines of code")]
    private static void Compute()
    {
        string folderPath = Path.Combine(Application.dataPath, "Scripts");
        int totalLines = 0;

        var linesInFiles = new List<(string, int)>();
        if (Directory.Exists(folderPath))
        {
            string[] csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            foreach (string file in csFiles)
            {
                int lineCount = File.ReadAllLines(file).Length;
               
                linesInFiles.Add((file, lineCount));
            }

            linesInFiles.Sort( (a,b)=>a.Item2.CompareTo(b.Item2));
            for (int i = 0; i < linesInFiles.Count; i++)
            {
                Debug.Log($"{i+1}. {linesInFiles[i].Item1} : {linesInFiles[i].Item2}");
            }
            Debug.Log($"Total lines of code in {linesInFiles.Count} files is {linesInFiles.Sum(x=>x.Item2)}");
        }
        else
        {
            Debug.LogError("Folder not found!");
        }
    }
}
