namespace LatticeHingeCalculator
{
    /// <summary>
    /// See <see href="https://www.defproc.co.uk/analysis/lattice-hinge-design-minimum-bend-radius/">Deferred Procrastination.co.uk</see> for more details on math
    /// </summary>
    public static class Calculator
    {
        public const double DEFAULT_LASER_KERF = 0.2;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="oa">Total bend angle of the piece (degrees)</param>
        /// <param name="cc">Number of columns of torsional links</param>
        /// <returns><c>k</c> = Clearance gap (mm)</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static double GetLinkClearance(double t, double oa, int cc)
        {
            if (t <= 0)
                throw new IndexOutOfRangeException("Torsional link width must be greater than zero");
            if (cc <= 0)
                throw new IndexOutOfRangeException("Torsional column count must be greater than zero");

            return -t + ((2*(Math.Sqrt(Math.Pow(t, 2) / 2))) * Math.Cos((Math.PI / 4) - (oa / cc)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="oa">Total bend angle of the piece (degrees)</param>
        /// <param name="k_laser">Laser kerf (mm)</param>
        /// <returns><c>cc</c> = Number of columns of torsional links</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static int GetTorsionalLinks(double t, double oa, double k_laser = DEFAULT_LASER_KERF)
        {
            if (t <= 0)
                throw new IndexOutOfRangeException("Torsional link width must be greater than zero");
            if (k_laser <= 0)
                throw new IndexOutOfRangeException("Laser kerf must be greater than zero");
            return (int)((oa * (t + k_laser)) / (2 * Math.PI * (t + k_laser)));
            var a = GetTwistAngle(t, k_laser);
            return (int)(oa / a);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="cc">Number of columns of torsional links</param>
        /// <returns><c>W</c> = Total hinge width (mm)</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static double GetTotalHingeWidth(double t, int cc, double k_laser = 0.2)
        {
            if (cc <= 0)
                throw new IndexOutOfRangeException("Number of torsional links must be greater than zero");
            return t * cc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="oa">Total bend angle of the piece (degrees)</param>
        /// <param name="n">Number of columns of torsional links</param>
        /// <returns>Inner radius</returns>
        public static double GetInnerRadius(double t, double oa, int n)
        {
            return GetTotalHingeWidth(t, n) / (oa * (Math.PI / 180));
            return GetOuterRadius(t, oa, n) - t;
            return ((2 * GetTotalHingeWidth(t, n)) / oa) - (t / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="oa">Total bend angle of the piece (degrees)</param>
        /// <param name="n">Number of columns of torsional links</param>
        /// <returns>Outer radius</returns>
        public static double GetOuterRadius(double t, double oa, int n)
        {
            return GetInnerRadius(t, oa, n) + t;
            return (GetTotalHingeWidth(t, n) / 2) / Math.Sin(oa / 2);
            return ((2 * GetTotalHingeWidth(t, n)) / oa) + (t / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h">Overall height (mm)</param>
        /// <param name="l">Connected length (mm)</param>
        /// <param name="t">Material thickness (mm)</param>
        /// <returns><c>rc</c> = Row Count</returns>
        public static int GetRowCount(double h, double l, double t)
        {
            return (int)(h / (l + t));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h">Overall height (mm)</param>
        /// <param name="l">Connected length (mm)</param>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="rc">Row Count</param>
        /// <returns><c>sl</c> = Segment Length</returns>
        public static double GetSegmentLength(double h, double l, double t, int rc)
        {
            return (h / rc) - (t / 2);
            var oddLength = (2 * l) + (t / 2);
            return  (h - ((rc - 1) * oddLength)) / rc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">Material thickness (mm)</param>
        /// <param name="k">Link clearance (mm)</param>
        /// <returns><c>a</c> = Angle of twist per link (degrees)</returns>
        public static double GetTwistAngle(double t, double k = 0.2)
        {
            var radians = Math.Acos((k + t) / (2 * Math.Sqrt(Math.Pow(t, 2) / 2)));
            return ((Math.PI / 4) - radians) * (180.0 / Math.PI);
        }
    }
}