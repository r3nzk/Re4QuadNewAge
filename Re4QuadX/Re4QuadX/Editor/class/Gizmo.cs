using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Re4QuadX.Editor
{
    public class Gizmo
    {
        public enum GizmoAxis
        {
            None,
            X,
            Y,
            Z,

            XY,
            YZ, 
            XZ
        }

        public Vector3 Position { get; set; }
        public float Scale { get; private set; }
        public GizmoAxis ActiveAxis { get; set; }

        private int shaderProgram;

        //translate tool handles
        private int vaoMove;
        private int vboMove;
        private readonly List<float> verticesMove = new List<float>();
        private const float ArrowLength = 1f;
        private const float ConeHeight = 0.25f;
        private const float ConeRadius = 0.0625f;
        private const int ConeSegments = 12;

        //translate planes
        private int vaoMovePlanes;
        private int vboMovePlanes;
        private readonly List<float> verticesMovePlanes = new List<float>();

        //rotate tool handles
        private int vaoRotate;
        private int vboRotate;
        private readonly List<float> verticesRotate = new List<float>();
        private const int CircleSegments = 64;

        private readonly Vector3[] axisColors = {
            new Vector3(1.0f, 0.0f, 0.0f), // X-red
            new Vector3(0.0f, 1.0f, 0.0f), // Y-green
            new Vector3(0.0f, 0.0f, 1.0f)  // Z-blue
        };

        //translate
        public static readonly Vector3 SelectColorMoveX = new Vector3(1.0f, 1.0f / 255.0f, 1.0f / 255.0f);
        public static readonly Vector3 SelectColorMoveY = new Vector3(1.0f, 2.0f / 255.0f, 1.0f / 255.0f);
        public static readonly Vector3 SelectColorMoveZ = new Vector3(1.0f, 3.0f / 255.0f, 1.0f / 255.0f);
        //translate combined
        public static readonly Vector3 SelectColorMoveXY = new Vector3(1.0f, 4.0f / 255.0f, 1.0f / 255.0f);
        public static readonly Vector3 SelectColorMoveYZ = new Vector3(1.0f, 5.0f / 255.0f, 1.0f / 255.0f);
        public static readonly Vector3 SelectColorMoveXZ = new Vector3(1.0f, 6.0f / 255.0f, 1.0f / 255.0f); 
        //rotate
        public static readonly Vector3 SelectColorRotateX = new Vector3(1.0f, 1.0f / 255.0f, 2.0f / 255.0f);
        public static readonly Vector3 SelectColorRotateY = new Vector3(1.0f, 2.0f / 255.0f, 2.0f / 255.0f);
        public static readonly Vector3 SelectColorRotateZ = new Vector3(1.0f, 3.0f / 255.0f, 2.0f / 255.0f);

        public Gizmo(){
            Position = Vector3.Zero;
            ActiveAxis = GizmoAxis.None;

            SetupShaders();

            CreateMoveGeometry();
            CreateRotationGeometry();
            CreateMovePlanesGeometry();

            SetupMoveBuffers();
            SetupRotationBuffers();
            SetupMovePlanesBuffers();
        }

        private void SetupMoveBuffers()
        {
            vaoMove = GL.GenVertexArray();
            vboMove = GL.GenBuffer();

            GL.BindVertexArray(vaoMove);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboMove);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesMove.Count * sizeof(float), verticesMove.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 3 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        private void SetupRotationBuffers()
        {
            vaoRotate = GL.GenVertexArray();
            vboRotate = GL.GenBuffer();

            GL.BindVertexArray(vaoRotate);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboRotate);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesRotate.Count * sizeof(float), verticesRotate.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 3 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }
        private void SetupMovePlanesBuffers()
        {
            vaoMovePlanes = GL.GenVertexArray();
            vboMovePlanes = GL.GenBuffer();

            GL.BindVertexArray(vaoMovePlanes);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboMovePlanes);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesMovePlanes.Count * sizeof(float), verticesMovePlanes.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 3 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        private void CreateMoveGeometry()
        {
            //arrow line
            AddLine(Vector3.Zero, Vector3.UnitX * ArrowLength); // x-axis green
            AddLine(Vector3.Zero, Vector3.UnitY * ArrowLength); // y-axis blue 
            AddLine(Vector3.Zero, Vector3.UnitZ * ArrowLength); // z-axis red
            //arrow head
            AddCone(Vector3.UnitX * ArrowLength); // x-axis green
            AddCone(Vector3.UnitY * ArrowLength); // y-axis blue 
            AddCone(Vector3.UnitZ * ArrowLength); // z-axis red
        }
        private void CreateMovePlanesGeometry()
        {
            const float planeSize = 0.24f;
            const float planeOffset = 0.045f;

            //offset
            Vector3 xOff = Vector3.UnitX * planeOffset;
            Vector3 yOff = Vector3.UnitY * planeOffset;
            Vector3 zOff = Vector3.UnitZ * planeOffset;
            //size
            Vector3 xSize = Vector3.UnitX * planeSize;
            Vector3 ySize = Vector3.UnitY * planeSize;
            Vector3 zSize = Vector3.UnitZ * planeSize;

            //helper shi
            void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
                AddPlaneVertex(p0); AddPlaneVertex(p1); AddPlaneVertex(p2);
                AddPlaneVertex(p0); AddPlaneVertex(p2); AddPlaneVertex(p3);
            }

            //xy
            Vector3 xy_p0 = xOff + yOff;
            AddQuad(xy_p0, xy_p0 + xSize, xy_p0 + xSize + ySize, xy_p0 + ySize);
            //yz
            Vector3 yz_p0 = yOff + zOff;
            AddQuad(yz_p0, yz_p0 + ySize, yz_p0 + ySize + zSize, yz_p0 + zSize);
            //xy
            Vector3 xz_p0 = xOff + zOff;
            AddQuad(xz_p0, xz_p0 + xSize, xz_p0 + xSize + zSize, xz_p0 + zSize);
        }

        private void CreateRotationGeometry()
        {
            AddRing(Vector3.UnitX, Vector3.UnitY); // x-axis green
            AddRing(Vector3.UnitY, Vector3.UnitZ); // y-axis blue 
            AddRing(Vector3.UnitZ, Vector3.UnitX); // z-axis red
        }

        private void AddLine(Vector3 start, Vector3 end)
        {
            AddMoveVertex(start);
            AddMoveVertex(end);
        }

        private void AddCone(Vector3 tip)
        {
            Vector3 direction = tip.Normalized();
            Vector3 coneBase = tip - direction * ConeHeight;

            Vector3 tempUp = Vector3.UnitY;
            if (Math.Abs(Vector3.Dot(direction, tempUp)) > 0.999f)
            {
                tempUp = Vector3.UnitZ;
            }

            Vector3 right = Vector3.Cross(direction, tempUp).Normalized();
            Vector3 up = Vector3.Cross(right, direction);

            for (int i = 0; i < ConeSegments; i++)
            {
                float angle1 = (i / (float)ConeSegments) * MathHelper.TwoPi;
                float angle2 = ((i + 1) / (float)ConeSegments) * MathHelper.TwoPi;

                Vector3 p1 = coneBase + right * (float)Math.Cos(angle1) * ConeRadius + up * (float)Math.Sin(angle1) * ConeRadius;
                Vector3 p2 = coneBase + right * (float)Math.Cos(angle2) * ConeRadius + up * (float)Math.Sin(angle2) * ConeRadius;

                AddMoveVertex(tip);
                AddMoveVertex(p1);
                AddMoveVertex(p2);
            }
        }

        private void AddMoveVertex(Vector3 pos)
        {
            verticesMove.Add(pos.X);
            verticesMove.Add(pos.Y);
            verticesMove.Add(pos.Z);
        }

        private void AddPlaneVertex(Vector3 pos)
        {
            verticesMovePlanes.Add(pos.X);
            verticesMovePlanes.Add(pos.Y);
            verticesMovePlanes.Add(pos.Z);
        }
        private void AddMoveQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Triangle 1
            AddMoveVertex(p0);
            AddMoveVertex(p1);
            AddMoveVertex(p2);
            // Triangle 2
            AddMoveVertex(p0);
            AddMoveVertex(p2);
            AddMoveVertex(p3);
        }

        private void AddRing(Vector3 axis, Vector3 startDirection)
        {
            for (int i = 0; i < CircleSegments; i++)
            {
                float angle1 = (i / (float)CircleSegments) * MathHelper.TwoPi;
                float angle2 = ((i + 1) / (float)CircleSegments) * MathHelper.TwoPi;

                Quaternion q1 = Quaternion.FromAxisAngle(axis, angle1);
                Quaternion q2 = Quaternion.FromAxisAngle(axis, angle2);

                Vector3 p1 = Vector3.Transform(startDirection, q1);
                Vector3 p2 = Vector3.Transform(startDirection, q2);

                verticesRotate.Add(p1.X); verticesRotate.Add(p1.Y); verticesRotate.Add(p1.Z);
                verticesRotate.Add(p2.X); verticesRotate.Add(p2.Y); verticesRotate.Add(p2.Z);
            }
        }



        public void UpdateScale(NsCamera.Camera camera)
        {
            if (camera == null){ return;}

            float distance = (camera.Position - Position).Length;
            const float scaleFactor = 0.14f;
            Scale = distance * scaleFactor;
        }

        public void Render(Matrix4 view, Matrix4 projection, EditorTool currentTool, GizmoSpace currentSpace, Matrix4 objectRotation, bool selectionMode = false)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.UseProgram(shaderProgram);

            Matrix4 rotationMatrix = Matrix4.Identity;
            if (currentSpace == GizmoSpace.Local)
            {
                rotationMatrix = objectRotation;
            }

            Matrix4 model = Matrix4.CreateScale(Scale) * rotationMatrix * Matrix4.CreateTranslation(Position);

            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            int colorLocation = GL.GetUniformLocation(shaderProgram, "u_Color");
            int alphaLocation = GL.GetUniformLocation(shaderProgram, "u_Alpha");

            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);
            GL.Uniform1(alphaLocation, 1.0f);

            if (selectionMode) //expand hit detection to make easy to use handles
                GL.LineWidth(10.0f);

            switch (currentTool)
            {
                case EditorTool.Move:
                    //arrow and cones
                    Matrix4 arrowModel = Matrix4.CreateScale(Scale) * rotationMatrix * Matrix4.CreateTranslation(Position);

                    GL.BindVertexArray(vaoMove);
                    GL.UniformMatrix4(modelLoc, false, ref arrowModel);

                    int coneVertexCount = ConeSegments * 3;
                    int lineVertexCount = 2;

                    //x
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveX : axisColors[0]);
                    GL.DrawArrays(PrimitiveType.Lines, 0, lineVertexCount);
                    GL.DrawArrays(PrimitiveType.Triangles, 6, coneVertexCount);
                    //y
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveY : axisColors[1]);
                    GL.DrawArrays(PrimitiveType.Lines, 2, lineVertexCount);
                    GL.DrawArrays(PrimitiveType.Triangles, 6 + coneVertexCount, coneVertexCount);
                    //z
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveZ : axisColors[2]);
                    GL.DrawArrays(PrimitiveType.Lines, 4, lineVertexCount);
                    GL.DrawArrays(PrimitiveType.Triangles, 6 + (coneVertexCount * 2), coneVertexCount);


                    //planar handles
                    Matrix4 cameraWorldMatrix = Matrix4.Invert(view);
                    Vector3 cameraWorldPosition = cameraWorldMatrix.Row3.Xyz;
                    Vector3 camToGizmoWorld = cameraWorldPosition - Position;
                    Matrix4 inverseRotation = Matrix4.Invert(rotationMatrix);
                    Vector3 camDirLocal = Vector3.TransformVector(camToGizmoWorld, inverseRotation);

                    float xSign = camDirLocal.X >= 0 ? 1.0f : -1.0f;
                    float ySign = camDirLocal.Y >= 0 ? 1.0f : -1.0f;
                    float zSign = camDirLocal.Z >= 0 ? 1.0f : -1.0f;

                    float planeAlpha = selectionMode ? 1.0f : 0.4f;
                    GL.Uniform1(alphaLocation, planeAlpha);
                    GL.BindVertexArray(vaoMovePlanes);

                    //zy
                    Matrix4 xyMat = Matrix4.CreateScale(Scale * xSign, Scale * ySign, Scale) * rotationMatrix * Matrix4.CreateTranslation(Position);
                    GL.UniformMatrix4(modelLoc, false, ref xyMat);
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveXY : axisColors[2]); // Blue
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                    //zy
                    Matrix4 yzMat = Matrix4.CreateScale(Scale, Scale * ySign, Scale * zSign) * rotationMatrix * Matrix4.CreateTranslation(Position);
                    GL.UniformMatrix4(modelLoc, false, ref yzMat);
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveYZ : axisColors[0]); // Red
                    GL.DrawArrays(PrimitiveType.Triangles, 6, 6);
                    //xz
                    Matrix4 xzMat = Matrix4.CreateScale(Scale * xSign, Scale, Scale * zSign) * rotationMatrix * Matrix4.CreateTranslation(Position);
                    GL.UniformMatrix4(modelLoc, false, ref xzMat);
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveXZ : axisColors[1]); // Green
                    GL.DrawArrays(PrimitiveType.Triangles, 12, 6);

                    GL.Uniform1(alphaLocation, 1.0f);
                    break;

                case EditorTool.Rotate:
                    //rotation circle
                    Matrix4 rotateModel = Matrix4.CreateScale(Scale) * rotationMatrix * Matrix4.CreateTranslation(Position);

                    GL.BindVertexArray(vaoRotate);
                    GL.UniformMatrix4(modelLoc, false, ref rotateModel);
                    int ringVertexCount = CircleSegments * 2;

                    //x
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveX : axisColors[0]);
                    GL.DrawArrays(PrimitiveType.Lines, 0, ringVertexCount);

                    //y
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveY : axisColors[1]);
                    GL.DrawArrays(PrimitiveType.Lines, ringVertexCount, ringVertexCount);

                    //z
                    GL.Uniform3(colorLocation, selectionMode ? SelectColorMoveZ : axisColors[2]);
                    GL.DrawArrays(PrimitiveType.Lines, ringVertexCount * 2, ringVertexCount);
                    break;
            }

            if (selectionMode) // correct hit detection back to default
                GL.LineWidth(1.5f);

            GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        private void SetupShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPosition;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                }";

            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                uniform vec3 u_Color;
                uniform float u_Alpha;
                void main()
                {
                    FragColor = vec4(u_Color, u_Alpha);
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }
    }
}