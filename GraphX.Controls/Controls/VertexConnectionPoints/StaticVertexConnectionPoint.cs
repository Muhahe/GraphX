﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphX.PCL.Common.Enums;

namespace GraphX.Controls
{
    public class StaticVertexConnectionPoint : ContentControl, IVertexConnectionPoint
    {
        #region Common part

        /// <summary>
        /// Connector identifier
        /// </summary>
        public int Id { get; set; }


        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(VertexShape), typeof(StaticVertexConnectionPoint), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets shape form for connection point (affects math calculations for edge end placement)
        /// </summary>
        public VertexShape Shape
        {
            get { return (VertexShape)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }


        private Rect _rectangularSize;
        public Rect RectangularSize { 
            get 
            { 
                if(_rectangularSize == Rect.Empty)
                    UpdateLayout();
                return _rectangularSize;
            }
            private set { _rectangularSize = value; }
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        private static VertexControl GetVertexControl(DependencyObject parent)
        {
            while (parent != null)
            {
                var control = parent as VertexControl;
                if (control != null) return control;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        #endregion

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image",
                                                                       typeof(Image),
                                                                       typeof(StaticVertexConnectionPoint),
                                                                       new PropertyMetadata(null, ImageChangedCallback));

        private static void ImageChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var parent = dependencyObject as StaticVertexConnectionPoint;
            if (parent == null)
                throw new Exception("EdgePointerImage -> ImageChangedCallback: Parent not found!");
            parent.Content = dependencyPropertyChangedEventArgs.NewValue;
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(Path), typeof(StaticVertexConnectionPoint), new PropertyMetadata(null, PathChangedCallback));

        private static void PathChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var parent = dependencyObject as StaticVertexConnectionPoint;
            if (parent == null)
                throw new Exception("StaticVertexConnectionPoint -> PathChangedCallback: Parent not found!");
            parent.Content = dependencyPropertyChangedEventArgs.NewValue;
        }


        /// <summary>
        /// Gets or sets geometry that specifies the path to be drawn
        /// </summary>
        public Path Path { get { return (Path)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }

        /// <summary>
        /// Image for edge pointer
        /// </summary>
        public Image Image
        {
            get { return (Image)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        private VertexControl _vertexControl;
        protected VertexControl VertexControl { get { return _vertexControl ?? (_vertexControl = GetVertexControl(VisualParent)); } }

        public StaticVertexConnectionPoint()
        {
            RenderTransformOrigin = new Point(.5, .5);
            LayoutUpdated += StaticVertexConnector_LayoutUpdated;
        }

        void StaticVertexConnector_LayoutUpdated(object sender, EventArgs e)
        {
            var position = TranslatePoint(new Point(), VertexControl);
            var vPos = VertexControl.GetPosition();
            position = new Point(position.X + vPos.X, position.Y + vPos.Y);
            RectangularSize = new Rect(position, DesiredSize);
        }

        public void Update()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Scales path by provided value
        /// </summary>
        /// <param name="scale">Point scale value</param>
        public void ScalePath(Point scale)
        {
            if (Path == null || Path.Data == null) return;
            var pathGeometry = Path.Data.Clone();
            pathGeometry.Transform = new ScaleTransform(scale.X, scale.Y);
            Path.Data = pathGeometry.GetFlattenedPathGeometry();
        }

        public void Dispose()
        {
            _vertexControl = null;
        }
    }
}