using System.Text;
using System.Xml;

namespace LatticeHingeCalculator
{
    public static class SvgFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="h">Overall height (mm)</param>
        /// <param name="w">Overall width (mm)</param>
        /// <param name="k">Clearence gap (mm)</param>
        /// <param name="k_laser">Laser Kerf (mm)</param>
        /// <param name="l">Connected length (mm)</param>
        /// <param name="cc">Number of columns of torsional links</param>
        /// <param name="rc">Number of rows of torsional links</param>
        /// <param name="sl">Segment length</param>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="W">Total hinge Width (mm)</param>
        /// <param name="oa">Total bend angle of the piece (degrees)</param>
        /// <param name="a">Angle of twist per link (degrees)</param>
        /// <returns></returns>
        public static XmlDocument Generate(double h, double w, double k, double k_laser, double l, int cc, int rc, double sl, double t, double W, double oa, double a)
        {
            var xDoc = new XmlDocument();
            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "UTF-8", "no"));
            xDoc.AppendChild(xDoc.CreateComment("Created with LightWorks Laser Batcher"));
            var xSvg = xDoc.CreateElement("svg");
            xSvg.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            xSvg.SetAttribute("version", "1.1");

            xSvg.SetAttribute("width", $"{w}mm");
            xSvg.SetAttribute("height", $"{h}mm");
            xSvg.SetAttribute("viewBox", $"0 0 {w} {h}");
            xSvg.SetAttribute("xmlns:svg", "http://www.w3.org/2000/svg");
            xSvg.SetAttribute("enable-background", $"new 0 0 {w} {h}");
            xDoc.AppendChild(xSvg);

            var xBox = xDoc.CreateElement("path");
            xSvg.AppendChild(xBox);
            xBox.SetAttribute("d", $"M 0,0 L{w},0 L{w},{h} L0,{h} Z");
            xBox.SetAttribute("fill", "none");
            xBox.SetAttribute("stroke", "red");
            xBox.SetAttribute("stroke-width", k_laser.ToString());

            var xPath = xDoc.CreateElement("path");
            xPath.SetAttribute("stroke", "black");
            xPath.SetAttribute("stroke-width", k_laser.ToString());
            xPath.SetAttribute("stroke-linecap", "butt");
            xSvg.AppendChild(xPath);

            var sb = new StringBuilder();
            // TODO: Outer path

            double x = (w - W) / 2,
                y = 0;

            sb.Append($"M {x},{y} ");

            bool isEdgeLink = true; // Opposite is center link
            for (int c = 0; c < cc; c++)
            {
                // Shift X,Y to start of next torsion link
                x += t;

                if (isEdgeLink)
                {
                    y = 0;
                } else
                {
                    y = t / 2;
                }

                sb.Append($"M {x},{y} ");

                for (int r = 0; r < rc; r++)
                {
                    if (isEdgeLink)
                    {
                        y += sl;
                    } else
                    {
                        if (r == rc - 1)
                            break;
                        var oddLength = (h / (rc - 1)) - t;
                        y += oddLength;
                    }
                    sb.Append($"L {x},{y} ");

                    y += t;

                    sb.Append($"M {x},{y} ");
                }

                isEdgeLink = !isEdgeLink;
            }

            xPath.SetAttribute("d", sb.ToString());

            return xDoc;
        }

        public static XmlDocument Generate(double h, double w, double t, double oa)
        {
            const double k_laser = 0.2;
            // TODO: Calculate connected length
            var idealRows = 3;

            var l = (h / idealRows) - (t / 2) - t;

            var cc = Calculator.GetTorsionalLinks(t, oa, k_laser);
            var k = Calculator.GetLinkClearance(t, oa, cc);
            var W = Calculator.GetTotalHingeWidth(t, cc);
            var rc = Calculator.GetRowCount(h, l, t);
            var sl = Calculator.GetSegmentLength(h, l, t, rc);
            var a = Calculator.GetTwistAngle(oa, rc);

            return Generate(h, w, k, k_laser, l, cc, rc, sl, t, W, oa, a);
        }
    }
}