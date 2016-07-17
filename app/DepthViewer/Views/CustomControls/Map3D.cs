using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DepthViewer.Contracts;
using DepthViewer.Shared.Models;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Camera = Urho.Camera;
using Color = Urho.Color;
using System.Threading.Tasks;
using Android.Graphics;
using MvvmCross.Platform;
using Urho.Urho2D;
using File = Urho.IO.File;
using FileMode = Urho.IO.FileMode;
using Path = System.IO.Path;
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

        double degreesToRadConstant = Math.PI / 180;

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

            await CreateScene();

            // Stitch images with EmguCV
            //await MariusCvStitch();
        }

        private async Task MariusCvStitch()
        {
            _currentMapping = _dataExchangeService.Payload["CurrentMapping"] as Mapping;

            await _downloadCache.GetAndCacheFile(_currentMapping.Measurements.First().ImageUrl);

            var img1Path = _currentMapping.Measurements.First().ImageUrl;
            var img2Path = _currentMapping.Measurements.ElementAt(1).ImageUrl;

            var imageStitcher = Mvx.Resolve<IImageStitcher>();
            var panoBytes = await imageStitcher.StitchImages(new List<string>() { img1Path, img2Path });

            Directory.CreateDirectory($"/data/data/de.marius.depthviewer/files/_Caches/Pictures.MvvmCross/");
            var docsDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(docsDirPath, $"s{DateTime.Now.Millisecond}s.jpg");

            System.IO.File.WriteAllBytes(path, panoBytes);

            InvokeOnMain(() =>
            {
                var stitchNode = _scene.CreateChild("StitchedNode");
                stitchNode.Position = new Vector3(0, 0, -2);
                stitchNode.SetScale(1.0f);
                //await PlaceSpriteInNode(path, stitchNode);
                // Display in sprite
                var sprite = new Sprite2D();
                var imgFile = new File(Context, path, FileMode.Read);
                sprite.Load(imgFile);

                StaticSprite2D staticSprite2D = stitchNode.CreateComponent<StaticSprite2D>();
                //staticSprite2D.Color = (new Color(NextRandom(1.0f), NextRandom(1.0f), NextRandom(1.0f), 1.0f));
                staticSprite2D.BlendMode = BlendMode.Alpha;
                staticSprite2D.Sprite = sprite;
            });
        }

        void ExportBitmapAsJpg(Bitmap bitmap, string path)
        {
            var stream = new FileStream(path, System.IO.FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            stream.Close();
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

        async Task CreateScene()
        {
            // Create a top-level _scene, must add the Octree
            // to visualize any 3D content.
            _scene = new Scene();
            _scene.CreateComponent<Octree>();
            _scene.CreateComponent<DebugRenderer>();

            await PlaceBoxes();

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
            CameraNode.Position = new Vector3(0f, 0f, 0);
            camera.Fov = 90f;

            // Add a light to the camera node
            var cameraLight = CameraNode.CreateComponent<Light>();
            cameraLight.Range = 200;
            cameraLight.LightType = LightType.Point;
            cameraLight.Color = Color.Yellow;


            // Viewport
            Renderer.SetViewport(0, new Viewport(_scene, camera, null));
            Renderer.DrawDebugGeometry(true);
        }

        private async Task PlaceBoxes()
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

            foreach (var measurement in _currentMapping.Measurements)
            {
                // Compute position
                /*
                 * 1. All measurements start from the horizontal and go down as the tilt angle increases
                 *    a) __ __ __      b) __ __ __   c) __ __ __
                 *      |                 \             __ __ __
                 *      | 90°              \ 45°            0°
                 *      |                   \
                 *    
                 * 2. Polar to Cartesian.  (https://en.wikipedia.org/wiki/Spherical_coordinate_system)
                 *      θ   :angle in XoY plane
                 *      Phi :angle from Z axis towards XoY plane
                 *      r   : distance from the origin 
                 *      
                 *     x = r × sin(θ) × cos(phi)
                 *     y = r × sin(θ) × sin(phi)
                 *     z = r × cos(theta)
                 * 
                 * 3. Careful: 
                 *  Scene Coordinate System           Spherical Coordinate System
                 *     (y) (z)                          (y)
                 *      |  /                             |           
                 *      | /                              | 
                 *      |/__ __ __(x)                    |__ __ __(x)
                 *                                      /                         
                 *                                     / 
                 *                                   (z)
                 *  theta = panAngle
                 *  phi = tiltAngle + 90
                 *  r = distanceCm                                   
                 */

                // Scale distance a bit 
                //measurement.DistanceCm *= 0.1;

                var x = (float)(measurement.DistanceCm *
                                Math.Cos(measurement.PanAngle * degreesToRadConstant) *
                                Math.Sin((measurement.TiltAngle + 90) * degreesToRadConstant));

                var y = (float)(measurement.DistanceCm *
                                Math.Sin(measurement.PanAngle * degreesToRadConstant) *
                                Math.Sin((measurement.TiltAngle + 90) * degreesToRadConstant));

                var z = (float)(measurement.DistanceCm *
                                Math.Cos((measurement.TiltAngle + 90) * degreesToRadConstant));

                var position = new Vector3(x, z, y);
                System.Diagnostics.Debug.WriteLine($"Positioning at X: {position.X} Y: {position.Y} Z: {position.Z}");

                // Compute rotation deltas to face to the point (0.0,0.0,0.0)
                var rotation = new Vector3(0, (float)-measurement.PanAngle, (float)-measurement.TiltAngle);

                // Add box to the scene
                var newBox = AddBox(position, rotation);
            }
        }

        private Node AddBox(Vector3 position, Vector3 rotationDeltas)
        {
            var boxNode = _scene.CreateChild();
            boxNode.Position = position;
            boxNode.RunActions(new Repeat(new RotateBy(1, rotationDeltas.X, rotationDeltas.Y, rotationDeltas.Z), 1));

            var modelObject = boxNode.CreateComponent<StaticModel>();
            modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");

            return boxNode;
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
    }
}