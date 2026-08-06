# Consola de Mantenimiento de PC (Windows Native)

Un sistema de mantenimiento preventivo y diagnóstico de PC portátil, ligero (aprox. 16.5 KB) y autoelevado, desarrollado en C# y diseñado para ejecutarse directamente en sistemas Windows utilizando únicamente comandos y herramientas nativas del sistema operativo.

Este proyecto se compila localmente de forma directa sin necesidad de instalar entornos de desarrollo pesados (como Visual Studio) o dependencias externas de terceros, aprovechando el compilador de .NET Framework preinstalado de fábrica en Windows.

---

## 🚀 Características Principales

1. **🧹 Limpieza Segura de Temporales y Caché:**
   - Limpieza profunda de temporales de usuario (`Temp`) y del sistema (`C:\Windows\Temp`).
   - Caché de precarga (`Prefetch`).
   - Caché de descargas de Windows Update (deteniendo e iniciando los servicios de forma limpia).
   - Vaciado forzado de la Papelera de Reciclaje.
   - **Historial de Archivos Recientes:** Eliminación limpia de accesos directos recientes y listas de salto (`Jump Lists`) del Explorador de Archivos sin alterar las carpetas del sistema.

2. **🛠️ Integridad y Reparación del Sistema:**
   - Ejecución guiada de **SFC (System File Checker)** para reparar archivos corruptos del sistema.
   - Ejecución de **DISM (Deployment Image Servicing and Management)** para restaurar la imagen del sistema a través de Windows Update.
   - Escaneo de inconsistencias en el disco en línea (`Chkdsk /scan`).

3. **💾 Optimización Inteligente de Disco (Segura para SSD/M.2/NVMe):**
   - El sistema detecta automáticamente mediante comandos de almacenamiento si la unidad `C:` es un **SSD / M.2** o un **HDD mecánico**.
   - **Si es SSD/M.2:** Ejecuta la función nativa **TRIM (Re-Trim)** para mejorar la velocidad de escritura y prolongar la vida útil del disco, omitiendo la desfragmentación física para evitar la degradación del hardware.
   - **Si es HDD:** Realiza la desfragmentación y ordenamiento clásico de sectores.

4. **🌐 Optimización de Conectividad y Red:**
   - Liberación y renovación de dirección IP.
   - Vaciado de la caché DNS (`ipconfig /flushdns`).
   - Restablecimiento completo del catálogo Winsock y del protocolo de internet TCP/IP.

5. **🔋 Diagnóstico de Energía e Información de Hardware:**
   - Resumen rápido de especificaciones de hardware (Nombre de PC, Sistema Operativo, Procesador, Memoria RAM y almacenamiento de C:) vía PowerShell CIM.
   - Generación de reportes detallados en HTML interactivo para la salud de la batería (`reporte_bateria.html`) y eficiencia energética (`reporte_energia.html`).

6. **📝 Registro de Actividad y Diagnóstico de Errores:**
   - Cada acción y detalle se almacena localmente en `mantenimiento_log.txt`.
   - **Traducción de Errores Nativos:** En lugar de mostrar códigos numéricos de error de Windows incomprensibles (como `0x800f081f` o `87`), la consola intercepta el código de salida de los subprocesos y proporciona un diagnóstico descriptivo en español explicando la causa común del fallo y cómo resolverlo.

---

## 🛠️ Estructura del Proyecto

* **`Program.cs`:** Código fuente principal en C# que maneja los menús interactivos, lógica de comandos nativos y manejo de errores.
* **`app.manifest`:** Manifiesto de la aplicación para forzar la elevación de permisos (requiere privilegios de administrador para ejecutar las herramientas de sistema).
* **`build.bat`:** Script automatizado de compilación que busca la versión nativa de `csc.exe` en tu PC y genera el `.exe`.

---

## 💻 Requisitos
* Sistema Operativo: Windows 8, 10 u 11.
* Permisos de Administrador (el ejecutable te los solicitará automáticamente al iniciar).
* .NET Framework 4.0 o superior (instalado por defecto en todos los sistemas operativos Windows modernos).

---

## 📦 Instrucciones de Compilación y Uso

### 1. Compilar el archivo ejecutable (.exe)
Si deseas compilar la aplicación tú mismo desde el código fuente:
1. Abre una consola de comandos (cmd) o PowerShell en la ruta del proyecto.
2. Ejecuta el archivo de compilación:
   ```cmd
   build.bat
   ```
3. El script ubicará el compilador nativo de C# de Windows y creará el ejecutable **`MantenimientoPC.exe`** en la misma carpeta.

### 2. Ejecutar la Aplicación
1. Haz doble clic en el archivo **`MantenimientoPC.exe`** generado.
2. Windows te mostrará una alerta de Control de Cuentas de Usuario (UAC) solicitando permisos de administrador. Acepta para continuar.
3. Se abrirá la consola interactiva con el menú de opciones guiado. Elige el número de la tarea de mantenimiento que deseas realizar o la opción `[6]` para hacer un mantenimiento completo automatizado.

---

## ⚠️ Advertencia y Descargo de Responsabilidad

Este programa ejecuta utilidades del sistema nativas de Microsoft Windows que realizan modificaciones en la red, caché del sistema y archivos temporales. Úsalo bajo tu propia responsabilidad.
