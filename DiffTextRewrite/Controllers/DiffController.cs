using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DifferenceEngine;
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

            var stream1 = new StreamReader(file1.OpenReadStream());
            var stream2 = new StreamReader(file2.OpenReadStream());

            DiffList_TextFile sLF = null;
            DiffList_TextFile dLF = null;

            try
            {
                sLF = new DiffList_TextFile(stream1);
                dLF = new DiffList_TextFile(stream2);
                double time = 0;
                DiffEngine de = new DiffEngine();
                time = de.ProcessDiff(sLF, dLF, DiffEngineLevel.SlowPerfect);
                int cnt = 1;
                int i;
                ArrayList rep = de.DiffReport();
                foreach (DiffResultSpan drs in rep)
                {
                    switch (drs.Status)
                    {
                        case DiffResultSpanStatus.DeleteSource:
                            for (i = 0; i < drs.Length; i++)
                            {
                                diffTextModel.left.Add("<div class=\"deleted\">" + ((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line + "</div><br>");
                                cnt++;
                            }

                            break;
                        case DiffResultSpanStatus.NoChange:
                            for (i = 0; i < drs.Length; i++)
                            {
                                diffTextModel.left.Add(((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line + "<br>");
                                diffTextModel.right.Add(((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line + "<br>");
                                cnt++;
                            }

                            break;
                        case DiffResultSpanStatus.AddDestination:
                            for (i = 0; i < drs.Length; i++)
                            {
                                diffTextModel.right.Add("<div class=\"added\">" + ((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line + "</div><br>");
                                cnt++;
                            }

                            break;
                        case DiffResultSpanStatus.Replace:
                            for (i = 0; i < drs.Length; i++)
                            {
                                diffTextModel.left.Add("<div class=\"deleted\">" + ((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line + "</div><br>");
                                diffTextModel.right.Add("<div class=\"added\">" + ((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line + "</div><br>");
                                cnt++;
                            }

                            break;
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