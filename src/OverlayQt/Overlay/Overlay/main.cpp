#include <QApplication>
#include <QQmlApplicationEngine>

#include "applicationloader.h"

int main(int argc, char *argv[])
{
    QApplication app(argc, argv);
    app.processEvents();
    app.setQuitLockEnabled(true);

    ApplicationLoader loader;
    loader.run();

    return app.exec();
}
