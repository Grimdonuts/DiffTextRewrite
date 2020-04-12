﻿using System;
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
                lines.Add(line);
            }
            List<string> sLF = lines;

            lines = new List<string>();
            while ((line = stream2.ReadLine()) != null)
            {
                if (line.Length > MaxLineLength)
                {
                    throw new InvalidOperationException(
                        string.Format("File contains a line greater than {0} characters.",
                        MaxLineLength.ToString()));
                }
                lines.Add(line);
            }
            List<string> dLF = lines;

            try
            {
                for (int j = 0; j < sLF.Count; j++)
                {
                    var sourceLine = sLF[j];
                    for (int k = 0; k < dLF.Count; k++)
                    {
                        var destLine = dLF[k];
                        if (k == j)
                        {
                            if (sourceLine == destLine)
                            {
                                if (notHtml)
                                {
                                    //No Change
                                    diffTextModel.left.Add(sourceLine + "<br>");
                                    diffTextModel.right.Add(destLine + "<br>");
                                }
                                else
                                {
                                    //No Change
                                    diffTextModel.left.Add(sourceLine);
                                    diffTextModel.right.Add(destLine);
                                }
                            }
                            else
                            {
                                //Replace
                                diffTextModel.left.Add("<div class=\"deleted\">" + sourceLine + "</div>");
                                diffTextModel.right.Add("<div class=\"added\">" + destLine + "</div>");
                            }
                        }
                    }
                    if (sLF.Count > dLF.Count)
                    {
                        if (j > (dLF.Count - 1))
                        {
                            //Delete
                            diffTextModel.left.Add("<div class=\"deleted\">" + sourceLine + "</div>");
                        }
                    }
                }

                for (int j = 0; j < dLF.Count; j++)
                {
                    var destLine = dLF[j];
                    if (sLF.Count < dLF.Count)
                    {
                        if (j > (sLF.Count - 1))
                        {
                            //Add
                            diffTextModel.right.Add("<div class=\"added\">" + destLine + "</div>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return diffTextModel;
        }
    }
}