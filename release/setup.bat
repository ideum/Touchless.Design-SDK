if not exist "%AppData%\Ideum\TouchlessDesign" mkdir "%AppData%\Ideum\TouchlessDesign"
robocopy .\TouchlessDesign\ "%AppData%\Ideum\TouchlessDesign" /E
cd /D "%AppData%\Ideum\TouchlessDesign\bin\Service\"
start /d "%AppData%\Ideum\TouchlessDesign\bin\Service\" TouchlessDesign.exe