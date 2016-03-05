using System;
using System.Collections.Generic;
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

namespace DepthViewer.Views.CustomControls
{
    class Map3D : Application
    {
        private Text helloText;
        private Mapping _currentMapping;
        private IDataExchangeService _dataExchangeService;

        protected override void Setup()
        {
            base.Setup();


        }

        protected override void Start()
        {
            _dataExchangeService = Mvx.Resolve<IDataExchangeService>();

            CreateScene();
            Input.KeyDown += (args) =>
            {
                if (args.Key == Key.Esc) Engine.Exit();
            };

        }

        private void TextChangerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (helloText != null)
            {
                InvokeOnMain(() =>
                {
                    helloText.Value = new Random().NextDouble().ToString(CultureInfo.InvariantCulture);
                });
            }
        }

        async void CreateScene()
        {
            // UI text 
            helloText = new Text()
            {
                Value = "Hello World from MySample",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            helloText.SetColor(new Color(0f, 1f, 1f));
            helloText.SetFont(
                font: ResourceCache.GetFont("Fonts/BlueHighway.ttf"),
                size: 30);
            UI.Root.AddChild(helloText);

            // Create a top-level scene, must add the Octree
            // to visualize any 3D content.
            var scene = new Scene();
            scene.CreateComponent<Octree>();

            await PlaceBoxes(scene);

            // Box
            Node boxNode = scene.CreateChild();
            boxNode.Position = new Vector3(0, 2, 10);
            boxNode.Rotation = new Quaternion(0, 0, 0);
            boxNode.SetScale(0f);
            StaticModel modelObject = boxNode.CreateComponent<StaticModel>();
            modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");
            // Light
            Node lightNode = scene.CreateChild(name: "light");
			lightNode.Translate (new Vector3(-5f, 0f, -3f));
            // lightNode.SetDirection(new Vector3(0f, 0f, -1f));
            var lightComponent = lightNode.CreateComponent<Light>();
			lightComponent.Brightness = 3f;
            lightComponent.LightType = LightType.Point;

            Node lightNode2 = scene.CreateChild(name: "light");
			lightNode2.Translate (new Vector3(5f, 0f, 0f));
			var lightComponent2 = lightNode2.CreateComponent<Light>();
			lightComponent2.Brightness = 10f;
            lightComponent2.LightType = LightType.Point;

            // Camera
            Node cameraNode = scene.CreateChild(name: "camera");
            Camera camera = cameraNode.CreateComponent<Camera>();
            cameraNode.Position = new Vector3(0f,0f,-7f);
            camera.Fov = 90f;
            // Viewport
            Renderer.SetViewport(0, new Viewport(scene, camera, null));
            // Perform some actions
            await boxNode.RunActionsAsync(
                new EaseBounceOut(new ScaleTo(duration: 1f, scale: 1)));

            var rotateAnim2 = lightNode.RunActionsAsync(
                new RepeatForever(
                    new RotateTo(3f, 0f, 360f, 0f)));

            var rotateAnim1 = boxNode.RunActionsAsync(
               new RepeatForever(new RotateBy(duration: 1,
                   deltaAngleX: 90, deltaAngleY: 90, deltaAngleZ: 90)));

            await Task.WhenAll(new List<Task>() {rotateAnim1, rotateAnim2});
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

            var previousTiltAngle = _currentMapping.Measurements.First().TiltAngle;
            var currentTiltAngle =previousTiltAngle;
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

            var columns = _currentMapping.Measurements.Count/rows;

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
                    var theta = (currentMeasurement.TiltAngle * Math.PI)/180.0d;
                    var phi = (currentMeasurement.PanAngle*Math.PI)/180.0d;
                    /*
                    * x=r \, \sin\theta \, \cos\varphi
                    * y=r \, \sin\theta \, \sin\varphi
                    * z=r \, \cos\theta
                    */
                    var x = r*Math.Sin(theta)*Math.Cos(phi);
                    var y = r * Math.Sin(theta) * Math.Sin(phi);
                    var z = r * Math.Cos(theta);

                    Node boxNode = scene.CreateChild();
                    boxNode.Position = new Vector3(j, i, (float) (r * 0.1f));
                    boxNode.Rotation = new Quaternion(0, 0, 0);
                    boxNode.SetScale(0.9f);
                    StaticModel modelObject = boxNode.CreateComponent<StaticModel>();
                    modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");

                    idx++; // Measurement #
                }
            }

        }
    }
}