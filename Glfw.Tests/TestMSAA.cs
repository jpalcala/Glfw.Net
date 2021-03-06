namespace Glfw3.Tests
{
    using CommandLine;
    using OpenGL;
    using System;

    /// <summary>
    /// This test renders two high contrast, slowly rotating quads, one aliased and one (hopefully)
    /// anti-aliased, thus allowing for visual verification of whether MSAA is indeed enabled.
    /// </summary>
    /// <remarks>
    /// Ported from <c>msaa.c</c>.
    /// </remarks>
    class TestMSAA : TestBase
    {
        class Options
        {
            [Option('s', HelpText = "Number of samples for MSAA", Default = 10)]
            public int Samples { get; set; }
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Space:
                    {
                        Glfw.SetTime(0.0);
                        break;
                    }

                default:
                    break;
            }
        }

        static void Main(string[] args)
        {
            Init();

            var samples = 4;

            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                if (options.Samples > -1)
                    samples = options.Samples;
            });

            Gl.Initialize();
            if (!Glfw.Init())
                Environment.Exit(1);

            if (samples > 0)
                Log("Requesting MSAA with {0} samples", samples);
            else
                Log("Requesting that MSAA not be available");

            Glfw.WindowHint(Glfw.Hint.Samples, samples);
            Glfw.WindowHint(Glfw.Hint.Visible, false);

            var window = Glfw.CreateWindow(800, 400, "Aliasing Detector");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.SetKeyCallback(window, KeyCallback);
            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            if (Glfw.ExtensionSupported("ARB_multisample"))
            {
                Log("Multisampling is not supported");
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.ShowWindow(window);

            Gl.Get(GetPName.Samples, out int s);

            if (s > 1)
                Log("Context reports MSAA is available with {0} samples", samples);
            else
                Log("Context reports MSAA is unavailable");

            Gl.MatrixMode(MatrixMode.Projection);
            Gl.Ortho(0.0, 1.0, 0.0, 0.5, 0.0, 1.0);
            Gl.MatrixMode(MatrixMode.Modelview);

            while (!Glfw.WindowShouldClose(window))
            {
                var time = (float)Glfw.GetTime();

                Gl.Clear(ClearBufferMask.ColorBufferBit);

                Gl.LoadIdentity();
                Gl.Translate(0.25f, 0.25f, 0f);
                Gl.Rotate(time, 0f, 0f, 1f);

                Gl.Disable(EnableCap.Multisample);
                Gl.Rect(-0.15f, -0.15f, 0.15f, 0.15f);

                Gl.LoadIdentity();
                Gl.Translate(0.75f, 0.25f, 0f);
                Gl.Rotate(time, 0f, 0f, 1f);

                Gl.Enable(EnableCap.Multisample);
                Gl.Rect(-0.15f, -0.15f, 0.15f, 0.15f);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            Glfw.Terminate();
        }
    }
}
