using Glfw3;
using OpenGL;
using System;

class TestExample
{
    static void Main(string[] args)
    {
        // If the library isn't in the environment path we need to set it
        Glfw.ConfigureNativesDirectory("External/");
        Gl.Initialize();
        // Initialize the library
        if (!Glfw.Init())
            Environment.Exit(-1);

        // Create a windowed mode window and its OpenGL context
        var window = Glfw.CreateWindow(640, 480, "Hello World");
        if (!window)
        {
            Glfw.Terminate();
            Environment.Exit(-1);
        }
        
        // Make the window's context current
        Glfw.MakeContextCurrent(window);

        Gl.Viewport(0, 0, 800, 600);
        // Loop until the user closes the window
        while (!Glfw.WindowShouldClose(window))
        {
            // Render here
            Gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            // Swap front and back buffers
            Glfw.SwapBuffers(window);

            // Poll for and process events
            Glfw.PollEvents();
        }

        Glfw.Terminate();
    }
}
