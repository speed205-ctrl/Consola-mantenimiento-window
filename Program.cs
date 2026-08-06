using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace MantenimientoPC
{
    class Program
    {
        private static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mantenimiento_log.txt");

        static void Main(string[] args)
        {
            // Verificar privilegios de administrador (aunque el manifiesto debería forzarlo)
            if (!IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [!] ERROR: Este programa requiere privilegios de Administrador.");
                Console.WriteLine("     Por favor, ejecute el programa haciendo clic derecho y seleccionando 'Ejecutar como Administrador'.");
                Console.ResetColor();
                Console.WriteLine("\n Presione cualquier tecla para salir...");
                Console.ReadKey();
                return;
            }

            Console.Title = "Sistema de Mantenimiento de PC - Comandos Nativos de Windows";

            while (true)
            {
                Console.Clear();
                DrawHeader();
                DrawMenu();
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n Seleccione una opción (1-4): ");
                Console.ResetColor();
                string choice = Console.ReadLine();

                if (choice == "4")
                {
                    Log("Aplicación de mantenimiento cerrada por el usuario.");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n ¡Gracias por usar el Sistema de Mantenimiento!");
                    Console.ResetColor();
                    ThreadSleep(1500);
                    break;
                }

                ProcessChoice(choice);
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n Presione cualquier tecla para volver al menú principal...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void DrawHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"  __  __         _  _              _            _      ");
            Console.WriteLine(@" |  \/  |__ _ _ _| |_ ___ _ _  _  (_)_ __  _ __| |__ _ ");
            Console.WriteLine(@" | |\/| / _` | ' \  _/ -_) ' \| | | | '  \| '_ \ / _` |");
            Console.WriteLine(@" |_|  |_\__,_|_||_\__\___|_||_|_| |_|_|_|_| .__/_\__,_|");
            Console.WriteLine(@"                                          |_|          ");
            Console.WriteLine(" ========================================================");
            Console.WriteLine("   Mantenimiento de PC con Comandos Nativos de Windows");
            Console.WriteLine(" ========================================================");
            Console.ResetColor();
        }

        static void DrawMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" [1] MANTENIMIENTO RÁPIDO / LIGERO");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("     -> Limpia archivos temporales de Windows, caché de navegadores (Chrome/Edge),");
            Console.WriteLine("        historial de archivos recientes, vacía la papelera de reciclaje, limpia la");
            Console.WriteLine("        caché DNS y ejecuta optimización de discos (TRIM en SSD / Defrag en HDD).");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("        (Proceso rápido, seguro y sin interrupciones de red).");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" [2] MANTENIMIENTO COMPLETO / PROFUNDO");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("     -> Ejecuta la limpieza completa del mantenimiento ligero más un escaneo profundo");
            Console.WriteLine("        de integridad de archivos del sistema (SFC/DISM), comprobación de disco en");
            Console.WriteLine("        línea (Chkdsk /scan), restablecimiento de red (Winsock y pila TCP/IP), y");
            Console.WriteLine("        reportes detallados de salud de batería y eficiencia energética.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("        (Tarda varios minutos, renueva conexión IP y se aconseja reiniciar al terminar).");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [3] Ejecutar Tareas Individuales (Modo Avanzado)");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("     -> Permite seleccionar y ejecutar de forma independiente cada uno de los");
            Console.WriteLine("        5 módulos de mantenimiento del sistema.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" [4] Salir");
            
            Console.ResetColor();
        }

        static void Log(string message, bool isError = false)
        {
            string formattedMessage = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}{2}", DateTime.Now, isError ? "[ERROR] " : "[INFO] ", message);
            
            if (isError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n [!] " + message);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n [*] " + message);
            }
            Console.ResetColor();

            try
            {
                File.AppendAllText(logFilePath, formattedMessage + Environment.NewLine);
            }
            catch { }
        }

        static void ProcessChoice(string choice)
        {
            Console.Clear();
            DrawHeader();
            
            switch (choice)
            {
                case "1":
                    RunMantenimientoLigero();
                    break;
                case "2":
                    RunMantenimientoProfundo();
                    break;
                case "3":
                    ShowSubmenuLoop();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" [!] Opción no válida. Intente de nuevo.");
                    Console.ResetColor();
                    break;
            }
        }

        #region Módulos de Mantenimiento

        static void RunLimpieza()
        {
            Log("=== MÓDULO 1: LIMPIEZA DE TEMPORALES Y CACHÉ ===");

            // 1. Temp de Usuario
            string userTemp = Path.GetTempPath();
            CleanDirectory(userTemp, "Temporales de Usuario");

            // 2. Temp del Sistema
            string systemTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            CleanDirectory(systemTemp, "Temporales del Sistema");

            // 3. Prefetch
            string prefetch = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
            CleanDirectory(prefetch, "Prefetch");

            // 4. Windows Update Cache
            CleanWindowsUpdateCache();

            // 5. Vaciar Papelera
            EmptyRecycleBin();

            // 6. Archivos Recientes
            CleanRecentFiles();

            // 7. Caché de Navegadores (Chrome / Edge)
            CleanBrowserCaches();

            Log("Módulo de Limpieza finalizado.");
        }

        static void RunIntegridad()
        {
            Log("=== MÓDULO 2: INTEGRIDAD Y REPARACIÓN DEL SISTEMA ===");

            Log("Ejecutando SFC (System File Checker)... Esto puede tardar varios minutos.");
            RunSystemCommand("sfc", "/scannow");

            Log("Ejecutando DISM (Deployment Image Servicing and Management)...");
            RunSystemCommand("dism", "/Online /Cleanup-Image /RestoreHealth");

            Log("Ejecutando Comprobación de Disco en línea (Chkdsk /scan)...");
            RunSystemCommand("chkdsk", "C: /scan");

            Log("Módulo de Integridad y Reparación finalizado.");
        }

        static void RunOptimizacionDisco()
        {
            Log("=== MÓDULO 3: OPTIMIZACIÓN Y DESFRAGMENTACIÓN DE DISCO ===");
            
            Log("Detectando tipo de unidad de almacenamiento para C:...");
            string mediaType = GetDriveMediaType("C");
            Log("Tipo de unidad detectado: " + mediaType);

            if (mediaType.Equals("SSD", StringComparison.OrdinalIgnoreCase))
            {
                Log("ADVERTENCIA DE SEGURIDAD: Unidad SSD/M.2 detectada.");
                Log("No se realizara desfragmentacion mecanica para evitar degradacion de la vida util del disco.");
                Log("Ejecutando optimizacion TRIM nativa (Re-Trim)...");
                RunSystemCommand("defrag", "C: /L /U");
            }
            else if (mediaType.Equals("HDD", StringComparison.OrdinalIgnoreCase))
            {
                Log("Unidad HDD (Disco Mecanico) detectada.");
                Log("Ejecutando desfragmentacion y optimizacion de disco...");
                RunSystemCommand("defrag", "C: /D /U");
            }
            else
            {
                Log("No se pudo determinar con precision el tipo de disco. Se usara la optimizacion segura por defecto (/O)...");
                RunSystemCommand("defrag", "C: /O /U");
            }

            Log("Módulo de Optimización de Disco finalizado.");
        }

        static void RunOptimizacionRed()
        {
            Log("=== MÓDULO 4: OPTIMIZACIÓN Y RESTABLECIMIENTO DE RED ===");

            Log("Vaciando la caché DNS...");
            RunSystemCommand("ipconfig", "/flushdns");

            Log("Liberación de dirección IP...");
            RunSystemCommand("ipconfig", "/release", true);

            Log("Renovación de dirección IP...");
            RunSystemCommand("ipconfig", "/renew", true);

            Log("Restableciendo el catálogo Winsock (Sockets de Red)...");
            RunSystemCommand("netsh", "winsock reset");

            Log("Restableciendo el protocolo TCP/IP...");
            int exitCode = RunSystemCommand("netsh", "int ip reset", true);
            if (exitCode == 0 || exitCode == 1)
            {
                Log("Protocolo TCP/IP restablecido. (Nota: Es normal ver un mensaje de 'Acceso denegado' en Windows 10/11).");
            }

            Log("Módulo de Red finalizado. (Nota: Se aconseja reiniciar el equipo).");
        }

        static void RunDiagnosticoEnergia()
        {
            Log("=== MÓDULO 5: DIAGNÓSTICO DE ENERGÍA Y HARDWARE ===");

            ShowHardwareInfo();

            string batteryReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reporte_bateria.html");
            Log(string.Format("Generando reporte de batería en: '{0}'...", batteryReportPath));
            
            // Se silencia el error del reporte de batería si falla (típico en PCs de escritorio sin batería)
            int exitCode = RunSystemCommand("powercfg", string.Format("/batteryreport /output \"{0}\"", batteryReportPath), true);
            if (exitCode == 0)
            {
                Log("Reporte de batería generado exitosamente en: reporte_bateria.html");
            }
            else
            {
                Log("Nota: No se generó el reporte de batería (esto es normal en PC de escritorio sin batería).", false);
            }

            string energyReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reporte_energia.html");
            Log("Ejecutando análisis de eficiencia energética (duración: 60 segundos)...");
            RunSystemCommand("powercfg", string.Format("/energy /output \"{0}\"", energyReportPath));

            Log("Reportes HTML procesados en la carpeta de la aplicación.");
            Log("Módulo de Diagnóstico finalizado.");
        }

        #endregion

        #region Utilidades y Helpers

        static void CleanDirectory(string path, string displayName)
        {
            if (!Directory.Exists(path))
            {
                Log(string.Format("La carpeta '{0}' ({1}) no existe.", displayName, path), true);
                return;
            }

            Log("Limpiando " + displayName + "...");
            int filesDeleted = 0;
            int dirsDeleted = 0;
            int skippedCount = 0;

            // Eliminar archivos
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    try
                    {
                        if ((File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                        }
                        File.Delete(file);
                        filesDeleted++;
                    }
                    catch
                    {
                        skippedCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("Error al leer archivos en {0}: {1}", displayName, ex.Message), true);
            }

            // Eliminar subcarpetas
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    try
                    {
                        SafeDeleteDirectory(dir);
                        dirsDeleted++;
                    }
                    catch
                    {
                        skippedCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("Error al leer subcarpetas en {0}: {1}", displayName, ex.Message), true);
            }

            Log(string.Format("Resultado: {0} archivos y {1} carpetas eliminados. ({2} elementos bloqueados/en uso omitidos).", filesDeleted, dirsDeleted, skippedCount));
        }

        static void CleanWindowsUpdateCache()
        {
            Log("Deteniendo servicio de Windows Update (wuauserv)...");
            RunSystemCommand("net", "stop wuauserv");

            string updatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution\\Download");
            CleanDirectory(updatePath, "Caché de Descargas de Windows Update");

            Log("Iniciando servicio de Windows Update (wuauserv)...");
            RunSystemCommand("net", "start wuauserv");
        }

        static void EmptyRecycleBin()
        {
            Log("Limpiando la Papelera de Reciclaje...");
            RunSystemCommand("powershell.exe", "-NoProfile -ExecutionPolicy Bypass -Command \"Clear-RecycleBin -Force -ErrorAction SilentlyContinue\"");
        }

        static int RunSystemCommand(string fileName, string arguments, bool suppressErrorDisplay = false)
        {
            int exitCode = -1;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = false // Corre dentro de la misma consola activa
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        exitCode = process.ExitCode;
                        if (exitCode != 0 && !suppressErrorDisplay)
                        {
                            Log(string.Format("El comando '{0} {1}' finalizó con código de salida no estándar: {2}", fileName, arguments, exitCode), true);
                            
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n [!] DIAGNÓSTICO DEL ERROR:");
                            Console.WriteLine("     Detalle: " + GetExitCodeDescription(fileName, exitCode));
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exitCode = -2;
                if (!suppressErrorDisplay)
                {
                    Log(string.Format("Fallo al ejecutar el comando '{0} {1}': {2}", fileName, arguments, ex.Message), true);
                    
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n [!] DIAGNÓSTICO DEL ERROR:");
                    if (ex is System.ComponentModel.Win32Exception)
                    {
                        var win32Ex = ex as System.ComponentModel.Win32Exception;
                        if (win32Ex.NativeErrorCode == 2) // File not found
                        {
                            Console.WriteLine("     Causa: El archivo ejecutable '" + fileName + "' no se encuentra en el sistema o en las rutas PATH.");
                        }
                        else if (win32Ex.NativeErrorCode == 5) // Access Denied
                        {
                            Console.WriteLine("     Causa: Acceso denegado. Asegúrese de que el programa tenga privilegios elevados de Administrador.");
                        }
                        else
                        {
                            Console.WriteLine("     Causa (Error de Windows " + win32Ex.NativeErrorCode + "): " + win32Ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("     Causa: " + ex.Message);
                    }
                    Console.ResetColor();
                }
            }
            return exitCode;
        }

        static string GetExitCodeDescription(string programName, int exitCode)
        {
            programName = programName.ToLower();
            
            if (exitCode == 0) return "Operación completada con éxito.";

            switch (programName)
            {
                case "sfc":
                    if (exitCode == 1) return "SFC encontró errores pero no pudo reparar algunos de ellos. Se recomienda revisar el archivo de registro CBS.log en C:\\Windows\\Logs\\CBS\\CBS.log.";
                    if (exitCode == 3) return "SFC detectó que hay un reinicio pendiente. Reinicie su computadora y vuelva a ejecutar el análisis.";
                    return "El escaneo de archivos del sistema reportó un estado inusual o errores que requieren revisión.";

                case "dism":
                    if (exitCode == 87) return "Parámetro incorrecto. La sintaxis del comando DISM es inválida.";
                    if (exitCode == 1726) return "Fallo en la llamada al procedimiento remoto (RPC). Esto suele suceder si el servicio del sistema está sobrecargado.";
                    if (exitCode == -2146498529 || exitCode == unchecked((int)0x800f081f)) 
                        return "No se encontraron los archivos de origen. Windows Update no pudo descargar los archivos necesarios para reparar la imagen del sistema. Verifique su conexión a internet.";
                    return "DISM finalizó con un código de error. Esto indica que la imagen del sistema podría estar dañada o inaccesible.";

                case "chkdsk":
                    if (exitCode == 1) return "Chkdsk no encontró errores en el disco, pero la verificación no fue exhaustiva.";
                    if (exitCode == 2) return "Se encontraron sectores defectuosos o inconsistencias leves en el sistema de archivos.";
                    if (exitCode == 3) return "El volumen de disco está bloqueado o en uso por otro proceso.";
                    return "Chkdsk detectó posibles problemas en el sistema de archivos.";

                case "defrag":
                    if (exitCode == 1) return "El motor de desfragmentación ya se encuentra activo en otra unidad o tarea.";
                    if (exitCode == 2) return "Operación cancelada por el usuario o por recursos insuficientes del sistema.";
                    if (exitCode == 3) return "La unidad C: tiene un formato no compatible o está protegida por BitLocker.";
                    return "El proceso de optimización/desfragmentación no se completó de forma esperada.";

                case "net":
                    if (exitCode == 2) return "El servicio ya se encuentra detenido o iniciado, o el nombre del servicio es incorrecto.";
                    return "Hubo un problema al iniciar o detener el servicio del sistema.";

                case "powershell.exe":
                case "powershell":
                    return "El script de PowerShell devolvió un error de ejecución de comandos internos.";
            }

            return "Error general del sistema. Código de salida no estándar.";
        }

        static void ShowHardwareInfo()
        {
            Console.WriteLine("\n--- INFORMACIÓN DE HARDWARE (VÍA POWERSHELL CIM) ---");
            string cmd = "Write-Host (' Nombre de PC: ' + $env:COMPUTERNAME); " +
                         "Write-Host (' S.O.:         ' + (Get-CimInstance Win32_OperatingSystem).Caption); " +
                         "Write-Host (' CPU:          ' + (Get-CimInstance Win32_Processor).Name); " +
                         "Write-Host (' Memoria RAM:  ' + [Math]::Round((Get-CimInstance Win32_OperatingSystem).TotalVisibleMemorySize / 1MB, 2) + ' GB'); " +
                         "$d = Get-CimInstance Win32_LogicalDisk -Filter 'DeviceID=''C:'''; " +
                         "Write-Host (' Disco C:      ' + [Math]::Round($d.FreeSpace / 1GB, 2) + ' GB libres de ' + [Math]::Round($d.Size / 1GB, 2) + ' GB');";
            
            RunSystemCommand("powershell.exe", "-NoProfile -ExecutionPolicy Bypass -Command \"" + cmd + "\"");
            Console.WriteLine("----------------------------------------------------\n");
        }

        static string GetDriveMediaType(string driveLetter)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = string.Format("-NoProfile -ExecutionPolicy Bypass -Command \"(Get-PhysicalDisk | Where-Object {{ `$_.DeviceID -eq (Get-Partition -DriveLetter {0}).DiskNumber }}).MediaType\"", driveLetter),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd().Trim();
                        process.WaitForExit();
                        return output;
                    }
                }
            }
            catch { }
            return "Unspecified";
        }

        static void CleanRecentFilesFilesOnly(string path)
        {
            if (!Directory.Exists(path)) return;
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    try
                    {
                        if ((File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                        }
                        File.Delete(file);
                    }
                    catch { }
                }
            }
            catch { }
        }

        static void CleanRecentFiles()
        {
            Log("Limpiando historial de Archivos Recientes...");
            string recentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\Windows\\Recent");
            
            CleanRecentFilesFilesOnly(recentPath);
            CleanRecentFilesFilesOnly(Path.Combine(recentPath, "AutomaticDestinations"));
            CleanRecentFilesFilesOnly(Path.Combine(recentPath, "CustomDestinations"));
            
            Log("Historial de Archivos Recientes limpiado con éxito.");
        }

        static void SafeDeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;
            try
            {
                // Limpiar atributos de solo lectura en archivos internos
                string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        if ((File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                        }
                    }
                    catch { }
                }
                
                // Limpiar atributos de solo lectura en directorios internos
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                foreach (string dir in dirs)
                {
                    try
                    {
                        if ((File.GetAttributes(dir) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(dir, File.GetAttributes(dir) & ~FileAttributes.ReadOnly);
                        }
                    }
                    catch { }
                }

                Directory.Delete(path, true);
            }
            catch
            {
                try { Directory.Delete(path, true); } catch { }
            }
        }

        static void RunMantenimientoLigero()
        {
            Log("=== INICIANDO MANTENIMIENTO RÁPIDO / LIGERO ===");
            RunLimpieza();
            RunOptimizacionDisco(); // TRIM/Defrag inteligente (muy rápido en SSD)
            
            Log("Vaciando caché DNS de red...");
            RunSystemCommand("ipconfig", "/flushdns");

            Log("=== ¡MANTENIMIENTO RÁPIDO / LIGERO FINALIZADO CON ÉXITO! ===");
        }

        static void RunMantenimientoProfundo()
        {
            Log("=== INICIANDO MANTENIMIENTO COMPLETO / PROFUNDO ===");
            RunLimpieza();
            RunIntegridad();
            RunOptimizacionDisco();
            RunOptimizacionRed();
            RunDiagnosticoEnergia();
            Log("=== ¡MANTENIMIENTO COMPLETO / PROFUNDO FINALIZADO CON ÉXITO! ===");
        }

        static void ShowSubmenuLoop()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ========================================================");
                Console.WriteLine("   MODO AVANZADO: TAREAS DE MANTENIMIENTO INDIVIDUALES");
                Console.WriteLine(" ========================================================");
                Console.WriteLine();
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" [1] Limpieza de Archivos Temporales, Caché y Papelera");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" [2] Integridad y Reparación del Sistema (SFC, DISM y Chkdsk)");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" [3] Optimización y Desfragmentación de Disco (C:)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" [4] Optimización y Restablecimiento de Red (IP / DNS / Sockets)");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" [5] Diagnóstico de Energía e Información de Hardware");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(" [6] Volver al Menú Principal");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n Seleccione un módulo a ejecutar (1-6): ");
                Console.ResetColor();
                string choice = Console.ReadLine();

                if (choice == "6") break;

                Console.Clear();
                DrawHeader();

                switch (choice)
                {
                    case "1":
                        RunLimpieza();
                        break;
                    case "2":
                        RunIntegridad();
                        break;
                    case "3":
                        RunOptimizacionDisco();
                        break;
                    case "4":
                        RunOptimizacionRed();
                        break;
                    case "5":
                        RunDiagnosticoEnergia();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" [!] Opción no válida. Intente de nuevo.");
                        Console.ResetColor();
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n Presione cualquier tecla para volver al menú avanzado...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        static void CleanBrowserCaches()
        {
            Log("Limpiando caché de navegadores Chrome y Edge...");

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Google Chrome
            string chromeCache = Path.Combine(localAppData, "Google\\Chrome\\User Data\\Default\\Cache");
            string chromeCodeCache = Path.Combine(localAppData, "Google\\Chrome\\User Data\\Default\\Code Cache");
            CleanDirectory(chromeCache, "Caché de Google Chrome");
            CleanDirectory(chromeCodeCache, "Caché de Código de Google Chrome");

            // Microsoft Edge
            string edgeCache = Path.Combine(localAppData, "Microsoft\\Edge\\User Data\\Default\\Cache");
            string edgeCodeCache = Path.Combine(localAppData, "Microsoft\\Edge\\User Data\\Default\\Code Cache");
            CleanDirectory(edgeCache, "Caché de Microsoft Edge");
            CleanDirectory(edgeCodeCache, "Caché de Código de Microsoft Edge");
        }

        static void ThreadSleep(int ms)
        {
            try
            {
                System.Threading.Thread.Sleep(ms);
            }
            catch { }
        }

        #endregion
    }
}
