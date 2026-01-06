# KAT: A Kinematic Analysis Tool for Manipulator Simulation

Kinematic analysis is a foundation for manipulator optimization. The involved directions include inverse kinematic solving, motion control, and path planning. Accordingly, this paper presents a general kinematic analysis tool (KAT) based on OpenGL. With an intuitive user interface, KAT allows users to control their manipulator by mouse operation. Through the simple system setting, different simulations can be carried out in KAT, such as forward and inverse kinematic solving, workspace visualization, and joint variation analysis. KAT is a versatile and accessible platform for scientific research on industrial robotic manipulators.

# Software Requirements:

To run and revise KAT, you will need the following:

- IDE: Visual Studio 2019 (recommended)
- For Windows User, .Net Framework 4.7.2 is necessary.

# Dependencies
To run KAT, you can use NuGet Package Manager to install the necessary dependencies. The details are as follows: 

- OpenTK 3.3.3
- AssimpNet 4.1.0
- MathNet.Numerics 5.0.0
- ClosedXML 0.102.3

# Function Overview

KAT is a simulation tool designed for kinematic analysis and motion visualization. It can be applied to different robotic systems, such as traditional robotic arms or heavy-duty manipulators with complex motion requirements. The user only needs to provide a 3D model and complete the necessary configuration to operate this software. 
Many functions have been designed and integrated into KAT. The center module is for user interaction with the digital model. OpenGL is responsible for the real-time rendering during motion operation. At the left panel, the joint control module is designed for manipulator motion. Right panel includes the setting of scene rendering and parts display. In addition, the modules at the bottom panel are designed for target position configuration, forward and inverse kinematics calculation, workspace visualization, and end effector pose output. The details of each function are discussed as follows. 

1. Manipulator joint control. Each joint of the manipulator can be adjusted by modifying the values of the corresponding trackbars. The specific joint values also be presented and edited directly in the text boxes. In this way, manipulator’s different configurations can be generated and visualized for further analysis.
2. Render and control setting. According to the different requirements, the scene can be set through the light setting, such as highlight intensity and spot size. In addition, the control sensitivities also are adjustable to suit different users.
3. Target configuration. The target configuration is a $4\times4$ transformation matrix, which can represent its position and orientation. The aim is to make the pose of the manipulator’s end effector identical to the target configuration.
 4. Kinematics analysis. The kinematics analysis includes three modules: inverse kinematics, forward kinematics, and end effector monitoring. On this basis, the mapping relationship between joint configurations and end effector poses can be analyzed. In addition, the manipulator’s workspace also can be visualized.


![Layout](https://github.com/xingzhiyuan000/Manipulator_KAT/blob/master/OpenTK_Winform_Robot/Resources/Textures/Software%20layout.jpg?raw=true)


# Usage Guideline

Some preliminary configurations are required before using KAT. The detailed steps are as follows:

1. Copy the prepared 3D model of the target manipulator in **FBX** format to the directory `./Resources/FBX`.
2. Open the project using an IDE (e.g., **Visual Studio 2019**). 
3. Specify the corresponding model paths in the **System Settings** tab. 
4. Configure the **MD–H parameters**, joint limits, and offset values according to the kinematic characteristics of the manipulator.
5. Run the project to perform kinematic analysis and visualization.

# Project Structure
```
KAT
├── GLSL               # OpenGL shader programs (vertex/fragment shaders)
├── Meshes             # Mesh data structures and geometric primitives
├── Resources          # Source folder for 3D models and textures
│   ├── FBX            # FBX format 3D models imported via Assimp
│   └── Textures       # Texture images used for rendering
├── Form1.cs           # Main UI form and application entry interface
├── AssimpLoader.cs    # 3D model import and parsing class based on AssimpNet
├── Camera.cs          # Camera control class (view/projection matrix handling)
├── Common.cs          # Common definitions, constants, and utility functions
├── DarwLine.cs        # Line rendering utilities
├── Geometry.cs        # Geometric computation and shape generation
├── Joint.cs           # Joint definition and kinematic parameter handling
├── Light.cs           # Lighting model and light source configuration
├── Material.cs        # Material properties (ambient, diffuse, specular)
├── Object.cs          # Scene object abstraction and transform handling
├── Program.cs         # Application startup and main execution entry
├── Renderer.cs        # Core rendering pipeline and OpenGL draw logic
├── Scene.cs           # Scene management and object organization
├── Shader.cs          # Shader loading, compilation, and uniform management
├── Solution.cs        # Kinematic solution handling (FK/IK results and data)
├── Texture.cs         # Texture loading, binding, and management
└── Tools.cs           # Auxiliary tools and mathematical helper functions
```

# Contributing
Contributions are welcome! Feel free to open issues or submit pull requests to improve the software.

# License
This project is licensed under the MIT License. See the LICENSE file for details.

# Contact
wangmingyuan@stu.haust.edu.cn
