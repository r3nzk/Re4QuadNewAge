using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Re4QuadExtremeEditor.Editor
{
    public enum GizmoAxis
    {
        None,
        X,
        Y,
        Z
    }

    public class Gizmo
    {
        public Vector3 Position { get; set; }
        public float Scale { get; private set; }
        public GizmoAxis ActiveAxis { get; set; }

        private int shaderProgram;

        //move tool handles
        private int vaoMove;
        private int vboMove;
        private readonly List<float> verticesMove = new List<float>();
        private const float ArrowLength = 0.8f;
        private const float ConeHeight = 0.2f;
        private const float ConeRadius = 0.05f;
        private const int ConeSegments = 12;

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

        public Gizmo(){
            Position = Vector3.Zero;
            Scale = 1.0f;
            ActiveAxis = GizmoAxis.None;

            SetupShaders();

            CreateMoveGeometry();
            CreateRotationGeometry();

            SetupMoveBuffers();
            SetupRotationBuffers();
        }

        private void CreateMoveGeometry()
        {
            //arrow line
            AddLine(Vector3.Zero, Vector3.UnitX * ArrowLength, axisColors[0]); // x-axis green
            AddLine(Vector3.Zero, Vector3.UnitY * ArrowLength, axisColors[1]); // y-axis blue 
            AddLine(Vector3.Zero, Vector3.UnitZ * ArrowLength, axisColors[2]); // z-axis red
            //arrow head
            AddCone(Vector3.UnitX * ArrowLength, axisColors[0]); // x-axis green
            AddCone(Vector3.UnitY * ArrowLength, axisColors[1]); // y-axis blue 
            AddCone(Vector3.UnitZ * ArrowLength, axisColors[2]); // z-axis red
        }

        private void CreateRotationGeometry()
        {
            AddRing(Vector3.UnitX, Vector3.UnitY, axisColors[0]); // x-axis green
            AddRing(Vector3.UnitY, Vector3.UnitZ, axisColors[1]); // y-axis blue 
            AddRing(Vector3.UnitZ, Vector3.UnitX, axisColors[2]); // z-axis red
        }

        private void AddLine(Vector3 start, Vector3 end, Vector3 color)
        {
            AddMoveVertex(start, color);
            AddMoveVertex(end, color);
        }

        private void AddCone(Vector3 tip, Vector3 color)
        {
            Vector3 direction = tip.Normalized();
            Vector3 coneBase = tip - direction * ConeHeight;

            Vector3 tempUp = Vector3.UnitY;
            if (Math.Abs(Vector3.Dot(direction, tempUp)) > 0.999f){
                tempUp = Vector3.UnitZ;
            }

            Vector3 right = Vector3.Cross(direction, tempUp).Normalized();
            Vector3 up = Vector3.Cross(right, direction);

            for (int i = 0; i < ConeSegments; i++){
                float angle1 = (i / (float)ConeSegments) * MathHelper.TwoPi;
                float angle2 = ((i + 1) / (float)ConeSegments) * MathHelper.TwoPi;

                Vector3 p1 = coneBase + right * (float)Math.Cos(angle1) * ConeRadius + up * (float)Math.Sin(angle1) * ConeRadius;
                Vector3 p2 = coneBase + right * (float)Math.Cos(angle2) * ConeRadius + up * (float)Math.Sin(angle2) * ConeRadius;

                AddMoveVertex(tip, color);
                AddMoveVertex(p1, color);
                AddMoveVertex(p2, color);
            }
        }

        private void AddMoveVertex(Vector3 pos, Vector3 color)
        {
            verticesMove.Add(pos.X);
            verticesMove.Add(pos.Y);
            verticesMove.Add(pos.Z);
            verticesMove.Add(color.X);
            verticesMove.Add(color.Y);
            verticesMove.Add(color.Z);
        }

        private void AddRing(Vector3 axis, Vector3 startDirection, Vector3 color)
        {
            for (int i = 0; i < CircleSegments; i++){
                float angle1 = (i / (float)CircleSegments) * MathHelper.TwoPi;
                float angle2 = ((i + 1) / (float)CircleSegments) * MathHelper.TwoPi;

                Quaternion q1 = Quaternion.FromAxisAngle(axis, angle1);
                Quaternion q2 = Quaternion.FromAxisAngle(axis, angle2);

                Vector3 p1 = Vector3.Transform(startDirection, q1);
                Vector3 p2 = Vector3.Transform(startDirection, q2);

                verticesRotate.Add(p1.X); verticesRotate.Add(p1.Y); verticesRotate.Add(p1.Z);
                verticesRotate.Add(color.X); verticesRotate.Add(color.Y); verticesRotate.Add(color.Z);
                verticesRotate.Add(p2.X); verticesRotate.Add(p2.Y); verticesRotate.Add(p2.Z);
                verticesRotate.Add(color.X); verticesRotate.Add(color.Y); verticesRotate.Add(color.Z);
            }
        }

        private void SetupMoveBuffers()
        {
            vaoMove = GL.GenVertexArray();
            vboMove = GL.GenBuffer();

            GL.BindVertexArray(vaoMove);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboMove);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesMove.Count * sizeof(float), verticesMove.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 6 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        private void SetupRotationBuffers()
        {
            vaoRotate = GL.GenVertexArray();
            vboRotate = GL.GenBuffer();

            GL.BindVertexArray(vaoRotate);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboRotate);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesRotate.Count * sizeof(float), verticesRotate.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 6 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        public void UpdateScale(NsCamera.Camera camera)
        {
            if (camera == null){ return;}

            float distance = (camera.Position - Position).Length;
            const float scaleFactor = 0.1f;
            Scale = distance * scaleFactor;
        }

        public void Render(Matrix4 view, Matrix4 projection, EditorTool currentTool, GizmoSpace currentSpace, Matrix4 objectRotation)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.UseProgram(shaderProgram);

            Matrix4 rotationMatrix = Matrix4.Identity;
            if (currentSpace == GizmoSpace.Local){
                rotationMatrix = objectRotation;
            }

            Matrix4 model = Matrix4.CreateScale(Scale) * rotationMatrix * Matrix4.CreateTranslation(Position);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "projection"), false, ref projection);

            switch (currentTool){
                case EditorTool.Move:
                    GL.BindVertexArray(vaoMove);

                    GL.DrawArrays(PrimitiveType.Lines, 0, 6);

                    int coneVertexCount = ConeSegments * 3;
                    int lineVertexCount = 6;

                    GL.DrawArrays(PrimitiveType.Triangles, lineVertexCount, coneVertexCount);
                    GL.DrawArrays(PrimitiveType.Triangles, lineVertexCount + coneVertexCount, coneVertexCount);
                    GL.DrawArrays(PrimitiveType.Triangles, lineVertexCount + (coneVertexCount * 2), coneVertexCount);
                    break;

                case EditorTool.Rotate:
                    GL.BindVertexArray(vaoRotate);
                    GL.DrawArrays(PrimitiveType.Lines, 0, verticesRotate.Count / 6);
                    break;
            }

            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest);
        }

        private void SetupShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec3 aColor;

                out vec3 fColor;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                    fColor = aColor;
                }";

            string fragmentShaderSource = @"
                #version 330 core
                in vec3 fColor;
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(fColor, 1.0);
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