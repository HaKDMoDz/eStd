namespace System.DrawingEx
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public struct RoundedRectangle
    {
        #pragma warning disable 649
        public static readonly RoundedRectangle Empty;

        #pragma warning restore 649

        #region Constructeurs

        /// <summary>
        /// Initialise un rectangle avec les coins arrondis dont le point supérieur gauche
        /// aura pour coordonnées (<i>x</i>,<i>y</i>), comme largeur/hauteur <i>width</i> et <i>height</i>, des arrondis
        /// aux coins <i>rc</i> et enfin les arrondis auront comme rayon <i>radius</i>
        /// </summary>
        /// <param name="x">Abscisse du point supérieur gauche</param>
        /// <param name="y">Ordonnée du point supérieur gauche</param>
        /// <param name="width">Largeur du rectangle</param>
        /// <param name="height">Hauteur du rectangle</param>
        /// <param name="rc">Coins arrondis</param>
        /// <param name="radius">Rayon des arrondis</param>
        public RoundedRectangle(int x, int y, int width, int height, RoundedCorner rc, float radius)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.radius = radius;
            this.roundedCorners = rc;
        }

        /// <summary>
        /// Initialise un rectangle à partir du Rectangle <i>rect</i>, des arrondis
        /// aux coins <i>rc</i> et enfin les arrondis auront comme rayon <i>radius</i>
        /// </summary>
        /// <param name="rect">Rectangle de base</param>
        /// <param name="rc">Coins arrondis</param>
        /// <param name="radius">Rayon des arrondis</param>
        public RoundedRectangle(Rectangle rect, RoundedCorner rc, float radius)
        {
            this.x = rect.X;
            this.y = rect.Y;
            this.width = rect.Width;
            this.height = rect.Height;
            this.radius = radius;
            this.roundedCorners = rc;
        }

        /// <summary>
        /// Initialise un rectangle avec les coins arrondis dont le point supérieur gauche
        /// aura pour coordonnées <i>location</i>, comme taille <i>size</i>, des arrondis
        /// aux coins <i>rc</i> et enfin les arrondis auront comme rayon <i>radius</i>
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="rc">Coins arrondis</param>
        /// <param name="radius">Rayon des arrondis</param>
        public RoundedRectangle(Point location, Size size, RoundedCorner rc, float radius)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
            this.radius = radius;
            this.roundedCorners = rc;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou défini une enumération indiquant les coins arrondis
        /// </summary>
        public RoundedCorner RoundedCorners
        {
            get
            {
                return this.roundedCorners;
            }
            set
            {
                this.roundedCorners = value;
            }
        }

        /// <summary>
        /// Obtient ou défini la taille du rectangle
        /// </summary>
        public Size Size
        {
            get
            {
                return new Size(this.width, this.height);
            }
            set
            {
                this.width = value.Width;
                this.height = value.Height;
            }
        }

        /// <summary>
        /// Obtient ou défini l'abscisse du coin supérieur gauche du rectangle
        /// </summary>
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Obtient ou défini l'ordonnée du coin supérieur gauche du rectangle
        /// </summary>
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// Obtient ou défini la largeur du rectangle
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Obtient ou défini la hauteur du rectangle
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        /// <summary>
        /// Obtient l'abscisse des coins de gauche
        /// </summary>
        public int Left
        {
            get
            {
                return this.x;
            }
        }

        /// <summary>
        /// Obtient l'abscisse des coins de droit
        /// </summary>
        public int Right
        {
            get
            {
                return this.x + this.width;
            }
        }

        /// <summary>
        /// Obtient l'ordonnée des coins du haut
        /// </summary>
        public int Top
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// Obtient l'ordonnée des coins du bas
        /// </summary>
        public int Bottom
        {
            get
            {
                return this.y + this.height;
            }
        }

        /// <summary>
        /// Obtient ou défini l'emplacement du coin supérieur gauche
        /// </summary>
        public Point Location
        {
            get
            {
                return new Point(this.x, this.y);
            }
            set
            {
                this.x = value.X;
                this.y = value.Y;
            }
        }

        /// <summary>
        /// Obtient ou défini le rayon de courbure des coins
        /// </summary>
        public float Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = value;
            }
        }

        #endregion

        #region Méthodes

        /// <summary>
        /// Agrandi le rectangle de la taille spécifiée
        /// </summary>
        /// <param name="width">largeur</param>
        /// <param name="height">hauteur</param>
        public void Inflate(int width, int height)
        {
            this.x -= width;
            this.y -= height;
            this.width += (2 * width);
            this.height += (2 * height);
        }

        /// <summary>
        /// Agrandi le rectangle de la taille spécifiée
        /// </summary>
        /// <param name="size">taille</param>
        public void Inflate(Size size)
        {
            this.Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// Crée et retourne une copie RoundedRectangle agrandi de la taille spécifié.
        /// </summary>
        /// <param name="rrect">RoundedRectangle</param>
        /// <param name="width">Largeur</param>
        /// <param name="height">Hauteur</param>
        /// <returns>Une copie du RoundedRectangle agrandie</returns>
        public static RoundedRectangle Inflate(RoundedRectangle rrect, int width, int height)
        {
            RoundedRectangle r = rrect;
            r.Inflate(width, height);
            return r;
        }

        /// <summary>
        /// Retourne un RoundRectangle correspondant à l'intersection des RoundRectangles a et b
        /// </summary>
        /// <param name="a">RoundedRectangle</param>
        /// <param name="b">RoundedRectangle</param>
        /// <param name="rc">Coins à arrondir</param>
        /// <param name="radius">Rayon de coubure des arrondis</param>
        /// <returns></returns>
        /// <remarks>Si aucune intersection n'est trouvée entre les RoundRectangles, un RoundedRectangle est retourné</remarks>
        public static RoundedRectangle Intersect(RoundedRectangle a, RoundedRectangle b, RoundedCorner rc, float radius)
        {
            int left = Math.Max(a.Left, b.Left);
            int right = Math.Min(a.Right, b.Right);
            int top = Math.Max(a.Top, b.Top);
            int bottom = Math.Min(a.Bottom, b.Bottom);

            if (left <= right && top <= bottom)
            {
                return new RoundedRectangle(
                    left, right - left, top, bottom - top, rc, radius);
            }
            return Empty;
        }

        /// <summary>
        /// Replace ce RoundedRectangle par l'intersection de celui-ci de rrect
        /// </summary>
        /// <param name="rrect">RoundedRectangle</param>
        public void Intersect(RoundedRectangle rrect)
        {
            RoundedRectangle r = Intersect(rrect, this, this.roundedCorners, this.radius);
            this.x = r.x;
            this.y = r.y;
            this.width = r.width;
            this.height = r.height;
        }

        /// <summary>
        /// Retourne <i>true</i> si ce RoundedRectangle possède une intersection avec rrect, <i>false</i> sinon
        /// </summary>
        /// <param name="rrect">RoundedRectangle</param>
        /// <returns><i>true</i> si intersection, <i>false</i> sinon</returns>
        public bool IntersectWith(RoundedRectangle rrect)
        {
            return ((rrect.Left < this.Right && this.Left < rrect.Right) &&
                    (rrect.Top < this.Bottom && this.Top < rrect.Bottom));
            //if (((rect.X < (this.X + this.Width)) && (this.X < (rect.X + rect.Width))) && (rect.Y < (this.Y + this.Height)))
            //{
            //    return (this.Y < (rect.Y + rect.Height));
            //}
            //return false;
        }

        /// <summary>
        /// Retourne un RoundedRectangle qui correspond à l'intersection des RoundedRectangle a et b et dont les
        /// coins <i>rc</i> sont arrondis d'un rayon de <i>radius</i>
        /// </summary>
        /// <param name="a">RoundedRectangle</param>
        /// <param name="b">RoundedRectangle</param>
        /// <param name="rc">Coins à arrondir</param>
        /// <param name="radius">Rayon de courbure des arrondis</param>
        /// <returns>RoundedRectangle de l'union</returns>
        public static RoundedRectangle Union(RoundedRectangle a, RoundedRectangle b, RoundedCorner rc, float radius)
        {
            int left = Math.Min(a.Left, b.Left);
            int right = Math.Max(a.Right, b.Right);
            int top = Math.Min(a.Top, b.Top);
            int bottom = Math.Max(a.Bottom, b.Bottom);

            return new RoundedRectangle(
                left, right - left, top, bottom - top, rc, radius);
        }

        /// <summary>
        /// Déplace le RoundedRectangle d'après les coordonnées spécifiés
        /// </summary>
        /// <param name="x">Déplacement sur l'axe des abscisses</param>
        /// <param name="y">Déplacement sur l'axe des ordonées</param>
        public void Offset(int x, int y)
        {
            this.x += x;
            this.y += y;
        }

        /// <summary>
        /// Déplace le RoundedRectangle d'après les ccordonnées spécifiées
        /// </summary>
        /// <param name="point">Point contenant les informations de déplacements</param>
        public void Offset(Point point)
        {
            this.x += point.X;
            this.y += point.Y;
        }

        /// <summary>
        /// Retourne un booléen indiquant si le point spécifié fait partie du RoundedRectangle
        /// </summary>
        /// <param name="x">Abscisse</param>
        /// <param name="y">Ordonnée</param>
        /// <returns><i>true</i> si le point se trouve à l'intérieur du RoundedRectangle, <i>false</i> sinon</returns>
        public bool Contains(int x, int y)
        {
            return (
                    this.x <= x &&
                    x <= this.Right &&
                    this.y <= y &&
                    y <= this.Bottom);
        }

        /// <summary>
        /// Retourne un booléen indiquant si le point spécifié fait partie du RoundedRectangle
        /// </summary>
        /// <param name="pt">Point à vérifier</param>
        /// <returns><i>true</i> si le point se trouve à l'intérieur du RoundedRectangle, <i>false</i> sinon</returns>
        public bool Contains(Point pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        /// <summary>
        /// Retourne un <see cref="GraphicsPath"/> représentant le RoundedRectangle.
        /// </summary>
        /// <returns><see cref="GraphicsPath"/></returns>
        /// <seealso cref="GraphicsPath"/>
        public GraphicsPath ToGraphicsPath()
        {
            GraphicsPath gp;
            gp = new GraphicsPath();

            //Rectangle baseRect = new Rectangle(this.Location, this.Size);

            // si le rayon est inférieur ou égal à 0
            // on retourne le rectangle de base
            if (this.radius <= 0F)
            {
                gp.AddRectangle(new Rectangle(this.Location, this.Size));
                gp.CloseFigure();
                return gp;
            }

            float diameter = this.radius * 2F;

            if (this.height <= this.width)
            {
                if (diameter >= this.height &&
                    (((this.roundedCorners & (RoundedCorner.BottomLeft | RoundedCorner.TopLeft)) ==
                      (RoundedCorner.BottomLeft | RoundedCorner.TopLeft)) ||
                     ((this.roundedCorners & (RoundedCorner.BottomRight | RoundedCorner.TopRight)) ==
                      (RoundedCorner.BottomRight | RoundedCorner.TopRight))))
                {
                    diameter = this.height;
                }
            }
            else
            {
                if (diameter >= this.width &&
                    (((this.roundedCorners & (RoundedCorner.BottomLeft | RoundedCorner.BottomRight)) ==
                      (RoundedCorner.BottomLeft | RoundedCorner.BottomRight)) ||
                     ((this.roundedCorners & (RoundedCorner.TopLeft | RoundedCorner.TopRight)) ==
                      (RoundedCorner.TopLeft | RoundedCorner.TopRight))))
                {
                    diameter = this.width;
                }
            }
            var size = new SizeF(diameter, diameter);
            var arc = new RectangleF(this.Location, size);

            // arc en haut à gauche
            if ((this.roundedCorners & RoundedCorner.TopLeft) == RoundedCorner.TopLeft)
            {
                gp.AddArc(arc, 180, 90);
            }
            else
            {
                gp.AddLine(new PointF(arc.Left, arc.Top), new PointF(arc.Left, arc.Top));
            }

            // arc en haut à droite
            arc.X = this.Right - diameter;
            if ((this.roundedCorners & RoundedCorner.TopRight) == RoundedCorner.TopRight)
            {
                gp.AddArc(arc, 270, 90);
            }
            else
            {
                gp.AddLine(new PointF(arc.Right, arc.Top), new PointF(arc.Right, arc.Top));
            }

            arc.Y = this.Bottom - diameter;

            // arc en bas à droite
            if ((this.roundedCorners & RoundedCorner.BottomRight) == RoundedCorner.BottomRight)
            {
                gp.AddArc(arc, 0, 90);
            }
            else
            {
                gp.AddLine(new PointF(arc.Right, arc.Bottom), new PointF(arc.Right, arc.Bottom));
            }

            // arc en bas à gauche
            arc.X = this.Left;
            if ((this.roundedCorners & RoundedCorner.BottomLeft) == RoundedCorner.BottomLeft)
            {
                gp.AddArc(arc, 90, 90);
            }
            else
            {
                gp.AddLine(new PointF(arc.Left, arc.Bottom), new PointF(arc.Left, arc.Bottom));
            }

            gp.CloseFigure();

            return gp;
        }

        /// <summary>
        /// Retourne un <see cref="Rectangle"/>
        /// </summary>
        /// <returns><see cref="Rectangle"/></returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(this.Location, this.Size);
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("{{X={0},Y={1},Width={2},Height={3},Radius={4}}}",
                this.x,
                this.y,
                this.width,
                this.height,
                this.radius);
        }

        public static bool operator ==(RoundedRectangle a, RoundedRectangle b)
        {
            return (a.x == b.x &&
                    a.y == b.y &&
                    a.width == b.width &&
                    a.height == b.height);
        }

        public static bool operator !=(RoundedRectangle a, RoundedRectangle b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj is RoundedRectangle)
            {
                var r = (RoundedRectangle)obj;
                ret = (r.x == this.x && r.y == this.y &&
                       r.width == this.width && r.height == this.height);
            }
            return ret;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        private int height;
        private float radius;
        private RoundedCorner roundedCorners;
        private int width;
        private int x;
        private int y;
    }

    [Flags]
    public enum RoundedCorner
    {
        TopLeft = 2,
        TopRight = 4,
        BottomLeft = 8,
        BottomRight = 16,
        All = TopLeft | TopRight | BottomLeft | BottomRight
    }
}