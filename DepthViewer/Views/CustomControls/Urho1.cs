using System;
using System.Globalization;
using System.Timers;
using Urho;
using Urho.Actions;
using Urho.Gui;

namespace DepthViewer.Views.CustomControls
{
    class Urho1 : Application
    {
        private Text helloText;
        protected override void Start()
        {
            CreateScene();
            Input.KeyDown += (args) => {
                if (args.Key == Key.Esc) Engine.Exit();
            };

            var textChanger = new Timer(2000.0d);
            textChanger.Elapsed += TextChangerOnElapsed;
            textChanger.Start();
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
            // Box
            Node boxNode = scene.CreateChild();
            boxNode.Position = new Vector3(0, 0, 5);
            boxNode.Rotation = new Quaternion(60, 0, 30);
            boxNode.SetScale(0f);
            StaticModel modelObject = boxNode.CreateComponent<StaticModel>();
            modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");
            // Light
            Node lightNode = scene.CreateChild(name: "light");
            lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
            lightNode.CreateComponent<Light>();
            // Camera
            Node cameraNode = scene.CreateChild(name: "camera");
            Camera camera = cameraNode.CreateComponent<Camera>();
            // Viewport
            Renderer.SetViewport(0, new Viewport(scene, camera, null));
            // Perform some actions
            await boxNode.RunActionsAsync(
                new EaseBounceOut(new ScaleTo(duration: 1f, scale: 1)));
            await boxNode.RunActionsAsync(
                new RepeatForever(new RotateBy(duration: 1,
                    deltaAngleX: 90, deltaAngleY: 0, deltaAngleZ: 0)));
        }
        
    }
}