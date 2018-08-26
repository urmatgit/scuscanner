
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCUScanner.Helpers
{
    public class CSVParser

    {
        public List<Part> Parts { get; private set; } = new List<Part>();
        public List<Email> Emails { get; private set; } = new List<Email>();
        public CSVParser(string path, string emailpath)
        {
            ReadAndParcePart(path);
            Debug.WriteLine($"Parts-{Parts.Count}");
            ReadAndParceEmail(emailpath);
            Debug.WriteLine($"Emails-{Emails.Count}");

        }
        private void ReadAndParceEmail(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            Emails = File.ReadAllLines(path)
                .Select(l => FromLineEmailCSV(l))
                .ToList();
        }
        private void ReadAndParcePart(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))  return;

                Parts = File.ReadAllLines(path)
                    .Skip(1)
                    .Select(l => FromLinePartCSV(l))
                    .ToList();
        }
        private Email FromLineEmailCSV(string l)
        {
            string[] values = l.Split(',');
            Email email = new Email();
            email.BB = values[0];
            email.email = values[1];
            return email;
        }
        private Part FromLinePartCSV(string l)
        {
            string[] values = l.Split(',');
            Part part = new Part();
            part.ID = Convert.ToInt32(values[0]);
            part.PartName = values[1];
            part.PartNumber = values[2];
            part.IssueDate = values[3];

            part.UpperPixel = Convert.ToInt32 (values[4]);

            part.LowerPixel = Convert.ToInt32(values[5]);
            if (part.UpperPixel > part.LowerPixel)
            {
                var tmp = part.UpperPixel;
                part.UpperPixel = part.LowerPixel;
                part.LowerPixel = tmp;
            }
            part.LeftPixel = Convert.ToInt32(values[6]);
            part.RightPixel = Convert.ToInt32(values[7]);
            if (part.LeftPixel > part.RightPixel)
            {
                var tmp = part.LeftPixel;
                part.LeftPixel = part.RightPixel;
                part.RightPixel = tmp;
            }
            part.Rect = new    SkiaSharp.SKRect( part.LeftPixel, part.UpperPixel, part.RightPixel, part.LowerPixel);
            part.OrgRect =    new SkiaSharp.SKRect( part.LeftPixel, part.UpperPixel, part.RightPixel , part.LowerPixel);
            return part;
        }
        public Part[] CheckContainInRect(float x, float y)
        {
            List<Part> parts = new List<Part>();
            if (Parts == null || Parts.Count == 0) return parts.ToArray();
            foreach(Part part in Parts)
            {
                if (part.Rect.Contains(x, y))
                    parts.Add(part);
            }
            return parts.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">point x</param>
        /// <param name="y"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <returns></returns>
        public Part[] CheckContainInRect(float x, float y, float dx, float dy, float tx, float ty)
        {
            List<Part> parts = new List<Part>();
            if (Parts == null || Parts.Count == 0) return parts.ToArray();
            foreach (Part part in Parts)
            {

                SkiaSharp.SKRect rec = new SkiaSharp.SKRect(part.Rect.Left * dx + tx, part.Rect.Top * dx + tx, part.Rect.Right * dx, part.Rect.Bottom * dy);
                if (rec.Contains(x,y))
                {
                    parts.Add(part);
                }
                //if (part.Rect.Contains(x, y))
                //    parts.Add(part);
            }
            return parts.ToArray();
        }
    }
}
