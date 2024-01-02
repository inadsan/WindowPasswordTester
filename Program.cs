using System;
using System.Configuration;
using System.Threading;
using System.Runtime.InteropServices;

namespace PasswordsTester
{
    internal class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, string lParam);
        
        const uint WM_SETTEXT = 0x000C;
        const uint BM_CLICK = 0x00F5;

        static void Main(string[] args)
        {
            string formName = NullIf(ConfigurationManager.AppSettings["FormName"]);
            string formClass = NullIf(ConfigurationManager.AppSettings["FormClass"]);
            string botonName = NullIf(ConfigurationManager.AppSettings["BotonName"]);
            string botonClass = NullIf(ConfigurationManager.AppSettings["BotonClass"]);
            string textName = NullIf(ConfigurationManager.AppSettings["TextName"]);
            string textClass = NullIf(ConfigurationManager.AppSettings["TextClass"]);

            IntPtr ventana = FindWindow(formClass, formName);
            Console.WriteLine($"Ventana: {formName}/{formClass} -> {ventana.ToString("X8")}");
            if (ventana == IntPtr.Zero)
                return;

            IntPtr boton = FindWindowEx(ventana, IntPtr.Zero, botonClass, botonName);
            Console.WriteLine($"Boton: {botonName}/{botonClass} -> {boton.ToString("X8")}");
            if (boton == IntPtr.Zero)
                return;
            
            IntPtr texto = FindWindowEx(ventana, IntPtr.Zero, textClass, textName);
            texto = FindWindowEx(ventana, texto, textClass, textName);
            Console.WriteLine($"Texto: {textName}/{textClass} -> {texto.ToString("X8")}");
            if (texto == IntPtr.Zero)
                return;
            

            int sleep = int.Parse(ConfigurationManager.AppSettings["Sleep"]);

            int inicio = int.Parse(ConfigurationManager.AppSettings["Inicio"]);
            int fin = int.Parse(ConfigurationManager.AppSettings["Fin"]);
            int last = inicio;
            int numDigitos = int.Parse(ConfigurationManager.AppSettings["NumDigitosInico"]);

            HandleRef hrefHWndTarget = new HandleRef(null, texto);
            SendMessage(hrefHWndTarget, WM_SETTEXT, IntPtr.Zero, last.ToString().PadLeft(numDigitos, '0'));
            
            
            Console.CancelKeyPress += delegate {
                Console.WriteLine($"Ultimo probado: {last.ToString().PadLeft(numDigitos, '0')}");
                Console.ReadLine();
            };

            int pre = last;
            ventana = IntPtr.Zero;
            
            int numDigitosFin = (int)Math.Floor(Math.Log10(fin) + 1);
            Console.WriteLine($"Inicio: {inicio.ToString().PadLeft(numDigitos, '0')}/{fin.ToString().PadLeft(numDigitosFin, '0')}");

            Console.WriteLine("Pulsa intro");
            Console.ReadLine();


            for (; last < fin; last++)
            {
                IntPtr ventanaPrevio = ventana;
                for (int i = 0; i < 10; i++)
                {
                    ventana = FindWindow(formClass, formName);
                    if (ventana != IntPtr.Zero && ventanaPrevio != ventana)
                        break;
                    Thread.Sleep(sleep);
                }
                if (ventana == IntPtr.Zero)
                    break;
                if (last % 100 == 0)
                {
                    Console.Write($"{last.ToString().PadLeft(numDigitos, '0')},");
                }
                if (last % 1000 == 0)
                {
                    Console.WriteLine();
                }
                boton = FindWindowEx(ventana, IntPtr.Zero, botonClass, botonName);
                if (boton == IntPtr.Zero)
                    break;
                
                texto = FindWindowEx(ventana, IntPtr.Zero, textClass, textName);
                texto = FindWindowEx(ventana, texto, textClass, textName);
                if (texto == IntPtr.Zero)
                    break;

                hrefHWndTarget = new HandleRef(null, texto);
                SendMessage(hrefHWndTarget, WM_SETTEXT, IntPtr.Zero, last.ToString().PadLeft(numDigitos, '0'));
                SendMessage(boton, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                pre = last;
                if ((int)Math.Floor(Math.Log10(last+1) + 1) > numDigitos)
                {
                    last = 0;
                    numDigitos++;
                }
            }
            Console.WriteLine($"Ultimo: {pre.ToString().PadLeft(numDigitos, '0')}");
            Console.ReadLine();
        }

        public static string NullIf(string left)
        {
            if (string.IsNullOrWhiteSpace(left))
                return null;
            return left;
        }
    }
}
