namespace MethodNewton.WinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        AllocConsole();
        Application.EnableVisualStyles();
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool AllocConsole();
}