if not exist "%AppData%\Ideum\TouchlessDesignService" mkdir "%AppData%\Ideum\TouchlessDesignService"
robocopy .\TouchlessDesignService\ "%AppData%\Ideum\TouchlessDesignService" /E
cd /D "%AppData%\Ideum\TouchlessDesignService\bin\Service\"
start /d "%AppData%\Ideum\TouchlessDesignService\bin\Service\" TouchlessDesignService.exe