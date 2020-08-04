if not exist "%AppData%\Ideum\TouchlessDesignService" mkdir "%AppData%\Ideum\TouchlessDesignService"
robocopy .\TouchlessDesignService\ "%AppData%\Ideum\TouchlessDesignService" /E
explorer "%AppData%\Ideum\TouchlessDesignService"