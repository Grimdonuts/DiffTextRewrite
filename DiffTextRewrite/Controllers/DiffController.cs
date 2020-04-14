using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DiffTextRewrite.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DiffTextRewrite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        [HttpPost]
        public DiffModel Post()
        {
            DiffModel diffTextModel = new DiffModel();
            diffTextModel.left = new List<string>();
            diffTextModel.right = new List<string>();
            var file1 = Request.Form.Files[0];
            var file2 = Request.Form.Files[1];
            var notHtml = true;
            if (file1.FileName.Contains("html") && file2.FileName.Contains("html"))
            {
                notHtml = false;
            }

            var stream1 = new StreamReader(file1.OpenReadStream());
            var stream2 = new StreamReader(file2.OpenReadStream());

            List<string> lines = new List<string>();
            String line;
            int MaxLineLength = 1024;
            while ((line = stream1.ReadLine()) != null)
            {
                if (line.Length > MaxLineLength)
                {
                    throw new InvalidOperationException(
                        string.Format("File contains a line greater than {0} characters.",
                        MaxLineLength.ToString()));
                }
                lines.Add(line + "\r\n");
            }
            string originalDoc = string.Join(" ", lines);

            lines = new List<string>();
            while ((line = stream2.ReadLine()) != null)
            {
                if (line.Length > MaxLineLength)
                {
                    throw new InvalidOperationException(
                        string.Format("File contains a line greater than {0} characters.",
                        MaxLineLength.ToString()));
                }
                lines.Add(line + "\r\n");
            }
            string modifiedDoc = string.Join(" ", lines);

            int[,] lookupTable = new int[originalDoc.Length + 1, modifiedDoc.Length + 1];

            lcsLength(originalDoc, modifiedDoc, originalDoc.Length, modifiedDoc.Length, lookupTable);

            diffStrings(originalDoc, modifiedDoc, originalDoc.Length, modifiedDoc.Length, lookupTable, diffTextModel);

            diffTextModel.left = string.Join("", diffTextModel.left).Replace("PIKA\rCHUPIKA\nCHU", " \r\n ")
                .Replace("PIKA", "<span class=\"deleted\">").Replace("CHU", "</span>").Split("\r\n").ToList();
            diffTextModel.right = string.Join("", diffTextModel.right).Replace("PIKA\rCHUPIKA\nCHU", " \r\n ")
                .Replace("PIKA", "<span class=\"added\">").Replace("CHU", "</span>").Split("\r\n").ToList();

            if (notHtml)
            {
                for (int i = 0; i < diffTextModel.left.Count; i++)
                {
                    diffTextModel.left[i] = diffTextModel.left[i] + "<br>";
                }
                for (int i = 0; i < diffTextModel.right.Count; i++)
                {
                    diffTextModel.right[i] = diffTextModel.right[i] + "<br>";
                }
            }
            return diffTextModel;
        }


        public static void diffStrings(string orig, string mod, int origLength, int modLength, int[,] lookupTable, DiffModel diffTextModel)
        {
            if (origLength > 0 && modLength > 0 && orig[origLength - 1] == mod[modLength - 1])
            {
                diffStrings(orig, mod, origLength - 1, modLength - 1, lookupTable, diffTextModel);
                diffTextModel.right.Add(""+ mod[modLength - 1]);
                diffTextModel.left.Add("" + orig[origLength - 1]);
            }
            else if (modLength > 0 && (origLength == 0 || lookupTable[origLength, modLength - 1] >= lookupTable[origLength - 1, modLength]))
            {
                diffStrings(orig, mod, origLength, modLength - 1, lookupTable, diffTextModel);
                diffTextModel.right.Add("PIKA" + mod[modLength - 1] + "CHU");
            }
            else if (origLength > 0 && (modLength == 0 || lookupTable[origLength, modLength - 1] < lookupTable[origLength - 1, modLength]))
            {
                diffStrings(orig, mod, origLength - 1, modLength, lookupTable, diffTextModel);
                diffTextModel.left.Add("PIKA" + orig[origLength - 1] + "CHU");
            }
        }

        public static void lcsLength(string orig, string mod, int origLength, int modLength, int[,] lookupTable)
        {
            for (int i = 0; i <= origLength; i++)
            {
                lookupTable[i, 0] = 0;
            }

            for (int j = 0; j <= modLength; j++)
            {
                lookupTable[0, j] = 0;
            }

            for (int i = 1; i <= origLength; i++)
            {
                for (int j = 1; j <= modLength; j++)
                {
                    if (orig[i - 1] == mod[j - 1])
                    {
                        lookupTable[i, j] = lookupTable[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        lookupTable[i, j] = Math.Max(lookupTable[i - 1, j], lookupTable[i, j - 1]);
                    }
                }
            }
        }
    }
}