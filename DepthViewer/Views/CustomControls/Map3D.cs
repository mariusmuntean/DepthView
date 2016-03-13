using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using Cirrious.CrossCore;
using DepthViewer.Contracts;
using DepthViewer.Models;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Camera = Urho.Camera;
using Color = Urho.Color;
using System.Threading.Tasks;
using Accord;

using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using Android.Views.Animations;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Urho.Resources;
using Urho.Shapes;
using Urho.Urho2D;
using static System.Drawing.Image;
using Environment = Android.OS.Environment;
using File = Urho.IO.File;
using FileMode = Urho.IO.FileMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Vector3 = Urho.Vector3;


namespace DepthViewer.Views.CustomControls
{
    class Map3D : Application
    {
        private Text helloText;
        private Mapping _currentMapping;
        private IDataExchangeService _dataExchangeService;
        private IDownloadCache _downloadCache;

        private Node CameraNode;
        private DebugRenderer _debugRenderer;
        private Scene _scene;
        protected float Yaw { get; set; }
        protected float Pitch { get; set; }
        protected const float TouchSensitivity = 2;

        protected override void Setup()
        {
            base.Setup();


        }

        protected override async void Start()
        {
            _dataExchangeService = Mvx.Resolve<IDataExchangeService>();
            _downloadCache = Mvx.Resolve<IDownloadCache>();

            Input.SubscribeToMultiGesture(args =>
            {
                if (CameraNode == null)
                {
                    return;
                }

                if (args.DDist < 0)
                {
                    CameraNode.Translate(-Vector3.UnitZ * 0.1f);
                }
                else
                {
                    CameraNode.Translate(Vector3.UnitZ * 0.1f);
                }
            });

            Input.KeyDown += (args) =>
            {
                if (args.Key == Key.Esc) Engine.Exit();
            };

            await CreateScene();
            //SimpleCreateInstructions("WASD");

            // Stitch some images togetcher
            await Stitch();
        }

        private async Task Stitch()
        {
            _currentMapping = _dataExchangeService.Payload["CurrentMapping"] as Mapping;
            var img1Path = await _downloadCache.GetAndCacheFile(_currentMapping.Measurements.First().ImageUrl);
            var img2Path = await _downloadCache.GetAndCacheFile(_currentMapping.Measurements.ElementAt(1).ImageUrl);

            Bitmap img1, img2;
            using (var fs = new FileStream(img1Path, System.IO.FileMode.Open))
            {
                using (var tmpBitmap = FromStream(fs) as Bitmap)
                {
                    img1 = tmpBitmap.Clone(PixelFormat.Format32bppArgb);
                }
            }
            using (var fs = new FileStream(img2Path, System.IO.FileMode.Open))
            {
                using (var tmpBitmap = FromStream(fs) as Bitmap)
                {
                    img2 = tmpBitmap.Clone(PixelFormat.Format32bppArgb);
                }
            }


            // Step 1: Detect feature points using Harris Corners Detector
            HarrisCornersDetector harris = new HarrisCornersDetector(0.04f, 1000f);


            var harrisPoints1 = harris.ProcessImage(img1).ToArray();
            var harrisPoints2 = harris.ProcessImage(img2).ToArray();

            // Show the marked points in the original images
            Bitmap img1mark = new PointsMarker(harrisPoints1).Apply(img1);
            Bitmap img2mark = new PointsMarker(harrisPoints2).Apply(img2);

            // Concatenate the two images together in a single image (just to show on screen)
            Concatenate concatenate = new Concatenate(img1mark);
            //pictureBox.Image = concatenate.Apply(img2mark);

            // Step 2: Match feature points using a correlation measure
            CorrelationMatching matcher = new CorrelationMatching(9, img1, img2);
            IntPoint[][] matches = matcher.Match(harrisPoints1, harrisPoints2);

            // Get the two sets of points
            var correlationPoints1 = matches[0];
            var correlationPoints2 = matches[1];

            // Concatenate the two images in a single image (just to show on screen)
            Concatenate concat = new Concatenate(img1);
            Bitmap img3 = concat.Apply(img2);

            //// Show the marked correlations in the concatenated image
            //PairsMarker pairs = new PairsMarker(
            //    correlationPoints1, // Add image1's width to the X points
            //                        // to show the markings correctly
            //    correlationPoints2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            //pictureBox.Image = pairs.Apply(img3);

            // Step 3: Create the homography matrix using a robust estimator
            RansacHomographyEstimator ransac = new RansacHomographyEstimator(0.001, 0.99);
            var homography = ransac.Estimate(correlationPoints1, correlationPoints2);

            // Plot RANSAC results against correlation results
            IntPoint[] inliers1 = Matrix.Submatrix(correlationPoints1, ransac.Inliers);
            IntPoint[] inliers2 = Matrix.Submatrix(correlationPoints2, ransac.Inliers);

            //// Concatenate the two images in a single image (just to show on screen)
            //Concatenate concat = new Concatenate(img1);
            //Bitmap img3 = concat.Apply(img2);

            //// Show the marked correlations in the concatenated image
            //PairsMarker pairs = new PairsMarker(
            //    inliers1, // Add image1's width to the X points to show the markings correctly
            //    inliers2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            //pictureBox.Image = pairs.Apply(img3);

            // Step 4: Project and blend the second image using the homography
            Blend blend = new Blend(homography, img1);
            System.Drawing.Bitmap stitchedImage = blend.Apply(img2);

            var path = Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "Download",
                DateTime.Now.Millisecond.ToString() + ".png");
            using (var fs = new FileStream(path, System.IO.FileMode.CreateNew))
            {
                stitchedImage.Save(fs, ImageFormat.Jpeg);
            }
            //stitchedImage.Save(path, GetEncoderInfo("image/jpeg"), new EncoderParameters()
            //                                                        {
            //                                                            Param = new[]
            //                                                            {
            //                                                                new EncoderParameter(Encoder.Quality, 100L)
            //                                                            }
            //                                                        });

            var stitchNode = _scene.CreateChild("StitchedNode");
            stitchNode.Position = new Vector3(0,0,-2);
            stitchNode.SetScale(1.0f);
            //await PlaceSpriteInNode(path, stitchNode);
            // Display in sprite
            var sprite = new Sprite2D();
            var imgFile = new File(Context, path, FileMode.Read);
            sprite.Load(imgFile);

            StaticSprite2D staticSprite2D = null;

            staticSprite2D = stitchNode.CreateComponent<StaticSprite2D>();
            //staticSprite2D.Color = (new Color(NextRandom(1.0f), NextRandom(1.0f), NextRandom(1.0f), 1.0f));
            staticSprite2D.BlendMode = BlendMode.Alpha;
            staticSprite2D.Sprite = sprite;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);
            DrawNewStuff();
            MoveCameraByTouches(timeStep);
        }

        private void DrawNewStuff()
        {
            // Draw stuff - it is fun!
            _debugRenderer = _scene.GetComponent<DebugRenderer>();
            _debugRenderer.AddCircle(Vector3.Zero, new Vector3(1, 1, 1), 3.0f, Color.Cyan, 33, false);
        }

        protected void MoveCameraByTouches(float timeStep)
        {
            if (CameraNode == null)
                return;

            var input = Input;
            if (input.NumTouches != 1)
            {
                return;
            }
            TouchState state = input.GetTouch(0);

            if (state.Delta.X != 0 || state.Delta.Y != 0)
            {
                var camera = CameraNode.GetComponent<Camera>();
                if (camera == null)
                    return;

                var graphics = Graphics;
                Yaw += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.X;
                Pitch += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.Y;
                CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
            }
            else
            {
                var cursor = UI.Cursor;
                if (cursor != null && cursor.Visible)
                    cursor.Position = state.Position;
            }
        }


        protected void SimpleCreateInstructions(string text = "")
        {
            var textElement = new Text()
            {
                Value = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            textElement.SetFont(ResourceCache.GetFont("Fonts/Anonymous Pro.ttf"), 15);
            UI.Root.AddChild(textElement);
        }

        async Task CreateScene()
        {
            // UI text 
            //helloText = new Text()
            //{
            //    Value = "Hello World from MySample",
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center
            //};
            //helloText.SetColor(new Color(0f, 1f, 1f));
            //helloText.SetFont(
            //    font: ResourceCache.GetFont("Fonts/BlueHighway.ttf"),
            //    size: 30);
            //UI.Root.AddChild(helloText);

            // Create a top-level _scene, must add the Octree
            // to visualize any 3D content.
            _scene = new Scene();
            _scene.CreateComponent<Octree>();
            _scene.CreateComponent<DebugRenderer>();

            PlaceBoxes(_scene).Wait(); 

            // Box
            var boxNode = _scene.CreateChild("demoBox");
            boxNode.Position = new Vector3(0, 0, 1);
            boxNode.Rotation = new Quaternion(0, 0, 0);
            boxNode.SetScale(0f);
            StaticModel modelObject = boxNode.CreateComponent<StaticModel>();
            modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");

            // Zone
            var zoneNode = _scene.CreateChild(name: "zoneNode");
            zoneNode.Position = new Vector3(0, 0, 0);

            var zone = zoneNode.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(-1000, 1000));
            zone.AmbientColor = new Color(0.1f, 0.2f, 0.4f);
            //zone.FogColor = new Color(0.1f, 0.2f, 0.3f);
            //zone.FogStart = 10;
            //zone.FogEnd = 100;


            // Camera
            CameraNode = _scene.CreateChild("Camera");
            Camera camera = CameraNode.CreateComponent<Camera>();
            CameraNode.Position = new Vector3(0f, 0f, -7f);
            camera.Fov = 90f;

            // Add a light to the camera node
            var cameraLight = CameraNode.CreateComponent<Light>();
            cameraLight.Range = 200;
            cameraLight.LightType = LightType.Point;
            cameraLight.Color = Color.Yellow;
            
            
            // Viewport
            Renderer.SetViewport(0, new Viewport(_scene, camera, null));
            Renderer.DrawDebugGeometry(true);

            // Perform some actions
            await boxNode.RunActionsAsync(
                new EaseBounceOut(new ScaleTo(duration: 1f, scale: 1)));

            boxNode.RunActionsAsync(
               new RepeatForever(new RotateBy(duration: 1,
                   deltaAngleX: 90, deltaAngleY: 90, deltaAngleZ: 90)));
        }

        private async Task PlaceBoxes(Scene scene)
        {
            _currentMapping = _dataExchangeService.Payload["CurrentMapping"] as Mapping;
            if (_currentMapping == null)
            {
                await Task.Delay(1000);
                _currentMapping = _dataExchangeService.Payload["CurrentMapping"] as Mapping;
            }
            if (_currentMapping == null)
            {
                return;
            }

            //// Test img
            //var cachedPath = await _downloadCache.GetAndCacheFile(_currentMapping.TeaserPath);

            //// Display in sprite
            //var sprite = new Sprite2D(Context);
            //var imgFile = new File(Context, cachedPath, FileMode.Read);
            //sprite.Load(imgFile);
            
            //// Position
            //var spriteNode = scene.CreateChild("SpriteNode");
            //spriteNode.Position = new Vector3(0,0, -1);

            //var staticSprite2D = spriteNode.CreateComponent<StaticSprite2D>();
            //staticSprite2D.Color = (new Color(NextRandom(1.0f), NextRandom(1.0f), NextRandom(1.0f), 1.0f));
            //staticSprite2D.BlendMode = BlendMode.Alpha;
            //staticSprite2D.Sprite = sprite;

            var previousTiltAngle = _currentMapping.Measurements.First().TiltAngle;
            var currentTiltAngle = previousTiltAngle;
            var rows = 1;

            foreach (var measurement in _currentMapping.Measurements)
            {
                currentTiltAngle = measurement.TiltAngle;
                if (currentTiltAngle != previousTiltAngle)
                {
                    rows++;
                    previousTiltAngle = currentTiltAngle;
                }
            }

            var columns = _currentMapping.Measurements.Count / rows;

            var left = -(columns / 2);
            var right = columns / 2;
            var top = rows / 2;
            var bottom = -(rows / 2);

            var idx = 0;

                for (int i = bottom; i < top; i++)
                {
                    for (int j = left; j < right; j++)
                    {
                        var currentMeasurement = _currentMapping.Measurements.ElementAt(idx);
                        var r = currentMeasurement.DistanceCm;
                        var theta = (currentMeasurement.TiltAngle * Math.PI) / 180.0d;
                        var phi = (currentMeasurement.PanAngle * Math.PI) / 180.0d;
                        /*
                        * x=r \, \sin\theta \, \cos\varphi
                        * y=r \, \sin\theta \, \sin\varphi
                        * z=r \, \cos\theta
                        */
                        var x = r * Math.Sin(theta) * Math.Cos(phi);
                        var y = r * Math.Sin(theta) * Math.Sin(phi);
                        var z = r * Math.Cos(theta);

                        var boxNode = scene.CreateChild();
                        boxNode.Position = new Vector3(j, i, (float)(r * 0.1f));
                        boxNode.Rotation = new Quaternion(0, 0, 0);
                        boxNode.SetScale(0.9f);

                        // Add a box
                        var modelObject = boxNode.CreateComponent<StaticModel>();
                        modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");

                        // Add an image
                        //await PlaceSpriteInNode(currentMeasurement.ImageUrl, boxNode);                      

                        idx++; // Measurement #
                    }
                }

        }

        private async Task PlaceSpriteInNode(string imagePath, Node node)
        {
            // Test img
            var cachedPath = await _downloadCache.GetAndCacheFile(imagePath);

            // Display in sprite
            var sprite = new Sprite2D();
            var imgFile = new File(Context, cachedPath, FileMode.Read);
            sprite.Load(imgFile);

            StaticSprite2D staticSprite2D = null;

            staticSprite2D = node.CreateComponent<StaticSprite2D>();
            //staticSprite2D.Color = (new Color(NextRandom(1.0f), NextRandom(1.0f), NextRandom(1.0f), 1.0f));
            staticSprite2D.BlendMode = BlendMode.Alpha;
            staticSprite2D.Sprite = sprite;
        }

        private static Random random;
        public static float NextRandom(float range)
        {
            random = random ?? new Random();
            return (float)random.NextDouble() * range;
        }
    }
}