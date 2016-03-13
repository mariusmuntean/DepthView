using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Timers;
using Android.Graphics;
using Cirrious.CrossCore;
using DepthViewer.Contracts;
using DepthViewer.Models;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Camera = Urho.Camera;
using Color = Urho.Color;
using System.Threading.Tasks;
using Android.Views.Animations;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Urho.IO;
using Urho.Resources;
using Urho.Shapes;
using Urho.Urho2D;

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

            await PlaceBoxes(_scene); 

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

            var rotateAnim1 = boxNode.RunActionsAsync(
               new RepeatForever(new RotateBy(duration: 1,
                   deltaAngleX: 90, deltaAngleY: 90, deltaAngleZ: 90)));

            await Task.WhenAll(new List<Task>() { rotateAnim1 });
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
                        await PlaceSpriteInNode(currentMeasurement.ImageUrl, boxNode);                      

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