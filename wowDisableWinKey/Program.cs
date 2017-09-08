using System;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;
using NLog;

namespace wowDisableWinKey
{
    static class Program
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var allProcesses = Process.GetProcesses();
            var process = Process.GetCurrentProcess();
            _log.Debug("point 0");

            foreach (var proc in allProcesses)
            {
                if (proc.ProcessName == process.ProcessName && proc.Id != process.Id)
                {
                    proc.Kill();
                    _log.Debug("Killed (1)");
                }
            }

            foreach (string arg in args)
            {
                if (arg.StartsWith("-d"))
                {
                    try
                    {
                        int delay = Convert.ToInt32(arg.Split('=')[1]);
                        System.Threading.Thread.Sleep(delay * 1000);
                        _log.Debug("Killed (1.delay)");
                    }
                    catch
                    {
                        Console.WriteLine("Задан не числовой аргумент -d");
                        Process.GetCurrentProcess().Kill();
                        _log.Debug("Killed (2)");
                    }
                }
            }
            try
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                string thisProcessName = Process.GetCurrentProcess().ProcessName;
                string[] prNameSplitted = thisProcessName.Split('.');
                bool isVisualDebugging = prNameSplitted[prNameSplitted.Length - 1].Contains("vshost");
                if (!hasAdministrativeRight && !isVisualDebugging)
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo(); //создаем новый процесс
                    processInfo.Verb = "runas"; //в данном случае указываем, что процесс должен быть запущен с правами администратора
                    processInfo.FileName = Application.ExecutablePath; //указываем исполняемый файл (программу) для запуска
                    try
                    {
                        _log.Debug("point 3");
                        Process.Start(processInfo); //пытаемся запустить процесс
                        _log.Debug("point 4");
                    }
                    catch (Win32Exception)
                    {
                        _log.Debug("point 5");
                        //Ничего не делаем, потому что пользователь, возможно, нажал кнопку "Нет" в ответ на вопрос о запуске программы в окне предупреждения UAC (для Windows 7)
                    }
                    _log.Debug("point 6");
                    System.Threading.Thread.Sleep(2000);
                    Application.Exit(); //закрываем текущую копию программы (в любом случае, даже если пользователь отменил запуск с правами администратора в окне UAC)
                }
                else //имеем права администратора, значит, стартуем
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                    _log.Debug("point 6");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

    }
}
