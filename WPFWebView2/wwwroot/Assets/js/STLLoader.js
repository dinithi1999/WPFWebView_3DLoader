window.STLLoader = {
    loadSTLViewer: function (canvasId, models) {
        // Get the canvas element
        var canvas = document.getElementById(canvasId);

        // Create the Babylon.js engine
        var engine = new BABYLON.Engine(canvas, true);

        // Create a scene
        var scene = new BABYLON.Scene(engine);

        // Set the background color to white
        scene.clearColor = new BABYLON.Color4(1, 1, 1, 1); // RGBA: 1, 1, 1, 1 for white

        // Add a camera
        var camera = new BABYLON.ArcRotateCamera(
            "Camera",
            Math.PI / 2,
            Math.PI / 4,
            10,
            BABYLON.Vector3.Zero(),
            scene
        );
        camera.attachControl(canvas, true);

        // Add a light
        var light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(0, 1, 0), scene);
        light.intensity = 1.5;

        var modelMap = {}; // To store loaded models for linking
        models.forEach(model => {

            // Extract model properties
            var id = model.id;
            var filePath = model.filePath;
            var fileName = model.fileName;
            var color = model.color;
            var translation = model.translation.split(',').map(Number);
            var rotation = model.rotation.split(',').map(Number);
            var offsetTranslation = model.offsetTranslation.split(',').map(Number);
            var offsetRotation = model.offsetRotation.split(',').map(Number);
            var linked = model.linked;

            // Create the material for the model
            var material = new BABYLON.StandardMaterial(id + "MaterialColor", scene);
            material.diffuseColor = BABYLON.Color3.FromHexString(color);
            material.specularColor = new BABYLON.Color3(0, 0, 0);  // Reduce specular reflection
            material.emissiveColor = new BABYLON.Color3(0, 0, 0);  // Reduce self-emission

            // Load the STL file
            BABYLON.SceneLoader.ImportMesh(
                "",
                filePath,
                fileName, // Only the file name
                scene,
                function (meshes) {
                    var mesh = meshes[0]; // Assuming there's one mesh per model
                    mesh.material = material;

                    // Apply transformations based on the model properties
                    mesh.position = new BABYLON.Vector3(...translation);
                    mesh.rotation = new BABYLON.Vector3(...rotation).scale(Math.PI / 180); // Convert to radians

                    // Apply offsets
                    mesh.position.addInPlace(new BABYLON.Vector3(...offsetTranslation));
                    mesh.rotation.addInPlace(new BABYLON.Vector3(...offsetRotation).scale(Math.PI / 180)); // Convert to radians

                    // Link model (apply transformation based on linked model if exists)
                    if (linked && modelMap[linked]) {
                        var parentMesh = modelMap[linked];
                        mesh.position.addInPlace(parentMesh.position);
                        mesh.rotation.addInPlace(parentMesh.rotation);
                    }

                    // Store the mesh in modelMap for future linking
                    modelMap[id] = mesh;
                },
                null,
                function (error) {
                    console.error("Error loading STL model:", error);
                }
            );
        });


        camera.setPosition(new BABYLON.Vector3(0, 6000, 0));
        camera.target = BABYLON.Vector3.Zero();

        // Expose `setView` globally
        window.STLLoader.setView = function (view) {
            switch (view) {
                case "top":
                    // Adjust the camera position for a natural top view
                    camera.setPosition(new BABYLON.Vector3(0, 5000, 0)); // Directly above the scene
                    camera.target = BABYLON.Vector3.Zero(10); // Look at the center of the scene

                    // Enable orthographic mode for a natural top view
                    //camera.mode = BABYLON.Camera.ORTHOGRAPHIC_CAMERA;
                    break;
                case "front":
                    camera.setPosition(new BABYLON.Vector3(0, 0, -5000));
                    camera.setTarget(BABYLON.Vector3.Zero());
                    break;
                case "back":
                    camera.setPosition(new BABYLON.Vector3(0, 0, 5000));
                    camera.setTarget(BABYLON.Vector3.Zero());
                    break;
                case "left":
                    camera.setPosition(new BABYLON.Vector3(-4000, 0, 0));
                    camera.setTarget(BABYLON.Vector3.Zero());
                    break;
                case "right":
                    camera.setPosition(new BABYLON.Vector3(4000, 0, 0));
                    camera.setTarget(BABYLON.Vector3.Zero());
                    break;
                default:
                    console.warn("Unknown view: " + view);
                    break;
            }
        };

        // Render the scene
        engine.runRenderLoop(function () {
            scene.render();
        });

        // Resize the canvas when the window is resized
        window.addEventListener("resize", function () {
            engine.resize();
        });

    }
};
